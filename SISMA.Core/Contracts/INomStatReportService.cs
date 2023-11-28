using SISMA.Core.Models.Reports;
using SISMA.Core.Models.StatReport;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using SISMA.Infrastructure.ViewModels.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SISMA.Core.Contracts
{
    public interface INomStatReportService : IBaseService
    {
        IQueryable<StatReportVM> StatReport_Select(FilterStatReportVM filter);
        Task<SaveResultVM> StatReport_SaveData(NomStatReport model);

        IQueryable<StatReportColVM> StatReportCol_Select(int statReportId);
        Task<SaveResultVM> StatReportCol_SaveData(NomStatReportCol model);

        IQueryable<StatReportCaseCodeVM> StatReportCaseCode_Select(int statReportId);
        Task<SaveResultVM> StatReportCaseCode_SaveData(NomStatReportCode model);
        IQueryable<StatReportCategoryVM> StatReportCategory_Select(FilterStatReportCategoryVM filter);
        Task<SaveResultVM> StatReportCategory_SaveData(NomStatReportCategory model);
        Task<SaveResultVM> StatReportCol_SaveData(ColSelectVM model);
    }
}
