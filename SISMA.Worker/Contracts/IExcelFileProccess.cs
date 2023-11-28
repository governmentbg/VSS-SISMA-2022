using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Worker.Contracts
{
    public interface IExcelFileProccess
    {
        Task<bool> ProcessAsync(string fileName, byte[] fileContent);
    }
}
