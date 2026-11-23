# Rusted Warfare - System Study (for Frontier War)

Research date: 2026-11-23. Purpose: understand HOW each system works, so Frontier War can have similar mechanics with its own original content.

**Sources used**
- (A) Official unit modding reference (1354 keys). This is the main source. Keys show what the engine can really do.
- (B) Fan wiki (rustedwarfare.fandom.com): Builder, Combat Engineer, Command, Extractor, Fabricator, Turret, Nuke Launcher, Experimental Units, Spider, Modular Spider, Difficulty Modes, Hovertank, Amphibious Units, Landing Craft, Carrier, Scout, Watch Tower, Custom Maps, and more. (Note: there is no "Laser Shield" page and no "Mech engineer" page with its own content; the second one copies Combat Engineer.)
- (C) corrodinggames.com and the Steam page (feature lists).
- The modding dump has **no map reference and no team reference section**. Map and settings facts below come from the wiki and general knowledge, and are marked **(uncertain)** where I could not confirm them.

Marks used: **(uncertain)** = not confirmed in sources. "RW" = Rusted Warfare.

---

## Summary

1. RW is a classic "Total Annihilation style" RTS: one main money (credits), builders that place buildings, factories with queues, and tiers T1-T3 plus experimental units.
2. The engine is very **data-driven**: every unit is a text file with sections (core, movement, attack, turrets, projectiles, actions). Almost all behaviour is switches and numbers, not code.
3. The strongest idea is the **action + logic system**: buttons, upgrades, auto-triggers and timers are all one "action" object with a price, a build time, conditions and outcomes.
4. Combat is simple but deep: HP + shield + optional armour, turrets with own rotation and target filters (air / land / underwater), many projectile types, and a "laser defence" that shoots down incoming projectiles using energy.
5. Weak points are the AI (just build priorities + income bonus), pathfinding, simple UI on mobile, and little content. Frontier War should copy the data-driven core and fix these.

---

## 1. Economy / resources

**How it works**
- One global team resource: `credits`. Income comes from units/buildings with `generation_credits` (amount added every `generation_delay` = 40 ticks, so about 1.5 times per second at 60 ticks/s) **(tick rate uncertain, reference says moveSpeed 1 = 60 px/s, so 60 ticks/s is likely)**.
- Income can be turned on/off with a condition (`generation_active`). So "income only when powered" or "only when not damaged" is possible.
- Wiki numbers show the design: an extractor pays back much faster than a fabricator. Cost per +1 credit/s: Extractor T1 ~100, T2 ~280, T3 ~500, Overclocked ~800; Fabricator ~1000. So: cheap, map-locked income first, expensive build-anywhere income later.
- Some units have a small built-in income (a support engineer unit +1/s, a carrier ship +8/s).
- Mods can add more resources:
  - `global_resource_NAME` = team-wide money (like gold, oil). Shown on top bar, with colour, icon, position.
  - `resource_NAME` = local resource stored **inside one unit** (like ammo, cargo).
  - Built-in local values: `energy`, `ammo`, `hp`, `shield`, plus 32 boolean flags per unit (`setFlag`).
- A price can mix resources: `price: credits=500, gold=5`.
- `streamingCost` = pay over time while building (like Supreme Commander). Build pauses if money runs out. Normal mode = pay full price at start.
- `borrowResourcesWhileAlive` / `WhileBuilt` = "population/supply": a resource is taken while the unit lives and given back when it dies. Used for houses / supply caps.
- Harvesting: neutral "resource node" units with `resourceRate`; harvester units with `canReclaimResources` collect them and look for the next node with the same tag in a search range. Limit on how many can mine one node at once.
- AI handicap: actions can add resources scaled by AI difficulty (`addResourcesScaledByAIHandicaps`).

**Takeaway for Frontier War**
- Keep: one main currency + map-locked high-yield income + build-anywhere low-yield income. Clear payback numbers make choices easy to read.
- Improve: design for 1-2 extra resources from day one in data (a "global resource" table), even if v1 uses only credits.
- Add: optional "supply/borrow" resource to control unit count instead of a hard cap only.

---

## 2. Construction & builders

**How it works**
- A unit is a builder if `isBuilder: true`. Builders have a "nano" beam with `nanoRange` (default 85 px) and speeds:
  - `nanoBuildSpeed` (build), `nanoRepairSpeed` (repair, default 0.2), `nanoReclaimSpeed` (reclaim normal units), `nanoUnbuildSpeed` (reclaim unfinished buildings), `resourceReclaimMultiplier`.
  - Separate ranges allowed for repair and reclaim, and "melee" mode (must touch target).
