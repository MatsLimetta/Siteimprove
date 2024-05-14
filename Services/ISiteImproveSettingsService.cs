using Siteimprove.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siteimprove.Services
{
    public interface ISiteImproveSettingsService
    {
        SiteImproveSettingsModel SelectTopRow();

        void Insert(SiteImproveSettingsModel row);

        void Update(SiteImproveSettingsModel row);

        Task<string> GetToken();

        Task<string> GetNewToken();

        string RequestToken();
    }
}
