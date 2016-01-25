using ClickTwice.Publisher.Core.Manifests;

namespace ClickTwice.Publisher.Core.Resources
{
    public class LaunchPageModel
    {
        public LaunchPageModel(AppManifest manifest)
        {
            if (manifest == null) manifest = new AppManifest();
            Manifest = manifest;
        }

        public LaunchPageModel(AppManifest manifest, ExtendedAppInfo appinfo) : this(manifest)
        {
            if (appinfo == null) appinfo = new ExtendedAppInfo();
            AppInfo = appinfo;
        }

        public AppManifest Manifest { get; set; }

        public ExtendedAppInfo AppInfo { get; set; }
    }
}