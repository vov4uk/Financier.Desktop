using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Financier.DataAccess;
using FinancistoAdapter.Entities;
using FinancistoAdapter.Monobank;

namespace FinancistoAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            var finDb = new FinancierDatabase();

            string fileName = null;
            string csv_fileName = null;
            string outputFileName = null;
            string arg = args.Length > 0 && args[0] != null ? args[0] : Environment.CurrentDirectory;
            if (arg != null)
            {
                if (File.GetAttributes(arg).HasFlag(FileAttributes.Directory))
                {
                    fileName = Directory.EnumerateFiles(arg, "*.backup")
                            .OrderByDescending(Path.GetFileNameWithoutExtension)
                            .FirstOrDefault();
                    csv_fileName = Directory.EnumerateFiles(arg, "*.csv")
                    .OrderByDescending(Path.GetFileNameWithoutExtension)
                    .FirstOrDefault();
                }
                else
                {
                    fileName = arg;
                }
            }

            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                Environment.Exit(-1);
            if (args.Length > 1 && args[1] != null)
                outputFileName = args[1];
            else
                outputFileName = Path.ChangeExtension(fileName, "csv");

            Entity[] entities = EntityReader.GetEntities(fileName).ToArray();

            var transactions =
                entities
                    .OfType<Transaction>()
                    .OrderBy(t => t.DateTime)
                    .ToArray();
            var payees = entities.OfType<Payee>().Select(x => x.ToBackupLines()).ToArray();
            var categories = entities.OfType<Category>().Select(x => x.ToBackupLines()).ToArray();
            var projects = entities.OfType<Project>().Select(x => x.ToBackupLines()).ToArray();
            var attributes = entities.OfType<AttributeDefinition>().Select(x => x.ToBackupLines()).ToArray();
            var locations = entities.OfType<Location>().Select(x => x.ToBackupLines()).ToArray();
            var budgets = entities.OfType<Budget>().Select(x => x.ToBackupLines()).ToArray();
            var accounts = entities.OfType<Account>().OrderBy(x => x.Title).ToArray();
            var attributeValues = entities.OfType<TransactionAttribute>().Select(x => x.ToBackupLines()).ToArray();
            var exchange = entities.OfType<ExchangeRate>().Select(x => x.ToBackupLines()).ToArray();

            foreach (var acc in accounts)
            {
                var balance = new List<RunningBalance>();
                double amount = 0.0;
                foreach (var item in transactions.Where(x => x.Category != -1 && !x.IsTemplate && (x.From == acc.Id || x.To == acc.Id)).OrderBy(x => x.DateTime))
                {
                    if (item.From == acc.Id)
                    {
                        amount += item.FromAmount;
                    }
                    if (item.To == acc.Id)
                    {
                        amount += item.ToAmount;
                    }
                    balance.Add(new RunningBalance { Transaction = item, Account = acc, Date = item.DateTime.Value, Balance = amount });
                }

                Console.WriteLine($"{acc.Title} : {amount} --- {acc.TotalAmount / 100.00}");
            }

            using (FileStream file = File.OpenRead(csv_fileName))
            {
                using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
                {
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<MonoTransaction>().ToArray();
                    }
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
