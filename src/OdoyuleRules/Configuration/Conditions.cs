// Copyright 2011-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Configuration
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;
    using SemanticModel;


    public static class Conditions
    {
        /// <summary>
        /// Creates a new predicate rule condition
        /// </summary>
        /// <typeparam name="T">The fact type used in the expression</typeparam>
        /// <param name="predicateExpression">The predicate expression</param>
        /// <returns>The rule condition</returns>
        public static PredicateExpressionCondition<T> Predicate<T>(Expression<Func<T, bool>> predicateExpression)
            where T : class
        {
            return new PredicateExpressionCondition<T>(predicateExpression);
        }

        public static PropertyEqualCondition<T, TProperty> Equal<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression, TProperty value)
            where T : class
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyEqualCondition<T, TProperty>(propertyInfo, propertyExpression, value);
        }

        public static PropertyNotEqualCondition<T, TProperty> NotEqual<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression, TProperty value)
            where T : class
			where TProperty : IComparable<TProperty>
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyNotEqualCondition<T, TProperty>(propertyInfo, propertyExpression, value);
        }

        public static PropertyGreaterThanCondition<T, TProperty> GreaterThan<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression, TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyGreaterThanCondition<T, TProperty>(propertyInfo, propertyExpression, value);
        }

        public static PropertyGreaterThanOrEqualCondition<T, TProperty> GreaterThanOrEqual<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression, TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyGreaterThanOrEqualCondition<T, TProperty>(propertyInfo, propertyExpression, value);
        }

        public static PropertyLessThanCondition<T, TProperty> LessThan<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression, TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyLessThanCondition<T, TProperty>(propertyInfo, propertyExpression, value);
        }

        public static PropertyLessThanOrEqualCondition<T, TProperty> LessThanOrEqual<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression, TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyLessThanOrEqualCondition<T, TProperty>(propertyInfo, propertyExpression, value);
        }

        public static PropertyNotNullCondition<T, TProperty> NotNull<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression)
            where T : class
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyNotNullCondition<T, TProperty>(propertyInfo, propertyExpression);
        }

        public static PropertyExistsCondition<T, TProperty> Exists<T, TProperty>(
            Expression<Func<T, TProperty>> propertyExpression)
            where T : class
            where TProperty : class, IEnumerable
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyExistsCondition<T, TProperty>(propertyInfo, propertyExpression);
        }

        public static PropertyEachCondition<T, TProperty, TElement> Each<T, TProperty, TElement>(
            Expression<Func<T, TProperty>> propertyExpression)
            where T : class
            where TProperty : class, IEnumerable
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return new PropertyEachCondition<T, TProperty, TElement>(propertyInfo, propertyExpression);
        }
    }
}