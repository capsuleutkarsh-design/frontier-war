<p align="center">
  <img src="Art/Readme/banner.jpg" alt="Frontier War" width="100%">
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6%20LTS%20%C2%B7%20URP-1f2a1f?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 6 URP">
  <img src="https://img.shields.io/badge/Platform-Android-3d5a19?style=for-the-badge&logo=android&logoColor=white" alt="Android">
  <img src="https://img.shields.io/badge/Mode-Single--player%20%C2%B7%20Offline-5b7a2e?style=for-the-badge" alt="Single-player offline">
  <img src="https://img.shields.io/badge/Status-M0%20Setup-c99a1d?style=for-the-badge" alt="Status M0">
</p>

<p align="center">
  <b>A big, fast sandbox RTS for phones.</b><br>
  Build a base, drill ore, and command tanks, mechs, aircraft, warships and giant experimental machines,<br>
  in true-to-scale 3D with a crisp pixel filter.
</p>

<p align="center">
  <a href="Docs/02_GameDesign/Frontier_War_GDD_Draft_1.0.pdf"><b>📘 Game Design (PDF)</b></a> &nbsp;·&nbsp;
  <a href="Docs/00_Index.md"><b>🗂️ All Docs</b></a> &nbsp;·&nbsp;
  <a href="Docs/04_Production/Roadmap.md"><b>🗺️ Roadmap</b></a> &nbsp;·&nbsp;
  <a href="Docs/04_Production/Task_Board.md"><b>✅ Task Board</b></a>
</p>

---

## 🎯 The game in 30 seconds

<table>
<tr>
<td width="55%">
<img src="Art/Readme/09_gameplay.jpg" alt="Gameplay target look" width="100%">
</td>
<td width="45%">

**Start small, end huge.** One Command Core and one Constructor at minute 1. Hundreds of units and a walking fortress by minute 25.

- 🏗️ **Real base building:** factories, drills, turrets, shields
- ⛏️ **Ore deposits** you must take and hold
- 🪖 **47 units** across land, mechs, air, naval and experimental
- 🤖 **Smart AI** with 6 personalities and 5 difficulty levels
- 📖 **50-mission campaign**, plus skirmish, survival and challenges
- 📱 **Built for touch**, with tactical pause and zoom-out icons

</td>
</tr>
</table>

## 🧭 Inspiration

| | Taken from | What we take |
|---|---|---|
| 🎮 **Gameplay** | *Rusted Warfare* | Sandbox base building, extractors on resource spots, tiers, experimental units, huge battles |
| 🎨 **Look & feel** | *Ebbtide* | Real 3D at true scale, fixed angled camera, hard shadows, calm colours, pixel filter |

> Everything inside Frontier War, including the names, numbers, story and art, is **original**. See [`Docs/01_Research`](Docs/01_Research).

## 🖼️ Concept gallery

<table>
<tr>
<td align="center" width="33%"><img src="Art/Readme/02_base.jpg" width="100%"><br><sub><b>Player base</b></sub></td>
<td align="center" width="33%"><img src="Art/Readme/03_land.jpg" width="100%"><br><sub><b>Land units</b></sub></td>
<td align="center" width="33%"><img src="Art/Readme/04_mechs.jpg" width="100%"><br><sub><b>Mechs & infantry</b></sub></td>
</tr>
<tr>
<td align="center"><img src="Art/Readme/05_air.jpg" width="100%"><br><sub><b>Air units</b></sub></td>
<td align="center"><img src="Art/Readme/06_naval.jpg" width="100%"><br><sub><b>Naval units</b></sub></td>
<td align="center"><img src="Art/Readme/07_experimental.jpg" width="100%"><br><sub><b>Experimentals</b></sub></td>
</tr>
</table>

<p align="center">
  <img src="Art/Readme/10_scale.jpg" width="80%"><br>
  <sub><b>True scale:</b> soldier · jeep · tank · mech · house · factory · experimental walker</sub>
</p>

## 🗺️ Roadmap

| | Milestone | Goal |
|---|---|---|
| 🟡 | **M0 · Setup** | Unity project, pixel filter, style test on the phone |
| ⚪ | M1 · Core simulation | Movement, pathfinding, combat, fog, 200 tanks at 60 FPS |
| ⚪ | M2 · Economy & base | Credits, drills, factories, production, AI v0 |
| ⚪ | **M3 · Fun gate** | A full skirmish that testers want to replay |
| ⚪ | M4 · All domains | Air, naval, hover, mechs, tiers, experimentals |
| ⚪ | M5 · Real AI | Managers, doctrines, difficulty |
| ⚪ | M6 · Art vertical slice | Final art for 12 units and one map |
| ⚪ | M7 · Content | 47 units, 24 maps, 50 missions |
| ⚪ | M8 · Polish & release | Device tests, store page, launch 🚀 |

Full details and exit criteria: [`Docs/04_Production/Roadmap.md`](Docs/04_Production/Roadmap.md)

## 📂 Repository layout

```
RTS/
├── Docs/
│   ├── 00_Index.md              ← start here
│   ├── 01_Research/             ← Rusted Warfare (mechanics) · Ebbtide (look)
│   ├── 02_GameDesign/           ← illustrated GDD (PDF + source)
│   ├── 03_TechnicalDesign/      ← every system, for Unity (00–12)
│   └── 04_Production/           ← roadmap, rules, task board, trackers
├── Art/
│   ├── Concepts/                ← locked look (pixel · clean · style tests)
│   ├── Readme/                  ← images for this page
│   └── Tools/                   ← export settings
└── Unity/FrontierWar/           ← the Unity 6 project
    └── Assets/_Project/         ← everything we make
```

## 🛠️ Getting started

1. Install **Unity 6 LTS** with **Android Build Support**, and **Git LFS**.
2. Clone (the images come through LFS):
   ```bash
   git clone https://github.com/capsuleutkarsh-design/frontier-war.git
   ```
3. Unity Hub ▸ **Add** ▸ `Unity/FrontierWar`
4. In Unity: **Frontier War ▸ Apply Project Settings**, then **Frontier War ▸ Build Style Test Scene**, then press ▶️

Step-by-step guide: [`Docs/04_Production/Unity_Project_Setup.md`](Docs/04_Production/Unity_Project_Setup.md)

## 📏 How we work

- **The simulation is separate from the view:** plain C# at 20 ticks per second, so it's easy to test and save ([`00_Architecture`](Docs/03_TechnicalDesign/00_Architecture.md))
- **Grey boxes until the fun gate:** no final art before M6
- **Every task ≤ 1 day**, a commit every day, and a weekly build on a real phone
- **Read every Monday:** [`Rules_To_Not_Get_Stuck.md`](Docs/04_Production/Rules_To_Not_Get_Stuck.md)

---

<p align="center"><sub>Frontier War · solo project · private repository · 2026</sub></p>
