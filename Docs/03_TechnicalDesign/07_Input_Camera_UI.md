# 07 · Input, camera and UI

Use the **Unity Input System** package with touch, and mouse and keyboard for PC testing. One `InputRouter` turns raw input into **intents** (Select, BoxSelect, Order, Pan, Zoom). The `SelectionController` and `OrderController` turn intents into Commands.

---

## 1. Camera

- **Orthographic**, pitch 50°, yaw 45°, **never rotates**.
- **Zoom:** orthographic size from 20 (close) to 400 (full map). **Default 60** gives the scale table in `Ebbtide_Look_Study`.
- **Pan:** one-finger drag on empty ground, with inertia (it slides and slows). The edges are clamped to the map plus a 20 m margin.
- **Strategic zoom:** above size 180, units fade to **icons** (see below), and above 300 terrain props fade out too. Orders still work.
- **Jump:** tap the minimap, double-tap an alert, or select a control group twice to centre on it.
- Pixel-filter note: the camera moves in **whole low-res pixel steps** ("pixel snapping") so the image doesn't shimmer while panning (`08_Rendering`).

## 2. Touch gestures (final mapping)

| Gesture | On empty ground | On own unit | On enemy | On own building |
|---|---|---|---|---|
| Tap | Move the selection (or deselect if nothing is selected) | Select it | Attack it | Select it (shows its panel) |
| Double-tap | – | Select all of that type on screen | – | Select all of that type |
| One-finger drag | Pan camera | – | – | – |
| Hold the **Box** button + drag (or two-finger drag) | Box select | | | |
| Long-press | Opens a radial order menu: Move · Attack-Move · Patrol · Guard here | Unit menu: Guard it, Load into transport | Force-attack | Set rally (factory) |
| Pinch | Zoom | | | |

- Hitboxes are at least **max(unit radius, 9 mm on screen)**. Tapping between units picks the nearest.
- Smoke, effects and wrecks are **never** hit-tested.

## 3. Selection

- Up to **200** units selected at once. The selection panel shows type icons with counts (tap one to sub-select that type).
- **Control groups:** 6 slots on the left edge. Long-press a slot saves the selection, tap recalls it, and double-tap centres the camera.
- **Smart filters:** "All army" (excludes builders), "All idle builders", "All air", "All naval".

## 4. Orders and feedback

- **Order bar** (bottom centre, shown when units are selected): Stop · Attack-Move · Guard · Patrol · Hold · Same type · Waypoint (+) · Keep together.
- **Feedback:** a green ring with arrows for Move, a red ring for Attack-Move, a blue line for Patrol, and a dotted line for queued waypoints (while selected).
- **Every order gives a tiny sound and haptic tick** (with a vibration setting).

## 5. HUD layout (landscape)

```
┌──────────────────────────────────────────────────────────────────────┐
│ ◆ 4,250 +62/s │ Units 86/300 │ CP 4 │ 12:40      [Alerts]  ⏸  ×1  ☰ │  top bar
│                                                                      │
│ [G1]                                                                 │
│ [G2]                      BATTLEFIELD                                │
│ [G3]                                                                 │
│ [G4]                                                                 │
│ ┌────────┐  [Stop][A-Move][Guard][Patrol][Hold]    ┌───────────────┐ │
│ │MINIMAP │                                         │ Build│Units│Up │ │
│ │        │  (selection panel)                      │ [ ][ ][ ][ ]  │ │
│ └────────┘                                         │ [ ][ ][ ][ ]  │ │
└──────────────────────────────────────────────────────────────────────┘
```
- **Context panel (right):** a builder shows Build tabs (Economy · Factories · Defence · Advanced). A factory shows Units, Upgrades, Queue, Repeat toggle and Rally. A drill shows Upgrade or the final choice.
- **Each build button** shows the icon, cost and a tier lock if locked. Long-press opens the info card: role, good vs, weak vs, HP, speed and range.
- **Alerts:** a stack of small toasts ("Drill under attack", "Experimental detected", "Unit cap", "Builder idle"). Tap one to jump there.
- **Left-handed option:** mirrors the layout.

## 6. UI technology

- **uGUI** (Canvas) for the in-game HUD (well documented and fine on mobile). Use one canvas for static HUD and one for dynamic parts, so they don't rebuild each other.
- **World-space bars:** health bars and selection rings are **not** UI objects. They're drawn in batches with `Graphics.DrawMeshInstanced` (one draw call for all bars).
- **Icons for strategic zoom:** the same instanced system with an icon atlas: type × team colour.
- **Menus** (main menu, settings, campaign map) also use uGUI with the same style kit (dark olive panels, white text). The UI scale setting runs from 80% to 140%.

## 7. Tactical pause and speed

- Pause sets `gameSpeed = 0`. Input still creates Commands, which apply on the first tick after unpause. Orders show as "planned".
- Speeds: ×0.5, ×1, ×1.5, ×2 (campaign and skirmish).

## 8. Tests

- On the target phone: tap a single tank inside a group of 30 and the right one is selected 9 out of 10 times.
- Box-select 150 units, issue a move, and it feels instant (under 100 ms to show the feedback ring).
- A blind test: a new player finds how to build a drill in under 60 s with only the tutorial tip.
