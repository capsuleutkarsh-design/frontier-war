# 02 · Movement and pathfinding

Players hate stuck units more than anything else, and weak pathing is the number one complaint about sandbox RTS games. This system gets the most care.

---

## 1. The map grid

- The map is a grid of **4 m × 4 m cells**. Map sizes: Small 128×128 cells (512 m), Medium 192×192, Large 256×256 (1,024 m).
- Each cell stores: `terrain` (enum), `height` (for visuals and hills), `blocked` (a building on it), `ore` (deposit id, or −1).

| Terrain | Notes |
|---|---|
| Grass, Dirt | normal |
| Road | faster for wheeled and tracked units |
| Forest | slow for vehicles, cover for infantry |
| Rough (rubble, rocks) | slow for vehicles, normal for mechs and infantry |
| Cliff | blocked for almost everything |
| ShallowWater | hover, amphibious and naval-light |
| DeepWater | ships, submarines, hover |
| Lava | blocked except air and the Colossus |
| Bridge | counts as Road; becomes DeepWater when destroyed |

## 2. Move classes

Each unit has one **MoveClass**. A small table gives the **cost** to enter each terrain (0 = cannot enter). This table is the whole "who can go where" rule.

| MoveClass | Grass | Road | Forest | Rough | Cliff | Shallow | Deep | Lava | Used by |
|---|---|---|---|---|---|---|---|---|---|
| Wheeled | 1.0 | 0.7 | 3.0 | 2.5 | 0 | 0 | 0 | 0 | trucks, buggy |
| Tracked | 1.0 | 0.8 | 2.0 | 1.8 | 0 | 0 | 0 | 0 | tanks |
| Legged | 1.0 | 1.0 | 1.5 | 1.0 | 0 | 2.0 | 0 | 0 | mechs |
| Infantry | 1.0 | 1.0 | 1.2 | 1.2 | 0 | 0 | 0 | 0 | squads |
| Hover | 1.0 | 0.9 | 3.0 | 2.0 | 0 | 1.0 | 1.0 | 0 | Skimmer, Hover Lander |
| Naval | 0 | 0 | 0 | 0 | 0 | 2.0 | 1.0 | 0 | ships (small ships only for Shallow) |
| Submerged | 0 | 0 | 0 | 0 | 0 | 0 | 1.0 | 0 | subs, Leviathan |
| Colossus | 1.0 | 1.0 | 1.0 | 1.0 | 1.5 | 1.0 | 1.0 | 1.5 | Colossus Walker |
| Air | ignores the grid | | | | | | | | all aircraft |

**Speed on a cell = unit.speed / cost.** For example, a tank on Forest moves at half speed.

**Unit size rule:** units with a radius over 6 m (big ships, experimentals) use a **clearance map**, so they only take paths at least 2 cells wide. Clearance is computed once at load and after buildings change.

## 3. Pathfinding: two tools

| Situation | Tool | Why |
|---|---|---|
| 1–4 units ordered to a point | **A\*** on the grid (8 directions), then path smoothing | Cheap and exact |
| 5+ units ordered to the same point | **Flow field** | One calculation serves the whole group, which makes it great for 200 tanks |
| Aircraft | No pathfinding. Fly straight, with steering. | Air ignores terrain |

### A\*
- Heuristic: octile distance × the cheapest terrain cost for that MoveClass.
- After finding the path, **smooth it**: remove waypoints you can see past (a line-of-walk check on the grid). Units then drive in straight lines, not zig-zags.
- Budget: at most **2,000 node expansions per path**, and at most **8 paths per tick** (a queue). If a path isn't found within the budget, move toward the closest reached cell.

### Flow field
1. Dijkstra from the **goal cell** over the whole grid, using that MoveClass's costs. The result is an "integration field" (cost to the goal from every cell).
2. Each cell points to its cheapest neighbour. That's the "flow direction".
3. Units sample the direction at their cell (bilinear between neighbours, for smooth curves).
4. **Cache:** keep the last 16 fields keyed by (MoveClass, goal cell, map version). The map version goes up when a building is placed, destroyed or a bridge falls. Old fields are then rebuilt lazily.
5. Cost: 65k cells × Dijkstra takes about 3–5 ms in C#. Spread it over ticks if needed, and Burst-compile it later if the profiler says so.

## 4. Steering (every tick, per unit)

