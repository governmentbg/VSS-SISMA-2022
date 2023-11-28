using SISMA.Core.Models.StatReport;
using System.Threading.Tasks;

namespace SISMA.Core.Contracts
{
    public interface IStatReportService : IBaseService
    {
        Task<DataTablesStatReportVM> Get_ReportData(int statReportId, int[] entityList, int periodNo, int periodYear);
    }
}
