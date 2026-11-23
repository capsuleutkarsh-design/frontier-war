# 11 · Performance and testing

---

## 1. Target device

Pick **one mid-range Android phone** you can test on (for example a 2–3-year-old mid-range Snapdragon or Dimensity phone with 6 GB RAM) and call it the **Target Phone**. Every milestone is judged on it, never in the editor.

## 2. Frame budget at 60 FPS (16.6 ms)

| Part | Budget |
|---|---|
| Simulation (20 ticks/s, so about 1 tick per 3 frames) | 4 ms per tick on average, spread over frames |
| View update (units, turrets, interpolation) | 3 ms |
| Rendering (CPU side) | 4 ms |
| UI | 1 ms |
| Spare | 4 ms |

**Per tick (with 500 units):** movement 1.2 ms · targeting 0.6 ms · weapons and projectiles 0.6 ms · vision 0.3 ms · AI 0.5 ms · the rest 0.3 ms.

## 3. Rules that keep it fast

1. **No allocations in Tick.** Pool every list, projectile and event. Check this with the Profiler's "GC Alloc" column. It must read 0 B per frame in battle.
2. **No `GetComponent`, `Find` or LINQ** in per-frame code.
3. **SpatialHash** for every "nearby" query. Never loop over all units inside a loop over all units.
4. **Stagger work:** targeting every 4 ticks, vision every 4, and AI every 10, offset per unit or team so it doesn't all happen on the same tick.
5. **Instanced drawing** for props, health bars, icons and rings.
6. Use **Burst/Jobs** only for proven hot spots (likely separation and flow fields).
7. **Profile on the Target Phone at every milestone.** Use the Unity Profiler connected over USB, plus a development build.

## 4. Test types

| Type | Tool | What |
|---|---|---|
| **EditMode unit tests** | Unity Test Framework (`FW.Tests`) | Damage formula, economy maths, production queue, pathfinding (does a path exist, is it the shortest), trigger conditions. They run in seconds. |
| **Simulation scenario tests** | EditMode, headless sim | "3 Light Tanks vs 1 Rail Tank → Rail Tank wins". "200 tanks cross the map, 0 stuck". These run without graphics. |
| **AI soak test** | Sandbox scene at ×4 speed, run overnight | AI vs AI, 20 games. Log wins, crashes, stuck units and exceptions to a file. |
| **Save/load determinism** | EditMode | `10_Save` test |
| **Device test** | Target Phone, weekly | FPS in a 300-unit battle, touch accuracy, heat after 20 minutes, memory |
| **Playtest** | Friends, monthly | Watch silently and write down where they get confused |

**Rule:** a system isn't "done" until its tests from the design doc pass. Each doc ends with a **Tests** section.

## 5. Debug tools (build them early, because they save weeks)

- **Debug overlay:**
  - Shows FPS, tick time per system, unit count and projectile count.
  - Shows the flow field under the cursor, the threat map and AI squads.
- **Sandbox scene:** spawn any unit for any team, set credits, toggle fog, change speed, kill all and reveal the map.
- **Cheat console (dev builds only):** `credits 10000`, `spawn rail_tank 20 team2`, `win`, `reveal`.
- **Crash and exception log:** written to file and shown on screen in dev builds.
