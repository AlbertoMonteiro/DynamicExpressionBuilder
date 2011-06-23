using System;
using System.Linq.Expressions;

namespace DynamicExpressionBuilder
{
    public class FilterExpression<T>
    {
        public Func<T, bool> ResultExpression { get { return GetWhereExpression(); } }
        private Expression<Func<T, bool>> _filter;

        public FilterExpression()
        {
            
        }

        protected FilterExpression(Expression<Func<T, bool>> expression)
        {
            _filter = expression;
        }

        public FilterExpression<T> Start(Expression<Func<T, bool>> expression)
        {
            return Start(expression, true);
        }

        public FilterExpression<T> Start(Expression<Func<T, bool>> expression, bool condition)
        {
            _filter = condition ? expression : (x => true);
            return new FilterExpression<T>(_filter);
        }


        public FilterExpression<T> And(Expression<Func<T, bool>> expression)
        {
            return And(expression, true);
        }

        public FilterExpression<T> And(Expression<Func<T, bool>> expression, bool condition)
        {
            if (condition)
            {
                Func<T, bool> func = _filter.Compile();
                Func<T, bool> func2 = expression.Compile();
                _filter = p => func(p) && func2(p);
            }
            return new FilterExpression<T>(_filter);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression)
        {
            return Or(expression, true);
        }

        public FilterExpression<T> Or(Expression<Func<T, bool>> expression, bool condition)
        {
            if (condition)
            {
                Func<T, bool> func = _filter.Compile();
                Func<T, bool> func2 = expression.Compile();
                _filter = p => func(p) || func2(p);
            }
            return new FilterExpression<T>(_filter);
        }

        private Func<T, bool> GetWhereExpression()
        {
            return _filter.Compile();
        }

    }
}