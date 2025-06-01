
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PANEGamepad.Scenes.Customization
{
    public static class OverseersScene
    {
        public const SceneCode Code = SceneCode.Overseers;

        public static string _lastOverseer = "CommerceOverseer";
        private static readonly Dictionary<string, string> OverseersButton = new()
        {
            {"ChiefOverseer", "chiefOverseer"},
            {"WorkersOverseer", "workforce"},
            {"MilitaryOverseer","military" },
            {"PoliticalOverseer", "political"},
            {"RatingsOverseer", "ratings"},
            {"CommerceOverseer", "commerce"},
            {"PopulationOverseer", "pop"},
            {"HealthOverseer", "health"},
            {"EducationOverseer", "education"},
            {"EntertainmentOverseer", "entertainment"},
            {"TempleOverseer", "religion"},
            {"TreasuryOverseer", "treasury"},
            {"MonumentOverseer", "monuments"}
        };

        public static bool Taste(IEnumerable<GameObject> _)
        {
            if (GameObject.Find("Canvas/UI/Overseers") is GameObject ovs && ovs.activeInHierarchy)
            {
                for (int i = 0; i < ovs.transform.childCount; i++)
                {
                    if (ovs.transform.GetChild(i).gameObject.activeSelf)
                    {
                        _lastOverseer = Current();
                        return true;
                    }
                }
            }
            return false;
        }

        private static string Current()
        {
            GameObject ovs = GameObject.Find("Canvas/UI/Overseers");

            if (ovs == null || !ovs.activeInHierarchy)
            {
                return _lastOverseer;
            }
            for (int i = 0; i < ovs.transform.childCount; i++)
            {
                GameObject current = ovs.transform.GetChild(i).gameObject;
                if (current != null && current.activeSelf)
                {
                    return current.name;
                }
            }
            return _lastOverseer;
        }

        private static bool PickOverseer()
        {
            return PickOverseer(_lastOverseer, true) || PickOverseer(_lastOverseer, false);
        }
        private static bool PickOverseer(string name, bool down)
        {
            List<GameObject> buttons = new();

            Plugin.Log.LogInfo($"GetOverseed {name}");
            GameObject button = GameObject.Find("Canvas/UI/Bars/LeftBar/OverseerButton");
            if (button == null || !button.activeInHierarchy)
            {
                return false;
            }

            GameObject selectors = GameObject.Find("Canvas/UI/Bars/LeftBar/OverseerButton/OverseerSelectors");

            if (selectors == null)
            {
                return false;
            }

            for (int i = 0; i < selectors.transform.childCount; i++)
            {
                GameObject b = selectors.transform.GetChild(i).gameObject;
                if (b.activeSelf && OverseersButton.ContainsValue(b.name))
                {
                    buttons.Add(b);
                }
            }

            string buttonName = OverseersButton[name] ?? "";
            string overseerName = "";
            GameObject selector = buttons.FirstOrDefault(b => b.name == buttonName);
            if (selector == null)
            {
                List<string> keys = OverseersButton.Keys.ToList();
                if (!down)
                {
                    keys.Reverse();
                }

                for (int idx = keys.IndexOf(name) + 1; idx < keys.Count; idx++)
                {
                    buttonName = OverseersButton[keys[idx]] ?? "";
                    selector = buttons.FirstOrDefault(b => b.name == buttonName);
                    if (selector != null)
                    {
                        overseerName = keys[idx];
                        break;
                    }
                }
            }

            if (selector != null)
            {
                try
                {
                    if (!selectors.activeSelf)
                    {
                        SceneController.PressButton(button);
                    }
                    SceneController.PressButton(selector);
                    Canvas.ForceUpdateCanvases();
                    InputTracker.SetFocus(selector);
                    InputTracker.Scene.SetSceneCode(Code);
                    _lastOverseer = OverseersButton.FirstOrDefault(p => p.Value == selector.name).Key;
                    Plugin.Log.LogInfo($"current {_lastOverseer}");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        private static bool SwitchOverseer(bool next = true)
        {
            GameObject ovs = GameObject.Find("Canvas/UI/Overseers");

            List<GameObject> overseers = new();
            if (ovs == null || !ovs.activeInHierarchy)
            {
                return false;
            }
            for (int i = 0; i < ovs.transform.childCount; i++)
            {
                overseers.Add(ovs.transform.GetChild(i).gameObject);
            }

            if (overseers.Count == 0)
            {
                return false;
            }

            List<GameObject> buttons = new();
            GameObject selectors = GameObject.Find("Canvas/UI/Bars/LeftBar/OverseerButton/OverseerSelectors");

            if (selectors == null || !selectors.activeInHierarchy)
            {
                return false;
            }

            for (int i = 0; i < selectors.transform.childCount; i++)
            {
                GameObject b = selectors.transform.GetChild(i).gameObject;
                if (b.activeSelf && OverseersButton.ContainsValue(b.name))
                {
                    buttons.Add(b);
                }
            }
            if (buttons.Count < 2)
            {
                return false;
            }

            GameObject activeDlg = overseers.FirstOrDefault(o => o.activeSelf);
            if (!activeDlg)
            {
                return false;
            }

            int idx = buttons.FindIndex(b => b.name == OverseersButton[activeDlg.name]);
            if (idx == -1)
            {
                return false;
            }
            if (next && idx < buttons.Count - 1)
            {
                idx++;
            }
            else if (!next && idx > 0)
            {
                idx--;
            }
            else
            {
                idx = -1;
            }

            if (idx != -1)
            {
                SceneController.PressButton(buttons[idx]);
                InputTracker.SetFocus(buttons[idx]);

                _lastOverseer = OverseersButton.FirstOrDefault(p => p.Value == buttons[idx].name).Key;
                return true;
            }
            return true;
        }

        public static bool OpenLast() => PickOverseer();
        public static bool OpenCommerce() => PickOverseer("CommerceOverseer", false);
        public static bool OpenPopulation() => PickOverseer("PopulationOverseer", true);

        public static bool Next() => SwitchOverseer(true);
        public static bool Previous() => SwitchOverseer(false);

    }
}