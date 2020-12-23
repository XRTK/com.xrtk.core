# The Input System

The Input System may look daunting, however, it is easy to master once the principles for how the system works.
In short, the Input system manages the flow of data from inputs such as controllers, headsets and more, then organizes all the data in to XRTK components for distributing through your project.  This greatly reduces the complexity of dealing with cross-platform input from many sources.

In short, the only thing your project needs to handle are:

* What things can happen in your project (what we call Input Actions)
* And, what should happen then those things happen

## Mixed Reality Root configuration system

In the Root configuration, the specific implementation for the camera system is selected, which defaults to the XRTK implementation for the Input system.  In most cases this does not need to be touched, however, advanced users can replace it with their own system if they wish.

![](../images/Configuation/InputSystem/InputSystemProfile.png)

## Input System platform configuration

The core settings for the Input system allow the configuration of various elements which alter the behavior of the Input system, these include:

* [Global Pointer Settings](#global-pointer-settings)
* [Global Hand Settings](#global-hand-settings)
* [Input Actions](#input-actions)
* Speech Commands
* Gestures 
* [Controller Actions](#controller-actions)
* [Controller Definitions](#controller-definitions)

These are all grouped together under a singe Input system profile for easy reuse between projects.

![](../images/Configuation/InputSystem/InputSystemSettings.png)

## Global Pointer Settings

Pointers provide an invisible line between your controller (or head) in to a scene to interact with other objects. These default settings provide the global defaults for the Distance, Physics layer masks and colours users by the pointers, as well as whether they are drawn in the scene or not. 

![](../images/Configuation/InputSystem/GlobalPointerSettings.png)

## Global Hands Settings

Providing default settings that control both the rendering and physical nature of hands. This also holds the default pose configuration available to hands while in use in the environment.

![](../images/Configuation/InputSystem/GlobalHandSettings.png)

## Input Actions

Input Actions are a crucial element of cross-platform XR development.  With the increasing number of controllers, inputs, buttons and sensors being made available to devices, it is not practical to keep adding more complexity within your Mixed Reality solution.

Input Actions solve this by simply defining logical actions or events that can happen in your solution, e.g. Grabbing, clicking, tapping, pulling, etc and using them within your project so that when that event is received, you can perform the action related to that action.

The list here simply defines what actions are available in your solution and the kind of data expected when that action occurs, e.g. A boolean for a button (on/off), a range from a trigger or a full 6dof movement.

> See [Controller Actions](#controller-actions) for how these Actions are then mapped to the various controllers you wish to support.

![](../images/Configuation/InputSystem/InputActions.png)

## Controller Actions

![](../images/Configuation/InputSystem/ShortcutControllerActionMappings.png)
![](../images/Configuation/InputSystem/ConfiguredControllerDataProviders.png)
![](../images/Configuation/InputSystem/ControllerDataProvider.png)
![](../images/Configuation/InputSystem/ControllerDataProviderProfile.png)
![](../images/Configuation/InputSystem/ControllerProfile.png)

## What you'll need

List previous steps if needed and a short list of required materials or skills.

## Getting Started

1. Step 1
2. Step 2

---

### Related Articles

* [Article 1]()
* [Article 2]()
* [Article 3]()

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
