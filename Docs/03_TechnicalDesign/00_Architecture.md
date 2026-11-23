# 00 · Architecture (read this first)

**Engine:** Unity 6 LTS · **Render pipeline:** URP · **Language:** C# · **Platform:** Android first (PC builds for testing).
**Team:** one person. Every choice below picks **simple and finishable** over clever.

---

## 1. The one big rule: Simulation and View are separate

```
            ┌───────────────────────── SIMULATION (pure C#) ─────────────────────────┐
 Input ───► │ Commands ─► Tick (20 per second) ─► Systems update GameState              │
            │   Economy · Construction · Production · Movement · Combat · Vision · AI   │
            └───────────────┬───────────────────────────────────────────────────────────┘
                            │ read only
            ┌───────────────▼─────────────── VIEW (Unity MonoBehaviours) ───────────────┐
            │ UnitView (model, turret aim, animation) · Effects · Fog texture · UI · Audio │
            └─────────────────────────────────────────────────────────────────────────────┘
```

- **The simulation** owns all game data (units, buildings, credits, fog). It is **plain C#** with no `MonoBehaviour`, no `Transform` and no `GameObject`. This means:
  - It runs at a fixed **20 ticks per second**, whatever the frame rate.
  - Save/load is just "write GameState to a file".
  - You can test it with Unity EditMode tests, without opening a scene.
  - 1,000 units are cheap, because units are data in arrays, not GameObjects with scripts.
- **The view** only *reads* the simulation and draws it. Views smoothly interpolate between the last two ticks.
- **Input never changes the game state directly.** It creates a **Command** (`MoveCommand`, `BuildCommand`…). Commands are applied at the start of the next tick. The AI uses exactly the same commands as the player.

> Why: in your earlier projects, logic probably lived inside MonoBehaviours, and every new feature broke three old ones. Here every system has one job and one place.

## 2. Main loop

```csharp
// GameRunner.cs (the only MonoBehaviour that drives the game)
void Update() {
    accumulator += Time.deltaTime * gameSpeed;      // gameSpeed: 0 (pause), 0.5, 1, 1.5, 2
    while (accumulator >= TickDt) {                   // TickDt = 0.05 s
        sim.ApplyCommands(commandQueue.Drain());
        sim.Tick();                                   // all systems, in fixed order
        accumulator -= TickDt;
    }
    view.Render(sim.State, alpha: accumulator / TickDt);   // interpolation 0..1
}
```

**Fixed system order each tick** (never change the order without a reason):
1. `CommandSystem`: applies queued commands
2. `EconomySystem`: income, spending
3. `ConstructionSystem`: build progress, repair, salvage
4. `ProductionSystem`: factory queues and spawning
5. `AISystem`: decisions (it only issues commands for the next tick)
6. `MovementSystem`: paths, steering, positions
7. `TargetingSystem`: choose targets (every 4 ticks)
8. `WeaponSystem`: turret aim, firing, projectiles
9. `DamageSystem`: apply damage, shields, deaths, wrecks, veterancy
10. `VisionSystem`: fog of war (every 4 ticks)
11. `TriggerSystem`: mission conditions and actions
12. `CleanupSystem`: remove dead entities, fire events for the view

## 3. Entities: simple, not ECS

Unity DOTS/ECS is powerful but hard to learn and debug alone. **Don't use it.** Use plain C# classes in pooled lists:

```csharp
public sealed class Unit {
    public int Id; public int Team; public UnitDef Def;     // Def = shared, read-only data
    public Vector2 Pos, PrevPos, Vel; public float Facing;   // 2D sim on the ground plane (x,z)
    public float Height;                                     // 0 for ground, flight height for air
    public float Hp, Shield; public int Rank; public float Xp;
    public Order CurrentOrder; public Queue<Order> Orders;
    public TurretState[] Turrets;                            // aim angle, cooldown, target id
    public PathState Path; public bool Alive;
}
```

