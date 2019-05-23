# Getting started with the Mixed Reality Toolkit

![](https://github.com/XRTK/XRTK-Core/raw/master/docs/logo.png)

The XRTK primary focus is to both make it extremely easy to get going from a new project and accelerate deployment to multiple platforms from the same project.

## Prerequisites

To get started with the Mixed Reality Toolkit you will need:

* [Visual Studio 2017+ (Community or full)](https://visualstudio.microsoft.com/downloads/)
* [Unity 2019.1+](https://unity3d.com/get-unity/download/archive)
* [Latest XRTK release](https://github.com/XRTK/XRTK-Core/releases)

> For Windows UWP projects, e.g. HL / HL2 / Windows 10
> * [Windows UWP SDK 18362+](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK)

## Upgrading from the HoloToolkit (HTK/MRTK v1)

There is not a direct upgrade path from the HoloToolkit to Mixed Reality Toolkit due to the rebuilt framework.  However, it is possible to import the XRTK into your project to begin the upgrade progress

* [Microsoft HoloToolkit Porting Guide](HTKToMRTKPortingGuide.md)

> Please refer to the Microsoft guide until we provide a specific XRTK guide as the frameworks are similar enough at the moment.


# Starting your new Mixed Reality project

One of the Mixed Reality Toolkit's primary goals was to ensure new projects can get up and running as fast as possible. To this end, all the default configuration ensures you can simply import, play and just run.

To get your first project up and running, the steps are as follows:

## 1. Create your new project (or start a new scene in your current project)

Setup your new (or open your existing) project to get ready for entering Mixed Reality.

> *Note* when creating a new project with Unity 2019, Unity gives you several templates to choose from. **Currently, the Scriptable Render Pipeline is not supported yet**, so the LWSRP, HDSRP and VRSRP projects are not compatible with MRTK projects.  Please stay tuned to the GitHub site for future announcements on SRP support.

## 2. Add the [Mixed Reality Toolkit](/docs/00-DownloadingTheXRTK.md) to your project

The Mixed Reality Toolkit is available via [multiple delivery mechanisms](/docs/00-DownloadingTheXRTK.md), primarily via the [Unity Package Management](/docs/) system.

> The SDK is optional but highly recommended for new users.  Once you have a feel for how the toolkit works, you can remove these safely if you are not using them.

Note that some prefabs and assets require TextMesh Pro, meaning you have to have the TextMesh Pro package installed and the assets in your project (Window -> TextMeshPro -> Import TMP Essential Resources).

> When installing via UPM, just be sure to wait until all the UPM packages have installed.  We're still looking for [feedback as to the optimal install process](https://github.com/XRTK/XRTK-Core/issues/173)

## 3. Configure your first Mixed Reality Toolkit scene

The toolkit has been designed so that there is just one object that is mandatory in your scene.  This is there to provide the core configuration and runtime for the Mixed Reality Toolkit (one of the key advantages in the new framework).

> **Important!**
> ---
> Before configuring your Mixed Reality Scene, ensure your scene is saved and included in the project *Build Settings* ***(File > Build Settings… )*** . Best to also restart Unity to be sure. 
> 
> There is a Unity bug which will crash Unity if you run **Configure** with an empty Build Settings list in your project.  We've logged this with Unity and will keep this page up to date.  After the crash, everything works perfectly without issue.
> 
> This does not affect projects built from source, or via submodules, so it's very confusing.  Basically, it's not us.

Configuring your scene is extremely simple by simply selecting the following from the Editor menu:
> Mixed Reality Toolkit -> Configure

![](/docs/images/01_01_MixedRealityConfigure.png)

Once this completes, you will see the following in your Scene hierarchy:

![](/docs/images/01_02_MixedRealityScene.png.png)

Which contains the following:

* Mixed Reality Toolkit - The toolkit itself, providing the central configuration entry point for the entire framework.
* MixedRealityPlayspace - The parent object for the headset, which ensures the headset/controllers and other required systems are managed correctly in the scene.
* The Main Camera is moved as a child to the Playspace - Which allows the playspace to manage the camera in conjunction with the SDK's
* UIRaycastCamera added as a child to the Main Camera - To enable seamless UI interactions through the toolkit

> The UIRaycastCamera is added today, but we are working to remove the dependency

> **Note** While working in your scene, **DON'T move the Main Camera** (or the Playspace) from the scene origin (0,0,0).  This is controlled by the Toolkit and the active SDK.
> If you need to move the player's start point, then **move the scene content and NOT the camera**!

### 5. Hit play

You are now ready to start building your Mixed Reality Solution, just start adding content and get building.
Switch to other platforms (ensure they have XR enabled in their player settings) and your project will still run as expected without change.

## Configuring your project

> **Note**
> We are currently taking feedback on ways to improve the configuration navigation for the XRTK.  Please feel free to suggest options in the XRTK Slack channel or raise an [RFI request](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

The Mixed Reality Toolkit configuration is all centralized on one place and attached to the MixedRealityToolkit object in your active scene.

![](/docs/images/01_03_MixedRealityActiveProfile.png)

Clicking on this profile will show the configuration screens for the Mixed Reality Toolkit:

![](/docs/images/01_04_MixedRealityProfileView.png)

From here you can navigate to all the configuration profiles for the MRTK, including:

> The "Default" profiles provided by the Mixed Reality Toolkit are locked by default, so when you view these in the inspector they will appear greyed out.  This is to ensure you always have a common default for any project.  We recommend you create your own profiles (see below) when you need to customize the configuration for your project.

* Main Mixed Reality Toolkit Configuration
* Camera Settings
* Input System Settings
* Boundary Visualization Settings
* Teleporting Settings
* Spatial Awareness Settings
* Diagnostics Settings
* Additional Services Settings
* Input Actions Settings
* Input Actions Rules
* Pointer Configuration
* Gestures Configuration
* Speech Commands
* Controller Mapping Configuration
* Controller Visualization Settings


As you can see there are lots of options available and more will come available as we progress through the beta.

When you start a new project, we provide a default set of configurations with every option turned on, styled for a fully cross-platform project.  These defaults are "Locked" to ensure you always have a common start point for your project and we encourage you to start defining your own settings as your project evolves.  For this we provide options to either:

* Copy the defaults into a new profile for you to start customizing it for your project
* Start afresh with a brand-new profile.

![](docs/images/01_05_MixedRealityProfileClone.png)

When profiles are created by the Toolkit, they are then placed in the following folder:

> "Assets\XRTK.Generated\CustomProfiles"

At each step in the configuration, you can choose to remove and create a new profile, or simply copy the existing settings and continue to customize:

![](/docs/images/01_06_MixedRealityProfileCloneButton.png)

### **[For more information on customizing the Configuration Profiles](MixedRealityConfigurationGuide.md)**
Please check out the [Mixed Reality Configuration Guide]() (TBC)

## Get building your project

Now your project is up and running, you can start building your Mixed Reality project.  

For more information on the rest of the toolkit, please check the following guides:

* [Mixed Reality Configuration Guide]() (Coming Soon)
* [Getting to know the Mixed Reality Toolkit Input System]() (Coming Soon)
* [Customizing your controllers in the MRTK]() (Coming Soon)
* [A walk through the UX components of the MRTK SDK]() (Coming Soon)
* [Using Solvers to bind your objects together]() (Coming Soon)
* [Creating interactions between the player and your project]() (Coming Soon)
* [Configuration Profile Usage Guide]() (Coming Soon)
* [Guide to building Registered Services]() (Coming Soon)
* [Guide to Pointers documentation]() (Coming Soon)

---
If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)
