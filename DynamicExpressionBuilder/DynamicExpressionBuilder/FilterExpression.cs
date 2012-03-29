using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicExpressionBuilder
{
    public class FilterExpression<T>
    {
        public Expression<Func<T, bool>> filter;

        public FilterExpression() {}

        protected FilterExpression(Expression<Func<T, bool>> expression)
        {
            filter = expression;
        }

        public Func<T, bool> ResultExpression
        {
            get { return filter.Compile(); }
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
            ParameterExpression parameter = expression.Parameters.Select(NewParameter).First();

            CombineExp(expression, condition, parameter, Expression.AndAlso);

            return this;
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression)
        {
            return Or(expression, true);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression, bool condition)
        {
            ParameterExpression parameter = expression.Parameters.Select(NewParameter).First();

            CombineExp(expression, condition, parameter, Expression.Or);

            return this;
        }

        private void CombineExp(Expression<Func<T, bool>> expression, bool condition, ParameterExpression parameter, Func<Expression, Expression, BinaryExpression> exp)
        {
            if (condition)
                if (filter != null)
                {
                    Expression left = Rebuild(filter.Body, parameter);
                    Expression right = Rebuild(expression.Body, parameter);
                    filter = (Expression<Func<T, bool>>) Expression.Lambda(exp(left, right), parameter);
                } else if (filter == null)
                    filter = expression;
        }

        private static Expression Rebuild(Expression body, ParameterExpression parameters)
        {
            var callExp = body as MethodCallExpression;
            if (callExp != null)
            {
                var arguments = callExp.Arguments.Select(expression => Rebuild(expression, parameters));
                return Expression.Call(Rebuild(callExp.Object, parameters), callExp.Method, arguments);
            }

            var memberExp = body as MemberExpression;
            if (memberExp != null)
                return Expression.Property(parameters, (PropertyInfo) memberExp.Member);

            var binExp = body as BinaryExpression;
            if (binExp != null)
                return Expression.MakeBinary(binExp.NodeType,
                                             Rebuild(binExp.Left, parameters),
                                             Rebuild(binExp.Right, parameters));

            return body;
        }

        private static ParameterExpression NewParameter(ParameterExpression parameterExpression)
        {
            return Expression.Variable(parameterExpression.Type, parameterExpression.Name);
        }
    }
}