- The **simulation works in 2D** (x, z on the ground). Air and sea are just **layers** (see `02_Movement`). This is much simpler than full 3D, and still looks 3D, because the view draws real 3D models.
- A `SpatialHash` (a grid of buckets, 16 m cells) answers "which units are near X?" quickly. It's used by targeting, separation and vision.
- **Burst/Jobs** are allowed later for the 2–3 hottest loops (separation, target search) **only if the profiler says so**.

## 4. Data-driven content

Units, buildings, weapons and upgrades are **ScriptableObject assets** (`UnitDef`, `WeaponDef`, `BuildingDef`, `UpgradeDef`), so you can edit numbers in the Inspector with no code. See `01_Data_and_Units.md`.
Later (for mods) a JSON loader can create the same runtime defs. Don't build it until the game is fun.

## 5. Code assemblies (asmdef) – keeps the code clean and compiling fast

| Assembly | Contains | May use |
|---|---|---|
| `FW.Core` | math helpers, ids, events, pools, SpatialHash | nothing |
| `FW.Data` | Def ScriptableObjects, enums | Core |
| `FW.Sim` | GameState, all Systems, Commands, pathfinding | Core, Data |
| `FW.AI` | AI players (they create Commands) | Core, Data, Sim |
| `FW.View` | unit views, effects, fog render, camera, pixel filter | Core, Data, Sim |
| `FW.UI` | HUD, menus, build panel | Core, Data, Sim, View |
| `FW.Game` | GameRunner, bootstrap, scene flow, save/load | everything |
| `FW.Editor` | editor tools (map tools, def validators) | everything (editor only) |
| `FW.Tests` | EditMode tests for Sim and AI | Sim, AI, Data |

**Rule:** `FW.Sim` must never reference `UnityEngine` classes except `Mathf`/`Vector2`/`Random`-like helpers. If you need a `Transform` in Sim, the design is wrong.

## 6. Unity project folder structure

```
Unity/FrontierWar/
  Assets/
    _Project/                    ← everything we make lives here
      Art/
        Models/  Land/ Mechs/ Air/ Naval/ Experimental/ Buildings/ Props/
        Materials/
        Textures/
        Effects/                 (VFX prefabs, particle textures)
        UI/                      (icons, panels, fonts)
      Audio/  Music/ SFX/ Voice/
      Data/                      ← ScriptableObject assets (the numbers)
        Units/  Buildings/  Weapons/  Upgrades/  Factions/  AIDoctrines/  Missions/
      Maps/                      (map assets + their scenes)
      Prefabs/  Units/ Buildings/ Effects/ UI/
      Scenes/  Boot.unity  MainMenu.unity  Battle.unity  Sandbox.unity  Tests/
      Scripts/
        Core/   Data/   Sim/   AI/   View/   UI/   Game/   Editor/   Tests/
      Settings/                  (URP assets, input actions, quality)
    Plugins/                     (third-party only)
  Packages/  ProjectSettings/
```

**Rules for neatness:**
- Nothing at the `Assets/` root except `_Project` and `Plugins`.
- One class per file, and the file name matches the class name.
- Names: `UnitDef_LightTank.asset`, `PF_Unit_LightTank.prefab`, `M_Tank_Olive.mat`, `T_Tank_Olive_Albedo.png`.
- No "Temp", "New Folder" or "Test2" folders. Experiments go in `Scenes/Tests/` and are deleted weekly.

## 7. Events from Sim to View

The simulation raises small events (`UnitSpawned`, `UnitDied`, `WeaponFired`, `BuildingCompleted`, `CreditsChanged`). The view listens and plays effects and sounds. The simulation never calls the view.

## 8. Things we deliberately do NOT build

- Multiplayer and networking (single-player only).
- DOTS/ECS, a custom physics engine, NavMesh (we use our own grid pathfinding, because NavMesh is weak for 500 units).
- The operations-map layer like Ebbtide (strategic zoom is enough).
- Mod loading (until after launch).
