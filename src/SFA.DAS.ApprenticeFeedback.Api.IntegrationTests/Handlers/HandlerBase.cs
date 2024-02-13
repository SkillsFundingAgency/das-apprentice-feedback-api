using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers
{
    public class HandlerBase
    {
        protected static string NullQueryParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector)
        {
            GetPropertyNameAndValue(item, propertySelector, out string propertyName, out TKey propertyValue);
            return $"[{propertyName}] {(propertyValue == null ? "IS NULL" : "= @" + LowercaseFirstLetter(propertyName))}";
        }

        protected static string NotNullQueryParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector)
        {
            GetPropertyNameAndValue(item, propertySelector, out string propertyName, out TKey propertyValue);

            return $"[{propertyName}] = @{LowercaseFirstLetter(propertyName)}";
        }

        protected static string BecauseParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector, bool allowAny = false)
        {
            GetPropertyInfo(item, propertySelector, out string propertyName, out TKey propertyValue, out Type propertyType);
            if(propertyType == typeof(string))
            {
                return propertyValue != null ? $"{propertyName} == '{propertyValue}'" : $"{propertyName} == " + (allowAny ? "<any>" : "null");
            }

            return propertyValue != null ? $"{propertyName} == {propertyValue}" : $"{propertyName} == " + (allowAny ? "<any>" : "null");
        }

        private static void GetPropertyNameAndValue<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector, out string propertyName, out TKey propertyValue)
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The provided expression is not a MemberExpression.", nameof(propertySelector));
            }

            propertyName = memberExpression.Member.Name;
            propertyValue = propertySelector.Compile().Invoke(item);
        }

        private static void GetPropertyInfo<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector, out string propertyName, out TKey propertyValue, out Type propertyType)
        {
            var member = propertySelector.Body as MemberExpression;
            propertyName = member?.Member.Name;
            propertyType = (member?.Member as PropertyInfo)?.PropertyType;

            var compiledSelector = propertySelector.Compile();
            propertyValue = compiledSelector(item);
        }

        private static string LowercaseFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}
