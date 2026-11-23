# 10 · Save/load, settings and audio

---

## 1. Save and load

The simulation is plain C#, so a save is just the **GameState written to a file**.
- **Format:** a binary writer (`BinaryWriter`) with a **version number** at the start. Use `SaveWriter/SaveReader` helpers per class. A JSON debug export is available in the editor only.
- **What is saved:** map id, tick number, random seed state, teams (credits, CP, upgrades, flags), every unit (all runtime fields, orders, cargo), buildings (progress, queues, rally), projectiles in flight, wrecks, vision explored-bits, trigger states and AI blackboard summaries.
- **What is not saved:** views, effects and the camera (it's rebuilt on load). Flow-field caches are rebuilt too.
- **Ids:** units refer to each other by `int Id`, never by object reference, which keeps saving simple.
- **Slots:** 1 autosave (every 3 minutes, plus on app pause), 1 quicksave, and 10 manual slots. Path: `Application.persistentDataPath/saves/`.
- **Android:** save on `OnApplicationPause(true)`, because phones kill apps without warning.
- **Save version rule:** when the save format changes, bump the version and write a small upgrader. Never break old campaign saves after launch.

**Test:** save at minute 12 of a big battle, load it, and run 60 s. The result matches a run with no save/load (same seed, same commands). This is the **determinism check** for the sim. It also catches bugs where state lives outside GameState.

## 2. Settings

| Group | Options |
|---|---|
| Graphics | Pixel size 3/4/5 · Shadows on/off · Effects low/high · Frame cap 30/60 |
| Gameplay | Game speed default · Edge alerts on/off · Friendly-fire default · Auto-pause on app switch |
| Controls | UI scale 80–140% · Left-handed · Double-tap timing · Vibration on/off |
| Audio | Master · Music · SFX · Voice |
| Accessibility | Colour-blind team colours (blue/orange) · Bigger text · Screen-shake off |

These are stored in `PlayerPrefs` (one JSON string) and applied at boot.

## 3. Audio

- **Unity's built-in audio** (no middleware needed for a solo project). Use one `AudioManager` with pooled `AudioSource`s (32 voices).
- **Rules:**
  - **Voice limit per sound type:** max 4 of the same gunshot at once. Extra shots are skipped, and nearby shots merge into one louder shot.
  - Sounds are **2D with simple panning** by screen-x position, and their volume drops with zoom-out. At strategic zoom only big events play.
  - Unit "acknowledge" voice lines play on select and order, max 1 per 0.5 s.
  - **Music:** calm build-up while building, and battle music when the combat value nearby is over a threshold. The crossfade takes 4 s.
- **Audio events come from Sim events** (`WeaponFired`, `UnitDied`…), exactly like visual effects.
- **Sources:** licensed libraries or commissioned sound. Keep a list with licence info in `Docs/04_Production/Credits_and_Licences.md`.
