# UltraLeap Hands tracking (Advance information)

The UltraLeap platform is currently in development and targeting the XRTK 0.3 release.

## Requirements

To develop for the UltraLeap hands tracking platform, you will need:

* A Windows PC, Windows 7 or greater.
* The XRTK.SDK package installed (first)
* The XRTK.UltraLeap package (once a Mixed Reality Scene has been setup)
* A UltraLeap device or simulator

## Platform considerations

The Platform considerations will be updated once the platform is released

## Capabilities

The following capabilities are in development and likely to be available for the UltraLeap platform

1. UltraLeap hands controller Tracking

## Quickstart (preview, subject to change)

1. Create a new Unity Project
2. Add the XRTK UPM registry to Unity by Opening the Unity Package Manager (`Window -> Package manager`), selecting the `Advanced` drop down and clicking on `Advanced Project Settings`, then Adding the following details:

    > Name: XRTK
    >
    > URL: http://upm.xrtk.io:4873
    >
    > Scope(s): com.xrtk

3. Return to the Unity Package manager and select `My Registries` in the Sources) dropdown (next to the `+` symbol)
4. Select the `XRTK.SDK` and click `Install`
5. Close the Unity Package Manager and return to your scene
6. Select or create the scene you want to create Mixed Reality Content in
7. Select `Mixed Reality Toolkit -> Configure` in the Unity Menu. THis will update your scene and add the MixedRealityToolkit instance.
8. Save your scene and return to the Unity Package Manager
9. Install the `XRTK.UltraLeap` platform
10. When prompted, install the SteamVR platform configuration in to your current project
11. Setup the Unity XR settings in "Edit -> Project Settings -> XR Plugin Management" for `UltraLeap`

Provided you have configured the XR Plugin Management correctly, the project will run.

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
