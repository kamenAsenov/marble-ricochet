# Marble Ricochet

Premium mobile puzzle game prototype built in Unity.

## Core idea

Pull the marble, release, bounce, and break all glass targets.

## Current features

- Slingshot aiming
- Ricochet preview
- Glass targets
- Metal bumpers
- Breakable glass
- Stars / par
- Coins
- Skins shop
- Rewarded ad placeholders
- Hints
- Progression
- 30 handmade levels
- Android build preparation

## Unity project folders tracked in Git

Tracked:

- `Assets/`
- `Packages/`
- `ProjectSettings/`

Ignored:

- `Library/`
- `Temp/`
- `Obj/`
- `Build/`
- `Builds/`
- `Logs/`

## Recommended workflow

Before applying a new update:

```bash
git status
git add .
git commit -m "Before update XX"
```

After applying and testing a new update:

```bash
git add .
git commit -m "Apply update XX"
git push
```

## Build

First target:

- Android APK internal test

Later:

- Google Play internal testing
- iOS/TestFlight when Mac/Xcode is available
