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
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;
    using RuntimeConfigurators;
    using RuntimeModel;
    using RuntimeModel.Nodes;


    public static class PropertyConfiguratorExtensions
    {
        public static Activation<T> Property<T, TProperty>(this RuntimeConfigurator configurator,
            Expression<Func<T, TProperty>> propertyExpression)
            where T : class
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            return Property<T, TProperty>(configurator, propertyInfo);
        }

        public static Activation<T> Property<T, TProperty>(this RuntimeConfigurator configurator,
            PropertyInfo propertyInfo)
            where T : class
        {
            PropertySelector propertySelector = configurator.GetPropertySelector(propertyInfo);

            Type propertyNodeType = typeof (PropertyNode<,,>)
                .MakeGenericType(typeof (T), propertySelector.PropertyType, propertySelector.ValueType);

            return configurator.CreateNode(
                id => (Activation<T>) Activator.CreateInstance(propertyNodeType, id, propertyInfo, propertySelector));
        }

        public static Activation<Token<T1, T2>> Property<T1, T2, TProperty>(this RuntimeConfigurator configurator,
            PropertyInfo propertyInfo,
            Action<Token<T1, T2>, Action<TProperty>> propertyAction)
            where T1 : class
            where T2 : class
        {
            PropertySelector propertySelector = configurator.GetPropertySelector(propertyInfo);

            Type propertyNodeType = typeof (PropertyNode<,,>)
                .MakeGenericType(typeof (Token<T1, T2>), propertySelector.PropertyType, propertySelector.ValueType);

            return configurator.CreateNode(
                id =>
                (Activation<Token<T1, T2>>)
                Activator.CreateInstance(propertyNodeType, id, propertyInfo, propertySelector, propertyAction));
        }

        public static PropertyNode<T, TProperty, TValue> Property<T, TProperty, TValue>(
            this RuntimeConfigurator configurator, PropertyInfo propertyInfo,
            PropertySelector<TProperty, TValue> propertySelector)
            where T : class
        {
            PropertyNode<T, TProperty, TValue> propertyNode = configurator.CreateNode(
                id => new PropertyNode<T, TProperty, TValue>(id, propertyInfo, propertySelector));

            return propertyNode;
        }
    }
}