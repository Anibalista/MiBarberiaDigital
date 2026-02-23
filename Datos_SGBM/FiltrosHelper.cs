using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Datos_SGBM
{
    public static class FiltrosHelper
    {
        // Para números
        public static IQueryable<TEntity> AplicarFiltroNumerico<TEntity, TProperty>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TProperty>> selector,
            string criterio,
            decimal valor) where TProperty : struct, IComparable
        {
            var parameter = selector.Parameters[0];
            var property = selector.Body;
            var constant = Expression.Constant(Convert.ChangeType(valor, typeof(TProperty)));

            Expression body = criterio.ToLower() switch
            {
                "mayor a" => Expression.GreaterThan(property, constant),
                "menor a" => Expression.LessThan(property, constant),
                "igual a" => Expression.Equal(property, constant),
                "mayor o igual a" => Expression.GreaterThanOrEqual(property, constant),
                "menor o igual a" => Expression.LessThanOrEqual(property, constant),
                _ => null
            };

            if (body == null) return query;

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            return query.Where(lambda);
        }

        // Para texto
        public static IQueryable<TEntity> AplicarFiltroTexto<TEntity>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, string>> selector,
            string criterio,
            string valor)
        {
            var parameter = selector.Parameters[0];
            var property = selector.Body;
            var method = criterio.ToLower() switch
            {
                "contiene" => typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                "comienza con" => typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                "termina con" => typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                "igual a" => typeof(string).GetMethod("Equals", new[] { typeof(string) }),
                "no contiene" => null, // lo tratamos aparte
                _ => null
            };

            if (criterio.ToLower() == "no contiene")
            {
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var nullCheck = Expression.Equal(property, Expression.Constant(null, typeof(string)));
                var call = Expression.Call(property, containsMethod!, Expression.Constant(valor));
                var notCall = Expression.Not(call);
                var orExpr = Expression.OrElse(nullCheck, notCall);

                var lambda = Expression.Lambda<Func<TEntity, bool>>(orExpr, parameter);
                return query.Where(lambda);
            }

            if (method == null) return query;

            var callExpr = Expression.Call(property, method!, Expression.Constant(valor));
            var lambdaFinal = Expression.Lambda<Func<TEntity, bool>>(callExpr, parameter);
            return query.Where(lambdaFinal);
        }
    }
}
