namespace OdoyuleRules.Configuration.RuntimeModelConfigurators.Selectors
{
    using System;
    using Models.RuntimeModel;
    using Visualization;


    public class NotEqualNodeSelector<T, TProperty> :
        RuntimeModelVisitorBase,
        NodeSelector
        where T : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly NodeSelector _next;
        readonly TProperty _value;
        EqualNode<T, TProperty> _node;

        public NotEqualNodeSelector(NodeSelector next, RuntimeConfigurator configurator, TProperty value)
        {
            _value = value;
            _next = next;
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
                EqualNode<T, TProperty> equalNode = _configurator.Equal<T, TProperty>();

                var parentNode = node as Node<Token<T, TProperty>>;
                if (parentNode == null)
                    throw new ArgumentException("Expected " + typeof (T).Tokens() + ", but was "
                                                + typeof (TNode).Tokens());

                parentNode.AddActivation(equalNode);

                _node = equalNode;
            }

            _next.Select(_node[_value]);
        }

        public void Select<TNode>(MemoryNode<TNode> node) where TNode : class
        {
            throw new NotImplementedException("MemoryNode is not supported for equal");
        }

        public override bool Visit<TT, TTProperty>(EqualNode<TT, TTProperty> node, Func<RuntimeModelVisitor, bool> next)
        {
            var locator = this as NotEqualNodeSelector<TT, TTProperty>;
            if (locator != null)
            {
                locator._node = node;
                return false;
            }

            return base.Visit(node, next);
        }

        public override string ToString()
        {
            return string.Format("NotEqual Node: [{0}], {1} => {2}", typeof (T).Tokens(), _value, typeof (TProperty).Name);
        }
    }
}