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

        public abstract string BankTitle { get; }

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                List<string> pages = new List<string>();
                using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions()))
                {
                    for (var i = 0; i < docReader.GetPageCount(); i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            pages.Add(pageReader.GetText());
                        }
                    }
                }

                return ParseTransactionsTable(pages);

            }
            return Array.Empty<BankTransaction>();
        }

        protected abstract IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages);
    }
}
