# Welcome to the Mixed Reality Toolkit

Developing Mixed Reality Applications in Unity is hard, and today I’m proud to announce the formation of the XRTK (pronounced “Mixed Reality Toolkit”). This is a direct fork of the [Microsoft Mixed Reality Toolkit (MRTK)](https://github.com/Microsoft/MixedRealityToolkit-Unity). We didn’t reach this decision to fork lightly but felt it was an important step in making a true open source framework where all companies can contribute, collaborate, and empower the developers that write software on their various platforms.

I know there are many [developers](/CONTRIBUTORS.md) who are frustrated with the current state of both game and general application development within the Mixed Reality ecosystem: a quickly developing market that encompasses the whole spectrum from Mobile Augmented Reality to high end Virtual Reality.

Compounded with the fact that developing for an emerging tech market can be time consuming, expensive, and fraught with peril, many developers are finding themselves forced into abandoning cross-platform development strategies and instead are writing their applications for a single niche platform. At the of end the day, this isn’t a very good business model and can be very difficult to make a profit with it. Even worse, at a time when we need to be showing the world how Mixed Reality can make their lives better, we are getting tangled up in the minutiae of platform implementation.

To improve this situation, our vision is to provide a complete cross-platform solution for AR/XR/VR development that supports three different developer skill levels:

- **Beginner** No Coding Required: Perfect for artists, Hackathons, and Quick Prototyping.
- **Intermediate** Customizable: The framework is flexible enough so coders can customize what they need to cover edge cases with ease.
- **Advanced** Extensible: The framework is easy to extend and modify to add additional custom services to meet specific criteria and needs.

Our philosophy is write it once, build it everywhere. We’d like to invite all the major hardware vendors to help guide their platform specific implementations, from Microsoft’s Windows Mixed Reality and Magic Leap’s Lumin OS to Google’s ARCore and Apple’s ARKit.

# Build Status

| Branches | Windows Build Agent | Mac Build Agent | Linux Build Agent |
|---|---|---|---|
|Master|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/Mixed%20Reality%20Toolkit-CI)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=2)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/Master%20Build%20Pipeline%20-%20Hosted%20macOS)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=4)| Not Setup |

# Supported Platforms

- [ ] Android
    - [ ] ARCore
- [ ] OSX Standalone
- [ ] iOS
    - [ ] ARKit
- [ ] Linux
- [ ] Lumin
- [ ] WebAssembly
    - [ ] WebVR
    - [ ] WebAR
- [x] Windows Standalone
    - [x] Open VR
    - [ ] Steam VR
- [x] Windows Mixed Reality

# FAQ

## How do I migrate from the MRTK to the XRTK?

All the project GUIDs have been regenerated so there will not be conflicts between assets.

To update any script references do a global find and replace for the following:
- `Microsoft.MixedReality.Toolkit` -> `XRTK`
- Check for any missing scripts in prefabs as e.g. `Pointer Click Handler, Canvas Utility, Mixed Reality Line Data Provider,...` has updated namespace

# Roadmap

- [x] The core service locator
- [x] The core interface service contracts
- [x] The core definitions for enums, structs, and configuration profiles
- [x] Unit tests for core features
- [x] Finish setting up main repository docs
    - [x] Main Readme
    - [x] Coding Guidelines
    - [x] [Contributor List](/CONTRIBUTORS.md)
- [ ] Setup docfx website
- [ ] A simple package manager for submodules
    - [ ] Should interface and work with Unity's built in package manager
