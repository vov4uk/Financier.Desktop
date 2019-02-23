using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Monads;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
	class Program
	{
		static void Main(string[] args)
		{
			string fileName = null;
			string outputFileName = null;
			string arg = args.Length > 0 && args[0] != null ? args[0] : Environment.CurrentDirectory;
			if (arg != null)
			{
				if (File.GetAttributes(arg).HasFlag(FileAttributes.Directory))
				{
					fileName = Directory.EnumerateFiles(arg, "*.backup")
							.OrderByDescending(Path.GetFileNameWithoutExtension)
							.FirstOrDefault();
				}
				else
				{
					fileName = arg;
				}
			}

			if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
				Environment.Exit(-1);
			if (args.Length > 1 && args[1] != null)
				outputFileName = args[1];
			else
				outputFileName = Path.ChangeExtension(fileName, "csv");

			var entities = EntityReader.GetEntities(fileName).ToArray();

			var transactions =
				entities
					.OfType<Transaction>()
					.Where(t => t.DateTime >= new DateTime(2015, 12, 1))
					.Where(t => t.To == null)
					.Where(t => t.Category != Category.Split)
					.OrderBy(t => t.DateTime)
					.ToArray();
			var payees =
				entities
					.OfType<Payee>()
					.ToArray();
			var categories =
				entities
					.OfType<Category>()
					.ToArray();
			var projects =
				entities
					.OfType<Project>()
					.ToArray();
			var attributes =
				entities
					.OfType<AttributeDefinition>()
					.ToArray();
			var attributeValues =
				entities
					.OfType<TransactionAttribute>()
					.ToArray();

			using (FileStream file = File.Create(outputFileName))
			{
				using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
				{
					using (var csv = new CsvWriter(writer))
					{
						csv.Configuration.CultureInfo = CultureInfo.InvariantCulture;

						//csv.WriteExcelSeparator();
						csv.WriteField("Date&Time");
						csv.WriteField("Account");
						csv.WriteField("Amount");
						csv.WriteField("Category");
						csv.WriteField("Payee");
						csv.WriteField("Project");
						csv.WriteField("Note");
						csv.NextRecord();

						foreach (Transaction tran in transactions)
						{
							csv.WriteField(tran.DateTime);
							csv.WriteField(tran.From?.Title);
							csv.WriteField(tran.FromAmount?.ToString("0.00"));
							csv.WriteField(tran.Category?.Title);
							csv.WriteField(tran.Payee?.Title);
							csv.WriteField(tran.Project?.Title);
							csv.WriteField(tran.Note);
							csv.NextRecord();
						}
					}
				}
			}

			Console.WriteLine("Done!");
		}
	}
}
