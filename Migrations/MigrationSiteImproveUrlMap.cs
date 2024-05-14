using Siteimprove.Models;
using Siteimprove.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Siteimprove.Migrations
{
    public class SiteImproveUrlMapComponent : IComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public SiteImproveUrlMapComponent(ICoreScopeProvider coreScopeProvider, IMigrationPlanExecutor migrationPlanExecutor, IKeyValueService keyValueService, IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan("MigrationSiteImproveUrlMap");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty)
                .To<MigrationSiteImproveUrlMap>("MigrationSiteImproveUrlMap table");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }

    public class MigrationSiteImproveUrlMap : MigrationBase
    {
        private readonly ISiteImproveSettingsService _siteImproveSettingsService;

        public MigrationSiteImproveUrlMap(IMigrationContext context, ISiteImproveSettingsService siteImproveSettingsService)
            : base(context)
        {
            _siteImproveSettingsService = siteImproveSettingsService;
        }

        protected override void Migrate()
        {
            if (!TableExists(Constants.SiteImproveUrlMapDbTable))
                Create.Table<SiteImproveUrlMap>().Do();
        }
    }
}