- Each unit has `buildSpeed` (time, for example 3s). Real time = target build time divided by builder speed **(the reference says "may multiply"; exact formula uncertain)**.
- **Assisting**: many builders on one project stack speed (wiki: "builders can be stacked... rapidly speeding up the progress"). Same for repair. Stacked repair can make a building "almost indestructible".
- Builders can be limited to repairing only buildings, only units, or only units with tags. `autoRepair` = repair nearby damaged allies without orders.
- `isUnrepairableUnit` blocks repair.
- **Placement rules**:
  - Buildings use a tile `footprint` (blocks movement) and a separate `constructionFootprint` (space kept free from other buildings). Tile = 20 px.
  - `placeOnlyOnResPool` = must be on a resource pool (extractors).
  - `placementRule_NAME` = search for units with tags near the spot; set min/max count. Example: "must be near a power node", "cannot be within X of another turret". Rules can be grouped (any one passes). Each rule has a "cannot place" message.
  - `extraBuildRangeWhenBuildingThis` = builder can reach further (for water buildings from shore).
- `selfBuildRate` = building grows by itself when placed without a builder. `dieOnConstruct` = the builder is used up (deploy-type units).
- Unfinished buildings: semi-transparent while built, smaller sight range (`fogOfWarSightRangeWhileNotBuilt`), can be reclaimed back.
- Reclaim of normal units gives credits back **(refund % uncertain)**. Wrecks: `image_wreak` sets the dead image; **I found no key that makes wrecks reclaimable for value**, so wrecks look cosmetic **(uncertain)**.

**Takeaway for Frontier War**
- Keep: nano-range building, stacking assist, auto-repair, footprint + reserved space, tag-based placement rules.
- Improve: show the build-time formula to the player (e.g. "3 builders: 12s"), and limit stacking with diminishing returns so 30 builders do not make instant bases.
- Decide early: do wrecks give resources? (A clear "yes, 25% of cost" is a nice extra mechanic.)

---

## 3. Production queues & factories

**How it works**
- A factory is any unit with `canBuild_NAME` sections. Each entry lists what it can build, its button position, tech colour, and can override the price.
- Each build button can be **locked or hidden by a condition** with a message, up to 3 different lock reasons (e.g. "Needs 2 Barracks"). This is how tech requirements work.
- Units come out at an exit point (`exit_x/y`, direction, move-away distance 70 px).
- `nanoFactorySpeed` multiplies factory build speed (the carrier builds air units 2x faster).
- **Rally**: adding `setRally` to a build list makes a rally button. New units get a "move away" waypoint and then go to the rally point.
- **Queue**: actions and units share one queue per unit. Keys: `allowMultipleInQueue`, `highPriorityQueue` (skip the line), `canPlayerCancel`, `refundAllQueuedItems`. Queue progress bar shown on unit.
- **Repeat**: **not found in the modding reference** (it is a UI/engine option). I believe there is a repeat/loop toggle **(uncertain)**.
- `transportUnitsKeepBuiltUnits` = built units stay inside (carrier-style).
- **Factory upgrades**: a factory upgrades by an action with `convertTo` the T2 version, with price and build time. Upgrades keep HP ratio and properties. The AI uses `ai_upgradePriority`. A better engineer can place the T2 factory directly (same total price, but faster than build + upgrade).
- Five factory families in vanilla: land, air, sea, mech, experimental.

**Takeaway for Frontier War**
- Keep: one queue per factory shared by units and upgrades; lock messages that explain why.
- Improve: add clear "repeat", "queue 5/10 at once", and "pause" buttons; let players queue on several factories at once (spread the queue).
- Keep factory "convert to T2" upgrades, but the factory should keep working or clearly show it is busy.

---

## 4. Tech / upgrades

**How it works**
- `techLevel` 1-3 mostly sets the button colour. Real tech gates are:
  1. Which factory tier can build the unit (T1 factory -> T2 factory via upgrade).
  2. Lock conditions on build buttons (e.g. "needs a unit with tag X on team").
- **Upgrade = conversion**: the unit becomes another unit type (`convertTo`). Turrets T1 -> T2 -> T3, or branch (turret -> artillery turret OR flame turret; extractor T3 -> Reinforced OR Overclocked).
- `whenBuilding_temporarilyConvertTo` = become a different form while an action runs (deploy mode).
- **Team-wide research**: `addGlobalTeamTags` gives the whole team a tag. Other units check `self.globalTeamTags()` in conditions. This is how mods do "research once, unlock for all".
- `setUnitStats` changes stats live without converting: maxHp, hp, shield, shieldRegen, energy, armour, mass, shoot delay / damage multipliers, moveSpeed, turn speed, attack range, sight range, nano range, self regen, height, factory speed.
- Vanilla has **no research tree**; all upgrades are per-building conversions.

