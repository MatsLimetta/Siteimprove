using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Manifest;

namespace Siteimprove.ManifestFilters
{
    public class SiteimproveManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            var assembly = typeof(SiteimproveManifestFilter).Assembly;

            manifests.Add(new PackageManifest
            {
                PackageName = "Siteimprove",
                Version = assembly.GetName()?.Version?.ToString(3) ?? "13.0.0",
                AllowPackageTelemetry = true,
                Scripts = new[]
                {
                    "/App_Plugins/Siteimprove/load.js",
                    "/App_Plugins/Siteimprove/dashboard.js",
                    "/App_Plugins/Siteimprove/siteimprove.helper.js",
                    "/App_Plugins/Siteimprove/siteimprove.main.js",
                    "/App_Plugins/Siteimprove/siteimprove.controller.js"
                },
                ContentApps = new ManifestContentAppDefinition[]
                {
                    new ManifestContentAppDefinition()
                    {
                        Name = "Siteimprove",
                        Alias = "siteimprove",
                        Weight = 0,
                        Icon = "icon-calculator",
                        View = "/App_Plugins/Siteimprove/views/dashboard.html"
                    }
                }
            });
        }
    }
}
