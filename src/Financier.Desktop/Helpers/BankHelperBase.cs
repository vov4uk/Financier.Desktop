using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using Docnet.Core.Models;
using Docnet.Core;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers
{
    public abstract class BankPdfHelperBase : IBankHelper
    {
        protected const string Space = " ";
        protected abstract string Header { get; }

        public async Task<IEnumerable<BankTransaction>> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Header);
                using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions()))
                {
                    for (var i = 0; i < docReader.GetPageCount(); i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            var pageText = ParseTransactionsTable(pageReader.GetText().Replace(Environment.NewLine, Space));
                            sb.AppendLine(pageText.TrimEnd());
                        }
                    }
                }

                using (TextReader streamReader = new StringReader(sb.ToString()))
                {
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        return await csv.GetRecordsAsync<BankTransaction>().ToListAsync();
                    }
                }
            }
            return Array.Empty<BankTransaction>();
        }

        protected abstract string ParseTransactionsTable(string pageText);
    }
}
