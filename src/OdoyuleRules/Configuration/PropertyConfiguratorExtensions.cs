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
    using Internal;
    using Models.RuntimeModel;
    using RuntimeModelConfigurators;


    public static class PropertyConfiguratorExtensions
    {
        public static PropertyNode<T, TProperty> Property<T, TProperty>(this RuntimeConfigurator configurator,
            Expression<Func<T, TProperty>> propertyExpression)
            where T : class
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            PropertyNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new PropertyNode<T, TProperty>(id, propertyInfo));

            return propertyNode;
        }

        public static PropertyNode<T, TProperty> Property<T, TProperty>(this RuntimeConfigurator configurator,
            PropertyInfo propertyInfo)
            where T : class
        {
            PropertyNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new PropertyNode<T, TProperty>(id, propertyInfo));

            return propertyNode;
        }

        public static PropertyNode<T, TProperty> Property<T, TProperty>(this RuntimeConfigurator configurator,
            PropertyInfo propertyInfo, Action<T, Action<TProperty>> propertyMatch)
            where T : class
        {
            PropertyNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new PropertyNode<T, TProperty>(id, propertyInfo, propertyMatch));

            return propertyNode;
        }
    }
}