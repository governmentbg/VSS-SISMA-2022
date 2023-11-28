using SISMA.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Core.Models.Reports
{
    public class EntitySelectVM
    {
        public int IntegrationId { get; set; }

        [Display(Name = "Апелативен район")]
        public int? ApealRegionId { get; set; }

        [Display(Name = "Област")]
        public int? DistrictId { get; set; }

        public string ListLabel { get; set; }

        public string SelectedContainerUL { get; set; }

        public string[] SelectedList { get; set; }

        public bool ShowApealRegion
        {
            get
            {
                int[] ints = { NomenclatureConstants.Integrations.EISS, NomenclatureConstants.Integrations.EDIS };
                return ints.Contains(IntegrationId);
            }
        }
        public bool ShowDistrict
        {
            get
            {
                int[] ints = { NomenclatureConstants.Integrations.EISS, NomenclatureConstants.Integrations.EDIS, NomenclatureConstants.Integrations.EISPP, NomenclatureConstants.Integrations.NSI };
                return ints.Contains(IntegrationId);
            }
        }
    }
}
