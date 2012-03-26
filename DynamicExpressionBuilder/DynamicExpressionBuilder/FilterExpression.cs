using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicExpressionBuilder
{
    public class FilterExpression<T> where T : new()
    {
        public Func<T, bool> ResultExpression { get { return GetWhereExpression(); } }
        public Expression<Func<T, bool>> filter;

        public FilterExpression()
        {

        }

        protected FilterExpression(Expression<Func<T, bool>> expression)
        {
            filter = expression;
        }

        public FilterExpression<T> Start()
        {
            return this;
        }

        public FilterExpression<T> Start(Expression<Func<T, bool>> expression)
        {
            return Start(expression, true);
        }

        public FilterExpression<T> Start(Expression<Func<T, bool>> expression, bool condition)
        {
            filter = condition ? expression : null;
            return new FilterExpression<T>(filter);
        }


        public FilterExpression<T> And(Expression<Func<T, bool>> expression)
        {
            return And(expression, true);
        }

        public FilterExpression<T> And(Expression<Func<T, bool>> expression, bool condition)
        {
            var parameters = expression.Parameters
                .Select(NewParameter)
                .ToArray();

            if (condition)
            {
                ApplySameParameter(filter.Body, parameters);
                ApplySameParameter(expression.Body, parameters);
                if (filter != null)
                    filter = (Expression<Func<T, bool>>)Expression.Lambda(Expression.AndAlso(filter.Body, expression.Body), parameters);
                else if(filter == null)
                    filter = expression;
            }

            return this;
        }

        private void ApplySameParameter(Expression body, ParameterExpression[] parameters)
        {
            if (body is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression) body;
                ApplySameParameter(methodCallExpression.Object,parameters);
            } else if (body is MemberExpression)
            {
                var memberExpression = (MemberExpression) body;
                memberExpression = Expression.Property(parameters[0], memberExpression.Member as PropertyInfo);
            }
        }

        private static ParameterExpression NewParameter(ParameterExpression parameterExpression)
        {
            return Expression.Variable(parameterExpression.Type, parameterExpression.Name);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression)
        {
            return Or(expression, true);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression, bool condition)
        {
            var binaryExpression = Expression.Or(Expression.Constant(true), Expression.Constant(true));
            if (condition)
                binaryExpression = Expression.Or(filter, expression);

            var lambdaExpression = Expression.Lambda(typeof(Func<T, bool>), binaryExpression, expression.Parameters.ToArray());
            return new FilterExpression<T>((Expression<Func<T, bool>>)lambdaExpression);
        }

        private Func<T, bool> GetWhereExpression()
        {
            return filter.Compile();
        }
    }
}