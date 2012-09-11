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
namespace OdoyuleRules.Configuration.RuntimeModelConfigurators.Selectors
{
    using System;
    using System.Reflection;
    using Internals.Reflection;
    using Models.RuntimeModel;
    using Visualization;


    public static class PropertyNodeSelector
    {
        public static NodeSelector Create<T, TProperty>(RuntimeConfigurator configurator,
            NodeSelectorFactory nextSelectorFactory,
            PropertyInfo propertyInfo)
        {
            PropertySelector<TProperty> propertySelector = configurator.GetPropertySelector<TProperty>(propertyInfo);

            Type type = typeof (PropertyNodeSelector<,,>)
                .MakeGenericType(typeof (T), propertySelector.PropertyType, propertySelector.ValueType);

            var nodeSelector =
                (NodeSelector)
                Activator.CreateInstance(type, configurator, nextSelectorFactory, propertyInfo, propertySelector);

            return nodeSelector;
        }

        public static NodeSelector Create(RuntimeConfigurator configurator, NodeSelectorFactory nextSelectorFactory,
            PropertyInfo propertyInfo, params Type[] tokenTypes)
        {
            if (tokenTypes.Length != 2)
                throw new NotImplementedException("Only handling two right now");

            PropertySelector propertySelector = configurator.GetPropertySelector(propertyInfo);

            Type type = typeof (PropertyNodeSelector<,,,>)
                .MakeGenericType(tokenTypes[0], tokenTypes[1], propertySelector.PropertyType, propertySelector.ValueType);

            var nodeSelector =
                (NodeSelector)
                Activator.CreateInstance(type, configurator, nextSelectorFactory, propertyInfo, propertySelector);

            return nodeSelector;
        }
    }


    /// <summary>
    /// Used to match a property on a type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PropertyNodeSelector<T, TProperty, TValue> :
        RuntimeModelVisitorBase,
        NodeSelector
        where T : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly NodeSelector _next;
        readonly PropertyInfo _property;
        readonly PropertySelector<TProperty, TValue> _propertySelector;
        PropertyNode<T, TProperty, TValue> _node;

