# Roadmap: start to finish (solo, Unity)

**The honest estimate:** about **12–15 months** of steady part-time-plus work for one person. The plan is built so you have a **playable game at the end of month 3**. Everything after that makes it bigger and better, but it is never broken.

Each milestone has **exit criteria**. You don't start the next milestone until every box is ticked **on the Target Phone**.

---

## M0 · Setup (1 week)
- [ ] Unity 6 LTS project at `Unity/FrontierWar` (see `Unity_Project_Setup.md`)
- [ ] Git repository, `.gitignore` and Git LFS set up. First commit.
- [ ] Folder structure, plus the asmdefs from `00_Architecture`
- [ ] Orthographic camera and **pixel filter**, with a grey-box ground, 20 box "tanks" and 10 box "houses" at the right scale
- [ ] Android build installs and runs on the Target Phone
**Exit:** the grey-box scene runs at 60 FPS on the phone and already "feels" like the style lock (scale, angle, shadows).

## M1 · Core simulation (3–4 weeks)
- [ ] GameRunner tick loop, GameState, Commands, SpatialHash
- [ ] Map grid, A\*, flow fields, steering, formations and stuck detection (`02_Movement`)
- [ ] Touch input: select, box select, move, attack, pan and zoom (`07`)
- [ ] 3 units as data: Light Tank, Rail Tank, SAM Truck (+1 aircraft: Light Gunship)
- [ ] Targeting, turrets, 3 projectile types (Instant, Shell, Missile), damage and death with wrecks (`03`)
- [ ] Vision and fog (`05`)
- [ ] Debug overlay and Sandbox scene (`11 §5`)
**Exit:** the movement tests and combat tests in docs 02 and 03 pass. 200 tanks are fine at 60 FPS on the phone.

## M2 · Economy and base (3 weeks)
- [ ] Credits, Command Core, Constructor, Ore Drill L1–L3, Vehicle Plant, Gun Turret (`04`)
- [ ] Build placement ghost, build rate, assist, cancel and refund
- [ ] Production queue, repeat, rally and unit cap
- [ ] **AI v0:** follows one fixed build order, then attacks when it has 10 units
- [ ] Win/lose check
**Exit:** a full 15-minute skirmish against AI v0 on one grey-box map, from start to a win screen.

## M3 · FUN GATE (2 weeks) ← the most important milestone
- [ ] HUD v1, strategic zoom icons, alerts, control groups, tactical pause
- [ ] Save/load and autosave (`10`)
- [ ] Tune the numbers until it's fun
- [ ] **3 people play it**, and you watch silently
**Exit:** testers say "one more game". **If it isn't fun, stay here. Don't add content to a game that isn't fun.**

## M4 · All domains (5–6 weeks)
- [ ] Airfield, jets (strafe runs), helicopters, rearm pads
- [ ] Shipyard, ships, submarines, stealth and detection, torpedoes
- [ ] Hover units, transports (Hover Lander, Tiltrotor)
- [ ] Mech Bay and mechs. Infantry and forest cover.
- [ ] T2/T3 tiers, factory upgrades, drill final choice, ActionDef system
- [ ] Shields, point defence, 1 experimental (Colossus), strategic missile + interceptor, support powers
- [ ] Map editor tool v1 (`09 §1`)
**Exit:** one mixed land/sea map where all domains matter. The counter tests pass.

## M5 · Real AI (3–4 weeks)
- [ ] Managers (Economy, Build, Production, Army, Scout, Defense)
- [ ] 6 doctrines, 5 difficulties, AI allies (`06`)
**Exit:** an AI-vs-AI soak test with 20 games, no crashes, and each doctrine wins 35–65%. Veteran AI beats you sometimes.

## M6 · Art vertical slice (4 weeks)
- [ ] Final models for 12 units and the Command Core, Drill, Vehicle Plant and Turret
- [ ] One final forest map with props, water and effects
- [ ] UI art pass, first sounds and music
**Exit:** a screenshot on the phone matches the style lock. A 2-minute gameplay video looks like a real game.

## M7 · Content production (16–24 weeks)
Do it in this order, one block at a time:
1. All 47 units, as **grey-box first**, then final art (`Asset_List.md`)
2. 24 skirmish maps (5 biomes)
3. Campaign chapters I → V, 10 missions each, using `Mission_Tracker.md`
4. Survival mode, 30 challenges, Sandbox mode for players
**Exit:** every item is "Polished" in the trackers.

## M8 · Polish and release (6–8 weeks)
- [ ] Tutorial pass, accessibility, settings and localisation hooks (English first)
- [ ] Device matrix: 5 phones (low, mid, high), plus heat and battery checks
- [ ] Store page: icon, screenshots and trailer
- [ ] Closed test on the Google Play Console (2 weeks), then fix the top 20 issues
- [ ] Release 1.0
**After launch:** the map editor and mod loading for players (JSON defs), as Update 1.
