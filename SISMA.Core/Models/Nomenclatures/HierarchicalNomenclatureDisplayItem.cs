using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SISMA.Core.Models.Nomenclatures
{
    public class HierarchicalNomenclatureDisplayItem : NomenclatureDisplayItem
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("rootId")]
        public int RootId { get; set; }

        [JsonProperty("locations")]
        public List<SelectListItem> Locations { get; set; }
    }
}
