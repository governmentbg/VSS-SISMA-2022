using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SISMA.Core.Contracts;
using SISMA.Core.Extensions;
using SISMA.Core.Models.Identity;
using SISMA.Infrastructure.Data.Common;
using SISMA.Infrastructure.Data.Models.Identity;
using SISMA.Infrastructure.ViewModels.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SISMA.Core.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly INomenclatureService nomService;
        public AccountService(
              IRepository _repo,
            ILogger<DataService> _logger,
            INomenclatureService _nomService)
        {
            repo = _repo;
            logger = _logger;
            nomService = _nomService;
        }

        public IQueryable<AccountVM> Select(FilterAccountVM filter)
        {
            filter.Sanitize();

            Expression<Func<ApplicationUser, bool>> whereUIC = x => true;
            if (!string.IsNullOrEmpty(filter.Uic))
            {
                whereUIC = x => x.UIC == filter.Uic;
            }
            Expression<Func<ApplicationUser, bool>> whereFullName = x => true;
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                whereFullName = x => EF.Functions.ILike(x.FullName, filter.FullName.ToPaternSearch());
            }
            Expression<Func<ApplicationUser, bool>> whereEmail = x => true;
            if (!string.IsNullOrEmpty(filter.Email))
            {
                whereEmail = x => EF.Functions.ILike(x.Email, filter.Email.ToPaternSearch());
            }

            return repo.AllReadonly<ApplicationUser>()
                            .Where(whereUIC)
                            .Where(whereFullName)
                            .Where(whereEmail)
                            .Select(x => new AccountVM
                            {
                                Id = x.Id,
                                Email = x.UserName,
                                FullName = x.FullName,
                                IsActive = x.IsActive
                            }).AsQueryable();
        }



        public async Task<SaveResultVM> CheckUser(AccountVM model)
        {

            if (await repo.AllReadonly<ApplicationUser>()
                        .Where(x => x.UserName == model.Email && x.Id != model.Id)
                        .AnyAsync())
            {
                return new SaveResultVM(false, "Съществува потребител с това потребителско име.");
            }

            if (await repo.AllReadonly<ApplicationUser>()
                        .Where(x => x.UIC == model.UIC && x.Id != model.Id)
                        .AnyAsync())
            {
                return new SaveResultVM(false, "Съществува потребител с това ЕГН.");
            }


            return new SaveResultVM(true);
        }

        public async Task<ApplicationUser> GetByUserName(string userName)
        {
            return await repo.All<ApplicationUser>()
                                      .Where(x => EF.Functions.ILike(x.UserName, userName))
                                      .Where(x => x.IsActive)
                                      .FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetByUIC(string uic)
        {
            return await repo.All<ApplicationUser>()
                            .Where(x => x.UIC == uic)
                            .Where(x => x.IsActive == true)
                            .FirstOrDefaultAsync();
        }
    }
}

