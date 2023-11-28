using SISMA.Core.Models.Common;
using SISMA.Core.Models.Reports;
using SISMA.Infrastructure.Contracts.Data;
using SISMA.Infrastructure.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Core.Contracts
{
    public interface IDataService
    {
        IQueryable<EkatteItemVM> Distance_Select(FilterEkatteItemVM filter);
        SaveResultVM Manage(ReportDataVM model);
        Task<SaveResultVM> SaveData(SismaModel model);
        IQueryable<ReportDataVM> Select(FilterReportData filter);
        Task<SismaModel> TestModel();
        Task<SismaModel> TestModelCourts();
        Task<SismaModel> TestModelNSI();
        Task<SismaModel> TestModelTotal(int catId, int year);
    }
}
