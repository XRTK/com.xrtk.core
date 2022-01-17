# Welcome to the Mixed Reality Toolkit

![The Mixed Reality Toolkit](https://raw.githubusercontent.com/XRTK/XRTK-Core/development/images/Branding/XRTK_Logo_1200x250.png)

The Mixed Reality Toolkit's primary focus is to make it extremely easy to get started creating Mixed Reality applications and to accelerate deployment to multiple platforms from the same Unity project.

[![Discord](https://img.shields.io/discord/597064584980987924.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/7DR6QJE)

## Build Status

| Modules | Build Status | OpenUpm |
|---|---|---|
|com.xrtk.core|[![main](https://github.com/XRTK/com.xrtk.core/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/XRTK/com.xrtk.core/actions/workflows/build.yml)|[![openupm](https://img.shields.io/npm/v/com.xrtk.core?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.core/)|
|[com.xrtk.sdk](https://github.com/XRTK/com.xrtk.sdk)|[![main](https://github.com/XRTK/com.xrtk.sdk/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/XRTK/com.xrtk.sdk/actions/workflows/build.yml)|[![openupm](https://img.shields.io/npm/v/com.xrtk.sdk?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.sdk/)|
|[com.xrtk.lumin](https://github.com/XRTK/com.xrtk.lumin)|[![main](https://github.com/XRTK/com.xrtk.lumin/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/XRTK/com.xrtk.lumin/actions/workflows/build.yml)|[![openupm](https://img.shields.io/npm/v/com.xrtk.lumin?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.lumin/)|
|[com.xrtk.oculus](https://github.com/XRTK/com.xrtk.oculus)|[![main](https://github.com/XRTK/com.xrtk.oculus/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/XRTK/com.xrtk.oculus/actions/workflows/build.yml)|[![openupm](https://img.shields.io/npm/v/com.xrtk.oculus?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.oculus/)|
|[com.xrtk.ultraleap](https://github.com/XRTK/com.xrtk.ultraleap)|[![main](https://github.com/XRTK/com.xrtk.ultraleap/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/XRTK/com.xrtk.ultraleap/actions/workflows/build.yml)|[![openupm](https://img.shields.io/npm/v/com.xrtk.ultraleap?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.ultraleap/)|
|[com.xrtk.wmr](https://github.com/XRTK/com.xrtk.wmr)|[![main](https://github.com/XRTK/com.xrtk.wmr/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/XRTK/com.xrtk.wmr/actions/workflows/build.yml)|[![openupm](https://img.shields.io/npm/v/com.xrtk.wmr?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.xrtk.wmr/)|

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

Our philosophy is to enable developers to focus on building content and structure and not have to worry about the underlying complexities for supporting multiple platforms in order to build it everywhere and on each device as required.  In short, built it once and ship it everywhere with as little effort as possible.

We’d like to invite all the major hardware vendors to help guide their platform-specific implementations, from Microsoft’s Windows Mixed Reality and Magic Leap’s Lumin OS to Google’s ARCore and Apple’s ARKit.  Including any upcoming Mixed Reality capable devices that would like to be included for adoption.

## Chat with the community

We recently moved our main conversations regarding Mixed Reality Toolkit over to Discord, which allows us to do a lot more (and the chat/streaming there is awesome), but we keep a Mixed Reality Toolkit presence on Slack too, in order to retain links to our friends on there.

[![Discord](https://cdn0.iconfinder.com/data/icons/free-social-media-set/24/discord-128.png)](https://discord.gg/rJMSc8Z)

### [Come join us on Discord!](https://discord.gg/rJMSc8Z)

## Sponsors

The XRTK is an MIT-licensed open source project with its ongoing development made possible entirely by the support of these awesome sponsors and backers.

|Sponsors||
|---|---|
|<a href="https://www.vimaec.com/">![[VIM](https://www.vimaec.com/)](https://raw.githubusercontent.com/XRTK/XRTK-Core/development/images/Sponsors/vim_logo.jpg)</a>|VIM provides a universal format for fast BIM access for large and complex projects in the AEC industry.|

We use the donations for continuous active development by core team members, web hosting, and licensing costs for build tools and infrastructure.

## Supported Platforms

A major component of the Mixed Reality Toolkit is the Platform definitions that was employed to both accurately determine what platforms have been added to the scope of the project and also to determine when a platform is active/running.

## Current Platforms

- [Magic Leap (Lumin)](articles/platforms/magicleap.md)
- [Oculus](articles/platforms/oculus.md)
- OpenVR - Default runtime platform for Windows Standalone, no special consideration.
- [Ultraleap](articles/platforms/ultraleap.md)
- [Windows Mixed Reality (UWP)](articles/platforms/windowsmixedreality.md)

## In development

- [SteamVR](articles/platforms/steamvr.md)
- [etee](articles/platforms/etee.md)
- [WebXR](articles/platforms/webxr.md)

> Want to add a platform? Check out our new [Template Generator](articles/03-template-generator.md#platform-template-generation)!
