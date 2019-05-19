# How to download the Mixed Reality Toolkit

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/logo.png)

The XRTK provides many ways for users and developers to get access to the XRTK and the various extensions/platforms it provides.  These methods are tuned to use the common patterns most familiar to Unity developers, these include:

* [Automatic UPM Install](https://github.com/XRTK/XRTK-Core/releases) - Provides the core package only, using UPM (Unity Package management) to deliver other modules

* [Manual UPM install](#manual-upm-install) - Don't download anything, simply register the core XRTK project in your unity solution and let Unity download it for you

* Offline Downloadable asset (tbc) - Provides the entire XRTK in an asset for use in offline scenarios

* [GIT Submodules](#github-submodule) - For advanced developers, simply clone the project into your solution and update its submodules to get access to all platforms.

In this article, we will walk through each approach to get you up and running, starting with the simplest first.

## Automatic UPM Install

Out preferred deployment approach is to fully utilize Unity's native package management solution to incorporate the XRTK in your solution, akin to the other modules Unity provides out of the box.
This is the best / quickest and safest way to get XRTK in your solution and automatically updated.

Just download the [XRTK-Core.unitypackage](https://github.com/XRTK/XRTK-Core/releases) asset and import it into your project to start building solutions straight away.  This adds a XRTK seed, which automatically registers the XRTK with the Unity package manager and starts its download and installation.

For more instructions for how to get started, [click here](/docs/A01-XRTKUPMInstall.md)

## Manual UPM Install

If you prefer to do things yourself, you can simply edit the ***manifest.json*** file in your Unity project to register the XRTK and begin the download and installation into your project.

Simply add the following dependency into your manifest file, save and return to Unity to begin:

```json
{
  "dependencies": {
    "com.xrtk.core": "https://github.com/XRTK/XRTK-Core.git",
    ...
  },
}
```

For more details, check the XRTK [UPM instructions here]()

## Offline Asset
> **(Coming Soon)**

We will provide a full bulk asset containing all of the XRTK components and platforms. This will include the XRTK, the XRTK SDK and all current platforms.
Examples will also be available as a separate asset too.

Once downloaded, you can delete any parts of the project you do not intend to use but updating and maintaining the XRTK will be your own responsibility.

## GitHub submodule 
> **(advanced users)**

Advanced developers and those wishing to contribute to the XRTK also have the option to clone the XRTK repository into their project in a few ways.

You can either:

* Clone the development XRTK repository into a folder and start building your project from there, pulling down the configured submodules in that project.
* Clone the UPM branch in to a live project as a Submodule in your source, adding the additional submodules as required

For more information, you can follow [this guide here](https://www.rageagainstthepixel.com/expert-import-mrtk/)

---
If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)