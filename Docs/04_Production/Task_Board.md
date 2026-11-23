# Task board

Every task takes **1 day or less**. Move tasks between sections. When a milestone is done, archive its tasks at the bottom.

## Now (max 3)
- [ ] **Open the project in Unity Hub** (Add ▸ `RTS/Unity/FrontierWar`). The first import finishes here and takes a few minutes. Watch the PC, and close Unity if it struggles.
- [ ] Menu **Frontier War ▸ Apply Project Settings**, then File ▸ Build Profiles ▸ switch to **Android**
- [ ] Menu **Frontier War ▸ Build Style Test Scene**, press Play, and compare with `Art/Concepts/pixel/11_style_lock.jpg`

## Next (M0 · Setup)
- [ ] Commit the `.meta` files Unity creates on first open ("M0: first Unity import")
- [ ] Add the **Test Framework** package and create the `FW.Tests` assembly (`Scripts/Tests/README.md`)
- [ ] Tune the StyleTest look: sun angle, ambient, colours, zoom 60 vs style lock
- [ ] First Android build on the Target Phone (Development Build, Build And Run)
- [ ] Check 60 FPS on the phone. Then M0 is done, so split M1 into 1-day tasks.

## Later
- M1 tasks are written when M0 is done (split `Roadmap.md` M1 into 1-day tasks).

## Done
- [x] Unity 6 project created from the URP template (`Unity/FrontierWar`)
- [x] git init, `.gitignore`, `.gitattributes` (LFS), first commit
- [x] Folder tree `Assets/_Project/…` and asmdefs (`00_Architecture §5–6`)
- [x] `GameRunner` (20-tick loop), `RtsCameraController` (pan, pinch, wheel), `PixelFilterController` (RT ÷4, snapping), `DebugOverlay`
- [x] Editor tools: Apply Project Settings, Build Style Test Scene
