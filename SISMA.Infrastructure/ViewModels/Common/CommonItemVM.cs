using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Infrastructure.ViewModels.Common
{
    public class CommonItemVM
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }

        public int? ParentId { get; set; }

        public string Code { get; set; }

        public string Label { get; set; }
    }
}
