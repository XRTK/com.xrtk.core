# Choosing between XRTK and MRTK

The choice between the XRTK and Microsoft's base MRTK today is a personal choice on the development path you, the developer or enterprise, wants to take when building your Mixed Reality vision.

## TL;DR

Both projects were originally designed/architected and built by the same developers over the course of 2018. From 2019 on, they have diverged under different principles, namely:

* The XRTK is a community-based platform for building cross-platform Mixed Reality solutions (AR / VR / XR / MR) for a multitude of devices and runtime platforms.  Creating solutions to meet the need of a much more diverse set of requirements required by cross-platform solutions.

* The MRTK is currently only being developed to focus on the needs of Microsoft partners and new HL2 deployments, whilst still retaining the core multi-platform architecture it was designed with.  Microsoft (for the time being) must focus its scope to achieve those deliverables.

Both projects share a lot of the same base code and standards. As such, projects created in one framework are easily transferable to the other with minimal effort, the teams have ensured you are never locked into one framework, should you wish to switch.
There will be an increasing number of touchpoints as time goes by, but both teams are invested in keeping the two projects aligned to allow easy adoption of new features in either direction.

## Core Differences

Where the two frameworks truly diverge, is an adherence to the core architecture, which maintains a highly organized design, tuned to deliver the best performance within the Unity Game Engine (akin to Unity's new ECS/DOTS path). This architecture aims to break the chains to old monolithic approaches for building solutions in Unity.

### These are the main differences in the XRTK

| Difference | Comment |
|---|---|
| UPM (Unity Package Management), submodules and Unity asset delivery | XRTK offers a more Unity focused development channel than that of the enterprise-focused MRTK, utilizing Unity's built-in packaging system.  Additionally, we have streamlined the developer pattern aligned with how community devs interact.
| [Locator pattern](https://medium.com/@stephen_hodgson/the-mixed-reality-framework-6fdb5c11feb2) based approach | The original architecture called for only a single Singleton to break the chains of the past.  We encourage developers to follow us on this journey which will set you in good stead for Unity new ECS/DOTS pattern, but in a more Unity friendly way. |
| C# based service pattern utilizing strong interfaces | Everything in the XRTK adheres to the service pattern whereas the MRTK has diverged based on the requirements of its direct customers. |
| Separate repositories for extensions | The MRTK is a single repository, which has its uses, but limits it's flexibility when it comes to extensions and new platforms. The XRTK provides proper separation with multiple repositories to ensure that new functionality cannot break the core framework or new projects|
| Targeted education patterns for different audiences (Beginner / Intermediate / Advanced) | We are working closely with the community to drive educational content for all, ranging from the beginner to the intermediate and advanced user.  Even opening an RFI (Request for Information) template on Github to ask anything |
| ECS / DOTS style separation of data and logic from core systems (Not yet ECS compatible) | We are ensuring that most (if not all) your code is easily transferable to Unity's ECS pattern and eventually even provide an ECS compatible version of the XRTK, future proofing the toolkit with new Unity developments. |

> Note, this list will change over time as BOTH the XRTK and MRTK evolve.

### In layman's terms

The MRTK focuses on delivery to new and pre-existing partner solutions and as such, can be limited in the paths it can take.  XRTK is more open and can easily adapt and educate the community to build almost any solution without compromise.

## Supported Platforms

Platform | Description | XRTK Support | MRTK Support
---|---|---|---|
OpenVR (Windows) | Running OpenVR projects on Windows / Windows 10 | Any Supported *OpenVR device | Any Supported *OpenVR device
OpenVR (Android) | Running OpenVR projects on Android | <ul><li>Oculus GO</li><li>Oculus Quest</li></ul> | **Not Supported**
UWP | Universal Windows Platform (Windows 10) | <ul><li>Windows 10 WMR</li><li>HoloLens 1</li><li>HoloLens 2</li>(Hands pending)</ul> | <ul><li>Windows 10 WMR</li><li>HoloLens 1</li><li>HoloLens 2</li></ul>
Lumin / Magic Leap | Magic leap support | <ul><li>Magic Leap One</li></ul> | **Not Supported**
Oculus (Windows) | Running native Oculus API on Windows | <ul><li>Rift</li><li>Rift S</li></ul> | **Not Supported**
Oculus (Android) | Running native Oculus API on Android | <ul><li>Oculus Go</li><li>Oculus Quest</li></ul> | **Not Supported**
Steam (Windows) | Running native Steam API on Windows | <ul><li>Vive</li><li>Vive Pro</li>(planned)<li>Vive index (planned)</li></ul> | **Not Supported**
ARCore (Android) | ARCore support / integration on Android | planned | tbc
ARKit (iOS) | ARKit support / integration | planned | tbc
|||||

> *OpenVR* - we support all devices that can run OpenVR based builds, but only support controllers for Vive, Rift, Quest, Go and WMR currently

## MRTK / XRTK parity

The XRTK team still works closely with Microsoft to ensure that we maintain a level of parity between the two implementation patterns, ensuring that new Microsoft developments (such as HL2, hand tracking, etc.) are incorporated into the XRTK, in a fashion that complements the XRTK. Most features as they are ported will require some enhancement to comply to the XRTK architecture, but the Developer Product adoption will remain as close as we can to aid porting between frameworks. (in other words, we preserve the runtime whilst finely tuning the backend)

## Feedback

Being community-based, the XRTK is more open to adopting and reacting to feedback, whilst the MRTK has to ensure it complies with its core audience first when prioritizing updates.  That being said, the MRTK team is trying to remain as open as it can to engage with developers.

Have a question, [just log a new RFI Issue](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=ask%20us%20anything) (Request for Information) and we'll answer asap.  All questions go to improve the documentation, so literally ask anything (about Mixed Reality!!)

## Conclusion

We'd like you to join the XRTK adventure in building a true community-based Mixed Reality Framework, whether you are trying it new for the first time or wanting to transfer from the MRTK.  We will always ensure you have a voice and be on hand to help you through your Mixed Reality projects on whichever platforms are important to you and your project, whilst ensuring you can meet any new demands.
