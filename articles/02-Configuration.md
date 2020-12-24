# Configuring your project

One of the goals of the Mixed Reality Toolkit was to greatly simplify how a project is configured, removing the need to hunt and search for specific objects and tweak configuration to change how a scene works, by consolidating the majority of the projects configuration in to a few top-level screens:

![Mixed Reality Toolkit Configuration](../images/Configuration/MixedRealityToolkitConfiguration.png)

While initially this may look a little complex, once a basic understanding is known, it is fairly easy to navigate.

> **Note:** We are currently taking feedback on ways to improve the configuration navigation for the Mixed Reality Toolkit.  Please feel free to suggest options in the XRTK Discord channel or raise an [RFI request](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

When profiles are created by the XRTK, they are then placed in the following folder:

> `Assets\XRTK.Generated`

This path can be customized by updating your preference in the preferences window.

## Overview of individual systems

The Mixed Reality Toolkit will only ever have a single active configuration profile running at any time, which is referenced on the `MixedRealityToolkit` GameObject in your configured scene.

![Main Configuration Profile](../images/Configuration/MixedRealityProfileView.png)

From here you can configure all the core Systems and their individual profile settings:

* [Camera System](systems/01-camera-system.md)
* [Input System](systems/02-input-system.md)
* [Boundary System](systems/03-boundary-system.md)
* [Teleport System](systems/04-teleporting-system.md)
* [Spatial Awareness System](systems/05-spatial-awareness-system.md)
* [Diagnostics System](systems/06-diagnostics-system.md)
* [Additional Service Providers](systems/07-additional-serivce-providers.md)

## Customizing your project

When you start a new project, we provide a copy each packages profiles with every system and service pre-configured and turned on, for a fully platform agnostic project. Each profile can be individually customized to your project's needs.

> **Note:** If you'd like to make a copy of a profile to test out a specific configuration you can easily clone and assign the profile using the `clone` button that appears to the right of each profile field.
![Clone Profile Button](../images/Configuration/MixedRealityProfileCloneButton.png)

---

### Related Articles

* [Getting Started](00-GettingStarted.md#getting-started-with-the-mixed-reality-toolkit)
* [Building and running your mixed reality application](00-GettingStarted.md#build-and-play)

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
