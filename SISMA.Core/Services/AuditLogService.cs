using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SISMA.Core.Contracts;
using SISMA.Core.Extensions;
using SISMA.Core.Models;
using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Common;
using SISMA.Infrastructure.Data.Models.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SISMA.Core.Services
{
    public class AuditLogService : BaseService, IAuditLogService
    {
        private readonly IUserContext userContext;
        public AuditLogService(
            IRepository _repo,
            ILogger<AuditLogService> _logger,
            IUserContext _userContext)
        {
            repo = _repo;
            logger = _logger;
            userContext = _userContext;
        }
        public async Task<bool> SaveAuditLog(string operation, string objectInfo, string clientIp, string requestUrl, string actionInfo = null)
        {
            try
            {
                var entity = new AuditLog()
                {
                    DateWrt = DateTime.Now,
                    UserId = userContext.UserId,
                    Operation = operation,
                    ObjectInfo = objectInfo,
                    ActionInfo = actionInfo,
                    ClientIP = clientIp,
                    RequestUrl = requestUrl
                };
                repo.Add(entity);
                await repo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IQueryable<AuditLogVM> Select(AuditLogFilterVM filter)
        {
            filter.UpdateNullables();
            Expression<Func<AuditLog, bool>> whereUser = x => true;
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                whereUser = x => EF.Functions.ILike(x.User.FullName, filter.UserName.ToPaternSearch());
            }
            Expression<Func<AuditLog, bool>> whereObjectInfo = x => true;
            if (!string.IsNullOrEmpty(filter.Object))
            {
                whereObjectInfo = x => EF.Functions.ILike(x.ObjectInfo, filter.Object.ToPaternSearch());
            }

            return repo.AllReadonly<AuditLog>()
                            .Include(x => x.User)
                            .Where(x => x.DateWrt >= (filter.DateFrom ?? x.DateWrt))
                            .Where(x => x.DateWrt <= (filter.DateTo.MakeEndDate() ?? x.DateWrt))
                            .Where(whereUser)
                            .Where(whereObjectInfo)
                            .OrderByDescending(x => x.Id)
                            .Select(x => new AuditLogVM
                            {
                                UserFullName = x.User.FullName,
                                DateWrt = x.DateWrt,
                                Operation = x.Operation,
                                Object = x.ObjectInfo,
                                Url = x.RequestUrl
                            }).AsQueryable();
        }
    }
}
