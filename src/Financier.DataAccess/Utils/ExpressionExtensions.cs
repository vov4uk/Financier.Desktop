using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Financier.DataAccess.Utils
{
    [ExcludeFromCodeCoverage]
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exprLeft, Expression<Func<T, bool>> exprRight)
        {
            return BuildExpression(
                exprLeft,
                exprRight,
                (left, right) => Expression.AndAlso(left, right));
        }

        private static Expression<Func<T, bool>> BuildExpression<T>(Expression<Func<T, bool>> exprLeft, Expression<Func<T, bool>> exprRight,
            Func<Expression, Expression, Expression> conditionalExpression)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));

            ReplaceVisitor leftVisitor = new ReplaceVisitor(exprLeft.Parameters[0], parameter);
            Expression left = leftVisitor.Visit(exprLeft.Body);

            var rightVisitor = new ReplaceVisitor(exprRight.Parameters[0], parameter);
            Expression right = rightVisitor.Visit(exprRight.Body);

            return Expression.Lambda<Func<T, bool>>(
                conditionalExpression(left, right),
                parameter);
        }

        private sealed class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression old;
            private readonly Expression newVal;

            public ReplaceVisitor(Expression oldValue, Expression newValue)
            {
                this.old = oldValue;
                this.newVal = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == this.old)
                {
                    return this.newVal;
                }

                return base.Visit(node);
            }
        }
    }
}
