# Welcome to the Mixed Reality Toolkit

![](/images/Branding/XRTK_Logo_1200x250.png)

The Mixed Reality Toolkit's primary focus is to make it extremely easy to get started creating Mixed Reality applications and to accelerate deployment to multiple platforms from the same Unity project.

## Build Status

| Modules | Build Agent |
|---|---|
|XRTK-Core|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/Mixed%20Reality%20Toolkit-CI)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=2)|
|[SDK](https://github.com/XRTK/SDK)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.SDK%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=23)|
|[WMR](https://github.com/XRTK/WindowsMixedReality)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.WMR%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=21)|
|[Lumin](https://github.com/XRTK/Lumin)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.Lumin%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=29)|
|[Oculus](https://github.com/XRTK/Oculus)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.Oculus%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=30)|

## [Getting Started](articles/00-GettingStarted.md)

- [Installing](articles/00-GettingStarted.md#adding-the-mixed-reality-toolkit-to-your-project)
- [Configuring](articles/00-GettingStarted.md#configure-your-base-scene)
- [Building](articles/00-GettingStarted.md#build-and-play)
- [Contributing](CONTRIBUTING.md)

## Overview

Developing Mixed Reality Applications in Unity is hard, and we know there are many [developers](./CONTRIBUTORS.md) who are frustrated with the current state of both game and general application development within the Mixed Reality ecosystem: a quickly developing market that encompasses the whole spectrum from Mobile Augmented Reality to high-end Virtual Reality.

To improve this situation, the Mixed Reality Toolkit's vision is simple, to provide a complete cross-platform solution for AR/XR/VR development that supports three different developer skill levels:

- **Beginner** No Coding Required: Perfect for artists, Hackathons, and Quick Prototyping.

- **Intermediate** Customizable: The framework is flexible enough so coders can customize what they need to cover edge cases with ease.

- **Advanced** Extensible: The framework is easy to extend and modify to add additional custom services to meet specific criteria and needs.

Our philosophy is to enable developers to focus on building content and structure and not have to worry about the underlying complexities for supporting multiple platforms in order to build it everywhere and on each device as required.  In Short, built it once and ship it everywhere with as little effort as possible.

We’d like to invite all the major hardware vendors to help guide their platform-specific implementations, from Microsoft’s Windows Mixed Reality and Magic Leap’s Lumin OS to Google’s ARCore and Apple’s ARKit.  Including any upcoming Mixed Reality capable devices that would like to be included for adoption.

## Chat with the community

We recently moved our main conversations regarding Mixed Reality Toolkit over to Discord, which allows us to do a lot more (and the chat/streaming there is awesome), but we keep a Mixed Reality Toolkit presence on Slack too, in order to retain links to our friends on there.

[![Discord](https://cdn0.iconfinder.com/data/icons/free-social-media-set/24/discord-128.png)](https://t.co/UeUSVjnoIZ?amp=1)

### [Come join us on Discord!](https://t.co/UeUSVjnoIZ?amp=1)

## Supported Platforms

By default, we support OpenVR on all platforms that support the standard, as well as Native SDK implementations for various vendors.

- [x] Windows Standalone
- [x] Linux
- [x] OSX Standalone
- [x] Open VR
- [ ] Open XR
- [x] [Windows Mixed Reality](https://github.com/XRTK/WindowsMixedReality)
    - [x] HoloLens
    - [x] Windows Mixed Reality HMDs
    - [-] HoloLens 2 (platform supported, hands / eyes pending)
- [x] [Lumin (aka Magic Leap)](https://github.com/XRTK/Lumin)
- [x] [Native Oculus API](https://github.com/XRTK/Oculus)
    - [x] Rift
    - [x] Quest
    - [ ] Go
- [ ] Native Steam VR
    - [ ] HTC Vive
    - [ ] Vive Index
- [ ] iOS
    - [ ] ARKit
- [ ] Android
    - [ ] ARCore
- [ ] WebAssembly
    - [ ] WebVR
    - [ ] WebXR