**Takeaway for Frontier War**
- Keep: per-building tier conversions with branch choices (e.g. defence vs income). Simple and easy to read on mobile.
- Add: a small team-wide research list (global tags + stat modifiers). RW only has this in mods.
- Avoid: too many tiers per building. 3 tiers + 1 branch is enough.

---

## 5. Units core stats

**How it works**
- Required: `name`, `maxHp`, `price`, `mass`, `radius`.
- `mass` = how hard to push in collisions. `radius` = selection and collision circle. Also `displayRadius`, `uiTargetRadius`, `softCollisionOnAll`, `disableAllUnitCollisions`.
- `selfRegenRate` = passive HP repair.
- **Shield**: `maxShield`, `shieldRegen`, can start at zero. Shield takes damage first. Projectiles can do more/less to shields (`shieldDamageMultiplier`), go through shields (`shieldDefectionMultiplier`), or hit only shields (`hullDamageMultiplier: 0` = EMP).
- **Armour**: flat value removed from each hit, with a minimum damage kept (default 1). Projectiles can ignore part of armour. Vanilla units do not use armour.
- **Energy**: `energyMax`, `energyRegen`, starting %. Used as ammo for turrets, laser defence and actions. Option: when empty, weapons stop until **fully** recharged (`energyNeedsToRechargeToFull`). `dieOnZeroEnergy` exists.
- **Unit memory** (v1.15): each unit can have custom variables (bool, number, unit ref, string, arrays). Example: experience points.
- Tags: free text labels used everywhere (targeting, repair, placement, damage modifiers).
- Unit cap: buildings are not counted by default (`ignoreInUnitCapCalculation`).
- Sight: `fogOfWarSightRange` in tiles (default 15).

**Takeaway for Frontier War**
- Keep: HP + shield + energy as three separate bars. Tags on everything.
- Improve: use armour in the base game (RW does not), but keep it simple (flat or % reduction by damage type), and show it in the info panel.
- Keep collision mass so heavy units push light ones.

---

## 6. Movement types

**How it works**
- One `movementType` per unit: `NONE`, `LAND`, `AIR`, `WATER`, `HOVER`, `BUILDING`, `OVER_CLIFF`, `OVER_CLIFF_WATER`.
  - LAND: ground only. WATER: water only. HOVER: land + water (not lava, not cliffs). AIR: anywhere. OVER_CLIFF: land + cliffs (walkers). OVER_CLIFF_WATER: land + cliffs + water (the big spider; "all terrain except lava").
  - Underwater units (submarines) are WATER units with a negative height / underwater layer **(exact key uncertain)**. Weapons must have `canAttackUnderwaterUnits`. Torpedoes use `canAttackNotTouchingWaterUnits: false` = only hit things touching water.
- Lava is a "liquid" tile. Checks exist: over water, over liquid, over cliff, over passable tile of a type.
- Movement stats: `moveSpeed` (1 = 60 px/s = 3 tiles/s), acceleration, deceleration, `maxTurnSpeed`, turn acceleration, reverse speed % (0.6 default; units reverse for short moves), sliding mode (hover drift), `moveIgnoringBody` (strafe, for ships/air).
- Air: `targetHeight` (default 35), height drift, `landOnGround` when idle, `slowDeathFall` for big aircraft, falling acceleration.
- Height is a real value: units can change height by action (`setHeight`), so a unit can "take off" or "dive".
- Formations: `joinsGroupFormations` default true (units take spaces around the target point).
- **Transports**: `maxTransportingUnits` slots; each unit uses `transportSlotsNeeded` (experimentals ~5). Filters by tag or movement type; by default only LAND units. Default unload rule: not over liquid and not moving. Options: heal passengers, passengers die with transport (default yes), unload delay, transports inside transports blocked by default. A landing craft carries 4.
- Turrets can `unloadUpToXUnitsAndGiveAttackOrder` (drop pods / launch fighters). Projectiles can teleport the shooter or unload units at the hit point.
- Attachments: units can be stacked onto other units (modules, turrets, riders). Damage can go to the parent.

