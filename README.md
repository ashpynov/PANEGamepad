# Pharaoh: A New Era Gamepad support Mod

Greetings to thee, O Pharaoh,
The splendid game lacks support for gamepad control, and our scribes have resolved to amend this.

In this, the offered modification "PANEGamepad" shall aid thee. On how to install it, read at the end of this papyrus. And here, thy nomarch shall recount what changes in the game’s command.

## Gamepad in game

Modification use native XInput support for gamemad so every controllers which support it or emulated should be supported.

I will use ['XBox One' styled notation](https://https://support.xbox.com/en-US/help/hardware-network/controller/xbox-one-wireless-controller) to describe buttons.

![](assets/controller-diagram.png)

Almost everytime gamepad just mimic keyboard or mouse pressing, with some adoptation to current game situation, so it required that your in-game keyboard binding is default.

## General control

* **D-Pad** - will move your mouse to closest interface element in specified direction.
* **A** - Confirm or Select or Click Left mouse button
* **B** - Cancel or Click Right mouse button.
* **Left thumbstick** - to Move mouse in any direction - more shifted - faster speed.
* **Right thumbstick** - to Scroll Map in specified direction (=> WASD), or Scroll Up/Down scrollable elements.
* **Left Trigger** and **Right Trigger** - work as shift buttons to assign more combinations. This combinations are game scene specific.

## Scene combinations
|Combination|Action|
-----------|------|
|***Main Gameplay***
|X          | Pick building*
|Y          | Rotate building*
|Menu       | Game Menu
|View       | Open overseers (last)
|LT + A     | House
|LT + B     | Remove
|LT + X     | Road
|LT + Y     | Roadblock
|RT + A     | Gardens
|RT + B     | Plaza
|RT + X     | Firehouse
|RT + Y     | Architect Post
|LT + Menu  | Undo
|LB         | Pause
|RB         | Normal Speed
|LT + LB    | Slower
|LT + RB    | Faster
|LT + RStick Up| Zoom View In
|LT + RStick Down| Zoom View Out
|LS         | Flat mode toggle
|LT + LS    | Grid mode toggle
|LT + Up    | Previous Overlay
|LT + Down  | Next Overlay
|LT + View  | Toggle overlay
|LT + Left  | Commerce overseer
|LT + RT + Menu | Qiuck Load
|LT + RT + View | Qiuck Save
| ***Overseers***
|LT + Up    | Previous overseer
|LT + Down  | Next overseer
| ***WorldMap***
|View       | Open Commerce overseer

## Extra Features
### Stick side panel
On main gameplay - press DPad in direction while mouse somewhere in scrren - focus will jubped to side pannels and stick to them. To release just move mouse

### Back to previous mouse location
On main gameplay - Last mouse position before DPAD navigation is stored and will be restored after biulding is picked or canceled

### Back to text box on dropdowns
Focus will return to text box combo part after item is selected in dropdown

### Single button screen
It there is no other buttons on screen except "proceed" no need to focus it, just press 'A' or 'B'

### Auto scroll
If focused item is close to scroll view boundaries, or even behind - it will automaticly scroll into view.

# Installation
If you gonna to play Pharaoh: A New Era on PC with gamepad, or on your handheld device just do simple steps:

### 1. BepInEx
*If Your already using other PANE mods - skip this step

Install BepInEx - this is tool to load 'Mods' into Unity game. To do it just download archive from their site. I use version BepInEx_win_x64_5.4.23.3.zip from [their repository](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23). Direct [link to archive](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.3/BepInEx_win_x64_5.4.23.3.zip).

Installation are [simple](https://docs.bepinex.dev/articles/user_guide/installation/index.html). TL;DR: Just unpack content into your game folder. It should BepInEx folder in Pharaoh A New Era.


### 2. PANEGamepad.dll
Download PANEGamepad.dll from [release page](https://github.com/ashpynov/PANEGamepad/releases) and put it into Pharaoh A New Era/BepInEx/plugins folder.

Thats it. If you have feedback or would like to suggest some new feature - welcome to [issues page](https://github.com/ashpynov/PANEGamepad/issues).


# TODO:
Here my thoughts what can be improved:

- Order screen buttons: x - accept none, y - accept all
- Commerce Overseer screen buttons: LB / RB - change amount, x - collect, y - open route
- Option to focus at default on scene enter
- Skip Inactive 'Accept' button within delivery request
- Zoom by trigger
- Configurable mapping





