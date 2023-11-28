using SISMA.Core.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Core.Models
{
    public class AuditLogVM
    {
        public string Operation { get; set; }
        public string UserFullName { get; set; }
        public string Object { get; set; }
        public DateTime DateWrt { get; set; }
        public string ClientIp { get; set; }
        public string Url { get; set; }
    }

    public class AuditLogFilterVM
    {

        [Display(Name = "Дата от")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Дата до")]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Потребител")]
        public string UserName { get; set; }

        [Display(Name = "Обект")]
        public string Object { get; set; }

        public void UpdateNullables()
        {
            UserName = UserName.EmptyToNull();
            Object = Object.EmptyToNull();
        }
    }

}
