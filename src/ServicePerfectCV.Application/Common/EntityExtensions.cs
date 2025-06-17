using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ServicePerfectCV.Application.Common
{
    /// <summary>
    /// Extension methods for entity updates.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Updates a property of an entity if the optional value is present.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TValue">The property value type.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <param name="propertySelector">A lambda expression selecting the property to update.</param>
        /// <param name="optionalValue">The optional value to update with.</param>
        public static void UpdateIfPresent<TEntity, TValue>(
            this TEntity entity,
            Expression<Func<TEntity, TValue>> propertySelector,
            Optional<TValue> optionalValue)
            where TEntity : class
        {
            if (optionalValue.HasValue)
            {
                var propertyInfo = GetPropertyInfo(propertySelector);
                propertyInfo.SetValue(entity, optionalValue.Value);
            }
        }

        /// <summary>
        /// Gets the property information from a property expression.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TValue">The property value type.</typeparam>
        /// <param name="propertySelector">The property selector expression.</param>
        /// <returns>The property information.</returns>
        private static PropertyInfo GetPropertyInfo<TEntity, TValue>(
            Expression<Func<TEntity, TValue>> propertySelector)
        {
            if (propertySelector.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException(
                    "Expression must be a member access expression.",
                    nameof(propertySelector));
            }

            return (PropertyInfo)memberExpression.Member;
        }
    }
}
