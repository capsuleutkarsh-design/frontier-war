# 09 · Maps, missions and campaign

---

## 1. Map asset

A map is a `MapDef` ScriptableObject plus its baked data. **The map is data, not a scene full of objects.**

| Field | Content |
|---|---|
| `id`, `displayName`, `biome`, `size` | `forest_crossing`, "Forest Crossing", Forest, Medium |
| `grid` | per cell: terrain, height, ore id (baked into a binary `.bytes` file) |
| `deposits` | list: position, richness (×1.0 normal, ×1.5 rich) |
| `startPositions` | 2–8 points, each with a starting Core position |
| `props` | instanced prop list (type, position, rotation, scale) |
| `neutralObjects` | bridges (destroyable), ruins, old neutral turrets |
| `regions` | named rectangles/circles for triggers ("RiverCrossing", "EnemyBase") |
| `skirmishAllowed`, `maxPlayers`, `hasWater` | |

### Map editor (an Editor tool, built in Milestone 4)
A Unity Editor window with paint brushes:
- **Terrain types** and **height** (raise, lower, flatten, smooth).
- **Deposits**, **start positions** and **regions**.
- **Props**: scatter brush with density and random rotation.

It has a **Validate** button. Validation checks every start position can path to every other one for Tracked units, every start has 3+ deposits within 120 m, and no deposit sits on blocked terrain.
**Bake** writes the `.bytes` grid and the terrain mesh.

## 2. Triggers (the same Condition/Effect system as ActionDef)

A mission is a `MissionDef` holding **Triggers**:
```
Trigger
  name, once (bool), enabled (bool)
  conditions : Condition[]   (ALL must be true)
  effects    : Effect[]
```
**Extra conditions for missions:** `TimeElapsed(s)`, `UnitsInRegion(team, region, count, filter)`, `UnitDestroyed(tag)`, `BuildingCount(team, id) >= n`, `TeamDefeated(team)`, `FlagSet(name)`, `CreditsAtLeast(x)`, `PlayerSelected(tag)` (for the tutorial).

**Extra effects for missions:** `ShowMessage(speaker, text, seconds)`, `SetObjective(id, text, state)`, `SpawnUnits(team, def, n, region)`, `GiveCredits(team, x)`, `SetAIDoctrine(team, doctrine)`, `AIAttack(team, region)`, `RevealRegion(region, s)`, `CameraPanTo(region)`, `EnableTrigger(name)`, `Victory()`, `Defeat(reason)`, `SetWeather(type)`, `PlayMusic(id)`.

**Example: "Hold the Bridge" (mission 07)**
```
T1 start        : TimeElapsed(0)   → SetObjective(main,"Hold the bridge for 8 minutes"), ShowMessage(...)
T2 wave1        : TimeElapsed(60)  → SpawnUnits(enemy, light_tank, 6, EastRoad), AIAttack(enemy, Bridge)
T3 wave2        : TimeElapsed(240) → SpawnUnits(enemy, battle_tank, 4, EastRoad), ...
T4 lose         : UnitsInRegion(enemy, Bridge, 5, any) for 20 s → Defeat("The bridge fell")
T5 win          : TimeElapsed(480) → SetObjective(main, done), Victory()
T6 optional     : no player losses in Bridge region → SetObjective(opt1, done)
```
Units placed in the editor get **tags** (for example `Convoy`, `Boss`) so triggers can refer to them.

## 3. Campaign flow

- `CampaignDef` holds 5 chapters × 10 missions (GDD page 21).
- The **progress save** stores: missions finished, stars per mission, unlocks and settings.
- **Mission screen:** a briefing (3 lines, with a map preview and objectives), then the battle, then results (stars, time, losses, battle report).
- Missions unlock in order. Each chapter's last mission unlocks the next chapter.
- **Difficulty per mission:** Easy / Normal / Hard. This changes AI difficulty and the starting credits only (no separate map versions).

## 4. Tutorial (missions 1–5)

These use triggers like `PlayerSelected` and `UnitsInRegion` with small **tip cards** ("Tap a Constructor", "Drag on empty ground to move the camera"). Each tip shows once. A "Skip tutorial" option is in settings.

## 5. Production rule for 50 missions (so it never stalls)

1. **Template first:** build one mission template scene with player start, enemy base, 3 triggers and the objective UI. Copy it for every mission.
2. **Grey-box each chapter in one go** (10 maps, rough triggers, beatable). Then playtest the whole chapter, then polish.
3. **One signature twist per mission** (from the GDD table). Don't add a second.
4. Track it in `Docs/04_Production/Mission_Tracker.md`: Greybox → Playable → Tested → Polished.
