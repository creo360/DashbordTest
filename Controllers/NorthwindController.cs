using DashboardTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DashboardTest.Helpers;
using System.Linq.Expressions;
using System.Reflection;



namespace DashboardTest.Controllers
{
    public class NorthwindController : Controller
    {
        //
        // GET: /Northwind/

        public ActionResult Index()
        {
            return View("Orders");
        }

        public ActionResult Orders(PageModel pageModel)
        {
            NorthwindEntities model = new NorthwindEntities();

            pageModel.PageNumber = pageModel.PageNumber.HasValue ? pageModel.PageNumber.Value : 1;
            pageModel.PageSize = pageModel.PageSize.HasValue ? pageModel.PageSize.Value : 5;
            var viewModel = model.Invoices.OrderBy(x => x.OrderID).Skip((pageModel.PageNumber.Value - 1) * pageModel.PageSize.Value).Take(pageModel.PageSize.Value).ToList();
            int totCount = model.Invoices.Count();

            List<Invoice> filteredResult = new List<Invoice>();
            if (pageModel.FilteredList != null)
            {

                Expression<Func<Invoice, bool>> completeExp = null;
                Expression<Func<Invoice, bool>> lambda = null;
                ParameterExpression parameterExp = Expression.Parameter(typeof(Invoice), "x"); //To keep one parameter expression in all conditions {x}

                foreach (var item in pageModel.FilteredList)
                {
                    lambda = GetExpression<Invoice>(item.Field, item.Value, parameterExp);//x => x.InvoiceID.ToString().Contains( propertyValue)
                    if (completeExp != null)
                    {
                        completeExp = Expression.Lambda<Func<Invoice, bool>>(Expression.AndAlso(completeExp.Body, lambda.Body), parameterExp);
                    }
                    else
                    {
                        completeExp = lambda;
                    }

                }

                if (completeExp == null)
                {
                    totCount = model.Invoices.Where(lambda).Count();
                    viewModel = model.Invoices.Where(lambda).OrderBy(x => x.OrderID).Skip((pageModel.PageNumber.Value - 1) * pageModel.PageSize.Value).Take(pageModel.PageSize.Value).ToList();
                }
                else
                {
                    totCount = model.Invoices.Where(completeExp).Count();
                    viewModel = model.Invoices.Where(completeExp).OrderBy(x => x.OrderID).Skip((pageModel.PageNumber.Value - 1) * pageModel.PageSize.Value).Take(pageModel.PageSize.Value).ToList();
                }

            }

            
            var list = JsonConvert.SerializeObject(viewModel, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
             

            //totCount = pageModel.FilteredList == null ? model.OrderSummaries.Count() : filteredResult.Count();

            return Json(new { totCount = totCount, data = viewModel }, JsonRequestBehavior.AllowGet);
        }



        public Expression<Func<T, bool>> GetExpression<T>(string propertyName, string propertyValue, ParameterExpression parameterExp)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName);
            Type propertyType = propertyInfo.PropertyType;

            //ParameterExpression parameterExp = Expression.Parameter(typeof(T), "x");//{x}
            MemberExpression propertyExp = Expression.Property(parameterExp, propertyName);//x.EmployeID
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            ConstantExpression someValue = Expression.Constant(propertyValue, typeof(string));
            MethodCallExpression containsMethodExp = null;


            if (propertyType == typeof(Nullable<long>) || propertyType == typeof(long))
            {
                Int64? castVal = null;
                Int64 outValue = 0;
                castVal = long.TryParse(propertyValue, out outValue) ? (Int64?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(long?)) : Expression.Constant(castVal, typeof(long)); // {5}
                    Expression numericExpression = Expression.Equal(propertyExp, someValue); //type.EmployeId == 5
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp); //type=>(false == true) so inside where condition Where(type=>false)

            }
            else if (propertyType == typeof(Nullable<int>) || propertyType == typeof(int))
            {
                int? castVal = null;
                int outValue = 0;
                castVal = int.TryParse(propertyValue, out outValue) ? (int?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(int?)) : Expression.Constant(castVal, typeof(int));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }

                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);
            }
            else if (propertyType == typeof(Nullable<short>) || propertyType == typeof(short))
            {
                Int16? castVal = null;
                Int16 outValue = 0;
                castVal = short.TryParse(propertyValue, out outValue) ? (Int16?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(Int16?)) : Expression.Constant(castVal, typeof(Int16));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);
            }
            else if (propertyType == typeof(Nullable<double>) || propertyType == typeof(double))
            {
                double? castVal = null;
                double outValue = 0;
                castVal = double.TryParse(propertyValue, out outValue) ? (double?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(double?)) : Expression.Constant(castVal, typeof(double));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);

            }
            else if (propertyType == typeof(Nullable<float>) || propertyType == typeof(float))
            {
                float? castVal = null;
                float outValue = 0;
                castVal = float.TryParse(propertyValue, out outValue) ? (float?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(float?)) : Expression.Constant(castVal, typeof(float));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);
            }
            else if (propertyType == typeof(Nullable<decimal>) || propertyType == typeof(decimal))
            {
                decimal? castVal = null;
                decimal outValue = 0;
                castVal = decimal.TryParse(propertyValue, out outValue) ? (decimal?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(decimal?)) : Expression.Constant(castVal, typeof(decimal));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);

            }
            else if (propertyType == typeof(Nullable<bool>) || propertyType == typeof(bool))
            {
                bool? castVal = null;
                bool outValue = false;
                castVal = bool.TryParse(propertyValue, out outValue) ? (bool?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(bool?)) : Expression.Constant(castVal, typeof(bool));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);
            }
            else if (propertyType == typeof(Nullable<byte>) || propertyType == typeof(byte))
            {
                byte? castVal = null;
                byte outValue = 0;
                castVal = byte.TryParse(propertyValue, out outValue) ? (byte?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(byte?)) : Expression.Constant(castVal, typeof(byte));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);
            }
            else if (propertyType == typeof(Nullable<DateTime>) || propertyType == typeof(DateTime))
            {
                DateTime? castVal = null;
                DateTime outValue = DateTime.MinValue;
                castVal = (DateTime.TryParse(propertyValue, out outValue) && outValue != DateTime.MinValue) ? (DateTime?)outValue : null;
                if (castVal.HasValue)
                {
                    someValue = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Expression.Constant(castVal, typeof(DateTime?)) : Expression.Constant(castVal, typeof(DateTime));
                    Expression numericExpression = Expression.Equal(propertyExp, someValue);
                    //containsMethodExp = Expression.Call(propertyExp, method, someValue);
                    return Expression.Lambda<Func<T, bool>>(numericExpression, parameterExp);
                }
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Constant(false), Expression.Constant(true)), parameterExp);
            }

            MethodInfo methodToStringX = typeof(string).GetMethod("ToString", Type.EmptyTypes);
            //containsMethodExp = Expression.Call(propertyExp, method, someValue);
            containsMethodExp = Expression.Call(propertyExp, methodToStringX);
            containsMethodExp = Expression.Call(containsMethodExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        }




    }

    public class FilterObj
    {
        public string Value { get; set; }
        public string Field { get; set; }
    }

    public class PageModel
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public List<FilterObj> FilteredList { get; set; }
    }

    public static class Utilities
    {
        /*
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left,Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                    Expression.Invoke(left, param),
                    Expression.Invoke(right, param)
                );
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }
         */

    }
}
