using Helpers.GenericIO;
using SISMA.Infrastructure.Contracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Infrastructure.Data.Models.Nomenclatures
{
    public class BaseCommonNomenclature : ICommonNomenclature, IOrderable
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Номер по ред")]
        public int OrderNumber { get; set; }

        [Display(Name = "Код")]
        //[AddToLog]
        public string Code { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Полето {0} е задължително")]
        [AddToLog]
        [AutoSanitize]
        public string Label { get; set; }

        [Display(Name = "Описание")]
        [AddToLog]
        [AutoSanitize]
        public string Description { get; set; }


        [Display(Name = "Активен запис")]
        [AddToLog]
        public bool IsActive { get; set; }

        [Display(Name = "Начална дата")]
        [Required(ErrorMessage = "Въведете {0}.")]
        [AddToLog]
        public DateTime DateStart { get; set; }

        [Display(Name = "Крайна дата")]
        [AddToLog]
        public DateTime? DateEnd { get; set; }
    }
}
