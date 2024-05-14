using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Siteimprove.Models
{
    [TableName(Constants.SiteImproveDbTable)]
    [PrimaryKey("id", AutoIncrement = true)]
    public class SiteImproveSettingsModel //: ColumnDefinition
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column("id")]
        public int Id { get; set; }

        [Column("Token")]
        [Length(255)]
        public string Token { get; set; }

        //[Column("CrawlIds")]
        //public string CrawlIds { get; set; }

        [Column("Installed")]
        public bool Installed { get; set; }
    }
}
