# 05 · Vision, fog of war and stealth

---

## 1. The vision grid

- A separate grid of **8 m cells** (half the resolution of the move grid: 64×64 to 128×128). It's cheap.
- Per team, each cell stores:
  - `visibleCount` (how many of my units see it now)
  - `explored` (seen at least once)
  - `detectCount` (how many detectors see it now)
- **Update every 4 ticks**, incrementally:
  - Units that **moved to a new cell** remove their old circle and add their new one.
  - Units that didn't change cell do nothing. That makes it very cheap with 1,000 units.
- A circle "stamp" is precomputed per radius (in cells), so there's no maths per update.

## 2. What a team can see

| Thing | Visible to enemy when… |
|---|---|
| Normal unit | its cell has `visibleCount > 0` for that enemy team |
| Stealth unit (bomber, sub, Leviathan when submerged) | its cell has `detectCount > 0` **or** it's firing (revealed for 2 s per shot) |
| Building | visible once seen. After that a **"ghost"** (last-known state) stays in grey fog until seen again |
| Projectiles and explosions | drawn if their cell is visible |

- **Targeting** only picks visible targets (`03_Combat`).
- **Hills:** units on a hill cell (height > 6 m above the neighbours) get +25% vision. There's no true line-of-sight (too expensive, and players don't miss it).
- **Weather and night** multiply vision: storm ×0.6 and night ×0.7. The Sensor Tower is not affected.

## 3. Fog rendering (view only)

- Every 4 ticks, write the local player's grid into a small `Texture2D` (R8): 0 = never seen, 128 = explored, 255 = visible.
- Upload it to the GPU and blur it 1 pixel in a shader, for soft edges.
- The fog is drawn as a full-screen pass **before** the pixel filter, so it gets pixelated with the world:
  - Unexplored = near-black (10% brightness).
  - Explored but not visible = 55% brightness, desaturated.
- The minimap uses the same texture.

## 4. Radar-style "seen recently" pings

- When an enemy unit **attacks** a building of mine that I can't see, a red ping appears at the attacker's position for 3 s. Players aren't blind to artillery.
- The **"Under attack" alert** fires at most once per 10 s per area (100 m) to avoid spam.

## 5. Settings

Skirmish setting: **Fog: Normal / Explored (whole map explored, units still hidden) / Off**.
Campaign: per mission.

## 6. Tests

- 1,000 moving units: the vision update stays under 1 ms per update on the target phone.
- A stealth bomber flies over a base with no Sensor Tower and can't be targeted. With a tower it can.
- Destroying an enemy building while it isn't visible leaves its ghost until the area is seen again.
