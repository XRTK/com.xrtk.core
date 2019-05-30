# How to download the Mixed Reality Toolkit UPM package

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/logo.png)

Unity (as of Unity 2018) now provides a full package management solution for delivering components in a safe and secure way in to your projects.  You no longer need to import external features as assets, they are referenced and downloaded in to a secure location accessible to your solution.

Sadly (at the time of writing), content developers cannot publish assets through the store using the Unity Package manager. So the XRTK needs to register directly in your project.

To enable this, the XRTK provides two ways to get it registered:

1: [Automatic Install](#automatic-download) - simply [download the latest seed package](https://github.com/XRTK/XRTK-Core/releases) from the GitHub release page, import it in to your project and off you go.

2: [Manual Install](#manual-download) - edit your UPM manifest in your project, add a single line, save and return to Unity to kick off the UPM download.

# Automatic Download

The simplest way to get XRTK in to your project is to download the XRTK seed asset (which hopefully will get published on the Unity asset store on release), this registers the XRTK project with your solution and enables the Unity Package Manager to download it to your project.

1. [Download XRTK seed asset](https://github.com/XRTK/XRTK-Core/releases)
2. [Import the XRTK Seed asset](#xrtk-seed-asset-install)
3. Profit...

For a little more detail, the sections below explain exactly what is happening.

## Unity Packages

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_01_UnityPackages.png)

Unity packages are separate to your project and act like references instead of raw code.  You can see them but not change them (at least not easily)
This is where the XRTK (by default) installs safely and securely.

## Unity Package Manager
![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_02_PackageManagerMenu.png)

You access the Unity Package manager in the editor from the *Unity -> Windows -> Package Manager* option in the menu, which brings up the package manager window.

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_03_PackageManager.png)

This lists all the packaged (by default) that Unity provides out of the box, items ticked are those already installed.

## XRTK Seed asset install

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_04_XRTKAutoInstallAsset.png)

As you can see above, there is minimal code in the seed asset, as this isn't the XRTK itself, It is simply an enabler to register itself with the Unity package manager, once you've installed it, the UPM will automatically download the full XRTK and remove the seed package, ensuring there is nothing cluttering up your project.

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_05_XRTKDialog.png)

# Manual Install

If you prefer to register the XRTK with your project yourself, then you simply need to add the XRTK reference in to your project's ***Manifest.json***

To locate the folder the manifest is in, simply right-click in your Unity project folder on the "Packages" folder as shown below, then select the "Show in Explorer" option:

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_06_LocatePackagesFolder.png)

In the packages folder, you will see the specific Manifest.json file which you will need to open for editing using your favorite tool ([We prefer VSCode as it's awesome](https://code.visualstudio.com/))

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_07_ManifestJSON.png)

Inside the Manifest file, you will see all the packages Unity provides "out of the box" listed:

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_08_ManifestDependandcies.png)

To this you simply need to add the extra entry for the XRTK as follows:

```json
{
  "dependencies": {
    "com.xrtk.core": "https://github.com/XRTK/XRTK-Core.git#upm",
    ...
  },
}
```

Once it's updated and you return to Unity, the packages will be refreshed and all the XRTK packages will be imported:

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_08_PackageReimport.png)

# Versioning

A powerful feature of the Unity package management system is that you can switch between versions of packages at any time including the XRTK.  As you can see below, on selecting the XRTK version in the Unity Package Manager window, you can select any "version" of the XRTK or even a specific branch on the development site if you wish.

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/Images/00_XRTKUPMInstall_10_XRTKVersioning.png)

This includes all the different XRTK packages, including:

* XRTK Core
* XRTK SDK
* XRTK platforms, including WMR / Lumin and others
* XRTK UPM extension (this is mandatory to enable connecting to the XRTK repository)

---
If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)