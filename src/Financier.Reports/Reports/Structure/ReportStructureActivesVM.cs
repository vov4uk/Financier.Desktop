﻿using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using System.Collections.Generic;
using System.Windows;

namespace Financier.Reports.Reports
{
    [Header("Структура активов")]
    public class ReportStructureActivesVM : BaseReportVM<ReportStructureActivesModel>
    {
        private const string BaseSqlText = @"
SELECT account_title,
       account_id,
       account_is_active,
       is_include_into_totals,
       sort_order,
       balance,
       symbol,
       balance_default_crr,
       default_crr_symbol,
       date
FROM   (SELECT a.title AS account_title,
               a.is_active AS account_is_active,
               a.is_include_into_totals,
               a.sort_order,
               a._id AS account_id,
               Row_number() OVER ( partition BY a._id
                                   ORDER BY Date(t.datetime / 1000, 'unixepoch') DESC, t.datetime DESC
               ) AS RowNum,
               r.balance / 100.0 AS balance,
               c.symbol,
               CASE( SELECT _id FROM currency WHERE is_default = 1)
               WHEN c._id THEN r.balance / 100.0
               ELSE Round((r.balance / 100.0 ) * (SELECT rate
                                                  FROM v_currency_exchange_rate
                                                  WHERE to_currency_id = (SELECT _id FROM   currency WHERE is_default = 1)
                                                        AND from_currency_id = c._id
                                                        AND((t.datetime BETWEEN rate_date AND rate_date_end) OR rate_date_end = 253402293599000 )), 0)
               END AS balance_default_crr,
               (SELECT symbol FROM   currency WHERE  is_default = 1) AS default_crr_symbol,
               Date(t.datetime / 1000, 'unixepoch') AS date
        FROM running_balance r
             INNER JOIN account a ON a._id = r.account_id
             INNER JOIN currency c ON a.currency_id = c._id
             INNER JOIN transactions t ON t._id = r.transaction_id
        WHERE Date(t.datetime / 1000, 'unixepoch') < {0}
        ORDER BY a._id, r.datetime DESC ) rep
WHERE RowNum = 1";

        public ReportStructureActivesVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            if (!DateFilter.HasValue)
            {
                MessageBox.Show("Please select date!");
                return string.Empty;
            }

            return string.Format(BaseSqlText, GetStandartTrnFilter());
        }

        protected override void SetupSeries(List<ReportStructureActivesModel> list)
        {

        }
    }
}