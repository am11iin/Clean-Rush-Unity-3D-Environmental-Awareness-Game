# 🌿 Clean Rush — 3D Environmental Awareness Game

> A 3D Unity game developed as a university project, where the player collects trash scattered across a low-poly environment to clean nature and fight pollution.

**Repository:** [Clean-Rush-3D-Unity-Cleaning-Game](https://github.com/am11iin/Clean-Rush-3D-Unity-Cleaning-Game)

---

## 📌 Table of Contents

1. [About the Project](#about-the-project)
2. [University Context](#university-context)
3. [Game Concept](#game-concept)
4. [Gameplay](#gameplay)
5. [Technologies Used](#technologies-used)
6. [Project Structure](#project-structure)
7. [⚠️ Missing Assets — Important Notice](#️-missing-assets--important-notice)
8. [Opening the Project in Unity](#opening-the-project-in-unity)
9. [Running the Executable Build](#running-the-executable-build)
10. [Demo Video](#demo-video)
11. [Educational Objectives](#educational-objectives)
12. [Author](#author)
13. [License](#license)

---

## About the Project

**Clean Rush** is a 3D game built with Unity and C#. The player navigates a low-poly 3D world — including a village, a natural landscape, and a city environment — and collects pieces of trash scattered across the ground. The goal is to clean the entire area within a given time limit, while avoiding obstacles that slow progress.

The game was designed with a clear environmental message: pollution is a real and urgent problem, and individual actions — even small ones — can make a difference.

---

## University Context

This project was developed as part of an academic assignment. It is intended to demonstrate practical skills in:

- 3D game development using the Unity engine
- Object-oriented programming in C#
- Game design, scene construction, and UI integration
- Use of third-party assets and package management in Unity

The game is entirely non-commercial and was created for educational purposes only.

---

## Game Concept

The theme of **Clean Rush** is **environmental awareness**. The game places the player inside a polluted world and gives them a direct role in cleaning it. By making the player actively participate in the act of trash collection, the game aims to create a sense of responsibility and empathy toward the environment.

The environments in the game — a village, a nature area, and a city — represent the real-world spaces most affected by littering and pollution. The visual contrast between a clean and a polluted environment is intentional: it reinforces the game's core message.

---

## Gameplay

- The player controls a character in a 3D environment using keyboard input.
- Trash items are scattered across the map. The player walks over or near them to collect them.
- A **timer** counts down — the player must collect all trash before time runs out.
- **Obstacles** patrol the environment and must be avoided. Colliding with them triggers a penalty or game-over state.
- The game tracks a **score** based on the amount of trash collected.
- A **camera system** follows the player and supports switching between views.
- The game includes a **Main Menu** scene and transitions between levels.
- Background **music and sound effects** enhance immersion.

### Scripts Overview

| Script | Role |
|---|---|
| `GameManager.cs` | Controls overall game state, score, and win/lose conditions |
| `TrashItem.cs` | Defines behavior for individual trash objects |
| `TrashSpawner.cs` | Manages spawning of trash across the environment |
| `Mover.cs` / `MoverLevelTwo.cs` | Handles player movement for each level |
| `Timer.cs` | Manages the countdown timer |
| `CameraFollow.cs` | Makes the camera follow the player |
| `CamSwitch.cs` | Handles camera view switching |
| `ObstacleCollision.cs` | Detects player-obstacle collisions |
| `ObstaclePatrol.cs` | Controls obstacle movement patterns |
| `PlayerCollisionForwarder.cs` | Forwards collision events to the game manager |
| `TriggerZone.cs` | Detects when the player enters a specific zone |
| `MainMenu.cs` | Controls the main menu UI and scene transitions |

---

## Technologies Used

| Technology | Purpose |
|---|---|
| **Unity** | Game engine and editor |
| **C#** | Game logic and scripting |
| **TextMesh Pro** | UI text rendering (score, timer, menus) |
| **Unity Asset Store** | 3D models, characters, environments, props |
| **Unity Standard Assets** | Character controllers and physics utilities |
| **Audio (.mp3)** | Background music and sound effects |

---

## Project Structure

```
Clean-Rush-3D-Unity-Cleaning-Game/
│
├── Packages/                        # Unity package manifest
├── ProjectSettings/                 # Unity project configuration
│
├── Scenes/                          # Game scenes (Main Menu, Level 1, Level 2)
│
├── [C# Scripts]
│   ├── GameManager.cs
│   ├── TrashItem.cs
│   ├── TrashSpawner.cs
│   ├── Mover.cs
│   ├── MoverLevelTwo.cs
│   ├── Timer.cs
│   ├── CameraFollow.cs
│   ├── CamSwitch.cs
│   ├── ObstacleCollision.cs
│   ├── ObstaclePatrol.cs
│   ├── PlayerCollisionForwarder.cs
│   ├── TriggerZone.cs
│   └── MainMenu.cs
│
├── [Audio Files]
│   ├── cartoon-intro-13087.mp3
│   ├── epic-drums-stomps-and-claps-ig-version-loop-01-7223.mp3
│   ├── suspend-sound-113941.mp3
│   └── ticking-clock_1-27477.mp3
│
├── GOLD.asset                        # Custom Unity asset
├── MyController.controller           # Animation controller
├── trash rush background image.jpg   # UI background image
│
└── [Missing Asset Folders — see section below]
    ├── Low_Poly_Mini_Village/
    ├── Simple city plain/
    ├── SimpleLowPolyNature/
    ├── SpaceZeta_PlasticTrashBins/
    ├── Standard Assets/
    ├── Supercyan Character Pack Animal People Sample/
    ├── Supercyan Character Pack Free Sample/
    └── TextMesh Pro/
```


---

## ⚠️ Missing Assets — Important Notice

### Why are some folders missing?

Several asset folders are **not included** in this repository due to **GitHub's file size limitations**. GitHub restricts individual file sizes to 100 MB and recommends keeping repositories well below that threshold. The Unity Asset Store packages used in this project contain large 3D models, textures, and animations that exceed these limits and cannot be uploaded directly.

The presence of `.meta` files (e.g., `Low_Poly_Mini_Village.meta`) in the repository confirms that these folders existed in the original project but had to be excluded from the upload.

### Missing Asset Folders

| Asset Folder | Source |
|---|---|
| `Low_Poly_Mini_Village` | Unity Asset Store |
| `Simple city plain` | Unity Asset Store |
| `SimpleLowPolyNature` | Unity Asset Store |
| `SpaceZeta_PlasticTrashBins` | Unity Asset Store |
| `Standard Assets` | Unity (Legacy Package) |
| `Supercyan Character Pack Animal People Sample` | Unity Asset Store |
| `Supercyan Character Pack Free Sample` | Unity Asset Store |
| `TextMesh Pro` | Unity Package Manager |

### How to Restore the Missing Assets — Step by Step

If you want to open and run the project in the Unity Editor, follow these steps to restore the missing content:

**Step 1 — Open Unity Hub**
Launch Unity Hub and make sure you have a compatible version of Unity installed (Unity 2021 or later is recommended).

**Step 2 — Clone and Open the Project**
Clone this repository to your local machine, then open the project folder from Unity Hub by clicking **"Add"** and selecting the project directory.

**Step 3 — Check the Console for Errors**
Once the project opens, go to **Window → Console**. Unity will display errors for any missing scripts, prefabs, materials, or textures. Note which assets are referenced but not found.

**Step 4 — Install TextMesh Pro**
Open **Window → Package Manager**. Search for **TextMesh Pro** and click **Install**. After installation, go to **Window → TextMeshPro → Import TMP Essential Resources** to restore UI text components.

**Step 5 — Download Missing Assets from the Unity Asset Store**
Open **Window → Asset Store** (or visit [assetstore.unity.com](https://assetstore.unity.com)) and search for each missing asset by name. Most of the assets listed above are available for free. Add them to your Unity account and import them directly into the project via the Package Manager under **"My Assets"**.

**Step 6 — Import the Assets**
After downloading, go to **Window → Package Manager → My Assets**, locate each package, and click **Import**. Place the imported content into the `Assets/` folder, matching the original folder names listed above.

**Step 7 — Handle Unavailable or Renamed Assets**
If a specific asset is no longer available on the Asset Store, replace it with a free equivalent that serves the same purpose (e.g., a similar low-poly nature pack or character model). After importing the replacement, manually reconnect any broken prefab references, materials, or textures in the Unity Inspector by dragging the new assets into the appropriate fields.

**Step 8 — Reinstall Standard Assets (if needed)**
Standard Assets is a legacy Unity package. If it is not available through the Package Manager, download it from the [Unity Archive](https://unity3d.com/unity/legacy-packages) and import it manually.

After completing these steps, all console errors should be resolved and the project should run correctly in the Unity Editor.

---

## Opening the Project in Unity

1. Clone this repository:
   ```
   git clone https://github.com/am11iin/Clean-Rush-3D-Unity-Cleaning-Game.git
   ```
2. Open **Unity Hub**.
3. Click **"Add"** and select the cloned project folder.
4. Open the project with a compatible Unity version (Unity 2021 LTS or later recommended).
5. Follow the [Missing Assets](#️-missing-assets--important-notice) steps above before running any scene.
6. Once all assets are restored, open the **Main Menu** scene from the `Scenes/` folder and press **Play**.

---

## Running the Executable Build

A standalone Windows build of the game is provided under the name **Zéro Déchet.exe**.

### Required Files

To run the game, all of the following files and folders **must be kept together in the same directory**:

```
📁 Build Folder/
├── Zéro Déchet.exe          ← Main executable
├── Zéro Déchet_Data/        ← Game data (scenes, assets, scripts)
├── UnityPlayer.dll           ← Unity runtime library
└── MonoBleedingEdge/         ← Mono scripting runtime
```

> ⚠️ **Do not move or delete any of these files or folders.** The executable will not launch correctly if any component is missing or placed in a different location.

### How to Run

1. Download the full build folder.
2. Ensure all four items listed above are present in the same directory.
3. Double-click **Zéro Déchet.exe** to launch the game.
4. No installation is required.

---

## Demo Video

A gameplay demonstration video was recorded for this project.

The video file is **not included in this repository** because it exceeds GitHub's file size limits.

📽️ **Watch the demo here:** :

---

## Educational Objectives

This project was developed with the following learning goals:

- Apply object-oriented programming principles in a real-time interactive application
- Design and implement game mechanics including collision detection, timers, and scoring
- Structure a Unity project using multiple scenes, scripts, and asset packages
- Develop a game with a meaningful social and environmental message
- Practice version control and project documentation for an academic submission

The environmental theme — centered on waste collection and pollution awareness — was chosen to make the project both technically educational and socially relevant.

---

## Author

Developed by **am11iin** as part of a university course in computer science / game development.

🔗 GitHub: [https://github.com/am11iin](https://github.com/am11iin)

---

## License

This project is intended **exclusively for academic and educational purposes**. It is not licensed for commercial use, redistribution, or modification outside of an educational context.

All third-party assets used in this project remain the property of their respective creators and are subject to their original licenses (Unity Asset Store Terms of Service).

---

*README written for academic submission — Clean Rush, University Project.*