```
desired  = directionFromPathOrFlowField × maxSpeed(cell)
desired += separation(nearby same-layer units, within radius×2.5)  × 1.2
desired += avoidStatic(blocked cells ahead within 2 cells)          × 1.5
vel      = turnTowards(vel, desired, turnRate, accel)               // tanks can't strafe
pos     += vel × dt
```
- **Separation** uses the SpatialHash. Units only push units on the **same layer** (ground, water, air).
- **Heavy pushes light:** separation strength is scaled by `otherMass / (myMass + otherMass)`.
- **Idle units step aside:** when a moving friendly unit is blocked by an idle one, the idle unit gets a small "make room" move to a free nearby cell.
- **Hard collision fix:** after moving, if a unit is inside a blocked cell, push it back to the nearest free cell. Units must never end up inside buildings or cliffs.

## 5. Arriving: formations and goal slots

The problem: 50 tanks ordered to one point all fight over the same pixel. The fix: **every unit gets its own slot.**
1. On a group move order, compute the group centre and the facing (from the centre to the target).
2. Lay out slots in rows behind the target point. Spacing is 2.2 × the largest radius, and the width is about √n.
   - Front row: units with the highest `aiRole` frontline (tanks, mechs).
   - Back rows: artillery, AA and support.
3. Assign each unit the nearest free slot (a greedy match is fine).
4. Each unit's goal is **its slot**, not the click point. The group flow field still uses the click point. A unit switches to A\* for the last 12 m to its slot.
5. **"Keep together" toggle:** all units move at the speed of the slowest one until they arrive.

## 6. Stuck detection (the safety net)

Every 1 second, each moving unit checks whether it moved less than 20% of its expected distance:
1. First fail: **re-path** (A\* from its current cell).
2. Second fail: **nudge**, a short random side-step to a free neighbour cell, then re-path.
3. Third fail: **give up**. Stop the order and mark the unit idle (it shows as idle in the idle-alert badge).
Also: if a goal cell is blocked, move the goal to the nearest reachable cell of the right MoveClass (a BFS from the goal).

## 7. Air movement

- **Helicopters and drones:** fly straight to the goal and hover there, with separation from other aircraft. They can stop in place.
- **Jets:** must keep a minimum speed (60% of max). To attack they fly past the target and loop back, in a "strafe run": approach, fire, overshoot 80 m, turn, repeat. When idle they circle their airfield or the order point.
- **Rearm and repair:** bombers have `ammo` (for example 2 bombs). At 0 ammo, or HP below 30%, they fly to the nearest friendly Airfield or Carrier pad. Each pad services one aircraft at a time: rearm in 6 s, repair 5% HP per second.
- Air units have a height for the view: 0 m parked, 25 m helicopters, 60 m jets, 40 m airships. This is visual only; the sim uses the Air layer.

## 8. Water, hover and transports

- **Ships** use the Naval layer. Shipyards and naval buildings sit on Shore cells.
- **Hover** units use Land + Water cells. They're the "bridge" between domains.
- **Load:** a unit ordered onto a transport paths to it. When it's within 6 m and the transport is stopped, it is hidden and stored in the transport's `cargo` list.
- **Unload:** order the transport to a point. For Hover Landers, the point must be land or shore. They unload onto the nearest free land cells within 12 m.
- **Transport destroyed:** each cargo unit has a 50% chance to survive if over land, and dies if over deep water.
- **Carrier:** holds 8 jets. They launch automatically to attack targets within 400 m of the carrier, and land again to rearm.

## 9. Order types handled by movement

| Order | Behaviour |
|---|---|
| Move | Go to the slot and ignore enemies (*force move*) |
| Attack-Move | Go to the slot, and stop to fight any enemy in weapon range, then continue |
| Attack (unit) | Chase the target until it's in range, then fire. Re-path every 2 s while chasing. |
| Guard (unit) | Stay within 15 m of a friendly unit and attack enemies that come within guard range (vision) |
| Patrol (A↔B) | Attack-move between points forever |
| Hold Position | Never move, only shoot what's in range |
| Stop | Clear orders |
| Waypoints | A queue of the orders above (hold "+" on mobile) |

## 10. Tests (must pass before moving on)

- 200 tanks ordered across a 1 km map through a forest gap: all arrive within 1.3× the time of a single tank, and none are stuck.
- 50 units ordered into a closed base: they stop at the nearest reachable cells.
- Destroying a bridge mid-move: units re-path in under 1 second.
- 300 aircraft circling: 60 FPS on the target phone.