**Takeaway for Frontier War**
- Keep: a small, fixed set of movement layers (land, hover, naval, sub, air, all-terrain walker) with a passability table per tile type. Easy to understand.
- Improve: pathfinding. Build one flow-field/nav grid **per movement type** and cache it. RW players complain about units getting stuck.
- Keep transport slot sizes; add a clear "unload here" and "load into nearest" on mobile.

---

## 7. Turrets & weapons

**How it works**
- Up to 30 `turret_NAME` per unit. Each turret: position, own turn speed (with acceleration), idle direction, idle spin, idle sweep (look left/right), reset when idle.
- Turrets can attach to another turret (turret on turret) and `slave` (share aim and cooldown, for multi-barrel guns).
- Firing rules: `delay` (cooldown), `warmup` (charge time before shot, with charge graphic), warmup that does not reset (minigun spin-up), `canAttackMaxAngle` (must face target within 5 degrees; 181 = missiles fire any direction), `limitingAngle` (arc, e.g. side guns of a ship), `limitingRange` and `limitingMinRange` (artillery dead zone).
- `isFixedFiring` = whole body must face the target (unit stops to aim). `turretMultiTargeting` = each turret picks its own target.
- Firing cost: `energyUsage` or `resourceUsage` (credits, energy, hp, shield, ammo).
- **Attack permissions** (unit level, then each turret can only narrow): `canAttack`, `canAttackFlyingUnits`, `canAttackLandUnits`, `canAttackUnderwaterUnits`, only touching water, only/without tags, plus any condition (`canAttackCondition`).
- `maxAttackRange` on the unit; `isMelee` adds both radii to range.
- Alternate projectile when a condition is true (e.g. different ammo at low HP).
- **Projectile types** (all are flags on one projectile object):
  - Instant hit (`instant`), laser or lightning drawing, beam (`instantReuseLast` keeps one projectile alive = continuous beam, with sweep).
  - Homing (default: follows target; `turnSpeed` limits homing so it can miss), retarget if target dies, retarget in flight (flak).
  - Ground target / arcing: `targetGround`, `ballistic`, gravity + initial vertical speed, random spread (`targetGroundSpread`, for flamethrowers and artillery).
  - Area damage with radius and falloff (or no falloff), expanding blast wave (nuke), from edge of unit.
  - Split / chain: spawn more projectiles on create (shotgun), on hit (cluster/shrapnel), on end of life; recursion limit stops infinite chains.
  - Push / pull force on hit. Convert hit units to your team (capture gun).
  - Lead targeting (aim ahead of moving targets), can be turned off.
  - Damage multipliers vs buildings, vs shields, "mutators" vs tags (e.g. x2 vs "armored").
- **Target priority**: `ai.notPassivelyTargetedByOtherUnits` and `lowPriorityTargetForOtherUnits` (walls). `canNotBeDirectlyAttacked`, `canOnlyBeAttackedByUnitsWithTags`. Beyond this, picking seems "nearest valid target" **(uncertain)**. Wiki: the command building "can only attack one foe at a time and cannot multitask between air and ground".

**Takeaway for Frontier War**
- Keep: turret as its own object with turn speed, arc, min/max range, and target filter. This creates real counters (artillery dead zone, AA vs ground).
- Keep: one projectile class with flags (instant, homing, arcing, area, split, beam). Very powerful for little code.
- Improve: add a real target priority score (threat, can-hit-back, low HP, unit class) and show it; RW units often shoot walls or the wrong thing.

---

## 8. Combat rules

