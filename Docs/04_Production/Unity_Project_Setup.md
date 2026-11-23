# Unity project setup (milestone M0, step by step)

## 1. Install
1. Unity Hub, then **Unity 6 LTS** (the newest 6000.x LTS) with the modules **Android Build Support** (plus OpenJDK and Android SDK & NDK).
2. **Git for Windows** (already installed) and **Git LFS**. Run this once:
   ```powershell
   git lfs install
   ```
3. **Blender** (free) for models.
4. An IDE: Visual Studio 2022 Community or JetBrains Rider.

## 2. Create the project
- Unity Hub ▸ New project ▸ **Universal 3D** template. Name it `FrontierWar` and put it in `C:\Users\AI10482\Documents\Utkarsh_Software\RTS\Unity\`.

## 3. Git (from the `RTS` folder)
```powershell
git init
```
Add a `.gitignore` for Unity (GitHub's standard "Unity.gitignore") at `Unity/FrontierWar/.gitignore`, plus this `.gitattributes` at the `RTS` root:
```
*.fbx filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.pdf filter=lfs diff=lfs merge=lfs -text
*.unity merge=unityyamlmerge eol=lf
*.prefab merge=unityyamlmerge eol=lf
*.asset merge=unityyamlmerge eol=lf
```
In Unity: Edit ▸ Project Settings ▸ Editor ▸ **Version Control: Visible Meta Files**, **Asset Serialization: Force Text**.

## 4. Packages (Window ▸ Package Manager)
- **Input System** (and say yes to restart; set Active Input Handling to "Input System Package (New)")
- **Test Framework** (usually installed already)
- URP (already in the template)
- Later, only if needed: Burst, Collections

## 5. Project settings
| Where | Setting |
|---|---|
| Player ▸ Resolution | Default orientation: **Landscape Left** (Auto-rotation: both landscapes) |
| Player ▸ Other | Scripting backend **IL2CPP**, Target architectures **ARM64**, Graphics APIs **Vulkan, OpenGLES3**, Minimum API level: Android 8.0 (API 26) |
| Quality | Make 2 levels, **Low** and **High**. Shadow resolution 1024 / 2048, shadow cascades 1. |
| URP Asset | HDR off, MSAA off, SRP Batcher on, Main Light Shadows on, Soft Shadows **off** (hard shadows = locked look) |
| Time | Fixed Timestep doesn't matter (we don't use physics for gameplay) |
| Physics | Auto Simulation **off** (we don't use Unity physics in the sim) |

## 6. Folders and assemblies
1. Create the folder tree from `03_TechnicalDesign/00_Architecture.md §6` under `Assets/_Project/`.
2. In each `Scripts/<Name>/` folder: right-click ▸ Create ▸ **Assembly Definition**, named `FW.<Name>`, and set its references as in the table in §5 of that document.
3. `FW.Tests`: right-click ▸ Create ▸ Testing ▸ **Tests Assembly Folder**.

## 7. First scripts (M0 only)
- `Game/GameRunner.cs`: the tick loop (empty systems for now)
- `View/PixelFilterController.cs`: the RenderTexture + RawImage + pixel snapping (`08_Rendering §2`)
- `View/RtsCameraController.cs`: pan, pinch-zoom and clamp (`07 §1`)
- `Scenes/Tests/StyleTest.unity`: a grey ground, a box tank (9.5 × 3.7 × 2.4 m), a box house (12 × 10 × 8 m) and box trees (15 m tall), to compare with `Art/Concepts/pixel/11_style_lock.jpg`

## 8. First Android build
File ▸ Build Profiles ▸ Android ▸ switch. Then enable Development Build and **Build And Run** with the phone connected (USB debugging on).
