# 01 · Data and units

All content is **data**. Code reads the data and never has a unit hard-coded ("if unit is Rail Tank…" is forbidden).
Each unit is described by small building blocks that are reused: **movement type, armour class, weapons and abilities**.

---

## 1. Definition assets (ScriptableObjects)

### UnitDef
| Field | Type | Example (Light Tank) | Notes |
|---|---|---|---|
| `id` | string | `light_tank` | Unique and never renamed (saves use it) |
| `displayName` | string | Light Tank | |
| `category` | enum | Land / Mech / Infantry / Air / Naval / Experimental / Builder | Used by the UI tabs, AI and stats |
| `tier` | int 1–3 | 1 | |
| `cost` | int | 450 | Credits |
| `buildTime` | float s | 12 | At 1 builder or 1 factory |
| `maxHp` | float | 700 | |
| `armour` | enum | Armoured | See table 2 |
| `maxShield` | float | 0 | 0 = no shield |
| `shieldRegen` / `shieldDelay` | float | 0 / 0 | Regen per second, seconds without a hit |
| `hpRegen` | float | 0 | Only Hero rank and some units |
| `radius` | float m | 3.5 | Collision and selection |
| `mass` | float | 30 | Used for push priority (heavy pushes light) |
| `movement` | MovementDef | Ground/Track | See `02_Movement` |
| `speed` / `accel` / `turnRate` | float | 5 m/s, 4, 120°/s | |
| `vision` / `detectRange` | float m | 60 / 0 | `detectRange` > 0 reveals stealth and subs |
| `stealth` | bool | false | Invisible unless detected |
| `weapons` | WeaponMount[] | 1 × cannon | Each mount = a WeaponDef + turret settings |
| `abilities` | AbilityDef[] | – | Build, repair, transport, shield aura… |
| `transportSlots` / `transportSize` | int | 0 / 2 | Slots a transport has / slots this unit uses |
| `canBuild` | BuildingDef[] | – | Builders only |
| `xpValue` | float | 450 | Defaults to cost. XP given to the killer. |
| `wreck` | WreckDef | medium | Wreck model, salvage value (25% cost), lifetime 180 s |
| `view` | prefab ref | `PF_Unit_LightTank` | View only. Sim never touches it. |
| `icon` / `strategicIcon` | sprite | | Build button / strategic-zoom icon |
| `aiRole` | flags | AntiLight, Raider | AI army planner |
| `tags` | string[] | `Tank`, `Ground` | Used by trigger conditions, target filters and AI (see `12_Mechanics_Coverage`) |
| `baseDef` | UnitDef | – | Optional: copy every value from another Def and override only some (for variants) |
| `experimental` | bool | false | Counts towards the experimental cap, global warning |

### WeaponDef
| Field | Example | Notes |
|---|---|---|
| `id`, `damage` | `cannon_light`, 60 | |
| `reload` | 1.6 s | Time between shots (or bursts) |
| `burst` / `burstGap` | 1 / 0 | Shots per burst |
| `range` / `minRange` | 55 m / 0 | Artillery has a `minRange` |
| `targets` | Ground, Naval | Flags: Ground, Air, Naval, Submerged, Structure |
| `projectile` | Instant / Bullet / Shell / Missile / Beam / Chain / Torpedo | See `03_Combat` |
| `projectileSpeed` | 120 m/s | |
| `splashRadius` / `splashFalloff` | 0 / – | Area damage |
| `vsArmour[]` | Infantry 1.2, Light 1.0, Armoured 0.7, Structure 0.6… | Multipliers. The default for any missing entry is 1.0. |
| `accuracy` | 0.9 | 1 = never misses. Misses land near the target. |
| `canFireWhileMoving` | true | Artillery = false |
| `needsLineOfSight` | false | Kept simple: the target must be visible to your team |
| `chainCount` / `chainRange` | 0 / 0 | Arc weapons |
| `fx` | ids | Muzzle, trail and impact effects (view only) |

### WeaponMount (on a unit)
`weapon`, `turretTurnRate` (°/s, 0 = fixed forward), `arcDegrees` (360 = full turret), `offset` (local position for the muzzle effect), `priority` (target type order).

### BuildingDef
Same core fields as a unit (hp, armour, vision, weapons, shield) plus:
`footprint` (cells w×h, 1 cell = 4 m) · `placement` (Ground / Water / Shore / OreDeposit) · `produces` (UnitDef[]) · `upgradesTo` (BuildingDef, cost, time) · `income` (credits/s) · `requires` (Command Core tier, other building) · `powerRadius` none (no power in this game) · `isCore` (Command Core) · `aura` (repair or shield radius).

