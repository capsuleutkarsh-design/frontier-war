# 12 · Mechanics coverage: every mechanic decided

This is the checklist from `01_Research/RustedWarfare_Study.md`, with a decision for each mechanic.
**YES** = in 1.0 (the doc says where) · **NO** = deliberately not in the game · **LATER** = after 1.0.

## Economy
| Mechanic | Decision | Where |
|---|---|---|
| Main currency and income tick | YES: credits, added every tick | 04 §1 |
| Resource pools + extractor | YES: ore deposits + Ore Drill | 04 §1 |
| Extractor tiers + final branch | YES: L1–L3, then Fortified / Overdrive / Relay | 04 §1, GDD p6 |
| Build-anywhere low-yield income | NO: replaced by Salvage (avoids "drill spam" turtling) | 04 §2 |
| Income from HQ / special units | YES: Command Core only | 04 §5 |
| Pay at start vs pay over time | YES: **pay over time** | 04 §1 |
| Supply / borrowed resource | NO: unit cap only | 04 §3 |
| Refund rules | YES: cancel 100%, sell 50%, wreck salvage 25% | 04 §2 |
| Extra resources later | LATER: the data allows it, but there's no UI | – |

## Construction
| Mechanic | Decision | Where |
|---|---|---|
| Builder range, build/repair/salvage speed | YES | 04 §2, 03 §6 |
| Build-time formula + assist stacking | YES: diminishing returns | 04 §2 |
| Footprint + reserved grid | YES: 4 m cells | 01, 02 §1 |
| Placement rules (deposit, water, shore) | YES | 04 §2 |
| Unfinished building state | YES: HP grows, no function | 04 §2 |
| Auto-repair / repair limits | YES: no repair within 3 s of damage. Hero rank regen. | 03 §6 |
| HQ income, production, weapon | YES | 04 §5 |

## Production and tech
| Mechanic | Decision | Where |
|---|---|---|
| Factory families | YES: Vehicle, Air, Ship, Mech, Experimental, plus the Core | 04 §3–5 |
| Queue add, cancel, refund, repeat, pause | YES. Pause is a long-press on the queue. | 04 §3 |
| Rally and exit point | YES | 04 §3 |
| Factory tier convert | YES: ActionDef `ConvertTo` | 01, 04 |
| Placing higher-tier buildings directly | NO: always build T1, then upgrade (simpler and readable) | – |
| Locked buttons with a reason | YES: "Needs Command Core T2" | 07 §5 |
| Defence tower upgrade branches | YES: Gun → Heavy Gun only (no branches, to keep it simple) | GDD p15 |
| Team-wide research | YES: factory upgrades apply team-wide by category | 04 §4 |

## Units
| Mechanic | Decision | Where |
|---|---|---|
| Core stats (HP, mass, radius, price, time, sight) | YES | 01 §1 |
| Shields | YES | 01, 03 §4 |
| Armour model / armour-piercing | YES: armour classes × weapon multipliers (no separate flat armour) | 01 §2 |
| Energy (unit mana) | NO: abilities use cooldowns instead | – |
| Self regen | YES: Hero rank and a few units | 01 §5 |
| **Tags** | YES: `tags[]` on UnitDef (for example `Builder`, `Detector`, `Boss`), used by triggers, AI and filters | 01 §1 |
| Veterancy | YES: 4 ranks | 01 §5 |
| Collision and pushing by mass | YES | 02 §4 |

## Movement
| Mechanic | Decision | Where |
|---|---|---|
| Layers: land, hover, naval, sub, air, all-terrain walker | YES: MoveClasses | 02 §2 |
| Terrain passability table | YES | 02 §2 |
| Speed, accel, turn, **reverse** | YES: tracked and wheeled units reverse up to 15 m instead of turning around | 02 §4 |
| Air height, **land when idle**, crash on death | YES: helicopters land when idle at a pad or for 30 s. Jets circle. | 02 §7, 03 §5 |
| Pathfinding per layer + avoidance | YES | 02 §3–4 |
| Formations | YES | 02 §5 |
| Transports | YES | 02 §8 |
| Attachments / module slots | NO: too complex for a solo project | – |

