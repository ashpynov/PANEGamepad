using System.Collections.Generic;
using System.Linq;
using PANEGamepad.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace PANEGamepad.Scenes.Customization
{
    public static class OverlayController
    {

        public static bool PreviousOverlay() => PickOverlay(false);
        public static bool NextOverlay() => PickOverlay(true);
        public static bool ToggleOverlay() => PickOverlay(null);

        private const string OverlayPath = "Canvas/UI/Bars/LeftBar/OverlayButton";
        private const string OverlaySelectorsPath = OverlayPath + "/OverlaySelectors";

        private static GameObject _lastOverlay = null;

        private static List<GameObject> GetOverlayButtons()
        {
            GameObject overlaySelectors = GameObject.Find(OverlaySelectorsPath);
            if (overlaySelectors == null)
            {
                return new();
            }
            List<GameObject> buttons = new();

            for (int i = 0; i < overlaySelectors.transform.childCount; i++)
            {
                bool skipNext = false;
                GameObject go = overlaySelectors.transform.GetChild(i).gameObject;
                if (go.activeSelf)
                {
                    for (int j = 0; j < go.transform.childCount; j++)
                    {
                        GameObject subCategories = go.transform.GetChild(j).gameObject;
                        if (subCategories.name == "SubCategories")
                        {
                            for (int k = 0; k < subCategories.transform.childCount; k++)
                            {
                                GameObject selector = subCategories.transform.GetChild(k).gameObject;
                                buttons.Add(selector);
                                if (selector.name.EndsWith("All"))
                                {
                                    break;
                                }
                            }
                            skipNext = true;
                            break;
                        }
                    }
                    if (skipNext)
                    {
                        continue;
                    }
                    buttons.Add(go);
                }
            }
            return buttons;
        }

        private static bool IsEnabled(GameObject overlayButton)
        {
            return overlayButton.GetFieldValue<bool>("OverlaySelector", "_isActive");
        }
        private static int GetActiveIndex(List<GameObject> buttons)
        {

            int first = buttons.FindIndex(IsEnabled);
            int last = buttons.FindLastIndex(IsEnabled);

            if (first != last)
            {
                Plugin.Log.LogInfo($"unknown overlay is enabled {buttons[first].name} or {buttons[last].name}");
            }
            return first == last ? first : -1;

        }
        private static bool PickOverlay(bool? next)
        {
            List<GameObject> buttons = GetOverlayButtons();
            if (buttons.Count == 0)
            {
                return false;
            }
            if (_lastOverlay == null)
            {
                _lastOverlay = buttons.Last();
            }
            GameObject active;
            GameObject toSelect;
            int idx = GetActiveIndex(buttons);
            //int idx = buttons.FindIndex(b => b.name == _lastOverlay.name);
            if (idx != -1)
            {
                active = buttons[idx];

                idx += next is true ? 1 : next is false ? -1 : 0;
                if (idx < 0)
                {
                    toSelect = buttons.Last();
                }
                else if (idx >= buttons.Count)
                {
                    toSelect = buttons.First();
                }
                else
                {
                    toSelect = buttons[idx];
                }
            }
            else
            {
                active = buttons[0];
                toSelect = next is true ? buttons[1] : buttons[0];
            }
            if (next is null)
            {
                if (active.name != buttons.First().name)
                {
                    _lastOverlay = active;
                    toSelect = buttons.First();
                }
                else
                {
                    _lastOverlay ??= buttons.Last();
                    toSelect = _lastOverlay;
                }
            }
            return EnableOverlay(toSelect);
        }
        private static bool EnableOverlay(GameObject toSelect)
        {
            if (toSelect == null)
            {
                return false;
            }

            if (GameObject.Find(OverlaySelectorsPath) is GameObject panel && !panel.activeSelf)
            {
                panel.GetComponentInParent<Button>().onClick.Invoke();
            }

            Button button = toSelect.GetComponent<Button>();
            button.onClick.Invoke();

            GameObject subPanel = toSelect.GetComponentInParent<Component>().gameObject;
            Button selector = null;
            if (subPanel.name == "SubCategories")
            {
                selector = subPanel.GetComponentInParent<Button>();
                if (!toSelect.name.EndsWith("All") && !subPanel.activeSelf)
                {
                    selector.onClick.Invoke();
                }
            }

            if (GameObject.Find(OverlaySelectorsPath) is GameObject selectorButtons)
            {
                foreach (GameObject go in selectorButtons.GetChildren())
                {
                    bool active = go.name == button.name || (selector != null && go.name == selector.name);
                    go.SendMessage("SetActive", active); // Single parameter
                }
            }

            if (selector != null && button.name.EndsWith("All"))
            {
                InputTracker.SetFocus(selector.gameObject);
            }
            else
            {
                InputTracker.SetFocus(button.gameObject);
            }

            return true;
        }
    }
}