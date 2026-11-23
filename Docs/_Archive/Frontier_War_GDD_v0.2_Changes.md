# Frontier War GDD: v0.2 changes

This revision covers the v0.1 first draft (23 Nov 2026). It replaces section 03 (Base and Economy), updates sections 04, 06–07, 09 and 18, and fixes the review findings. Any section not mentioned here stays as written in v0.1.

**Headline decision:** a **hybrid base model**. The Command HQ is still the only *production* building. The player also gets a small, capped set of **deployable structures** and **upgradeable captured sites**. Base-building decisions come back without a sprawling building menu.

---

## 03 / BASE AND ECONOMY (replaces v0.1)

### Three layers

| Layer | What the player does | Limit |
|---|---|---|
| **Command HQ** | Places it once. Produces every unit through domain tabs. Has its own income and point defence. | One per player |
| **Captured sites** | Captures pre-placed metal fields, energy stations, airstrips and harbours, then **upgrades** them. | Number of sites on the map |
| **Deployables** | Places defences and support structures from the HQ panel. | **Fortification slots** set by HQ tier |

There are still no separate factories, power plants or research labs.

### Command HQ
- The HQ gives a **small base income** of metal and energy, so the player never stalls at zero.
- It has **point defence** that hits ground and air, one target at a time. It is strong against raids but easy to out-range with artillery.
- It has three upgrade tiers, unchanged from v0.1: Field, Theatre and Strategic Command.
- **Loss rule:**
  - Skirmish and survival: losing the HQ is defeat.
  - Campaign: each mission states its own defeat condition (see 17 / Mission framework). Escort missions can run without an HQ.

### Site upgrades (new)
An engineer must stand on a captured metal or energy site to upgrade it.

| Level | Income (relative) | Notes |
|---|---|---|
| L1 (captured) | ×1.0 | The default after capture |
| L2 | ×1.5 | Needs HQ Tier II |
| L3 | ×2.0 | Needs HQ Tier III |
| **L3 specialisation:** choose one | | Permanent while the site is held |
| — *Hardened* | ×2.0 | Heavy armour, slow self-repair and a small local AA/AT emplacement. Built for the front line. |
| — *Surge* | ×3.0 | Half health; it goes offline for 60 s after taking heavy damage. For safe rear sites. |

- **Losing a site:** the enemy first has to *neutralise* it (it stops paying), then capture it. Each change of owner drops it **one level**, so a lost L3 site comes back as L2. That makes losing a site hurt without resetting everything.
- **Airstrips and harbours** use the same L1–L3 levels:
  - Each level adds **launch slots**, meaning how many aircraft can rearm or ships can launch at once, plus faster turnaround.
  - At L3 they offer the *Hardened* choice only.
- All multipliers above are placeholders to be tuned in the prototype.

### Deployables (new)
They are placed from an **HQ "Deploy" tab** inside the HQ command radius or near a captured forward logistics point. Each deployable builds over time, and nearby engineers speed it up.

| Deployable | Tier | Slots | Role |
|---|---|---|---|
| Gun emplacement | I | 1 | Anti-ground defence |
| AA battery | I | 1 | Anti-air defence |
| Sensor mast | I | 1 | Vision radius; reveals stealth and submarines near the coast |
| Repair pad | II | 1 | Repairs ground vehicles and mechs |
| Artillery emplacement | II | 2 | Long-range static fire |
| Shield projector | III | 2 | Dome that absorbs projectiles; drains energy |
| Missile defence battery | III | 2 | Intercepts strategic missiles in a radius |
| Strategic missile silo | III | 4 | One per player. Long build and long reload; telegraphed to the enemy when it launches. |

**Fortification slots by HQ tier:** Tier I: 3 · Tier II: 6 · Tier III: 10. Captured forward logistics points add +1 each.
The slot cap keeps bases readable on a phone and turns every defence into a choice.

---

## 04 / ECONOMY AND PROGRESSION (updates)
- There are still two resources: metal and energy.
- **Energy drain:** shield projectors, the strategic silo, and anything Tier III or experimental count as *advanced*. Low energy slows their production and switches shields off first.
- **Late-game stalemate breaker:** a Tier III research option, *Field Reclamation*, makes destroyed wrecks yield a small amount of metal when an engineer salvages them. It replaces any need for a build-anywhere income structure.

---

## 06–07 / UNIT ROSTER (updates)

### Changes
- **Added:** an *Amphibious Hover Tank* (ground, Tier II). It is fast and light, crosses water and shorelines, and is weak to anti-tank weapons.
- **Removed:** *Amphibious Assault Ship*, because it overlapped with Landing Craft.
- **Moved:** *Light Recon Mech* goes to Tier II, so the first mech class can be built as soon as Chapter II unlocks it.
- The total is still **48**: infantry 8, ground 17, air 12, naval 11.

