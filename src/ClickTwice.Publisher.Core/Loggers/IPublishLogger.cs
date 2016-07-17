namespace ClickTwice.Publisher.Core.Loggers
{
    public interface IPublishLogger
    {
        void Log(string content);
        bool IncludeBuildMessages { get; }
        string Close(string outputPath);
    }
}
