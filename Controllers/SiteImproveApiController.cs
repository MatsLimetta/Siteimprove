using Microsoft.AspNetCore.Mvc;
using Siteimprove.Models;
using Siteimprove.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Extensions;
using Newtonsoft.Json;

namespace Siteimprove.Controllers
{
    public class SiteImproveApiController : UmbracoAuthorizedApiController
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ISiteImproveSettingsService _siteImproveSettingsService;
        private readonly ISiteImproveUrlMapService _siteImproveUrlMapService;

        public SiteImproveApiController(IUmbracoContextFactory umbracoContextFactory,
            ISiteImproveSettingsService service,
            ISiteImproveUrlMapService siteImproveUrlMapService)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _siteImproveSettingsService = service;
            _siteImproveUrlMapService = siteImproveUrlMapService;
        }

        [HttpGet]
        public async Task<ActionResult> GetSettings(int pageId)
        {
            var urlMap = await _siteImproveUrlMapService.GetByPageId(pageId);
            var model = new
            {
                token = await _siteImproveSettingsService.GetToken(),
                crawlingIds = GetCrawlIds(),
                currentUrlPart = urlMap?.CurrentUrlPart ?? string.Empty,
                newUrlPart = urlMap?.NewUrlPart ?? string.Empty
            };

            return Content(JsonConvert.SerializeObject(model), "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> SaveUrlMap([FromBody] SaveUrlMapParams saveUrlMapParams)
        {
            try
            {
                var current = await _siteImproveUrlMapService.GetByPageId(saveUrlMapParams.PageId);
                if (current == null)
                {
                    current = new SiteImproveUrlMap
                    {
                        PageId = saveUrlMapParams.PageId,
                        CurrentUrlPart = saveUrlMapParams.CurrentUrlPart,
                        NewUrlPart = saveUrlMapParams.NewUrlPart
                    };
                    await _siteImproveUrlMapService.Insert(current);
                }
                else
                {
                    current.CurrentUrlPart = saveUrlMapParams.CurrentUrlPart;
                    current.NewUrlPart = saveUrlMapParams.NewUrlPart;
                    await _siteImproveUrlMapService.Update(current);
                }

                var model = new
                {
                    success = true,
                    message = "Saved"
                };
                return Content(JsonConvert.SerializeObject(model), "application/json");
            }
            catch (Exception ex)
            {
                var model = new
                {
                    success = false,
                    message = ex.Message
                };
                return Content(JsonConvert.SerializeObject(model), "application/json");
            }
        }

        public class SaveUrlMapParams
        {
            public int PageId { get; set; }
            public string CurrentUrlPart { get; set; }
            public string NewUrlPart { get; set; }
        }


        /// <summary>
        /// Get node id's that will execute the Siteimprove recrawling method
        /// </summary>
        /// <returns></returns>
        public string GetCrawlIds()
        {
            using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var publishedRootPages = umbracoContextReference.UmbracoContext.Content?.GetAtRoot().ToList();
                return publishedRootPages?.Any() == true ? publishedRootPages.First().Id.ToString() : null;
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetToken()
        {
            return Content(await _siteImproveSettingsService.GetToken());
        }

        [HttpGet]
        public async Task<ActionResult> RequestNewToken()
        {
            return Content(await _siteImproveSettingsService.GetNewToken());
        }

        [HttpGet]
        public ActionResult GetCrawlingIds()
        {
            return Content(GetCrawlIds());
        }

        [HttpGet]
        public async Task<ActionResult> GetPageUrl(int pageId)
        {
            using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var node = umbracoContextReference.UmbracoContext.Content?.GetById(pageId);
                var originalUrl = node != null ? node.Url(mode: UrlMode.Absolute) : "";
                var urlMaps = await _siteImproveUrlMapService.GetAll();
                var currentMapping = GetMapping(pageId, urlMaps);
                var url = currentMapping == default
                    ? originalUrl
                    : originalUrl.Replace(currentMapping.CurrentUrlPart, currentMapping.NewUrlPart);

                var model = new
                {
                    success = node != null,
                    status = node != null ? "OK" : "No published page with that id",
                    url
                };

                return Content(JsonConvert.SerializeObject(model), "application/json");
            }
        }

        private SiteImproveUrlMap GetMapping(int pageId, List<SiteImproveUrlMap> maps)
        {
            if (maps.Any(m => m.PageId == pageId && !string.IsNullOrWhiteSpace(m.CurrentUrlPart)))
            {
                return maps.First(m => m.PageId == pageId);
            }

            using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var node = umbracoContextReference.UmbracoContext.Content?.GetById(pageId);
                return node.Parent != null ? GetMapping(node.Parent.Id, maps) : default;
            }
        }
    }
}
