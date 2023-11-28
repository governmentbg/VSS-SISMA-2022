using SISMA.Core.Models.Identity;
using SISMA.Infrastructure.Data.Models.Identity;
using SISMA.Infrastructure.ViewModels.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SISMA.Core.Contracts
{
    public interface IAccountService : IBaseService
    {
        Task<SaveResultVM> CheckUser(AccountVM model);
        Task<ApplicationUser> GetByUIC(string uic);
        Task<ApplicationUser> GetByUserName(string userName);
        IQueryable<AccountVM> Select(FilterAccountVM filter);
    }
}
