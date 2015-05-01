using Binbin.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace DashboardTest.Helpers
{
    public static class EntityHelperMethods
    {
        public static IQueryable<TEntity> WhereAllPropertiesOfSimilarTypeAreEqual<TEntity, TProperty>(this IQueryable<TEntity> query, TProperty value)
        {
            var param = Expression.Parameter(typeof(TEntity));

            var predicate = PredicateBuilder.True<TEntity>();

            foreach (var fieldName in GetEntityFieldsToCompareTo<TEntity, TProperty>())
            {
                var predicateToAdd = Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Equal(
                        Expression.PropertyOrField(param, fieldName),
                        Expression.Constant(value)), param);

                predicate = predicate.And(predicateToAdd);
            }

            return query.Where(predicate);
        }

        // TODO: You'll need to find out what fields are actually ones you would want to compare on.
        //       This might involve stripping out properties marked with [NotMapped] attributes, for
        //       for example.
        private static IEnumerable<string> GetEntityFieldsToCompareTo<TEntity, TProperty>()
        {
            Type entityType = typeof(TEntity);
            Type propertyType = typeof(TProperty);

            var fields = entityType.GetFields()
                                .Where(f => f.FieldType == propertyType)
                                .Select(f => f.Name);

            var properties = entityType.GetProperties()
                                    .Where(p => p.PropertyType == propertyType)
                                    .Select(p => p.Name);

            return fields.Concat(properties);
        }
    }
}