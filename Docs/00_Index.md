# Docs index: what to read, and when

## Read once, in this order
1. `02_GameDesign/Frontier_War_GDD_Draft_1.0.pdf`: **what** the game is (units, economy, campaign, look)
2. `01_Research/RustedWarfare_Study.md`: how the gameplay inspiration works, system by system
3. `01_Research/Ebbtide_Look_Study.md`: the rules of the locked look, and the scale table
4. `04_Production/Rules_To_Not_Get_Stuck.md`: **read every Monday**
5. `04_Production/Roadmap.md`: milestones M0 → M8, with exit criteria

## Read when you build that system
| Doc | Build it in |
|---|---|
| `03_TechnicalDesign/00_Architecture.md`: sim/view split, tick loop, assemblies, Unity folders | M0 |
| `01_Data_and_Units.md`: Def assets, armour table, ActionDef, veterancy | M1 |
| `02_Movement_and_Pathfinding.md`: grid, move classes, A\*, flow fields, steering, stuck fix | M1 |
| `03_Combat.md`: targeting, turrets, projectiles, damage, wrecks, super-weapon, powers | M1 / M4 |
| `04_Economy_Construction_Production.md`: credits, building, queues, tiers, win rules | M2 |
| `05_Vision_Fog_Stealth.md`: fog grid, stealth, detection | M1 / M4 |
| `06_AI.md`: managers, doctrines, difficulty | M2 (v0) / M5 |
| `07_Input_Camera_UI.md`: touch, selection, orders, HUD | M1 / M3 |
| `08_Rendering_and_Art.md`: URP, **pixel filter**, model rules, effects | M0 / M6 |
| `09_Maps_Missions_Campaign.md`: map asset, editor, triggers, campaign | M4 / M7 |
| `10_Save_Settings_Audio.md` | M3 |
| `11_Performance_and_Testing.md`: budgets, tests, debug tools | every milestone |
| `12_Mechanics_Coverage.md`: every mechanic: yes / no / later, skirmish settings, survival | when unsure "do we have X?" |

## Use every day
`04_Production/`: `Task_Board.md` · `Devlog.md` · `Decision_Log.md` · `Ideas_Later.md` · `Asset_List.md` · `Mission_Tracker.md` · `Credits_and_Licences.md` · `Unity_Project_Setup.md`

## Rule for these docs
If the game changes, **update the doc in the same commit**. The docs are the truth. When a doc and the code disagree, fix one of them the same day.
