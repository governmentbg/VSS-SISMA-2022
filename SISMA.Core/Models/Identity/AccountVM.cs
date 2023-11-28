using Helpers.GenericIO;
using SISMA.Core.Extensions;
using SISMA.Infrastructure.Constants;
using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.ViewModels.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SISMA.Core.Models.Identity
{
    public class AccountVM
    {
        public string Id { get; set; }

        [Display(Name = "ЕГН")]
        [Required(ErrorMessage = "Въведете {0}.")]
        [AddToLog]
        [AutoSanitize]
        [RegularExpression(@"^[0-9]{10}", ErrorMessage = "Невалидно {0}.")]
        public string UIC { get; set; }

        [Display(Name = "Имена")]
        [Required(ErrorMessage = "Въведете {0}.")]
        [AddToLog]
        [AutoSanitize]
        [RegularExpression(@"^[а-яА-Я /-]+$", ErrorMessage = "Невалидна стойност в поле {0}.")]
        public string FullName { get; set; }

        [Display(Name = "Електронна поща")]
        [Required(ErrorMessage = "Въведете {0}.")]
        [RegularExpression(AccountConstants.EmailRegexPatern, ErrorMessage = "Невалидна електронна поща")]
        [AutoSanitize]
        [AddToLog]
        public string Email { get; set; }

        [Display(Name = "Активен запис")]
        [AddToLog]
        public bool IsActive { get; set; }

        [Display(Name = "Роли")]
        public IList<CheckListVM> Roles { get; set; }

        public AccountVM()
        {
            Roles = new List<CheckListVM>();
        }
    }

    public class FilterAccountVM
    {
        [Display(Name = "ЕГН")]
        public string Uic { get; set; }
        [Display(Name = "Имена")]
        public string FullName { get; set; }
        [Display(Name = "Електронна поща")]
        public string Email { get; set; }

        public void Sanitize()
        {
            Uic = Uic.EmptyToNull();
            FullName = FullName.EmptyToNull();
            Email = Email.EmptyToNull();
        }
    }
}
