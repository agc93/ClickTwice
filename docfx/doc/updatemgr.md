# ClickTwice Update Manager

## Introduction

The ClickTwice Update Manager is a standalone library for use in WPF applications to more easily integrate with your ClickOnce deployment environment. It's fully compatible with any ClickOnce network deployment, even if you're not using the rest of the ClickTwice toolchain.

You can install the update manager using the following command: 

> Install-Package ClickTwice.UpdateManager

This will install the library into your application. The Update Manager is wholly self-contained in a special `UpdateManagerViewModel` class, that you can either use as a `DataContext` directly, inherit from, or use as a property in your existing view models. A short summary of the functions available from the view model is below.

## Message Properties

These are simple `string` properties, perfect for binding to a `TextBlock`, providing key pieces of information:

- `ApplicationInfoMessage` - returns a simple summary of current application version and build date
- `DeploymentInfoMessage` - returns a summary of the current deployment version and source
- `UpdateCheckStatusMessage` - returns a message for the current update status
- `UpdateProgressMessage` - returns a message on the progress of any ongoing update operations
- `UpdateProgress` - (`int`) returns the progress of any current update operations

## Toggle Properties

These are simple `boolean` properties, handy for toggling visibility of messages and buttons

- `IsDeployed` - `true` if the current application is ClickOnce-deployed
- `IsUpdateAvailable` - `true` if there is an update available
- `IsUpdateInProgress` - `true` if there is an update operation currently in progress


## Events and commands

You can also use these events and commands to control updates from within your application

- `OnUpdateStarting` - `event` fires when an update operation is started
- `OnUpdateComplete` - `event` fires to indicate a completed update operation
- `UpdateApplicationCommand` - `ICommand` for invoking a update operation on-demand
- `CheckUpdatesCommand` - `ICommand` to check for available updates from the deployment source

## Full API Reference

Click API reference above to check the full API for `UpdateManagerViewModel` and take back control of your ClickOnce updates the easy way!