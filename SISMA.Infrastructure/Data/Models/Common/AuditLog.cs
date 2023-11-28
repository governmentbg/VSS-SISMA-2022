using SISMA.Infrastructure.Data.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISMA.Infrastructure.Data.Models.Common
{
    /// <summary>
    /// Журнал на промените
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime DateWrt { get; set; }

        public string Operation { get; set; }

        public string ObjectInfo { get; set; }

        public string ActionInfo { get; set; }

        public string RequestUrl { get; set; }

        public string ClientIP { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
    }
}