### UpgradeDef (factory research)
`id`, `cost`, `time`, `building` (where it's bought), `requires` (tier), `effects[]`. An effect is a `{stat, affects (category or unit list), op (add/multiply), value}`. Example: Composite Armour = `{maxHp, Land, ×1.15}`.

### ActionDef: one system for abilities, upgrades and triggers
The Rusted Warfare study found that one general "action" object powers its upgrades, abilities, deploy modes and scripted behaviour. We do the same, as data:
```
ActionDef
  id, displayName, icon
  cost (credits), time (s), cooldown (s)
  showWhen  : Condition[]   (button visible)
  allowWhen : Condition[]   (button usable)
  autoWhen  : Condition[]   (fires by itself, with no button)
  effects   : Effect[]      (what happens)
```
- **Conditions:** `HasCredits(x)`, `TeamHasBuilding(id, tier)`, `HpBelow(%)`, `EnemyWithin(m, flags)`, `IsMoving`, `TimerElapsed(s)`, `UnitCount(id) < n`, `MissionFlag(name)`.
- **Effects:** `ConvertTo(defId)` (upgrade in place), `AddStatModifier(stat, op, value, duration)`, `Spawn(defId, n)`, `GiveCredits(x)`, `SetFlag(name)`, `PlayFx(id)`, `ShieldOn/Off`, `Reveal(radius, s)`.
- **Used by:** factory upgrades, drill upgrades (`ConvertTo drill_L2`), the Overdrive shutdown (`autoWhen HpBelow`), support powers, and mission triggers (`09_Missions`). **One engine, many features, and no special-case code.**

## 2. Armour classes and damage table

| Armour | Typical units |
|---|---|
| Infantry | Rifle Squad, Rocket Squad, Stinger Team |
| Light | Buggy, drones, Strider, Patrol Boat, Light Gunship |
| Armoured | Tanks, mechs, Hover Lander |
| Air | Jets, helicopters, airships (the Air layer is a target flag, *and* this is its armour) |
| Naval | Ships |
| Submerged | Submarines (only `Submerged` weapons can hit them) |
| Structure | Buildings and turrets |
| Experimental | All experimentals (takes reduced splash) |

**Damage formula** (one place in code: `DamageSystem.Compute`):
```
dmg = weapon.damage × weapon.vsArmour[target.armour] × rankBonus(attacker) × (1 – terrainCover(target))
if target.shield > 0: shield absorbs first, and the rest goes to hp
```
Starting multipliers (balance later in a spreadsheet):

| Weapon family → / armour ↓ | MG / flame | Cannon | Rail / AT rocket | Artillery | Flak / SAM | Torpedo |
|---|---|---|---|---|---|---|
| Infantry | 1.5 | 0.6 | 0.3 | 1.2 | 0.2 | – |
| Light | 1.0 | 1.0 | 0.8 | 0.8 | 1.2 (air) | – |
| Armoured | 0.2 | 1.0 | 1.6 | 0.8 | 0.2 | – |
| Air | 0.6 | 0 | 0.5 | 0 | 1.6 | – |
| Naval | 0.3 | 0.8 | 1.0 | 1.0 | 0.2 | 1.5 |
| Submerged | 0 | 0 | 0 | 0 | 0 | 1.5 |
| Structure | 0.3 | 0.8 | 0.8 | 1.6 | 0.1 | 1.0 |
| Experimental | 0.2 | 0.8 | 1.4 | 0.7 | 0.8 (air exp.) | 1.0 |

## 3. Unit roster → data (launch target 47)

The full roster with costs and roles is in the GDD (pages 10–14). In data terms:
- **Builders (4):** Constructor, Naval Constructor, Forge Mech, Air Constructor. They have the `Build` ability plus `canBuild`.
- **Transports (3):** Hover Lander (4 vehicle slots), Tiltrotor (4 small or 1 tank), Carrier (8 jets, which launch automatically).
- **Detectors:** Destroyer, Sea Skimmer, Jammer Drone, Sensor Tower and Relay Drill (`detectRange > 0`).
- **Stealth:** Stealth Bomber and all submarines.
- **Shield units:** Shield Truck (aura), Shield Dome (building), Fortified Drill, Shielded experimentals.

## 4. Balance workflow (so numbers never get lost)

1. All numbers live in the Def assets.
2. The editor tool `Tools ▸ Frontier War ▸ Export Balance CSV` writes every unit's stats to `Docs/04_Production/balance.csv`. Open it in Excel or Google Sheets to compare.
3. Simple checks run in the editor (`DefValidator`): cost > 0, buildTime > 0, every weapon has targets, every unit has a view prefab and icon, no duplicate ids.
4. **The balance rule:** cost ≈ how useful the unit is. Compare "credits of unit A needed to kill credits of unit B" in the Sandbox scene. The counter should win at about 1.5× efficiency.

## 5. Veterancy

| Rank | XP needed (× own cost) | Bonus |
|---|---|---|
| Rookie | 0 | – |
| Veteran | 1× | +10% damage, +10% max HP |
| Elite | 2.5× | +20% / +20% |
| Hero | 5× | +30% / +30%, +1% HP regen per second |

XP is gained when this unit deals the killing hit, equal to the victim's `xpValue`. Damage-share XP is not used (simpler).
