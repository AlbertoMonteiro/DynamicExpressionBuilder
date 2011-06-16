using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DynamicExpressionBuilder
{
    public class FilterExpression<T>
    {
        public Func<T, bool> ResultExpression { get { return GetWhereExpression(); } }
        private Expression<Func<T, bool>> _completeExpression;

        public FilterExpression<T> Start(Expression<Func<T, bool>> expression)
        {
            _completeExpression = expression;
            return this;
        }

        public FilterExpression<T> And(Expression<Func<T, bool>> expression)
        {
            return And(expression, true);
        }

        public FilterExpression<T> And(Expression<Func<T, bool>> expression, bool condition)
        {
            if (condition)
            {
                var c1 = _completeExpression.Compile();
                var c2 = expression.Compile();

                _completeExpression = p => c1(p) && c2(p);
            }
            return this;
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression)
        {
            return Or(expression, true);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression, bool condition)
        {
            if (condition)
            {
                var c1 = _completeExpression.Compile();
                var c2 = expression.Compile();

                _completeExpression = p => c1(p) || c2(p);
            }
            return this;
        }

        private Func<T, bool> GetWhereExpression()
        {
            var invocationExpression = Expression.Invoke(_completeExpression, (IEnumerable<Expression>)_completeExpression.Parameters);
            var binaryExpression = Expression.AndAlso(_completeExpression.Body, invocationExpression);
            var andAlso = Expression.AndAlso(binaryExpression, invocationExpression);
            var lambdaExpression = Expression.Lambda<Func<T, bool>>(andAlso, _completeExpression.Parameters);

            return lambdaExpression.Compile();
        }

    }
}