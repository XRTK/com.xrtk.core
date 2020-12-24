# The Camera System

The camera system is the entry point for any platform and sets up the working environment required for that platform.

This is broken up in to THREE distinct components:

## Mixed Reality Root configuration system

In the Root configuration, the specific implementation for the camera system is selected, which defaults to the XRTK implementation for the camera system.  In most cases this does not need to be touched, however, advanced users can replace it with their own system if they wish.

![](/images/Configuration/CameraSystem/CameraSystemProfile.png)

## Camera System platform configuration

The Camera system for the XRTK allows for different implementations to be utilized for different runtime or build platforms. Each platform provided by the XRTK includes a default camera system configuration for that platform which can be customized if you wish:

![](/images/Configuration/CameraSystem/CameraSystemSettings.png)

The configuration holds:

* The default configuration - a Camera profile to be used where none is specified, a fallback default
* Additional Camera Profile variants, which specifies the Name, Type, Camera Profile and platforms this variant is meant to run on.

> For more detail on the [Platforms](08-platform-framework.md) that are available to configure, see the [XRTK Platform Framework](08-platform-framework.md) documentation.

## Camera Profile

Each specific camera profile (as shown below) holds the configuration options to control the camera settings for the XRTK environment.  Please note, that some platforms may provide additional options specific to that platform as required.

![](/images/Configuration/CameraSystem/CameraSystemOptions.png)


## Further notes

For the most part, you should not need to alter the camera system configuration unless required, or you wish to manually add a new Camera System platform manually (Each platform should automatically install the required Camera System for each platform)

---

### Related Articles

* [XRTK Platform Framework](08-platform-framework.md)

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
