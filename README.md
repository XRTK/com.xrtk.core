# Welcome to the Mixed Reality Toolkit

Developing Mixed Reality Applications in Unity is hard, and today we're proud to announce the formation of the XRTK (pronounced “Mixed Reality Toolkit”). This is a direct fork of the [Microsoft Mixed Reality Toolkit (MRTK)](https://github.com/Microsoft/MixedRealityToolkit-Unity).

There are many [developers](/CONTRIBUTORS.md) who are frustrated with the current state of both game and general application development within the Mixed Reality ecosystem: a quickly developing market that encompasses the whole spectrum from Mobile Augmented Reality to high end Virtual Reality.

Compounded with the fact that developing for an emerging tech markets can be time consuming, expensive, and fraught with peril. There are many complexities with targeting multiple platforms and no single framework currently available is able to fulfil this in a true Mixed Reality approach (covering AR / XR / VR completely). This results in developers and enterprises having to build a multitude of solutions on the many SDK's provided by suppliers to fulfil the needs of the business.

To improve this situation, our vision is simple, to provide a complete cross-platform solution for AR/XR/VR development that supports three different developer skill levels:

- **Beginner** No Coding Required: Perfect for artists, Hackathons, and Quick Prototyping.
- **Intermediate** Customizable: The framework is flexible enough so coders can customize what they need to cover edge cases with ease.
- **Advanced** Extensible: The framework is easy to extend and modify to add additional custom services to meet specific criteria and needs.

Our philosophy is to enable developers to focus on building content and structure and not have to worry about the underlying complexities for supporting multiple platforms in order to build it everywhere and on each device as required.  In Short, built it once and ship it everywhere with as little effort as possible.

We’d like to invite all the major hardware vendors to help guide their platform specific implementations, from Microsoft’s Windows Mixed Reality and Magic Leap’s Lumin OS to Google’s ARCore and Apple’s ARKit.  Including any upcoming Mixed Reality capable devices that would like to be included for adoption.

## [Join us on Slack](https://holodevelopersslack.azurewebsites.net/)

There are many channels on Slack, covering the wide variety of Mixed Reality platforms out there, as well as a dedicated XRTK channel for discussing XRTK implementations.

## Build Status

| Branches | Windows Build Agent | Mac Build Agent |
|---|---|---|
|Master|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/Mixed%20Reality%20Toolkit-CI)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=2)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/Master%20Build%20Pipeline%20-%20Hosted%20macOS)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=4)|

## Supported Platforms

- [x] Linux
- [x] OSX Standalone
- [ ] iOS
  - [ ] ARKit
- [ ] Android
  - [ ] ARCore
- [ ] WebAssembly
  - [ ] WebVR
  - [ ] WebXR
- [x] Windows Standalone
  - [x] Open VR
  - [ ] Steam VR
- [x] [Windows Mixed Reality](https://github.com/XRTK/WindowsMixedReality)
  - [x] HoloLens
  - [x] Windows Mixed Reality HMDs
  - [ ] HoloLens 2
- [ ] [Lumin (aka Magic Leap)](https://github.com/XRTK/Lumin)

## Installation

### 1. Unity's Package Manager (upm)

Find `Packages/manifest.json` in your project and edit it to look like this:

```json
{
  "dependencies": {
    "com.xrtk.core": "https://github.com/XRTK/XRTK-Core.git#0.1.0",
    ...
  },
}
```

### 2. Download unity package

[Download the latest unity package from the releases](https://github.com/XRTK/XRTK-Core/releases) and drag it into yor editor's project window.

## Roadmap

- [ ] [Project Setup](https://github.com/XRTK/XRTK-Core/projects/1)
- [ ] [Docs, Demos, & Tutorials](https://github.com/XRTK/XRTK-Core/projects/2)


## FAQ

### How do I get started quickly?

While we are still preparing XRTK specific documentation and guidance, all the current Tutorials and Videos related to the MRTK will also work for the XRTK (which some subtle differences in screens).

Most notably:

- [Mixed Reality Toolkit Quickstart](https://www.youtube.com/watch?v=-ODnfcv5Rzg) video session demonstrating getting started with the XRTK and building your project for multiple platforms.
- [Getting Started With The XRTK](README.md) (coming soon) Documented walk-through for creating your first project.
- [Mixed Reality Configuration Guide](README.md) (coming soon) Screen by screen walk through of the major MRTK configuration screens (some will vary from the XRTK, but similar enough to get the picture)

### What is the relationship between the XRTK and Microsoft's MRTK SDK

We work very closely with the MRTK to evolve their road-map to ensure it is the best it can be.  When new features are added to the MRTK, then being a fork of the MRTK, the XRTK project can easily port those features over easily (for instance the upcoming HL2 support and hand tracking).
In a lot of cases, there will be many extensions that will simply work seamlessly on both MRTK and XRTK without change, such as the Light Estimation extension released in January 2019.

It is our intention to always cooperate with the MRTK as they have similar goals, we just different aspirations to the developers adoption strategy.

### How do I migrate from the MRTK to the XRTK?

All the project GUIDs have been regenerated so there will not be conflicts between assets.

To update any script references do a global find and replace for the following:

- `Microsoft.MixedReality.Toolkit` -> `XRTK`
- Check for any missing scripts in prefabs as e.g. `Pointer Click Handler, Canvas Utility, Mixed Reality Line Data Provider,...` has updated namespace

### How do I migrate from the HoloToolkit to the XRTK?

Currently there isn't a direct path from the HTK to the XRTK, also Microsoft are still in the process of defining an "upgrade guide" for HTK -> MRTK upgrade.  Once available we shall update that here with similar guidance.
