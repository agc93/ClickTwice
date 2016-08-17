# Manifests

Application manifests describe important metadata about your published application.

In ClickTwice, there are two kinds of manifests generated as part of a "standard" publish: the ClickOnce manifest and the ClickTwice manifest.

ClickOnce manifests are special `.application` files that Windows (and Internet Explorer) know how to handle in order to launch a deployed application. These XML doucments are full of data, including your app's metadata, information on digital signatures and keys in the application, deployment and framework information.

The ClickTwice manifest is a simplified JSON document (`*.cltw`) with only the relevant application metadata used in building, publishing and distributing your application. You can find the full (C#) schema for this format [here](/api/ClickTwice.Publisher.Core.Manifests.AppManifest.html). This file is used internally by ClickTwice and some of the built-in handlers for more detailed app metadata. Note that this file is wholly independent of the ClickOnce `.application` manifest, which is still present in the output.

#### `app.info`

> Please note that the `app.info` file will be merged into the ClickTwice manifest (as `AppInfo`) in a future update.

The `app.info` file is a separate metadata file that contains generic user-friendly messages, such as a summary of the app's functionality, contact and support information and author details. This information is mostly of use to post-deployment tools, such as page generators, forms and scripts. This file is wholly optional and is currently provided by the `AppInfoHandler`.