create view v_report_location AS 
select 
       l._id as _id,
       l.title as name,
       t.datetime as datetime,
       t.from_account_currency_id as from_account_currency_id,
       t.from_amount as from_amount,
       t.to_account_currency_id as to_account_currency_id,
       t.to_amount as to_amount,
       t.original_currency_id as original_currency_id,
       t.original_from_amount as original_from_amount,
       t.is_transfer as is_transfer,
       t.from_account_id as from_account_id,
       t.to_account_id as to_account_id,
       t.category_id as category_id,
       t.category_left as category_left,
       t.category_right as category_right,
       t.project_id as project_id,
       t.location_id as location_id,
       t.payee_id as payee_id,
       t.status as status
from locations l
inner join v_blotter_for_account t on t.location_id=l._id
where l._id != 0 and from_account_is_include_into_totals=1;