### Air launch rule (clarified)
- **Drones and helicopters** launch from an HQ helipad or a carrier, with no airstrip needed.
- **Fixed-wing aircraft** need a captured airstrip or a carrier.
- **Sorties:** fixed-wing aircraft carry limited ammunition and return to rearm automatically. Losing every airstrip and carrier means no rearming until one is retaken.
- A **carrier** is a mobile airstrip with a fixed number of launch slots.

### Tier per unit (new)

| Tier | Infantry | Ground | Air | Naval |
|---|---|---|---|---|
| I | Rifle, MG, Anti-Tank, Anti-Air, Scout/Sniper, Engineer, Medic | Recon Jeep, APC, Light Tank, Mobile AA | Recon Drone | Patrol Boat, Landing Craft |
| II | Special Forces | IFV, MBT, Heavy Tank, Tank Destroyer, SP Artillery, MRL, Mobile Radar, Amphibious Hover Tank, Light Recon Mech | Attack Drone, Interceptor, Air-Superiority Fighter, Multirole Fighter, Ground-Attack Aircraft, Attack Heli, Transport Heli | Missile Boat, Corvette, Frigate, Destroyer, Attack Submarine |
| III | — | Assault Mech, Siege Mech | Tactical Bomber, Gunship Heli, AWACS | Cruiser, Battleship, Missile Submarine, Carrier |
| III · Experimental | — | Experimental Walker, Super-Heavy Tank | Experimental VTOL | — |

---

## 08 / SYSTEMS USED BY MISSIONS (definitions added)

| System | Rule | Used in |
|---|---|---|
| Fog of war | Unexplored areas are black; explored areas grey out after vision is lost and show last-known buildings only | All, taught in 06 |
| Weather | Per-mission modifier: sandstorm/storm cuts vision 40% and grounds fixed-wing aircraft while active | 15, 27 |
| Night | Vision −30%; sensor masts, radar and AWACS matter more; muzzle flashes reveal units briefly | 25 |
| Shields | Shield projector deployable (above); enemy shields work the same way | 43 |
| Strategic missiles | Silo + missile defence battery (above); launch gives a 30 s warning | 46 |
| HQ support link | Mission-scripted only, not a general system | 06 |

---

## 09 / ANDROID UX (updates)
- **Strategic zoom:** zoom all the way out to a full-map view. Units become **domain icons** (infantry, ground, air, naval, experimental) with team colour and health pips, and orders still work at that zoom.
- **Deploy tab:** placing a deployable shows a ghost footprint, slot cost, build time and range rings before you confirm.

---

## 18 / SYSTEMS AND ARCHITECTURE (updates)
- **Data-driven units from day one.** Every unit, deployable and site upgrade is defined in a plain data file (stats, weapons, tier, cost, sprite set), not in code. This makes balancing faster now and allows modding later.
- **Multiplayer readiness:** decide during the core prototype between two routes:
  - **(a) Lockstep:** fixed-point simulation is required from the start.
  - **(b) Host-authoritative:** float simulation is fine, but uses more bandwidth.

  It is still not built for launch, but this choice can't be made later without a rewrite.

---

## FIXES FROM THE v0.1 REVIEW
1. Mission 03: "Upgrade the Command HQ to Tier I" now reads **Tier II**.
2. Section numbering: the campaign chapters become **11.1–11.5**, so 17 → **12** (Mission framework), 18 → **13**, 19 → **14**, 20 → **15**, 21 → **16** and 22 → **17**.
3. Duplicate "Chapter Design Intent" removed from Chapters II and V.
4. **Chapter rewards** now mean *unlocked permanently for skirmish and survival*. Inside the campaign, a mission can hand you units early.
5. **Mission 02** now also teaches **site upgrade L1→L2** once the HQ reaches Tier II in mission 03. Mission 07 (Hold the Bridge) introduces **deployables** (gun emplacement and AA battery).
6. **Mission 14 (The Foundry)** now introduces the **Hardened vs Surge** site choice.
7. **Mission 46 (Nuclear Option)** uses the strategic silo and missile-defence rules above. The player builds their first missile defence battery here.
8. Still missing, for v0.3: audio direction, save/load (mid-mission saves), tutorial flow, localisation, team and schedule.

---

## IDENTITY NOTE
Rusted Warfare is the inspiration for the genre and its feel. Frontier War keeps its own names, numbers, factions and art. Don't reuse Rusted Warfare's unit or building names, or its exact stats and income values.

## v0.2 ACCEPTANCE ADDITIONS (prototype checklist)
- An engineer can upgrade a captured site to L2, and losing it drops it one level.
- At least two deployables can be placed within the slot cap, with ghost preview and range rings.
- Strategic zoom shows domain icons and accepts orders.
- At least 3 units are defined entirely in data files and can be rebalanced without code changes.
