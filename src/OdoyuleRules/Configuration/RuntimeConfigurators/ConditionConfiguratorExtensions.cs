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
namespace OdoyuleRules.Configuration.RuntimeConfigurators
{
    using System;
    using System.Collections;
    using OdoyuleRules.RuntimeModel;
    using OdoyuleRules.RuntimeModel.Nodes;
    using OdoyuleRules.RuntimeModel.Values;


    public static class ConditionConfiguratorExtensions
    {
        public static ConditionNode<T> Condition<T>(
            this RuntimeConfigurator configurator, Predicate<T> condition)
            where T : class
        {
            ConditionNode<T> conditionNode = configurator.CreateNode(
                id => new ConditionNode<T>(id, (value, next) =>
                    {
                        if (condition(value))
                            next();
                    }));

            return conditionNode;
        }

        public static ConditionNode<Token<T1, T2>> Condition<T1, T2>(
            this RuntimeConfigurator configurator, Predicate<T2> condition)
            where T1 : class
        {
            ConditionNode<Token<T1, T2>> conditionNode = configurator.CreateNode(
                id => new ConditionNode<Token<T1, T2>>(id, (value, next) =>
                    {
                        if (condition(value.Item2))
                            next();
                    }));

            return conditionNode;
        }

        public static NotNullNode<T, TProperty> NotNull<T, TProperty>(this RuntimeConfigurator configurator)
            where T : class
            where TProperty : class
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditionals.Property<T, TProperty>();

            return configurator.CreateNode(id => new NotNullNode<T, TProperty>(id, tokenValue));
        }

        public static ExistsNode<T, TProperty> Exists<T, TProperty>(this RuntimeConfigurator configurator)
            where T : class
            where TProperty : class, IEnumerable
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditionals.Property<T, TProperty>();

            return configurator.CreateNode(id => new ExistsNode<T, TProperty>(id, tokenValue));
        }

        public static EachNode<T, TProperty, TElement> Each<T, TProperty, TElement>(
            this RuntimeConfigurator configurator,
            Action<TProperty, Action<TElement, int>> elementMatch)
            where T : class
            where TProperty : class, IEnumerable
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditionals.Property<T, TProperty>();

            return configurator.CreateNode(id => new EachNode<T, TProperty, TElement>(id, tokenValue, elementMatch));
        }
    }
}