CREATE VIEW v_account AS
SELECT _id,
       title,
       creation_date,
       currency_id,
       type,
       issuer,
       number,
       sort_order,
       is_active,
       is_include_into_totals,
       last_category_id,
       last_account_id,
       total_limit,
       card_issuer,
       closing_day,
       payment_day note,
       last_transaction_date,
       updated_on,
       remote_key,
       total_amount,
       CASE (SELECT _id FROM currency WHERE is_default = 1)
                              WHEN currency_id THEN total_amount
                              ELSE Round(total_amount * (SELECT rate
                                                         FROM v_currency_exchange_rate
                                                         WHERE to_currency_id = (SELECT _id FROM currency WHERE is_default = 1 )
                                         AND from_currency_id = currency_id
                                         AND rate_date_end = 253402293599000), 0)
                            END
       total_amount_indef
FROM   account;