# 04 · Economy, construction, production and tech

---

## 1. Credits

- One number per team: `credits` (float internally, shown rounded).
- **Income** is added every tick: `credits += Σ(income of all owned, finished, alive buildings) × TickDt`.

| Source | Income/s | Rule |
|---|---|---|
| Command Core T1 / T2 / T3 | 15 / 20 / 25 | Always pays |
| Ore Drill L1 / L2 / L3 | 8 / 13 / 20 | Only on an ore deposit |
| Fortified / Relay Drill | 20 | Final L3 choice |
| Overdrive Drill | 32 | Shuts down 30 s after taking more than 25% HP of damage |
| Salvage | one-time | 25% of the wreck's unit cost |

- **Starting credits** (skirmish setting): Low 2,000 · **Normal 4,000** · High 10,000.
- **Spending is gradual.** Construction and production take credits *a little each tick*, in proportion to progress:
```
want    = def.cost × (progressThisTick / 1.0)
paid    = min(want, credits)
credits -= paid
progress += progressThisTick × (paid / want)    // low bank → slower, never "stuck at 99%"
```
- If several things are being built and credits are short, the money is shared equally (each job gets the same fraction). The UI turns the income number red.

## 2. Construction (buildings)

### Placing
1. The player taps a builder, then picks a building in the Build tab, which shows a **ghost** that follows the finger.
2. The ghost is **green** if every footprint cell is valid:
   - The terrain fits `placement`: Ground = Grass/Dirt/Road/Rough; Water = DeepWater; Shore = touching both land and water; OreDeposit = must cover a free deposit cell.
   - No building is on the cell, and there are no enemy units on it.
   - The cell is explored (not black fog).
3. Tap ✓ to place. This creates a **construction site** (a building at 0% progress, HP 10%), and the builder gets a Build order.
4. Several placements can be queued (hold "+"), and the builder does them in order.

### Building
- A builder works when it's within `radius + 8 m` of the site.
- **Build rate:** 1 builder finishes in `buildTime` seconds. Extra builders add speed with diminishing returns:
  `rate = 1 + 0.8 + 0.6 + 0.5 + 0.4 …` (each extra builder adds less, and there's no limit).
- HP grows with progress (from 10% up to 100%).
- **Assist:** any builder given a Guard order on a builder helps with whatever that builder is building.
- **Cancel / delete a site:** refund 100% of the credits spent. **Sell a finished building:** refund 50%.
- **Unfinished sites** don't do anything (no income, no weapons) and can be attacked.

### Upgrading in place
- Drills, factories, turrets and the Command Core upgrade where they stand. Upgrading works like construction: it's progress plus gradual cost, done by the building itself (no builder needed). Builders nearby speed it up by 50% each.
- While upgrading: **drills keep their old income**, **factories can't produce**, and **turrets keep shooting**.

### Repair and salvage
- Repair: see `03_Combat §6`.
- **Salvage a wreck:** a builder within 8 m gains `25% × unitCost` over `unitCost / 200` seconds (a 900-credit tank wreck gives 225 credits in 4.5 s).

## 3. Production (units)

- Each factory has its own **queue** (max 10 items). The UI groups identical units, for example "Light Tank ×4".
- The **first item** builds; the others wait. Build speed is 1× (a factory doesn't stack builders, but T2 factories build 25% faster).
- **Repeat mode:** when an item finishes, it's added again to the end of the queue. There's an optional **credit floor**: don't start a repeat item if the credits are below X.
- **Spawn:** the unit appears at the factory's exit cell and drives to the **rally point** (if set), or 15 m in front of the factory.
  - If the exit is blocked, the unit waits there, and the blocking friendly unit gets a "make room" move.
- **Unit cap:** if `teamUnitCount + 1 > cap`, production pauses and shows "Unit cap" on the queue.
  - Infantry squads count as 1. Experimentals count as 10.
- **Experimental cap:** 3 alive or in production per team (a skirmish setting).
  - **When one starts:** all teams get a warning and a map ping.
- **Air:** aircraft spawn on the airfield's pad. Each Airfield is also a rearm pad (see `02_Movement §7`).

## 4. Tech tiers and requirements

```
Command Core T1 ──3,000──► T2 ──8,000──► T3
   │                        │                │
   ├ Vehicle Plant          ├ Mech Bay        ├ Experimental Forge
   ├ Airfield               ├ Shield Dome     ├ Missile Silo
   ├ Shipyard               ├ Artillery Empl. ├ Missile Interceptor
   ├ Ore Drill L1           ├ Arc Turret      ├ Drill L3 + final choice
   ├ Gun / Flak Turret      ├ Drill L2        └ Carrier (Shipyard T2)
   ├ Sensor Tower           └ factory T2 upgrades (each factory is upgraded separately)
   └ Repair Bay
```
- A building in `requires` must be **finished and alive**. If the requirement is lost (for example the Core drops to rubble), units already built stay, but new T2/T3 items can't start.
- **Upgrades (research):** each factory has 2–3 one-time upgrades (GDD page 8). An upgrade applies to all current and future units of the matching category, by recalculating their stats from `Def + active upgrades`.

## 5. The Command Core

- Every team starts with one. It **can't be built** by players in skirmish (campaign missions may give more).
- It produces: Constructor, Scout Buggy, Rifle Squad, Rocket Squad, Stinger Team.
- It has a weapon: a missile battery (ground or air, one target at a time) with 280 m range.
- **Losing it:** income −15/s, and its tier unlocks are lost until a new Core exists. In skirmish there's a "Core kill = defeat" option.

## 6. Win and lose checks (EconomySystem calls VictorySystem every 1 s)

- Default skirmish: a team is **defeated** when it has **0 buildings** (construction sites count). Units alone, even builders, don't keep a team alive, so hiding a unit can't stall the game (GDD page 5).
- Optional rules: Core-kill, Domination (hold 70% of deposits for 180 s), Score at a time limit.
- Campaign: rules come from the mission's triggers (`09_Missions`).

## 7. Tests

- With 0 credits and income 10/s, a 1,000-credit building finishes in about 100 s (no stall).
- 4 builders on one 60 s building finish in 60 / (1 + 0.8 + 0.6 + 0.5) ≈ 20.7 s.
- Repeat mode with a 1,000 floor stops starting new units when the credits fall below 1,000.
- Cancelling at 50% refunds exactly the credits that were spent.
