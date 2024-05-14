using Siteimprove.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Siteimprove.Services
{
    public class SiteImproveUrlMapService : ISiteImproveUrlMapService
    {
        private readonly IScopeProvider _scopeProvider;

        public SiteImproveUrlMapService(IScopeProvider scopeProvider)
        {
            this._scopeProvider = scopeProvider;
        }

        public Task<object> Insert(SiteImproveUrlMap row)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                return scope.Database.InsertAsync(row);
            }
        }

        public Task<int> Update(SiteImproveUrlMap row)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                return scope.Database.UpdateAsync(row);
            }
        }

        public Task<List<SiteImproveUrlMap>> GetAll()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().SelectAll().From<SiteImproveUrlMap>();
                var selectResult = scope.Database.FetchAsync<SiteImproveUrlMap>();
                return selectResult;
            }
        }
        public Task<SiteImproveUrlMap> GetByPageId(int pageId)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().SelectAll().From<SiteImproveUrlMap>().Where<SiteImproveUrlMap>(m => m.PageId == pageId);
                var selectResult = scope.Database.FirstOrDefaultAsync<SiteImproveUrlMap>(sql);
                return selectResult;
            }
        }
    }
}
