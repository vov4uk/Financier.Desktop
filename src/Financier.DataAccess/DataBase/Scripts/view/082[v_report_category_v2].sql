CREATE VIEW v_report_category_v2 AS
SELECT t._id,
       t.parent_id,
       t.name,
       t.datetime,
       t.from_account_currency_id,
       t.from_amount,
       t.to_account_currency_id,
       t.to_amount,
       t.original_currency_id,
       t.original_from_amount,
       t.is_transfer,
       t.from_account_id,
       t.to_account_id,
       t.category_id,
       t.category_left,
       t.category_right,
       t.project_id,
       t.location_id,
       t.payee_id,
       t.status,
       CASE (SELECT _id FROM currency WHERE is_default = 1)
         WHEN fc.currency_id THEN from_amount
         ELSE Round(from_amount * (SELECT rate
                                   FROM   v_currency_exchange_rate
                                   WHERE  to_currency_id = (SELECT _id FROM currency WHERE is_default = 1)
                                          AND from_currency_id = fc.currency_id
                                          AND ( ( datetime BETWEEN rate_date AND rate_date_end )
                                                 OR rate_date_end = 253402293599000)), 0)
       END
       from_amount_default_currency,
       CASE (SELECT _id FROM currency WHERE is_default = 1)
         WHEN tc.currency_id THEN to_amount
         ELSE Round(to_amount * (SELECT rate
                                 FROM   v_currency_exchange_rate
                                 WHERE  to_currency_id = (SELECT _id FROM currency WHERE  is_default = 1)
                                        AND from_currency_id = tc.currency_id
                                        AND ( ( datetime BETWEEN rate_date AND rate_date_end )
                                               OR rate_date_end = 253402293599000 )), 0)
       END
       to_amount_default_currency
FROM   v_report_category t
       LEFT JOIN account fc
              ON t.from_account_id = fc._id
       LEFT JOIN account tc
              ON t.to_account_id = tc._id;