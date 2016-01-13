using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			var entities = EntityReader.GetEntities("test.backup").ToArray();
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

			var aushan =
				entities.OfType<Transaction>()
					.Where(t => t.DateTime >= new DateTime(2015, 12, 25) && t.Payee.With(_ => _.Title) == "Ашан");

			using (FileStream file = File.Create("test.csv"))
			{
				using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
				{
					var csv = new CsvWriter(writer);

					csv.WriteExcelSeparator();
					csv.WriteField("Date&Time");
					csv.WriteField("Account");
					csv.WriteField("Amount");
					csv.WriteField("Category");
					csv.WriteField("Payee");
					csv.WriteField("Note");
					csv.NextRecord();

					foreach (Transaction tran in transactions)
					{
						csv.WriteField(tran.DateTime);
						csv.WriteField(tran.From.With(_ => _.Title));
						csv.WriteField(Math.Abs((double) tran.FromAmount).ToString("0.00"));
						csv.WriteField(tran.Category.With(_ => _.Title));
						csv.WriteField(tran.Payee.With(_ => _.Title));
						csv.WriteField(tran.Note);
						csv.NextRecord();
					}
				}
			}
		}
	}
}
