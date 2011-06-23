using System;
using System.Linq.Expressions;

namespace DynamicExpressionBuilder
{
    public class FilterCommand<T>
    {
        public Expression<Func<T, bool>> filter;
        public TypeFilterCommand typeCommand;

        public FilterCommand(Expression<Func<T, bool>> expression, TypeFilterCommand typeFilterCommand)
        {
            filter = expression;
            typeCommand = typeFilterCommand;
        }

    }
}