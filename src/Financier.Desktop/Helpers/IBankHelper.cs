using System.Collections.Generic;
using System.Threading.Tasks;
using Financier.DataAccess.Monobank;

namespace Financier.Desktop.Helpers
{
    public interface IBankHelper
    {
        Task<IEnumerable<BankTransaction>> ParseReport(string filePath);
    }
}
