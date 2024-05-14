using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Siteimprove.Models
{
    [TableName(Constants.SiteImproveUrlMapDbTable)]
    [PrimaryKey("id", AutoIncrement = true)]
    public class SiteImproveUrlMap
    {
        public SiteImproveUrlMap()
        {
            CurrentUrlPart = string.Empty;
            NewUrlPart = string.Empty;
        }

        [PrimaryKeyColumn(AutoIncrement = true)]
        [Column("id")]
        public int Id { get; set; }

        [Column("CurrentUrlPart")]
        [Length(1024)]
        public string CurrentUrlPart { get; set; }

        [Column("NewUrlPart")]
        [Length(1024)]
        public string NewUrlPart { get; set; }

        [Column("PageId")]
        public int PageId { get; set; }

    }
}
