# Windows Mixed Reality

The Windows Mixed Reality platform provides a wide range of capabilities able to run under Windows 10 machines with a Mixed Reality headset and the HoloLens 1 / 2 devices.

## Requirements

To develop for the Windows Mixed Reality platform, you will need:

* A Windows 10 PC
* Visual Studio 2019 community edition or greater
* The Mixed Reality Portal, installed and running (for Mixed Reality Headset)
* The XRTK.SDK package installed (first)
* The XRTK.WindowsMixedReality package (once a Mixed Reality Scene has been setup)

It is advantageous to also have a Mixed Reality headset, however the Mixed Reality Portal also provides simulation capabilities for testing.

For HoloLens development:

* The development tools are now included with the Windows 10 SDKs
* (Optionally) if you do not have a device, you can install the [HoloLens Emulator](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/platform-capabilities-and-apis/using-the-hololens-emulator)
* The [Hololens Emulator guide](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/platform-capabilities-and-apis/using-the-hololens-emulator) also covers the necessary steps to deploy to a HoloLens Device (or simulator)

## Platform considerations

* For OpenVR on Windows Mixed Reality, you simply need to ensure that `OpenVR` is one of the configured platforms in the Unity legacy XR settings for the `Standalone` platform
* For Windows Mixed Reality on UWP, you simply need to ensure that `Windows Mixed Reality` is one of the configured platforms in the Unity legacy XR settings for the `UWP` platform. This is also required for HoloLens development.


## Capabilities

The following capabilities are currently available for the Windows Mixed Reality platform

1. Headset Tracking
2. Boundary tracking
3. Windows Mixed Reality controllers (Windows 10 only)
4. HoloLens Hands implementation (HoloLens 2 only)
5. Spatial Awareness (HoloLens 1 & 2 only)

## Quickstart

1. Create a new Unity Project
2. Switch to the `UWP` platform in `Build Settings`
2. Setup the Legacy XR settings in "Edit -> Project Settings -> Player -> XR Settings" for `Windows Mixed Reality` to the `UWP` platform
3. Add the XRTK UPM registry to Unity by Opening the Unity Package Manager (`Window -> Package manager`), selecting the `Advanced` drop down and clicking on `Advanced Project Settings`, then Adding the following details:
    > Name: XRTK
    > URL: http://upm.xrtk.io:4873
    > Scope(s): com.xrtk
4. Return to the Unity Package manager and select `My Registries` in the Sources) dropdown (next to the `+` symbol)
5. Select the `XRTK.SDK` and click `Install`
6. Close the Unity Package Manager and return to your scene
7. Select or create the scene you want to create Mixed Reality Content in
8. Select `Mixed Reality Toolkit -> Configure` in the Unity Menu. THis will update your scene and add the MixedRealityToolkit instance.
9. Save your scene and return to the Unity Package Manager
10. Install the `XRTK.WindowsMixedReality` platform
11. When prompted, install the Windows Mixed Reality platform configuration in to your current project


Provided you have configured the Legacy XR settings for each platform correctly, the project will run.

For Windows 10

* Ensure the Mixed Reality Portal is running before playing
* For builds, you will need to open the Visual Studio project and package the solution for Windows 10

For HoloLens 1 / 2

* Ensure you have correctly setup the Device portal on the device and it is accessbile
* For builds, you will need to open the Visual Studio project and package the solution for HoloLens

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
