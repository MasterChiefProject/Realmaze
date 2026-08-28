# Realmaze

[![Unity](https://img.shields.io/badge/Unity-6000.0.47f1-black?logo=unity)](https://unity.com/)
[![WebGL](https://img.shields.io/badge/WebGL-browser%20build-5b7fff)](https://masterchiefproject.github.io/Realmaze/)

**Realmaze** is a first-person escape game built with Unity 6. The player explores a realistic outdoor maze, collects the coins required to open a chest, recovers a key, unlocks the exit gates, avoids hostile zombies, and reaches the escape zone.

**Playable WebGL build:** https://masterchiefproject.github.io/Realmaze/

The browser version uses a deliberately reduced rendering profile to keep a large Unity environment practical for WebGL while preserving the original scene flow and gameplay logic.

## Gameplay

A run begins in `MainMenuScene` and continues into `GameScene`.

Progression is built around a simple gated objective chain:

1. Explore the maze and collect coins.
2. Reach 20 coins to unlock the chest.
3. Recover the key.
4. Unlock the gates.
5. Avoid zombies and environmental hazards.
6. Reach the escape zone.

The runtime includes collectible state, gated interactions, enemy patrol behavior, death/restart handling, and a victory flow.

## Controls

| Action | Keyboard / Mouse |
| --- | --- |
| Move | `WASD` / Arrow Keys |
| Look | Mouse |
| Jump | `Space` |
| Sprint | `Left Shift` |
| Crouch | `Left Ctrl` |
| Zoom | Right Mouse Button |

Coin collection, chest progression, key pickup, gate unlocking, and the final escape objective are trigger-driven interactions.

## Technology

- Unity `6000.0.47f1`
- C#
- Universal Render Pipeline 17
- Unity Input System
- AI Navigation package
- Custom waypoint-based enemy movement
- uGUI
- WebGL / WebAssembly
- Git LFS
- GitHub Pages

## Production scenes

The WebGL release contains the two runtime scenes:

```text
Assets/Scenes/MainMenuScene.unity
Assets/Scenes/GameScene.unity
```

Development and imported asset-demonstration scenes are excluded from the production build.

## Runtime systems

Realmaze combines several small gameplay systems:

- collectible tracking and progression state
- chest gating based on coin count
- key acquisition and gate unlocking
- first-person movement and camera control
- enemy waypoint patrol and collision avoidance
- player death and restart handling
- escape-zone victory logic
- WebGL-specific runtime scaling for the browser target

## Project structure

```text
Realmaze/
├── Assets/
│   ├── Editor/
│   │   └── RealmazeWebGLBuild.cs
│   ├── Imports/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   │   └── WebGLPerformanceBootstrap.cs
│   └── WebGLTemplates/
│       └── Realmaze/
├── Packages/
├── ProjectSettings/
├── ASSET-NOTICE.md
└── README.md
```

The deployable `docs/` directory is generated from Unity and committed for GitHub Pages.

## WebGL performance engineering

The browser build applies a dedicated performance profile to reduce download size, startup cost, GPU load, and memory pressure.

The production configuration includes:

- Gzip compression with decompression fallback
- browser data caching
- WebAssembly 2023
- no WebGL threads
- size-oriented IL2CPP code generation
- managed-code and engine stripping
- reduced texture mip usage
- a fixed `960 x 540` internal canvas
- device pixel ratio `1`
- reduced shadow, reflection, HDR, and post-processing cost
- shorter camera and terrain draw distances
- lower terrain detail distances
- bounded asynchronous upload work

The WebGL runtime also uses a lower texture mip limit and more conservative environment settings than the desktop/editor presentation.

Enemy patrol logic is adapted for browser constraints through a reusable `Physics.OverlapSphereNonAlloc` buffer, cached terrain access, and staggered local-avoidance sampling.

These optimizations are scoped to WebGL where applicable, so the Unity Editor retains the higher-fidelity project configuration.

## Unity and build workflow

The project targets:

```text
Unity 6000.0.47f1
```

Large binary Unity assets are stored with Git LFS. A complete fresh clone requires the repository's LFS objects:

```powershell
git lfs install
git lfs pull
```

The main entry scene is:

```text
Assets/Scenes/MainMenuScene.unity
```

The production browser build is generated through:

```text
Realmaze > Build WebGL for GitHub Pages
```

The build helper:

- targets WebGL
- packages exactly the two production scenes
- applies `MasterChiefProject / Realmaze / 1.0.0` metadata
- applies the browser performance profile during the build
- builds into `Builds/RealmazeWebGLStaging/`
- publishes `docs/` only after a successful Unity build
- creates `docs/.nojekyll`
- restores the original editor quality configuration afterward

The build keeps Unity's normal import/build cache available rather than forcing a clean cache on every run.

## Local WebGL validation

The generated site is served over HTTP:

```powershell
py -m http.server 8000 --directory docs
```

Local URL:

```text
http://localhost:8000/
```

This mirrors the hosting model used by GitHub Pages and allows the WebAssembly and compressed Unity assets to load normally.

## Deployment

GitHub Pages serves the committed `docs/` build from the `main` branch.

**Live build:** https://masterchiefproject.github.io/Realmaze/

## Assets and licensing

Realmaze contains imported environment art, audio, textures, models, shaders, and other third-party Unity content.

See [`ASSET-NOTICE.md`](ASSET-NOTICE.md) for redistribution and provenance information. The project documentation does not assert a repository-wide license over third-party assets.
