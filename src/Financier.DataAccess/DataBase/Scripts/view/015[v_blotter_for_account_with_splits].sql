CREATE VIEW v_blotter_for_account_with_splits AS
SELECT
    t._id as _id,
    t.parent_id as parent_id,
    a._id as from_account_id,
    a.title as from_account_title,
    a.is_include_into_totals as from_account_is_include_into_totals,
    c._id as from_account_currency_id,
    a2._id as to_account_id,
    a2.title as to_account_title,
    a2.currency_id as to_account_currency_id,
    cat._id as category_id,
    cat.title as category_title,
    cat.left as category_left,
    cat.right as category_right,
    cat.type as category_type,
    p._id as project_id,
    p.title as project,
    loc._id as location_id,
    loc.title as location,
    pp._id as payee_id,
    pp.title as payee,
    t.note as note,
    t.from_amount as from_amount,
    t.to_amount as to_amount,
    t.datetime as datetime,
    t.original_currency_id as original_currency_id,
    t.original_from_amount as original_from_amount,
    t.is_template as is_template,
    t.template_name as template_name,
    t.recurrence as recurrence,
    t.notification_options as notification_options,
    t.status as status,
    t.is_ccard_payment as is_ccard_payment,
    t.last_recurrence as last_recurrence,
    t.attached_picture as attached_picture,
    rb.balance as from_account_balance,
    0 as to_account_balance,
    t.to_account_id as is_transfer
FROM
    transactions as t
    INNER JOIN account as a ON a._id=t.from_account_id
    INNER JOIN currency as c ON c._id=a.currency_id
    INNER JOIN category as cat ON cat._id=t.category_id
    LEFT OUTER JOIN running_balance as rb ON rb.transaction_id=(CASE WHEN t.parent_id=0 THEN t._id ELSE t.parent_id END) AND rb.account_id=t.from_account_id
    LEFT OUTER JOIN account as a2 ON a2._id=t.to_account_id
    LEFT OUTER JOIN locations as loc ON loc._id=t.location_id
    LEFT OUTER JOIN project as p ON p._id=t.project_id
    LEFT OUTER JOIN payee as pp ON pp._id=t.payee_id
WHERE is_template=0
UNION ALL
SELECT
    t._id as _id,
    t.parent_id as parent_id,
    a._id as from_account_id,
    a.title as from_account_title,
    a.is_include_into_totals as from_account_is_include_into_totals,
    c._id as from_account_currency_id,
    a2._id as to_account_id,
    a2.title as to_account_title,
    a2.currency_id as to_account_currency_id,
    cat._id as category_id,
    cat.title as category_title,
    cat.left as category_left,
    cat.right as category_right,
    cat.type as category_type,
    p._id as project_id,
    p.title as project,
    loc._id as location_id,
    loc.title as location,
    pp._id as payee_id,
    pp.title as payee,
    t.note as note,
    t.to_amount as from_amount,
    t.from_amount as to_amount,
    t.datetime as datetime,
    t.original_currency_id as original_currency_id,
    t.original_from_amount as original_from_amount,
    t.is_template as is_template,
    t.template_name as template_name,
    t.recurrence as recurrence,
    t.notification_options as notification_options,
    t.status as status,
    t.is_ccard_payment as is_ccard_payment,
    t.last_recurrence as last_recurrence,
    t.attached_picture as attached_picture,
    rb.balance as from_account_balance,
    0 as to_account_balance,
    -1 as is_transfer
FROM
    transactions as t
    INNER JOIN account as a ON a._id=t.to_account_id
    INNER JOIN currency as c ON c._id=a.currency_id
    INNER JOIN category as cat ON cat._id=t.category_id
    LEFT OUTER JOIN running_balance as rb ON rb.transaction_id=t._id AND rb.account_id=t.to_account_id
    LEFT OUTER JOIN account as a2 ON a2._id=t.from_account_id
    LEFT OUTER JOIN locations as loc ON loc._id=t.location_id
    LEFT OUTER JOIN project as p ON p._id=t.project_id
    LEFT OUTER JOIN payee as pp ON pp._id=t.payee_id
WHERE is_template=0;