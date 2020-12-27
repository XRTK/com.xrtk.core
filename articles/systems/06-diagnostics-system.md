# The Diagnostics System

The Diagnostics system a very useful tool for showing critical diagnostics information in a running scene, it comes in very handy while debugging and sourcing scene based performance issues.

## Mixed Reality Root configuration system

In the Root configuration, the specific implementation for the Diagnostics system is selected, which defaults to the XRTK implementation for the Diagnostics system.  In most cases this does not need to be touched, however, advanced users can replace it with their own system if they wish.

![Diagnostics System Profile](../../images/Configuration/Diagnostics/DiagnosticsSystemProfile.png)

## Diagnostics System platform configuration

The Diagnostics system for the XRTK allows for different instruments to be called to display information in the diagnostics prefab.

![Diagnostics System Settings](../../images/Configuration/Diagnostics/DiagnosticsSystemSettings.png)

The basic configuration simply identifies the prefab to be displayed in the scene when activated and whether the window is active on startup or called programmatically later.

## Diagnostics Providers

The diagnostic framework is very dynamic allowing different instruments to be added to collect data and customized so that they are run either for all platforms or just specific ones.  New instruments can be added, so long as they derive to the Diagnostics Providers `BaseMixedRealityDiagnosticsDataProvider` implementation.  Simply view the existing implementations for reference.

Additionally, the prefab used will need to provide the capability to display the data, review the default DiagnosticsPrefab for detail.

![Diagnostics Data Providers](../../images/Configuration/Diagnostics/DiagnosticsDataProviders.png)

> Check out all the [Platforms that are available to configure](../platforms/platforms.md).

## Further notes

For the most part, you should not need to alter the Diagnostics system configuration unless required, or you wish to manually add a new Diagnostics System instrument manually.

---

### Related Articles

* [Getting Started](../00-GettingStarted.md#getting-started-with-the-mixed-reality-toolkit)
* [Configuration](../02-Configuration.md)
* [XRTK Platforms](../platforms/platforms.md)

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
