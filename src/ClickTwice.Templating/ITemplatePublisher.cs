namespace ClickTwice.Templating
{
    public interface ITemplatePublisher
    {
        TemplatePackageSettings Metadata { get; }
        PackagingMode PackagingMode { get; }
        ITemplatePublisher ToPackageFile(string outputPath);
        ITemplatePublisher ToGallery(string apiKey = null, string galleryUri = null);
    }
}