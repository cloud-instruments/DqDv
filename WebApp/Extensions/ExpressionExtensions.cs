using System;
using System.Linq.Expressions;

namespace WebApp.Extensions
{
    public static class ExpressionExtensions
    {
        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// LinqKit extension for LINQ predicate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expr">expression to be compiled</param>
        /// <param name="arg1">type of first parameter</param>
        /// <param name="arg2">type of second parameter</param>
        /// <returns>true or false</returns>
        public static bool Invoke<T1, T2>(this Expression<Func<T1, T2, bool>> expr, T1 arg1, T2 arg2)
        {
            try
            {
                return expr.Compile()(arg1, arg2);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
