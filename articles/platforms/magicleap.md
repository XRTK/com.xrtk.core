# Magic Leap

The Magic Leap platform provides a wide range of capabilities able to run on Magic leap devices.

> As the Magic Leap platform **REQUIRES** the new XR Plugin Management system, this means a Magic Leap project has to be separate from other platforms.  This will be resolved in the near future once the other platforms have been updated.
> However, you can still build the project for other platforms and simply copy the assets folder to your Magic Leap project to build and run. The XRTK Toolkit remains the same.

## Requirements

To develop for the Windows Mixed Reality platform, you will need:

* A Windows 10 PC
* Visual Studio 2019 community edition or greater
* The Magic Leap Developer Hub installed
* The XRTK.SDK package installed (first)
* The XRTK.Lumin package (once a Mixed Reality Scene has been setup)
* The Unity XR Subsystem for Magic Leap

It is advantageous to also have a Magic Leap headset, however the Magic Leap Developer Hub also provides simulation capabilities for testing.

## Platform considerations

* Please check the Magic Leap Developer Hub guidance for correctly setting up your machine for Unity development


## Capabilities

The following capabilities are currently available for the Windows Mixed Reality platform

1. Headset Tracking
2. Magic Leap controller
3. Spatial Awareness

The following capabilities are not currently available or are in development

1. Magic Leap Hands implementation

## Quickstart

1. Create a new Unity Project
2. Switch to the `Lumin` platform in `Build Settings`
3. Setup the Unity XR settings in "Edit -> Project Settings -> XR Plugin Management" for `Lumin`
4. Add the XRTK UPM registry to Unity by Opening the Unity Package Manager (`Window -> Package manager`), selecting the `Advanced` drop down and clicking on `Advanced Project Settings`, then Adding the following details:
    > Name: XRTK
    > URL: http://upm.xrtk.io:4873
    > Scope(s): com.xrtk
5. Return to the Unity Package manager and select `My Registries` in the Sources) dropdown (next to the `+` symbol)
6. Select the `XRTK.SDK` and click `Install`
7. Close the Unity Package Manager and return to your scene
8. Select or create the scene you want to create Mixed Reality Content in
9. Select `Mixed Reality Toolkit -> Configure` in the Unity Menu. THis will update your scene and add the MixedRealityToolkit instance.
10. Save your scene and return to the Unity Package Manager
11. Install the `XRTK.Lumin` platform
12. When prompted, install the Lumin platform configuration in to your current project


Provided you have configured the XR Plugin Management correctly, the project will run.

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
