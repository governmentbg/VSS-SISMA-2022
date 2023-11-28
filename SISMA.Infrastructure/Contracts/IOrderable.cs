using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Infrastructure.Contracts
{
    public interface IOrderable
    {
        int Id { get; set; }
        int OrderNumber { get; set; }
    }
}
