# The Additional Service Providers extension

With the built in services and systems the XRTK provides, it also provides the capability to implement custom services within its framework, this enables developers to take advantage of the performance improvements for operating runtime services or integrations from within the XRTK.

For more information on creating services that can be hosted within the Mixed Reality Framework, check out this awesome article by Stephen Hodgson.

<a href="https://stephen-hodgson.medium.com/a-practical-deep-dive-into-the-mixed-reality-framework-a26401edb2aa">![[A Practical Deep Dive into the Mixed Reality Framework](https://stephen-hodgson.medium.com/a-practical-deep-dive-into-the-mixed-reality-framework-a26401edb2aa)](https://miro.medium.com/max/1250/1*ebPN0spgicLY4ODa6vB9pg.jpeg)</a>
> [A Practical Deep Dive into the Mixed Reality Framework](https://stephen-hodgson.medium.com/a-practical-deep-dive-into-the-mixed-reality-framework-a26401edb2aa)

## Mixed Reality Root configuration system

In the Root configuration, the Additional Service Providers profile provides an entry point to register any and all additional services with the Mixed Reality Toolkit.

![](../../images/Configuration/AdditionalServices/AdditionalServiceProvidersProfile.png)

## Additional Service Providers

From the Additional Service Providers profile, you can register all the services you have created or obtained for use in the Mixed Reality Toolkit.  Once your service is available in your project, it will automatically be identified through its use of the `IMixedRealityExtensionService` interface and be available to add below.  To register a service in your project, simply add a new entry to the list and configure the following:

* Instanced Type - use the dropdown to select your service implementation, this will automatically pull the name from the service and display in the `Name` field.
* Profile - If your service requires a profile to configure, this will automatically display below the `Instanced Type` and allow you to provide or create a new profile for the service.  This is optional if your service does not require additional configuration.
* Runtime Platforms - Select which platforms your service should run on.

![](../../images/Configuration/AdditionalServices/AdditionalServiceProvidersList.png.png)

> Check out all the [Platforms that are available to configure](../platforms/platforms.md).

## Further notes

Services registered with the XRTK can be accessed like any other service through the MixedRealityToolkit.Instance using a similar call to the following:

```csharp
    var MyService = MixedRealityToolkit.Instance.GetService<MyService>();
```

or

```csharp

    MyService myServiceInstance = null
    if(!MixedRealityToolkit.Instance.TryGetService<MyService>(out myServiceInstance))
    {
        //Do something because the service was not found
    }
```

This is available anywhere and at anytime within your project, but should be avoided in Update calls, better to get a reference on startup and store that reference.
---

### Related Articles

* [Getting Started](../00-GettingStarted.md#getting-started-with-the-mixed-reality-toolkit)
* [Configuration](../02-Configuration.md#getting-started-with-the-mixed-reality-toolkit)
* [XRTK Platforms](../platforms/platforms.md)
* [A Practical Deep Dive into the Mixed Reality Framework](https://stephen-hodgson.medium.com/a-practical-deep-dive-into-the-mixed-reality-framework-a26401edb2aa)

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).
