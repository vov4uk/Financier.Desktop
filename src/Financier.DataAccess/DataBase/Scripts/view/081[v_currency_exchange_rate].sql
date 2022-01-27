CREATE VIEW v_currency_exchange_rate AS
SELECT from_currency_id,
       to_currency_id,
       rate_date,
       Ifnull((SELECT rate_date
               FROM   currency_exchange_rate t1
               WHERE  t1.from_currency_id = t.from_currency_id
                      AND t1.to_currency_id = t.to_currency_id
                      AND t1.rate_date > t.rate_date
               ORDER  BY rate_date
               LIMIT  1), 253402293599000) AS rate_date_end,
       rate,
       updated_on,
       remote_key
FROM   currency_exchange_rate t
ORDER  BY from_currency_id ASC,
          to_currency_id ASC,
          rate_date DESC ;