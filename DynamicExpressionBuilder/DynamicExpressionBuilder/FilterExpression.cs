using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicExpressionBuilder
{
    public class FilterExpression<T> where T : new()
    {
        public Expression<Func<T, bool>> filter;

        public FilterExpression() { }

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
            ParameterExpression[] parameters = expression.Parameters
                .Select(NewParameter)
                .ToArray();

            if (condition)
            {
                Expression exp1 = RebuildExpression(filter.Body, parameters[0]);
                Expression exp2 = RebuildExpression(expression.Body, parameters[0]);
                if (filter != null)
                    filter = (Expression<Func<T, bool>>)Expression.Lambda(Expression.AndAlso(exp1, exp2), parameters);
                else if (filter == null)
                    filter = expression;
            }

            return this;
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression)
        {
            return Or(expression, true);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression, bool condition)
        {
            ParameterExpression[] parameters = expression.Parameters
                .Select(NewParameter)
                .ToArray();

            if (condition)
            {
                Expression exp1 = RebuildExpression(filter.Body, parameters[0]);
                Expression exp2 = RebuildExpression(expression.Body, parameters[0]);
                if (filter != null)
                    filter = (Expression<Func<T, bool>>)Expression.Lambda(Expression.Or(exp1, exp2), parameters);
                else if (filter == null)
                    filter = expression;
            }

            return this;
        }

        private static Expression RebuildExpression(Expression body, ParameterExpression parameters)
        {
            if (body is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)body;
                Expression expression = RebuildExpression(methodCallExpression.Object, parameters);
                return Expression.Call(expression, methodCallExpression.Method, methodCallExpression.Arguments);
            }
            if (body is MemberExpression)
            {
                var memberExpression = (MemberExpression)body;
                return Expression.Property(parameters, (PropertyInfo)memberExpression.Member);
            }
            if (body is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression)body;
                Expression left = RebuildExpression(binaryExpression.Left, parameters);
                Expression right = RebuildExpression(binaryExpression.Right, parameters);
                return Expression.MakeBinary(binaryExpression.NodeType, left, right);
            }
            if (body is ConstantExpression)
                return body;
            return Expression.Empty();
        }

        private static ParameterExpression NewParameter(ParameterExpression parameterExpression)
        {
            return Expression.Variable(parameterExpression.Type, parameterExpression.Name);
        }

        public Func<T, bool> ResultExpression
        {
            get { return filter.Compile(); }
        }
    }
}