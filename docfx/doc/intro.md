
# ClickTwice - The ultimate WPF build toolchain

## Introduction

As the name implies, ClickTwice is the missing step from the ClickOnce deployment technology. ClickTwice allows you to decouple your build and deploy steps from Visual Studio, instead integrating them with your preferred build environment. Currently, three environments are supported:

- Cake build scripts
- ScriptCs scripts
- Self-hosting (custom tool)

ClickTwice doesn't just build and publish your app: it also provides a powerful framework for pre- and post-processing your application at publish-time, allowing for a wide range of new abilities as pluggable handlers.

## How it works

The ClickTwice toolchain, at a high-level, performs the following operations when you publish your app:

1. Runs input handlers on the project source
1. Runs build configurators (if present)
1. Builds your app (using MSBuild) to a temporary folder
1. Generates a ClickTwice manifest in the output location
1. Runs output handlers on the published app
1. Copies the publish files to the target destination

Much of the power of ClickTwice is in it's flexible design. Publishing can invoke every step of this process or only a few, and each step is pluggable and extensible so you can tailor your build code to match your workflow.

## How to use

ClickTwice is usually run by a *"host"*, a context to help control and configure ClickTwice itself. Out-of-the-box, the easiest way to get started is by using the Cake or ScriptCs host. Alternatively, you can self-host the toolchain to get more fine-tuned control over the build. Plus, since it's all just .NET, you can integrate the toolchain directly into your own applications.