# Logging

ClickTwice includes its own built-in logging subsystem, based on the `IPublishLogger` interface. This simplified interface allows for logging just two types of messages: normal logging, and build messages. Due to the verbosity that build logs often include, build messages are generally disabled by default.

Logging is implemented in the host, and is used to log the output of all handlers, the build itself and all the parts of a publish operation. Individual hosts may therefore ship their own loggers.

There are only two loggers shipped in the main library at this time: 

- `ConsoleLogger` - Basic logger to write non-build messages to the system console
- `FileLogger` - Writes log messages to file, and publishes the file in the publish directory (under "Logs")

## Building a logger

Building a custom logger is very simple. Just create a new class and implement the `IPublishLogger` interface, which is just:

```csharp
void Log(string content);
bool IncludeBuildMessages { get; }
string Close(string outputPath);
```

`IncludeBuildMessages` controls whether the logger should be given full build logs or not, while the `Close` method is called at the end of the publish operation for any cleanup operations (writing to file etc). You can return null or throw a new `NotImplementedException` if your logger has no clean up to do.