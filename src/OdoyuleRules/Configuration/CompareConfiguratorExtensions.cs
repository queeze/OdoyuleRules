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
    using RuntimeConfigurators;
    using RuntimeModel;
    using RuntimeModel.Comparators;
    using RuntimeModel.Nodes;
    using RuntimeModel.Values;


    public static class CompareConfiguratorExtensions
    {
        public static CompareNode<T, TProperty> Compare<T, TProperty>(this RuntimeConfigurator configurator,
            Comparator<TProperty, TProperty> comparator,
            TProperty value)
            where T : class
        {
            Value<TProperty> rightValue = Conditionals.Constant(value);

            return Compare<T, TProperty>(configurator, comparator, rightValue);
        }

        public static CompareNode<T, TProperty> Compare<T, TProperty>(this RuntimeConfigurator configurator,
            Comparator<TProperty, TProperty> comparator,
            Value<TProperty> value)
            where T : class
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditionals.Property<T, TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, value));
        }

        public static CompareNode<T, TProperty> GreaterThan<T, TProperty>(this RuntimeConfigurator configurator,
            TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            var comparator = new GreaterThanValueComparator<TProperty>();

            return Compare<T, TProperty>(configurator, comparator, value);
        }

        public static CompareNode<T, TProperty> GreaterThanOrEqual<T, TProperty>(this RuntimeConfigurator configurator,
            TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            var comparator = new GreaterThanOrEqualValueComparator<TProperty>();

            return Compare<T, TProperty>(configurator, comparator, value);
        }

        public static CompareNode<T, TProperty> LessThan<T, TProperty>(this RuntimeConfigurator configurator,
            TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            var comparator = new LessThanValueComparator<TProperty>();

            return Compare<T, TProperty>(configurator, comparator, value);
        }

        public static CompareNode<T, TProperty> LessThanOrEqual<T, TProperty>(this RuntimeConfigurator configurator,
            TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            var comparator = new LessThanOrEqualValueComparator<TProperty>();

            return Compare<T, TProperty>(configurator, comparator, value);
        }
    }
}