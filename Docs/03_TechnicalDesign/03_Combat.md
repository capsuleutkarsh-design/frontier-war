# 03 · Combat: targeting, weapons, damage

---

## 1. Targeting (every 4 ticks, i.e. 5 times per second)

For each unit with weapons, and for each weapon mount:
1. **Keep the current target** if it's still alive, visible to my team and within range (+10% to stop flicker).
2. Otherwise query the SpatialHash for enemies within `range`, filtered by:
   - The weapon's `targets` flags match the target's layer (Ground, Air, Naval, Submerged, Structure).
   - The target is **visible** to my team (fog/stealth rules, `05_Vision`).
   - The target is outside `minRange`.
3. **Score** each candidate and pick the highest:
```
score = 100 × typeBonus(weapon.vsArmour[target.armour])   // prefer what I'm good against
      + 40  × (target is attacking me or my nearby allies)
      + 30  × (1 – target.hp / target.maxHp)              // finish weak targets
      – 0.5 × distance
      + 25  × (target is a unit, not a structure)         // units first, buildings later
      + 200 × (target == player's forced target)
```
4. Order rules:
   - **Attack order:** only that target (other turrets on the unit may still pick their own).
   - **Hold position / attack-move:** auto-target.
   - **Move (force):** turrets still shoot while moving if `canFireWhileMoving`, but the unit never stops.
   - **Guard/idle:** auto-target, and chase up to 1.5× vision, then return.

## 2. Turrets

- Each WeaponMount has a turret angle. Each tick it rotates toward the target at `turretTurnRate`.
- It can fire when the angle error is below 5° (or 30° for missiles).
- `arcDegrees` < 360 means the turret is limited to a cone around the hull's facing. If the target is outside the cone, the **hull turns** (if the unit is idle) or the weapon simply can't fire.
- The view rotates the turret model to the sim angle (interpolated).

## 3. Firing and projectiles

When cooldown ≤ 0, the turret is aimed and the target is valid: **fire** and set cooldown = `reload`. With bursts, fire `burst` shots, `burstGap` apart.

| Projectile | Sim behaviour | Used for |
|---|---|---|
| **Instant** | Damage is applied right away and the view draws a tracer | MGs, light guns (cheap, no projectile object) |
| **Bullet / Shell** | Travels in a straight line at speed. Hits the target (or the miss point) on arrival. | Cannons, rail (very fast) |
| **Arc (artillery)** | Aims at the target's position **when fired** (or a lead position), flies a ballistic arc, and does splash on impact. It can miss moving units. | Howitzer, mortar, rockets |
| **Missile (homing)** | Follows the target, can be shot down by interceptors, and loses lock if the target goes stealthy | SAM, AT rockets, missile ships |
| **Beam** | Damage over time while held on target, with no travel time | Arc/laser-style weapons, flame (short cone) |
| **Chain** | Hits the target, then jumps to `chainCount` more enemies within `chainRange`, losing 30% each jump | Arc Mech, Arc Turret |
| **Torpedo** | Travels on the Naval/Submerged layer only. Blocked by land cells. | Subs, destroyers |
| **Strategic missile** | Special: see section 7 | Missile Silo |

- **Accuracy:** roll once per shot. A miss lands at a random point 2–6 m from the target, and still does splash.
- Projectiles are pooled plain C# objects in a list (no GameObjects in the sim). The view draws them with pooled effects.
- **Max projectiles alive:** 2,000. If the limit is reached, the oldest instant-style effects are skipped visually; the sim is never skipped.

### Point defence (projectile interception)
The study found that a "laser defence" which shoots down incoming projectiles is a key counter mechanic. Ours:
- A weapon with `intercepts = true` (the **Missile Interceptor**, the Fortified Drill's point-defence gun, and the AA Cruiser's secondary) targets **enemy Missile and Arc projectiles** within its range, instead of units.
- Each projectile has `interceptHp` (missile 1, rocket 1, heavy shell 3). Each intercept shot removes 1.
- **Instant, Beam, Chain and Bullet weapons can't be intercepted.** That's the counter-play: a turtle with interceptors loses to rail tanks and arc mechs.

## 4. Damage

One function, `DamageSystem.Apply(target, amount, source)`:
```
amount × vsArmour[target.armour]
       × (1 + 0.1 × source.rank)
       × (1 – cover)            // infantry in forest 0.4, units behind a Shield Dome 0 (the shield takes it instead)
→ shield first (if shield > 0), remainder to hp
→ splash: everyone in splashRadius takes amount × falloff(distance) (linear to 25% at the edge); experimentals take ×0.5 splash
→ friendly fire: off by default. If on, splash hits own units at 50%.
```
- **Shields:** regen starts after `shieldDelay` (5 s) with no damage. EMP sets shield = 0 and stops regen for 8 s.
- **Cover:** only Infantry in Forest cells (40%), plus garrison later if added (not at launch).
- **Damage numbers** are not shown by default (a debug toggle exists). Health bars show it instead.

## 5. Death, wrecks and salvage

When hp ≤ 0:
1. Raise `UnitDied(unit, killer)`. The view plays the explosion and death animation.
2. Give the killer XP (`01_Data`, veterancy).
3. Spawn a **wreck** (ground and naval units over shallow water only):
   - It blocks only units with radius < 2 m (infantry) a little, through extra cost on the cell.
   - Salvage value = 25% of the unit's cost. Lifetime is 180 s, then it fades.
4. **Transports** drop their cargo (see movement). **Buildings** leave rubble, which turns their cells into Rough for 60 s and then clear.
5. Aircraft crash at their position and do small splash (10% of max HP) on the ground.

## 6. Repair

- A Constructor or Forge Mech with a Repair order heals **2% of the target's max HP per second**, costing 15% of the unit's cost per 100% repaired.
- A Repair Bay heals units parked on it by 4% per second, free.
- The Repair Truck heals vehicles within 15 m by 1.5% per second, free.
- Units can't be repaired while they've taken damage in the last 3 seconds (stops "repair tanking").

## 7. Strategic missile (super-weapon)

1. The Silo builds a missile (6,000 credits, 90 s). It can store 1.
2. Player picks a target point, and the launch happens.
3. **All players get a 10-second warning** and see a red target circle.
4. Missile Interceptors within 300 m of the target fire automatically (each holds 2 interceptors and needs 60 s per reload). Each interceptor has an 80% chance to destroy the missile.
5. Impact: 60 m radius. Damage is 5,000 at the centre, falling to 20% at the edge, applied with armour multipliers. Fog is revealed around the impact for 10 s.

## 8. Support powers (Command Points)

- **Command Points (CP):** +1 per 3,000 credits of enemy value destroyed, and +1 per 2 minutes per 4 held deposits. Max 10.

| Power | CP | Effect | Cooldown |
|---|---|---|---|
| Recon Sweep | 2 | Reveal a 120 m radius for 15 s | 60 s |
| Supply Drop | 3 | +1,500 credits | 120 s |
| EMP Strike | 5 | 80 m radius: stun units and turrets for 8 s, shields to 0 | 180 s |
| Air Strike | 6 | 3 bomb runs along a 200 m line after a 4 s warning | 180 s |

## 9. Tests

- Light Tank ×3 against Rail Tank ×1 (similar cost): the Rail Tank wins, with 30–60% HP left.
- 20 SAM Trucks against 20 Light Gunships: the SAMs win and lose fewer than 8.
- An artillery shell aimed at a moving buggy misses most of the time. Aimed at a building, it always hits.
- A stealth bomber is untargetable until a Sensor Tower is in range.