## Weapons and combat
| Mechanic | Decision | Where |
|---|---|---|
| Turret object (turn, arc, ranges, cooldown) | YES. **Warmup:** optional `warmup` seconds (Rail Tank 0.4 s charge). | 01, 03 §2 |
| Target filters | YES: layer flags + tags | 03 §1 |
| Target priority score | YES | 03 §1 |
| Projectile types | YES: 8 types | 03 §3 |
| **Lead targeting** and spread | YES: Shell and Arc aim at a lead position (`target.vel × flightTime × 0.7`). `accuracy` gives the spread. | 03 §3 |
| Damage pipeline + multipliers | YES | 03 §4 |
| Projectile interceptor | YES: point defence with `interceptHp` | 03 §3 |
| Friendly fire | YES: option, off by default | 03 §4 |
| Death: explosion, wreck, spawn on death | YES. Spawn-on-death is an ActionDef with `autoWhen OnDeath` (for example a Hive's drones scatter). | 03 §5 |
| Capture of neutral units/buildings | LATER: missions use `SetOwner` effects instead | 09 §2 |

## Vision
| Mechanic | Decision | Where |
|---|---|---|
| Fog modes | YES: Normal / Explored / Off | 05 §5 |
| Sight, scout units, watch tower | YES: Sensor Tower | 05 |
| Radar blips | YES: attacker pings | 05 §4 |
| Stealth + detection | YES | 05 §2 |

## Abilities and triggers
| Mechanic | Decision | Where |
|---|---|---|
| Ability object | YES: ActionDef | 01 §1 |
| Effects list | YES | 01 §1, 09 §2 |
| Event triggers (created, damaged, kill, death, timer) | YES: `autoWhen` conditions `OnCreated`, `OnDamaged`, `OnKill`, `OnDeath`, `TimerElapsed` | 01 §1 |
| Deploy / temporary mode | NO at launch (an ActionDef could add it LATER) | – |

## Late game
| Mechanic | Decision | Where |
|---|---|---|
| Experimental factory + 5 experimentals with weaknesses | YES | GDD p14, 04 |
| Super-weapon: stockpile, delay, warning | YES | 03 §7 |
| Interceptor stockpile | YES | 03 §7 |
| Lobby option to disable super-weapons | YES | §Skirmish settings below |

## AI
| Mechanic | Decision | Where |
|---|---|---|
| Per-unit AI hints | YES: `aiRole` flags | 01, 06 |
| Strategic AI | YES: managers | 06 |
| Difficulty (decisions first, money last) | YES | 06 §4 |

## Modes, maps and UX
| Mechanic | Decision | Where |
|---|---|---|
| Skirmish lobby settings | YES: see below | – |
| Survival / waves | YES: wave director (see below) | – |
| Campaign / challenges with triggers | YES | 09 |
| Map format | YES | 09 §1 |
| **Neutral team** | YES: team 0 owns bridges, ruins and neutral turrets. It never attacks (unless a mission sets `hostile`). | 09 §1 |
| Aggressive creep team | LATER | – |
| Win/lose conditions | YES | 04 §6 |
| Mobile selection | YES | 07 §2–3 |
| **Orders:** move, attack-move, attack, guard, patrol, **follow**, stop, hold, **repair**, **salvage**, **load**, **unload** | YES. Follow = Guard without attacking. Repair, salvage, load and unload are context taps (builder on damaged unit / wreck, unit on transport). | 02 §9, 07 |
| Waypoint queue | YES | 02 §9 |
| Strategic zoom icons | YES | 07 §1 |
| Idle builder / factory alerts | YES | 07 §5 |
| Unit info card (DPS, range, strong/weak vs) | YES: long-press | 07 §5 |
| Data defs with inheritance | YES: a `baseDef` field copies values from another Def (the editor shows overrides in bold) | 01 |

---

## Skirmish settings (lobby)
Map · players 2–8 (teams or free-for-all) · each AI's doctrine and difficulty · starting credits Low/Normal/High · **income multiplier ×0.5 / ×1 / ×2** · unit cap 100–600 · experimental cap 0–5 · **super-weapons on/off** · fog Normal/Explored/Off · start tech T1/T2/T3 · win rule (Destroy all / Core kill / Domination / Score) · weather and night on/off · game speed.

## Survival mode (wave director)
- The player starts with a Core, 2 Constructors and 6,000 credits on a survival map. Enemies come from 2–4 edge spawn regions.
- **Wave n** has a budget of `600 + 450 × n^1.25` credits. The director buys units from the enemy roster, matched to the wave's theme (a cycle of Ground → Mixed → Air → Ground+Artillery → Boss wave every 10, with an experimental).
- There's a 60 s break between waves, which the player can skip. The score is waves survived × efficiency.
- The waves use the same AI ArmyManager (attack-move to the player's base).
