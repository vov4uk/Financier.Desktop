﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Financier.DataAccess.DataBase.Scripts {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SQL_views_files {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SQL_views_files() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Financier.DataAccess.DataBase.Scripts.SQL_views_files", typeof(SQL_views_files).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_all_transactions AS 
        ///SELECT
        ///    t._id as _id,
        ///    t.parent_id as parent_id,
        ///    a1._id as from_account_id,
        ///    a1.title as from_account_title,
        ///    a1.is_include_into_totals as from_account_is_include_into_totals,
        ///    c1._id as from_account_currency_id,
        ///    a2._id as to_account_id,
        ///    a2.title as to_account_title,
        ///    c2._id as to_account_currency_id,
        ///    cat._id as category_id,
        ///    cat.title as category_title,
        ///    cat.left as category_left,
        ///    cat.right as category_right,
        ///     [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _010_v_all_transactions_ {
            get {
                return ResourceManager.GetString("_010_v_all_transactions_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_blotter_for_account_with_splits AS
        ///SELECT
        ///    t._id as _id,
        ///    t.parent_id as parent_id,
        ///    a._id as from_account_id,
        ///    a.title as from_account_title,
        ///    a.is_include_into_totals as from_account_is_include_into_totals,
        ///    c._id as from_account_currency_id,
        ///    a2._id as to_account_id,
        ///    a2.title as to_account_title,
        ///    a2.currency_id as to_account_currency_id,
        ///    cat._id as category_id,
        ///    cat.title as category_title,
        ///    cat.left as category_left,
        ///    cat.right as cat [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _015_v_blotter_for_account_with_splits_ {
            get {
                return ResourceManager.GetString("_015_v_blotter_for_account_with_splits_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_blotter AS 
        ///SELECT *
        ///FROM v_all_transactions
        ///WHERE is_template = 0 AND parent_id=0;.
        /// </summary>
        internal static string _020_v_blotter_ {
            get {
                return ResourceManager.GetString("_020_v_blotter_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_blotter_for_account AS 
        ///SELECT *
        ///FROM v_blotter_for_account_with_splits
        ///WHERE is_template=0 AND parent_id=0;.
        /// </summary>
        internal static string _021_v_blotter_for_account_ {
            get {
                return ResourceManager.GetString("_021_v_blotter_for_account_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_blotter_flatsplits
        ///AS
        ///  SELECT *
        ///  FROM   v_all_transactions
        ///  WHERE  is_template = 0
        ///         AND _id NOT IN (SELECT DISTINCT parent_id
        ///                         FROM   transactions
        ///                         WHERE  is_template = 0
        ///                                AND parent_id &gt; 0); .
        /// </summary>
        internal static string _022_v_blotter_flatsplits_ {
            get {
                return ResourceManager.GetString("_022_v_blotter_flatsplits_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_category AS 
        ///SELECT 
        ///    node._id as _id,
        ///    node.title as title,
        ///    node.left as left,
        ///    node.right as right,
        ///    node.type as type,
        ///    node.last_location_id as last_location_id,
        ///    node.last_project_id as last_project_id,
        ///    node.sort_order as sort_order,
        ///    count(parent._id)-1 as level
        ///FROM
        ///    category as node,
        ///    category as parent
        ///WHERE node.left BETWEEN parent.left AND parent.right
        ///GROUP BY node._id ORDER BY node.left;.
        /// </summary>
        internal static string _030_v_category_ {
            get {
                return ResourceManager.GetString("_030_v_category_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_category_list AS
        ///SELECT 
        ///    B._id AS parent_id,
        ///    B.title AS parent_title,
        ///    B.left AS parent_left,
        ///    B.right AS parent_right,
        ///    B.type AS parent_type,
        ///    P._id as _id,
        ///    P.title AS title,
        ///    P.left as left,
        ///    P.right as right,
        ///    P.type as type
        ///FROM category AS B, category AS P
        ///WHERE P.left BETWEEN B.left AND B.right
        ///AND B._id = (SELECT MAX(S._id)
        ///FROM category AS S
        ///WHERE S.left &lt; P.left
        ///AND S.right &gt; P.right);
        ///.
        /// </summary>
        internal static string _035_v_category_list_ {
            get {
                return ResourceManager.GetString("_035_v_category_list_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_attributes AS 
        ///SELECT
        ///    a._id as _id,
        ///    a.title as title,
        ///    a.type as type,
        ///    a.list_values as list_values,
        ///    a.default_value as default_value,
        ///    c._id as category_id,
        ///    c.left as category_left,
        ///    c.right as category_right
        ///FROM
        ///    attributes as a,
        ///    category_attribute as ca,
        ///    category c
        ///WHERE
        ///    ca.attribute_id=a._id
        ///    AND ca.category_id=c._id;.
        /// </summary>
        internal static string _050_v_attributes_ {
            get {
                return ResourceManager.GetString("_050_v_attributes_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_report_category AS 
        ///select
        ///       c._id as _id,
        ///       c.parent_id as parent_id,
        ///       c.title as name,
        ///       t.datetime as datetime,
        ///       t.from_account_currency_id as from_account_currency_id,
        ///       t.from_amount as from_amount,
        ///       t.to_account_currency_id as to_account_currency_id,
        ///       t.to_amount as to_amount,
        ///       t.original_currency_id as original_currency_id,
        ///       t.original_from_amount as original_from_amount,
        ///       t.is_transfer as is_transfer,
        ///       t.f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _060_v_report_category_ {
            get {
                return ResourceManager.GetString("_060_v_report_category_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_report_sub_category AS 
        ///select 
        ///       c._id as _id,
        ///       c.left as left,
        ///       c.right as right,
        ///       c.title as name,
        ///       t.datetime as datetime,
        ///       t.from_account_currency_id as from_account_currency_id,
        ///       t.from_amount as from_amount,
        ///       t.to_account_currency_id as to_account_currency_id,
        ///       t.to_amount as to_amount,
        ///       t.original_currency_id as original_currency_id,
        ///       t.original_from_amount as original_from_amount,
        ///       t.is_transfer as is_ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _060_v_report_sub_category_ {
            get {
                return ResourceManager.GetString("_060_v_report_sub_category_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_report_period AS 
        ///select 
        ///       0 as _id,
        ///       null as name,
        ///       t.datetime as datetime,
        ///       t.from_account_currency_id as from_account_currency_id,
        ///       t.from_amount as from_amount,
        ///       t.to_account_currency_id as to_account_currency_id,
        ///       t.to_amount as to_amount,
        ///       t.is_transfer as is_transfer,
        ///       t.original_currency_id as original_currency_id,
        ///       t.original_from_amount as original_from_amount,
        ///       t.from_account_id as from_account_id,
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _061_v_report_period_ {
            get {
                return ResourceManager.GetString("_061_v_report_period_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_report_location AS 
        ///select 
        ///       l._id as _id,
        ///       l.title as name,
        ///       t.datetime as datetime,
        ///       t.from_account_currency_id as from_account_currency_id,
        ///       t.from_amount as from_amount,
        ///       t.to_account_currency_id as to_account_currency_id,
        ///       t.to_amount as to_amount,
        ///       t.original_currency_id as original_currency_id,
        ///       t.original_from_amount as original_from_amount,
        ///       t.is_transfer as is_transfer,
        ///       t.from_account_id as from_account_id [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _062_v_report_location_ {
            get {
                return ResourceManager.GetString("_062_v_report_location_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_report_project AS 
        ///select 
        ///       p._id as _id,
        ///       p.title as name,
        ///       t.datetime as datetime,
        ///       t.from_account_currency_id as from_account_currency_id,
        ///       t.from_amount as from_amount,
        ///       t.to_account_currency_id as to_account_currency_id,
        ///       t.to_amount as to_amount,
        ///       t.original_currency_id as original_currency_id,
        ///       t.original_from_amount as original_from_amount,
        ///       t.is_transfer as is_transfer,
        ///       t.from_account_id as from_account_id, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _063_v_report_project_ {
            get {
                return ResourceManager.GetString("_063_v_report_project_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_report_payee AS
        ///select
        ///       p._id as _id,
        ///       p.title as name,
        ///       t.datetime as datetime,
        ///       t.from_account_currency_id as from_account_currency_id,
        ///       t.from_amount as from_amount,
        ///       t.to_account_currency_id as to_account_currency_id,
        ///       t.to_amount as to_amount,
        ///       t.is_transfer as is_transfer,
        ///       t.original_currency_id as original_currency_id,
        ///       t.original_from_amount as original_from_amount,
        ///       t.from_account_id as from_account_id,
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _064_v_report_payee_ {
            get {
                return ResourceManager.GetString("_064_v_report_payee_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create view v_transaction_attributes AS 
        ///SELECT
        ///    t._id as _id,
        ///    t.parent_id as parent_id,
        ///    a._id as attribute_id,
        ///    a.type as attribute_type,
        ///    a.title as attribute_name,
        ///    a.list_values as attribute_list_values,
        ///    a.default_value as attribute_default_value,
        ///    ta.value as attribute_value
        ///FROM
        ///    transactions t
        ///    INNER JOIN transaction_attribute ta ON ta.transaction_id=t._id
        ///    INNER JOIN attributes a ON a._id=ta.attribute_id
        ///ORDER BY 
        ///    a.title;.
        /// </summary>
        internal static string _080_v_transaction_attributes_ {
            get {
                return ResourceManager.GetString("_080_v_transaction_attributes_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_currency_exchange_rate AS
        ///SELECT from_currency_id,
        ///       to_currency_id,
        ///       rate_date,
        ///       Ifnull((SELECT rate_date
        ///               FROM   currency_exchange_rate t1
        ///               WHERE  t1.from_currency_id = t.from_currency_id
        ///                      AND t1.to_currency_id = t.to_currency_id
        ///                      AND t1.rate_date &gt; t.rate_date
        ///               ORDER  BY rate_date
        ///               LIMIT  1), 253402293599000) AS rate_date_end,
        ///       rate,
        ///       updated_on,
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _081_v_currency_exchange_rate_ {
            get {
                return ResourceManager.GetString("_081_v_currency_exchange_rate_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_account AS
        ///SELECT _id,
        ///       title,
        ///       creation_date,
        ///       currency_id,
        ///       type,
        ///       issuer,
        ///       number,
        ///       sort_order,
        ///       is_active,
        ///       is_include_into_totals,
        ///       last_category_id,
        ///       last_account_id,
        ///       total_limit,
        ///       card_issuer,
        ///       closing_day,
        ///       payment_day note,
        ///       last_transaction_date,
        ///       updated_on,
        ///       remote_key,
        ///       total_amount,
        ///       CASE (SELECT _id FROM currency WHERE is_default = 1)
        ///    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _083_v_account_ {
            get {
                return ResourceManager.GetString("_083_v_account_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW v_report_transactions AS
        ///SELECT t._id,
        ///       t.from_account_id,
        ///       fc.is_include_into_totals as from_account_is_include_into_totals,
        ///       t.to_account_id,
        ///       t.category_id,
        ///       t.project_id,
        ///       t.location_id,
        ///       t.from_amount,
        ///       t.to_amount,
        ///       t.datetime,
        ///       t.provider,
        ///       t.accuracy,
        ///       t.latitude,
        ///       t.longitude,
        ///       t.payee,
        ///       t.is_template,
        ///       t.template_name,
        ///       t.recurrence,
        ///       t.notification_options,
        ///  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _084_v_report_transactions_ {
            get {
                return ResourceManager.GetString("_084_v_report_transactions_", resourceCulture);
            }
        }
    }
}
