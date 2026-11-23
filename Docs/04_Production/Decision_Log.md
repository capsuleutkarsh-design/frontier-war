# Decision log

Write each decision once, with its reason. Don't reopen one without **new information**.

| Date | Decision | Why | Status |
|---|---|---|---|
| 2026-11-23 | Gameplay is a Rusted Warfare-style sandbox RTS (builders, factories, drills on deposits, tiers, experimentals) | It's the user's main inspiration and a proven fun loop | Locked |
| 2026-11-23 | All names, numbers, factions, story and art are original | Inspiration, not a copy | Locked |
| 2026-11-23 | Single-player only, fully offline | Solo scope, and no networking risk | Locked |
| 2026-11-23 | Engine: Unity 6 LTS + URP, Android first | The user knows Unity | Locked |
| 2026-11-23 | Look: true-to-scale low-poly 3D, orthographic camera, hard shadows, pixel filter (render ÷4, nearest upscale). Look and feel inspired by Ebbtide. | Chosen by the user from the style test | Locked |
| 2026-11-23 | Simulation is plain C# at 20 ticks/s, separate from the view. No DOTS. | Easy to test and save, and keeps code clean for one person | Locked |
| 2026-11-23 | Own grid pathfinding (A\* + flow fields + steering), not NavMesh | NavMesh is weak for 500 units | Locked |
| 2026-11-23 | Content as ScriptableObject defs. JSON mod loading after 1.0. | Inspector editing now, mods later | Locked |
| 2026-11-23 | One Condition/Effect "action" system for abilities, upgrades and mission triggers | One engine, many features (learned from the Rusted Warfare study) | Locked |
| 2026-11-23 | One currency (credits) | Easy to understand | Locked |
