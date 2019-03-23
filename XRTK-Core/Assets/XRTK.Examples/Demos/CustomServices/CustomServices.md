# Custom Extension Services Demo

With the new service locator pattern of the Mixed Reality Toolkit, adding your own services is fairly simple and straight forward.

Each service will require the following files:

1. [Interface Contract](./IDemoCustomExtensionService.cs)
2. [Concrete Implementation](./DemoCustomExtensionService.cs)
3. [Configuration Profile Definition](./DemoCustomExtensionServiceProfile.cs)
4. [Configuration Profile Inspector](./Inspectors/DemoCustomExtensionServiceProfileInspector.cs)
5. [Configuration Profile Asset](./Profiles/DemoCustomExtensionServiceProfile.asset)

Once each of these exist then an extension service can be added to the extension service registry of the main Mixed Reality Toolkit's configuration profile.