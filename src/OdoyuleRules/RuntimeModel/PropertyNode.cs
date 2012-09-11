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
namespace OdoyuleRules.RuntimeModel
{
    using System;
    using System.Reflection;
    using Internals.Reflection;
    using JoinNodes;


    /// <summary>
    /// A property node matches a property on a fact and activates successors
    /// with a token of the fact and the property
    /// </summary>
    /// <typeparam name="T">The declaring type</typeparam>
    /// <typeparam name="TProperty">The declared property value</typeparam>
    /// <typeparam name="TValue">The actual property type (that is activated forward)</typeparam>
    public class PropertyNode<T, TProperty, TValue> :
        NodeImpl<Token<T, TValue>>,
        Node<Token<T, TValue>>,
        Activation<T>
        where T : class
    {
        readonly Action<T, Action<TProperty>> _propertyAction;
        readonly PropertyInfo _propertyInfo;
        readonly PropertySelector<TProperty, TValue> _propertySelector;

        public PropertyNode(int id, PropertyInfo propertyInfo, PropertySelector<TProperty, TValue> propertySelector)
            : base(id)
        {
            _propertyInfo = propertyInfo;
            _propertySelector = propertySelector;

            var property = new ReadOnlyProperty<T, TProperty>(propertyInfo);
            _propertyAction = (x, next) =>
                {
                    if (x != null)
                    {
                        TProperty propertyValue = property.Get(x);
                        next(propertyValue);
                    }
                };
        }

        public PropertyNode(int id, PropertyInfo propertyInfo, PropertySelector<TProperty, TValue> propertySelector,
            Action<T, Action<TProperty>> propertyAction)
            : base(id)
        {
            _propertyInfo = propertyInfo;
            _propertySelector = propertySelector;
            _propertyAction = propertyAction;
        }

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        public void Activate(ActivationContext<T> context)
        {
            _propertyAction(context.Fact, propertyValue =>
                {
                    TValue value;
                    if (_propertySelector.TryGetValue(propertyValue, out value))
                    {
                        ActivationContext<Token<T, TValue>> propertyContext =
                            context.CreateContext(new Token<T, TValue>(context, value));

                        base.Activate(propertyContext);
                    }
                });
        }

        public bool Accept(RuntimeModelVisitor visitor)
        {
            return visitor.Visit(this, Successors);
        }
    }
}