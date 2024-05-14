using Siteimprove.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Siteimprove.Composing
{
    public class RegisterServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<ISiteImproveSettingsService, SiteImproveSettingsService>();
            builder.Services.AddTransient<ISiteImproveUrlMapService, SiteImproveUrlMapService>();
        }
    }
}
