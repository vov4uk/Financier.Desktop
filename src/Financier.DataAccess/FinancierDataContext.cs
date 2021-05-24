using Microsoft.EntityFrameworkCore;

namespace Financier.DataAccess
{
    public class FinancierDataContext : DbContext
    {
        public FinancierDataContext(DbContextOptions<FinancierDataContext> options)
                : base(options)
        {
        }
    }
}
