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
    using Internals.Extensions;
    using RuntimeModel;
    using RuntimeModel.Nodes;
    using Visualization;


    public static class EqualNodeSelector
    {
        public static NodeSelector Create(RuntimeConfigurator configurator, Type tokenType,
            NodeSelectorFactory nextSelectorFactory, object value)
        {
            Type[] arguments = tokenType.GetGenericArguments();
            if (!arguments[1].IsInstanceOfType(value))
                throw new RuntimeConfigurationException("Value type does not match token type: " + value.GetType().GetTypeName());

            Type type = typeof(EqualNodeSelector<,>)
                .MakeGenericType(arguments);

            var nodeSelector = (NodeSelector)Activator.CreateInstance(type, configurator, nextSelectorFactory, value);

            return nodeSelector;
        }
    }


    public class EqualNodeSelector<T, TValue> :
        RuntimeVisitorBase,
        NodeSelector
        where T : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly NodeSelector _next;
        readonly TValue _value;
        EqualNode<T, TValue> _node;

        public EqualNodeSelector(RuntimeConfigurator configurator, NodeSelectorFactory nextSelectorFactory, TValue value)
        {
            _value = value;
            _next = nextSelectorFactory.Create<Token<T, TValue>>();
            _configurator = configurator;
        }

        public NodeSelector Next
        {
            get { return _next; }
        }

        public void Select()
        {
            throw new NotImplementedException("An input node is required");
        }

        public void Select<TNode>(Node<TNode> node)
            where TNode : class
        {
            _node = null;
            node.Accept(this);

            if (_node == null)
            {
                EqualNode<T, TValue> equalNode = _configurator.Equal<T, TValue>();

                var parentNode = node as Node<Token<T, TValue>>;
                if (parentNode == null)
                    throw new ArgumentException("Expected " + typeof(T).Tokens() + ", but was "
                                                + typeof(TNode).Tokens());

                parentNode.AddActivation(equalNode);

                _node = equalNode;
            }

            _next.Select(_node[_value]);
        }

        public void Select<TNode>(MemoryNode<TNode> node) where TNode : class
        {
            throw new NotImplementedException("MemoryNode is not supported for equal");
        }

        public override bool Visit<TT, TTProperty>(EqualNode<TT, TTProperty> node, Func<RuntimeVisitor, bool> next)
        {
            var locator = this as EqualNodeSelector<TT, TTProperty>;
            if (locator != null)
            {
                locator._node = node;
                return false;
            }

            return base.Visit(node, next);
        }

        public override string ToString()
        {
            return string.Format("Equal Node: [{0}], {1} => {2}", typeof(T).Tokens(), _value, typeof(TValue).Name);
        }
    }
}