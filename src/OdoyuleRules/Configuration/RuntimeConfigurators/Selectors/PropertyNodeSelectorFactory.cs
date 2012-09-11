// Copyright 2011-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed 
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Configuration.RuntimeConfigurators.Selectors
{
    using System;
    using System.Reflection;
    using RuntimeModel;


    public static class PropertyNodeSelectorFactory
    {
        public static NodeSelectorFactory Create(RuntimeConfigurator configurator, NodeSelectorFactory factory,
            PropertyInfo propertyInfo)
        {
            Type type = typeof(PropertyNodeSelectorFactory<>).MakeGenericType(propertyInfo.PropertyType);

            return (NodeSelectorFactory)Activator.CreateInstance(type, configurator, factory, propertyInfo);
        }
    }


    /// <summary>
    /// Creates a selector to find a property node, uses the RuntimeConfigurator to obtain the propery value
    /// factory for the property so that value types, reference types, and user-specified types can be
    /// properly accessed (to avoid null reference exceptions, etc.)
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    public class PropertyNodeSelectorFactory<TProperty> :
        NodeSelectorFactory
    {
        readonly RuntimeConfigurator _configurator;
        readonly NodeSelectorFactory _nextFactory;
        readonly PropertyInfo _propertyInfo;

        public PropertyNodeSelectorFactory(RuntimeConfigurator configurator, NodeSelectorFactory nextFactory,
            PropertyInfo propertyInfo)
        {
            _nextFactory = nextFactory;
            _configurator = configurator;
            _propertyInfo = propertyInfo;
        }

        public NodeSelector Create<T>()
            where T : class
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Token<,>))
            {
                Type[] arguments = typeof(T).GetGenericArguments();

                return PropertyNodeSelector.Create(_configurator, _nextFactory, _propertyInfo, arguments[0],
                    arguments[1]);
            }

            return PropertyNodeSelector.Create<T, TProperty>(_configurator, _nextFactory, _propertyInfo);
        }
    }
}