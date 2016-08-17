# Handlers

Handlers are one of the most powerful parts of the ClickTwice toolchain. Handlers are generic processing modules: when included in a publish, they are automatically invoked at the correct stage in the build to perform any sort of operation on your application.

Handlers can be *input handlers*, *output handlers* or sometimes both. Input handlers are run on the project source, before building. An input handler can transform your source, add missing or generated files, update metadata or anything you need to do before publishing. Output handlers, on the other hand, are run on the published output, before copying to the target location. This means they have access to the final published app files, including any generated manifests, and can do any post-build or pre-deploy steps as required.

## Using handlers

The exact syntax for using handlers depends on your choice of host, but essentially comes down to adding `IInputHandler` and `IOutputHandler` objects to the `InputHandlers` and `OutputHandlers` collections on your `PublishManager`. Since that is a scary way of putting it, the built-in hosts both include a simple `WithHandler` method, so you can do the following:

```csharp
//PublishApp(...) //for Cake
//Configure(...) //for scriptcs
publish.WithHandler(new InstallPageHandler);
```

Remember, this syntax may vary slightly between hosts, so check the documentation for more detail.

## Building handlers

Building your own build handlers is dead simple: just create a `public` class, that inherits from `IInputHandler` or `IOutputHandler`. Give your handler a `Name` property, and implement the required `Process(string)` method and you're good to go!

```csharp
using ClickTwice.Publisher.Core.Handlers;

public class SimpleHandler : IInputHandler 
{
    public string Name => "Simple Handler";

    HandlerResponse Process(string inputPath) {
        return new HandlerResponse(this, true);
    }
}
```

That's it! You can do anything you like in your handler. The `HandlerResponse` object you return just needs to return a `bool` to indicate if it was successful or not, and an optional message for the user. The `inputPath` parameter will be the path to the project directory (i.e. the source).

### Configurators

There is also a special kind of quasi-handler called a *build configurator*. Configurators run during the build phase, right before invoking MSBuild, and can be used to perform low-level tweaks of the build configuration before building, without any post-processing. Note that since these have the option of modifying your builds in unpredictable and potentially dangerous ways, make sure to only ever use configurators you fully trust! Some hosts will even print a warning before invoking configurators for clarity. 

> Configurators are an early preview feature and may change in future releases

#### Custom configurator

Configurators inherit from `BuildConfigurator`, and can override the `ProcessConfiguration` and `ProcessTargets` methods to read or manipulate the dictionary of build properties or list of build targets. These configurators are perfect for fine-tuned control of build properties and parameters such as suppressing errors or adding new custom targets to your publish.