        public PropertyNodeSelector(RuntimeConfigurator configurator, NodeSelectorFactory nextSelectorFactory,
            PropertyInfo property, PropertySelector<TProperty, TValue> propertySelector)
        {
            if (configurator == null) throw new ArgumentNullException("configurator");
            if (property == null) throw new ArgumentNullException("property");
            if (propertySelector == null) throw new ArgumentNullException("propertySelector");

            if (nextSelectorFactory != null)
                _next = nextSelectorFactory.Create<Token<T, TValue>>();

            _configurator = configurator;
            _propertySelector = propertySelector;
            _property = property;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public NodeSelector Next
        {
            get { return _next; }
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public void Select<TNode>(Node<TNode> node) where TNode : class
        {
            _node = null;
            node.Accept(this);

            if (_node == null)
            {
                PropertyNode<T, TProperty, TValue> propertyNode = _configurator.Property<T, TProperty, TValue>(
                    Property, _propertySelector);

                var parentNode = node as Node<T>;
                if (parentNode == null)
                    throw new ArgumentException("Expected propertyNode, but was " + node.GetType().Name);

                parentNode.AddActivation(propertyNode);

                _node = propertyNode;
            }

            if (_next != null)
                _next.Select(_node);
        }

        public void Select<TNode>(MemoryNode<TNode> node) where TNode : class
        {
            _node = null;

            node.Accept(this);

            if (_node == null)
            {
                PropertyNode<T, TProperty, TValue> propertyNode = _configurator.Property<T, TProperty, TValue>(
                    Property, _propertySelector);

                var parentNode = node as MemoryNode<T>;
                if (parentNode == null)
                    throw new ArgumentException("Expected propertyNode, but was " + node.GetType().Name);

                parentNode.AddActivation(propertyNode);

                _node = propertyNode;
            }

            if (_next != null)
                _next.Select(_node);
        }

        public override string ToString()
        {
            return string.Format("Property: [{0}], {1} => {2}", typeof (T).Tokens(), Property.Name,
                typeof (TProperty).Name);
        }

        public override bool Visit<TT, TTProperty, TTValue>(PropertyNode<TT, TTProperty, TTValue> node,
            Func<RuntimeModelVisitor, bool> next)
        {
            var locator = this as PropertyNodeSelector<TT, TTProperty, TTValue>;
            if (locator != null)
            {
                if (locator.Property.Equals(node.PropertyInfo))
                {
                    locator._node = node;
                    return false;
                }
            }

            return base.Visit(node, next);
        }
    }


    public class PropertyNodeSelector<T1, T2, TProperty, TValue> :
        RuntimeModelVisitorBase,
        NodeSelector
        where T1 : class
        where T2 : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly NodeSelector _next;
        readonly PropertyInfo _property;
        readonly PropertySelector<TProperty, TValue> _propertySelector;
        Node<Token<Token<T1, T2>, TValue>> _node;

        public PropertyNodeSelector(RuntimeConfigurator configurator, NodeSelectorFactory nextSelectorFactory,
            PropertyInfo property, PropertySelector<TProperty, TValue> propertySelector)
        {
            if (configurator == null) throw new ArgumentNullException("configurator");
            if (property == null) throw new ArgumentNullException("property");
            if (propertySelector == null) throw new ArgumentNullException("propertySelector");

            _configurator = configurator;
            _propertySelector = propertySelector;
            _property = property;

            if (nextSelectorFactory != null)
                _next = nextSelectorFactory.Create<Token<Token<T1, T2>, TValue>>();
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public NodeSelector Next
        {
            get { return _next; }
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public void Select<TNode>(Node<TNode> node) where TNode : class
        {
            _node = null;
            node.Accept(this);

            if (_node == null)
            {
                var fastProperty = new ReadOnlyProperty<T2, TProperty>(_property);

                Activation<Token<T1, T2>> propertyNode = _configurator.Property<T1, T2, TProperty>(_property,
                    (x, next) =>
                        {
                            if (x.Item2 != null)
                                next(fastProperty.Get(x.Item2));
                        });

                var parentNode = node as Node<Token<T1, T2>>;
                if (parentNode == null)
                    throw new ArgumentException("Expected propertyNode, but was " + node.GetType().Name);

                parentNode.AddActivation(propertyNode);

                _node = propertyNode as Node<Token<Token<T1, T2>, TValue>>;
            }

            _next.Select(_node);
        }

        public void Select<TNode>(MemoryNode<TNode> node) where TNode : class
        {
            _node = null;

            node.Accept(this);

            if (_node == null)
            {
                var fastProperty = new ReadOnlyProperty<T2, TProperty>(_property);

                Activation<Token<T1, T2>> propertyNode = _configurator.Property<T1, T2, TProperty>(_property,
                    (x, next) =>
                        {
                            if (x.Item2 != null)
                                next(fastProperty.Get(x.Item2));
                        });

                var parentNode = node as MemoryNode<Token<T1, T2>>;
                if (parentNode == null)
                    throw new ArgumentException("Expected propertyNode, but was " + node.GetType().Name);

                parentNode.AddActivation(propertyNode);

                _node = propertyNode as Node<Token<Token<T1, T2>, TValue>>;
            }

            _next.Select(_node);
        }

        public override string ToString()
        {
            return string.Format("Property: [{0}], {1} => {2}", typeof(Token<T1, T2>).Tokens(), _property.Name,
                typeof (TProperty).Name);
        }

        public override bool Visit<TT, TTProperty, TTValue>(PropertyNode<TT, TTProperty, TTValue> node,
            Func<RuntimeModelVisitor, bool> next)
        {
            if (typeof (TT).IsGenericType && typeof (TT).GetGenericTypeDefinition() == typeof (Token<,>))
            {
                Type[] arguments = typeof (TT).GetGenericArguments();

                MethodInfo methodInfo = GetType().GetMethod("VisitTokenPropertyNode",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                bool matchesFirst = methodInfo.GetGenericArguments()[0].IsAssignableFrom(arguments[0]);
                bool matchesSecond = methodInfo.GetGenericArguments()[1].IsAssignableFrom(arguments[1]);
                bool matchesThird = methodInfo.GetGenericArguments()[2].IsAssignableFrom(typeof (TProperty));

                bool matchesArguments = matchesFirst && matchesSecond && matchesThird;

                bool keepGoing = !matchesArguments || (bool) methodInfo
                                                                 .MakeGenericMethod(arguments[0], arguments[1],
                                                                     typeof (TProperty))
                                                                 .Invoke(this, new object[] {node, next});

                return keepGoing || base.Visit(node, next);
            }

            return base.Visit(node, next);
        }

        bool VisitTokenPropertyNode<TT1, TT2, TTProperty, TTValue>(
            PropertyNode<Token<TT1, TT2>, TTProperty, TTValue> node,
            Func<RuntimeModelVisitor, bool> next)
            where TT1 : class
            where TT2 : class
        {
            var locator = this as PropertyNodeSelector<TT1, TT2, TTProperty, TTValue>;
            if (locator != null)
            {
                if (locator.Property.Equals(node.PropertyInfo))
                {
                    locator._node = node;
                    return false;
                }
            }

            return true;
        }
    }
}