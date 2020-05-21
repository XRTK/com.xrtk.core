# Welcome to the Mixed Reality Toolkit

![](/images/Branding/XRTK_Logo_1200x250.png)

The Mixed Reality Toolkit's primary focus is to make it extremely easy to get started creating Mixed Reality applications and to accelerate deployment to multiple platforms from the same Unity project.

## Build Status

| Modules | Azure Pipelines | OpenUpm |
|---|---|---|
|XRTK-Core|[![Build Status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/com.xrtk.core?branchName=master)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=44&branchName=master)|[![openupm](https://img.shields.io/npm/v/com.xrtk.core?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.core/)|
|[SDK](https://github.com/XRTK/SDK)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.SDK%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=23)|[![openupm](https://img.shields.io/npm/v/com.xrtk.sdk?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.sdk/)|
|[ARCore](https://github.com/XRTK/ARCore)|[![Build Status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/com.xrtk.arcore?branchName=master)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=56&branchName=master)||
|[Etee](https://github.com/XRTK/Etee)|[![Build Status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/com.xrtk.etee?branchName=master)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=54&branchName=master)||
|[Lenovo](https://github.com/XRTK/Lenovo)|[![Build Status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/com.xrtk.lenovo?branchName=master)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=53&branchName=master)||
|[Lumin](https://github.com/XRTK/Lumin)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.Lumin%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=29)|[![openupm](https://img.shields.io/npm/v/com.xrtk.lumin?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.lumin/)|
|[Nreal](https://github.com/XRTK/Nreal)|[![Build Status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/com.xrtk.nreal?branchName=master)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=52&branchName=master)||
|[Oculus](https://github.com/XRTK/Oculus)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.Oculus%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=30)|[![openupm](https://img.shields.io/npm/v/com.xrtk.oculus?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.oculus/)|
|[Ultraleap](https://github.com/XRTK/Ultraleap)|[![Build Status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/com.xrtk.ultraleap?branchName=master)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=51&branchName=master)||
|[WMR](https://github.com/XRTK/WindowsMixedReality)|[![Build status](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_apis/build/status/XRTK.WMR%20Master%20Build)](https://dev.azure.com/xrtk/Mixed%20Reality%20Toolkit/_build/latest?definitionId=21)|[![openupm](https://img.shields.io/npm/v/com.xrtk.wmr?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.wmr/)|

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

[![Discord](https://cdn0.iconfinder.com/data/icons/free-social-media-set/24/discord-128.png)](https://discord.com/invite/zYyFKfX)

### [Come join us on Discord!](https://discord.com/invite/zYyFKfX)

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
