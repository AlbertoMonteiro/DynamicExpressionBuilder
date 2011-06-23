using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace DynamicExpressionBuilder
{
    public class FilterExpression<T>
    {
        public Func<T, bool> ResultExpression { get { return GetWhereExpression(); } }
        private readonly List<FilterCommand<T>> _filtersCommands;

        public FilterExpression()
        {
            _filtersCommands = new List<FilterCommand<T>>();
        }

        public FilterExpression<T> Start(Expression<Func<T, bool>> expression)
        {
            var filterCommand = new FilterCommand<T>(expression, TypeFilterCommand.And);
            _filtersCommands.Add(filterCommand);
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
                if (_filtersCommands.Count < 1)
                    throw new InvalidExpressionException("Você precisa iniciar a expressão com o método Start");
                var filterCommand = new FilterCommand<T>(expression, TypeFilterCommand.And);
                _filtersCommands.Add(filterCommand);
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
                if (_filtersCommands.Count < 1)
                    throw new InvalidExpressionException("Você precisa iniciar a expressão com o método Start");
                var filterCommand = new FilterCommand<T>(expression, TypeFilterCommand.Or);
                _filtersCommands.Add(filterCommand);
            }
            return this;
        }

        private Func<T, bool> GetWhereExpression()
        {
            Expression<Func<T, bool>> filtro = entidade => true;

            foreach (var expression in _filtersCommands)
            {
                var func = expression.filter.Compile();
                var compile1 = filtro.Compile();
                if (expression.typeCommand == TypeFilterCommand.And)
                    filtro = entidade => func(entidade) && compile1(entidade);
                else
                    filtro = entidade => func(entidade) || compile1(entidade);
            }

            var invocationExpression = Expression.Invoke(filtro, (IEnumerable<Expression>)filtro.Parameters);
            var binaryExpression = Expression.AndAlso(filtro.Body, invocationExpression);
            var andAlso = Expression.AndAlso(binaryExpression, invocationExpression);
            var lambdaExpression = Expression.Lambda<Func<T, bool>>(andAlso, filtro.Parameters);

            return lambdaExpression.Compile();
        }

    }
}