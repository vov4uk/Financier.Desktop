CREATE VIEW v_report_transactions AS
SELECT t._id,
       t.from_account_id,
       t.to_account_id,
       t.category_id,
       t.project_id,
       t.location_id,
       t.note,
       t.from_amount,
       t.to_amount,
       t.datetime,
       t.provider,
       t.accuracy,
       t.latitude,
       t.longitude,
       t.payee,
       t.is_template,
       t.template_name,
       t.recurrence,
       t.notification_options,
       t.status,
       t.attached_picture,
       t.is_ccard_payment,
       t.last_recurrence,
       t.payee_id,
       t.parent_id,
       t.updated_on,
       t.remote_key,
       t.original_currency_id,
       t.original_from_amount,
       t.blob_key,
       fc.currency_id AS from_account_crc_id,
       tc.currency_id AS to_account_crc_id,
       CASE (SELECT _id FROM currency WHERE  is_default = 1)
         WHEN fc.currency_id THEN from_amount
         ELSE Round(from_amount * (SELECT rate
                                   FROM   v_currency_exchange_rate
                                   WHERE  to_currency_id = (SELECT _id FROM currency WHERE is_default = 1)
                                          AND from_currency_id = fc.currency_id
                                          AND ( ( datetime BETWEEN rate_date AND rate_date_end )
                                                 OR rate_date_end = 253402293599000)), 0)
       END
       from_amount_default_crr,
       CASE (SELECT _id FROM currency WHERE  is_default = 1)
         WHEN tc.currency_id THEN to_amount
         ELSE Round(to_amount * (SELECT rate
                                 FROM   v_currency_exchange_rate
                                 WHERE  to_currency_id = (SELECT _id FROM currency WHERE  is_default = 1)
                                        AND from_currency_id = tc.currency_id
                                        AND ( ( datetime BETWEEN rate_date AND rate_date_end )
                                               OR rate_date_end = 253402293599000 )), 0)
       END
       to_amount_default_crr,
       Strftime('%Y', Date(datetime / 1000, 'unixepoch')) date_year,
       Strftime('%m', Date(datetime / 1000, 'unixepoch')) date_month,
       Strftime('%d', Date(datetime / 1000, 'unixepoch')) date_day,
       Strftime('%W', Date(datetime / 1000, 'unixepoch')) date_week,
       Strftime('%w', Date(datetime / 1000, 'unixepoch')) date_weekday
FROM   v_report_category t
       LEFT JOIN account fc
              ON t.from_account_id = fc._id
       LEFT JOIN account tc
              ON t.to_account_id = tc._id;