**How it works**
- Damage order: shield first (with multipliers), then armour reduction, then HP. `directDamage` hits one unit; `areaDamage` hits all in radius (with falloff).
- Area damage: air and land hit separately unless `areaHitAirAndLandAtSameTime`; underwater only if `areaHitUnderwaterAlways`.
- **Friendly fire**: off by default. `friendlyFire` lets area weapons hurt your own team (not allies). Nukes use it.
- **Laser defence**: a turret with `laserDefenceEnergyUse` shoots down incoming projectiles, paying energy per shot. Each projectile has `deflectionPower` (energy needed; -1 = cannot be stopped, e.g. flames). Instant weapons (lasers, lightning) cannot be stopped. When energy is empty it must recharge. Counter = lasers/lightning, or many shots at once.
- Shield visuals: show only on hit, custom images.
- Low HP shows fire and smoke.
- **Death**: explode effect by size, scorch mark, debris bits, wreck image, spawn units on death (e.g. pilots, eggs; can go to the killer's team), fire a turret at self on death, `nukeOnDeath` (experimentals and a carrier explode like a nuke; can be disabled if nukes are off). Transported units die with the transport by default.
- `disableDeathOnZeroHp` + trigger = custom death (e.g. become a wreck unit that can be rebuilt).
- Kill counter per unit (`self.kills()`), last attacker known.
- Capture: neutral units are captured by touching them (`stayNeutral` stops it).

**Takeaway for Frontier War**
- Keep: shield -> armour -> HP order; laser defence vs projectile "deflection power" is a great rock-paper-scissors.
- Keep friendly fire only for big area weapons. Show it clearly.
- Improve: add simple veterancy (kills -> small bonus), RW only does it through mods.

---

## 9. Vision, fog, radar, stealth, detection

**How it works**
- Each unit sees `fogOfWarSightRange` tiles (default 15). Scouts and a watch tower building have very long sight.
- Game setting has fog modes. Wiki: scouts are "not as effective when the fog is permanently cleared upon being discovered, and completely useless if there is no fog". So modes are likely: none / reveal once (map stays known) / line of sight **(names uncertain)**.
- Projectiles can reveal fog on explode or always be visible in fog. Nukes show on the minimap when fired.
- Stealth: `isVisibleToEnemies: false` + `showOnMinimapToEnemies: false`. The engine has **no real detection/radar system** in the reference; mods fake it with conditions and tags.
- Minimap: units can be hidden from it.

**Takeaway for Frontier War**
- Keep: sight range per unit, long-range scout and tower.
- Add: a real radar (shows blips, not unit type) and a stealth + detector pair. RW lacks these; they add strategy.
- Use "explored but not visible" grey fog (shows buildings last seen).

---

## 10. AI

**How it works**
- The AI is a **priority list**, not a planner. Hints in each unit's `[ai]` section:
  - `buildPriority` 0-1 (vanilla: first land factory 0.8, air factory 0.48, first turret 0.47).
  - `noneInBaseExtraPriority`, `noneGlobalExtraPriority` (boost if you have none).
  - `recommendedInEachBaseNum` + priority if unmet; `maxGlobal`, `maxEachBase`.
  - `upgradedFrom` (count T1 and T2 together), `ai_upgradePriority` (default 0.06).
  - `useAsBuilder`, `useAsTransport`, `useAsHarvester`, `disableUse`, harvester counts.
  - Action hints: `ai_isHighPriority`, `ai_isDisabled`.
- A "base" = area around an extractor or spawn point. The AI expands by grabbing resource pools.
- Difficulty mostly changes **income** (multiplier): Very Easy 0.357, Easy 0.643, Medium 1.0, Hard 1.357, Very Hard 1.786, Impossible 3.643. Higher levels are "more aggressive" too.
- Players say the AI is repetitive; mods exist only to improve it.

**Takeaway for Frontier War**
- Keep: data hints on units (priority, max per base, role) so new units work with AI without code.
- Improve: add a simple strategic layer: scout -> count enemy unit types -> build counters; attack in waves with a target; retreat when losing; defend expansions. Make difficulty change decisions and reaction time, not only income.
- Avoid: "cheat income" as the main difficulty knob; players notice it.

---

## 11. Actions / abilities & triggers

**How it works**
- `action_NAME` is the universal ability object. It is a button (or hidden). It has:
  - **Requirements**: price (any resources), `isActive`, `isVisible`, `isLocked` + 3 messages, cooldown.
  - **Queue behaviour**: build time, high priority, can't cancel, can't move while running, rotate to a direction, play animation, temporary convert.
  - **Outcomes** (many): convert to unit, add/set resources, set stats, set memory, spawn units / projectiles / effects, fire a turret at ground (player picks a target), teleport, add waypoints (move, attackMove, guard, loadInto, loadUp, attack, reclaim, repair, touchTarget, build, follow, setPassiveTarget), load/unload transports, add/remove attachments, add/remove tags, team tags, change team, take resources from nearby units, convert one resource to another, show messages, play sounds, add cooldowns, send a message to another unit.
  - **Chaining**: `alsoTriggerAction`, `alsoQueueAction`, `otherwiseTriggerAction` (else), repeat N times (loops).
- **Auto-triggers**: `autoTrigger: <condition>` fires the action instantly when the condition is true (ignores price). Cooldown default 1s. Check rate every frame / 4 / 8 frames.
- **Events**: created, completeAndActive, destroyed, killedAnyUnit, queuedUnitFinished, queueItemAdded/Cancelled, teleported, touchTargetSuccess, newWaypointGivenByPlayer, teamChanged, transport in/out, tookDamage (by projectile tag), newMessage, attachmentRemoved.
- **Logic language**: `if`, and/or/not, comparisons, maths, functions on units (hp, energy, isMoving, isFlying, isOverWater, kills, timeAlive, customTimer, number of units with tag in range, team victory...), unit references (self, parent, attacking, lastDamagedBy, nearestUnit), markers, random numbers.
- **Timers**: `self.customTimer()` + `resetCustomTimer`; `timeAlive`; `hasTakenDamage(withinSeconds)`.

**Takeaway for Frontier War**
- Keep: ONE ability object for upgrades, abilities, deploy modes and scripted behaviour. This is the best part of RW.
- Simplify for v1: use ScriptableObjects with a list of "Condition" and "Effect" components (no text language). Add events: OnCreated, OnDamaged, OnKill, OnDeath, OnTimer.
- Avoid: per-frame checks on thousands of units; use events and slow tick rates (RW warns about CPU cost).

---

## 12. Experimentals

**How it works**
- Built in an experimental factory (one is a ship from the sea factory). Very expensive, late game, "boss" units.
- Tagged `experimental: true`: special zoomed-out icon and end-game stats. Own draw layer.
- Common traits: very high HP (top unit ~10,000 HP; a module version +5,000 shield), laser defence, self repair, several weapon types, nuke on death, take ~5 transport slots, can cross more terrain (cliffs + water), some can build.
- Examples of design ideas (not to copy): a walker that is a mobile base, a carrier that builds aircraft 2x faster and has income, a module walker with 6 slots for 11 module types (guns, AA, income, laser defence, shield regen, speed, anti-nuke, short teleport). Modules have own HP and can be rebuilt.
- Weaknesses are built in: slow, cannot hit underwater, weak to instant weapons (lightning/laser bypass laser defence).

**Takeaway for Frontier War**
- Keep: few, clearly special late units with one clear weakness each.
- Use the **module slot** idea (attachments) for one original experimental; it is the most loved design.
- Balance: nuke-on-death must be optional and visible (warning ring).

---

## 13. Super-weapons

**How it works**
- Nuke launcher: very expensive building (wiki: ~$450,000 building, 15,000 HP, each missile ~$11,000 and built in the queue). Fire = action `fireTurretXAtGround` (player taps a target).
- Nuke projectile: tag `nuke`, `nukeWeapon` (shows on minimap to everyone), area blast that expands (`areaExpandTime`), friendly fire.
- Anti-nuke: a turret with `interceptProjectiles_withTags: nuke` and distance/height limits. Each interceptor missile is stocked and costs money (stockpile shown as a number). Experimental ships can carry anti-nukes.
- Game setting can disable nukes; units then lock (`isLockedIfGameModeNoNuke`) and nuke-on-death turns off if set.

**Takeaway for Frontier War**
- Keep: stockpile model (build missile, then fire), global warning, interceptor with stock.
- Add: a long fire delay with a visible target marker so the defender can react.
- Keep the "disable super-weapons" lobby option.

---

## 14. Game modes & settings

**How it works**
- Content: 10 campaign missions, 48 skirmish maps, 5 survival maps, 7 challenge missions, sandbox, level editor, multiplayer (cross-platform, dedicated servers, reconnect, save/load MP, shared unit control, replays with "play from here").
- Skirmish settings (from wiki + reference): AI difficulty (6 levels), fog mode, nukes on/off, starting unit (can pick special units, e.g. an experimental; can "fall from sky"), starting credits, income multiplier, unit cap **(starting credits / income / cap values uncertain)**.
- Neutral team (captured by touch) and "aggressive" team (enemy to all; for creeps/survival).
- Map spawn hooks: spawn neutral units on empty resource pools, at map centre, or at each team spawn.
- Game speed control in single player **(uncertain)**.
- Victory/defeat checks skip untargetable units; logic can test `teamVictory`, `teamWipedOut`.

**Takeaway for Frontier War**
- Keep: skirmish lobby with difficulty, fog, start money, income x, unit cap, super-weapons on/off, start unit.
- Add: survival/waves mode early (cheap content, good for solo dev), and challenge missions with simple goals.
- Defer: online multiplayer until the simulation is deterministic and stable.

---

## 15. Controls / UX on mobile

**How it works** (mostly from play knowledge; reference gives order types)
- Tap to select, drag box to select, select all of a type / all army buttons **(uncertain)**, control groups **(uncertain on mobile)**.
- Order types that exist in the engine (waypoint list): move, attack-move, guard, attack, follow, reclaim, repair, build, load into, load up, touch target, set passive target. **Patrol** is not in the reference list **(uncertain if it exists)**.
- Waypoints queue (orders added in a chain); actions can prepend waypoints.
- Formations: group moves keep spacing (`joinsGroupFormations`).
- **Infinite zoom**: zoom out to a strategic view where units become icons (`icon_zoomed_out`); you can give orders at any zoom.
- On mobile: actions can be "single press" (no confirm); other buttons need a second tap to confirm.
- Range circles shown for selected turrets; health/energy/shield/queue/shot-delay bars on units.
- Sounds per order type; messages and "quick war log" in the corner.

**Takeaway for Frontier War**
- Keep: infinite zoom with icons; it is the key mobile feature for big maps.
- Improve: big, simple order buttons (Move, Attack-Move, Guard, Patrol, Stop, Hold); one-tap "select all army on screen"; easy control groups (bottom bar slots); double-tap to select same type.
- Add: idle builder button and "factory idle" alerts. RW players often lose track of idle builders.

---

## 16. Map system

**How it works** (wiki "Custom Maps" + general knowledge; **mostly uncertain**, no map reference in dump)
- Maps are made in **Tiled** (.tmx). Tile size 20x20 px. Custom tilesets can be embedded (as Base64 PNG).
- Tile layers for ground and items; tiles carry terrain type (land, water, deep water, cliff, lava) which decides passability for each movement type.
- Resource pools are placed tiles/objects; extractors must be built on them.
- Units are placed in a units layer (start units per team, neutral units).
- An objects layer holds triggers for missions (areas, win conditions, spawns, messages) **(uncertain)**.
- Team spawn points; maps support many players.
- The in-game sandbox editor lets players place units and play the result in multiplayer.

**Takeaway for Frontier War**
- Keep: tile grid with terrain types + object layer for resource pools, spawns, triggers.
- Use Unity Tilemap + ScriptableObject map data; or import Tiled files to let players make maps later.
- Make mission triggers use the same condition/effect system as unit actions.

---

## 17. Modding / data format

**How it works**
- Each unit = one `.ini` text file with sections: `[core]`, `[graphics]`, `[attack]`, `[movement]`, `[ai]`, many `turret_`, `projectile_`, `action_`, `canBuild_`, `effect_`, `animation_`, `leg_`, `arm_`, `attachment_`, `decal_`, `placementRule_`, resources.
- Inheritance: `copyFrom` (other files/templates), `@copyFromSection`, `all-units.template` (applies to every unit), variables (`@define`, `${...}` maths at load time), `%{...}` live text.
- `overrideAndReplace` swaps a vanilla unit for a modded one everywhere (factories, maps). `isLocked` removes units.
- Language support per text key. Mod info file with dependencies.
- Mods load at runtime, even during a game; multiplayer sync is automatic. 1000+ Steam Workshop mods; most work on mobile too.
- Why it is powerful: the engine is a set of general systems (turrets, projectiles, actions, conditions). New units need **zero code**. Content growth came from the community, not the developer.

**Takeaway for Frontier War**
- Keep: build every unit from data (ScriptableObjects first, maybe JSON later for mods). Share defaults with templates/inheritance.
- Design the general systems first (turret, projectile, action, condition), then make units from them. This is the fastest path for a solo dev.
- Avoid: a full text scripting language in v1; it is hard to debug. RW added debug tools later for this reason.

---

## Weaknesses to fix

1. **AI**: only build priorities + income cheat. Fix with scouting, counter-building, wave attacks, retreat, expansion defence, and difficulty that changes decisions.
2. **Pathfinding**: units get stuck, groups jam in chokes. Fix with per-layer flow fields, local avoidance, and formation slots.
3. **Performance on phones** with thousands of units: fix with fixed-tick simulation, spatial grid for targeting, pooled projectiles, event-based triggers, LOD icons when zoomed out.
4. **Thin vanilla content**: ~50 units, 10 missions. Fix with fewer units but more distinct roles, a waves mode, and challenge maps.
5. **Target selection** is basic (shoots walls, wrong targets). Add a priority score.
6. **No radar / detection / stealth** in base game. Add simple versions.
7. **Weak mobile UX**: small buttons, hard multi-select, idle units lost. Add idle buttons, select-all-on-screen, clear order bar.
8. **Hidden numbers**: build time formula, armour, counters not shown. Show DPS, range, "strong vs / weak vs" in the info panel.
9. **Visual style is dated**: invest in clear readable silhouettes and team colours (cheap 2.5D works).
10. **Stacked builders** make instant bases and nearly unkillable buildings. Add diminishing returns.

---

## Checklist of every mechanic Frontier War must design

**Economy**
- [ ] Main currency and income tick rate
- [ ] Resource pools on map + extractor building
- [ ] Extractor tier upgrades and final branch choice
- [ ] Build-anywhere low-yield income building
- [ ] Income from HQ and from special units
- [ ] Pay-at-start vs pay-over-time build cost (pick one)
- [ ] Supply / population (borrowed resource) and/or unit cap
- [ ] Refund rules: cancel, reclaim unit, reclaim unfinished building, wrecks
- [ ] Data support for extra resources later

**Construction**
- [ ] Builder unit: build range, build / repair / reclaim speeds
- [ ] Build time formula and assist stacking (with diminishing returns)
- [ ] Building footprint and reserved space grid
- [ ] Placement rules (on pool, near X, not near Y, water/shore)
- [ ] Unfinished building state (HP grows, sight reduced, can be reclaimed)
- [ ] Auto-repair and repair restrictions
- [ ] HQ: income, small production, self-defence weapon

**Production & tech**
- [ ] Factory families and what each builds
- [ ] Production queue: add, cancel, refund, priority, repeat, pause
- [ ] Rally point and exit point
- [ ] Factory tier upgrade (convert) and direct higher-tier placement
- [ ] Build button lock conditions with reason messages
- [ ] Defence tower upgrade paths with branches
- [ ] Optional team-wide research (global tags + stat modifiers)

**Units**
- [ ] Core stats: HP, mass, radius, price, build time, sight
- [ ] Shield (max, regen, start at zero)
- [ ] Armour model and armour-piercing
- [ ] Energy (max, regen, recharge-to-full rule)
- [ ] Self regen
- [ ] Tags for every unit
- [ ] Veterancy / kills
- [ ] Collision and pushing by mass

**Movement**
- [ ] Movement layers: land, hover, naval, submarine, air, all-terrain walker
- [ ] Terrain types and passability table (land, shallow, deep, cliff, lava)
- [ ] Speed, acceleration, turn speed, reverse
- [ ] Air height, landing when idle, crash on death
- [ ] Pathfinding per layer + local avoidance
- [ ] Group formations
- [ ] Transports: slots, size, filters, load/unload rules, death rule
- [ ] Attachments / module slots

**Weapons & combat**
- [ ] Turret object: position, turn speed, arc, min/max range, cooldown, warmup
- [ ] Target filters: air / land / underwater / tags
- [ ] Target priority score
- [ ] Projectile flags: instant, homing, arcing, area, falloff, split, beam
- [ ] Lead targeting and spread
- [ ] Damage pipeline: shield -> armour -> HP, with multipliers (vs building, vs shield, vs tag)
- [ ] Projectile interceptor (laser defence) with energy and deflection power
- [ ] Friendly fire rules
- [ ] Death: explosion size, wreck, spawn on death, nuke on death
- [ ] Capture of neutral units

**Vision**
- [ ] Fog modes (off / explored / line of sight)
- [ ] Sight ranges, scout unit, watch tower
- [ ] Radar blips
- [ ] Stealth and detection

**Abilities & triggers**
- [ ] Ability object: price, cooldown, build time, conditions, effects
- [ ] Effects list (convert, spawn, fire at ground, teleport, add stats, add tags, messages)
- [ ] Event triggers (created, damaged, kill, death, timer)
- [ ] Deploy / temporary mode

**Late game**
- [ ] Experimental factory and 2-4 original experimentals with clear weaknesses
- [ ] Super-weapon: stockpile, fire delay, global warning
- [ ] Interceptor defence with stockpile
- [ ] Lobby option to disable super-weapons

**AI**
- [ ] Per-unit AI hints (priority, max per base, role)
- [ ] Strategic AI: expand, scout, counter, attack waves, retreat
- [ ] Difficulty levels (decisions + optional income bonus)

**Modes, maps, UX**
- [ ] Skirmish lobby settings (difficulty, fog, money, income, cap, start unit)
- [ ] Survival / waves mode
- [ ] Campaign / challenge missions with triggers
- [ ] Map format: tiles, terrain types, pools, spawns, trigger areas
- [ ] Neutral and aggressive (creep) teams
- [ ] Win / lose conditions
- [ ] Mobile selection: tap, box, same type, all on screen, control groups
- [ ] Orders: move, attack-move, attack, guard, patrol, follow, stop, hold, repair, reclaim, load, unload
- [ ] Waypoint queue
- [ ] Infinite zoom with strategic icons
- [ ] Idle builder / idle factory alerts
- [ ] Unit info panel (DPS, range, strong vs / weak vs)
- [ ] Data-driven unit definitions with templates/inheritance
