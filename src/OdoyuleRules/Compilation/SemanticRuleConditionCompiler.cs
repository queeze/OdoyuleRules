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
namespace OdoyuleRules.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Configuration;
    using Configuration.RuntimeConfigurators;
    using Configuration.RuntimeConfigurators.Selectors;
    using RuntimeModel;
    using RuntimeModel.Nodes;
    using SemanticModel;


    public class SemanticRuleConditionCompiler :
        SemanticVisitorBase,
        RuleConditionCompiler
    {
        readonly IList<RuleNodeSelector> _alphaNodes;
        readonly RuntimeConfigurator _configurator;

        public SemanticRuleConditionCompiler(RuntimeConfigurator configurator)
        {
            _configurator = configurator;
            _alphaNodes = new List<RuleNodeSelector>();
        }

        public override bool Visit<T, TProperty>(PropertyNotNullCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            Compile(condition.PropertyExpression, x => new NotNullNodeSelectorFactory<TProperty>(x, _configurator));

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyEqualCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            Compile(condition.PropertyExpression, x => new EqualNodeSelectorFactory(x, _configurator, condition.Value));

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyNotEqualCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
			CompareNode<T, TProperty> compareNode = _configurator.NotEqual<T, TProperty>(condition.Value);

			AddCompareCondition(condition.PropertyExpression, compareNode);

			return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyGreaterThanCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            CompareNode<T, TProperty> compareNode = _configurator.GreaterThan<T, TProperty>(condition.Value);

            AddCompareCondition(condition.PropertyExpression, compareNode);

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyGreaterThanOrEqualCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            CompareNode<T, TProperty> compareNode = _configurator.GreaterThanOrEqual<T, TProperty>(condition.Value);

            AddCompareCondition(condition.PropertyExpression, compareNode);

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyLessThanCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            CompareNode<T, TProperty> compareNode = _configurator.LessThan<T, TProperty>(condition.Value);

            AddCompareCondition(condition.PropertyExpression, compareNode);

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyLessThanOrEqualCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            CompareNode<T, TProperty> compareNode = _configurator.LessThanOrEqual<T, TProperty>(condition.Value);

            AddCompareCondition(condition.PropertyExpression, compareNode);

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty>(PropertyExistsCondition<T, TProperty> condition,
            Func<SemanticVisitor, bool> next)
        {
            Compile(condition.PropertyExpression,
                x => new ExistsNodeSelectorFactory<TProperty>(x, _configurator));

            return base.Visit(condition, next);
        }

        public override bool Visit<T, TProperty, TElement>(PropertyEachCondition<T, TProperty, TElement> condition,
            Func<SemanticVisitor, bool> next)
        {
            Compile(condition.PropertyExpression,
                x => new EachNodeSelectorFactory<TProperty, TElement>(x, _configurator));

            return base.Visit(condition, next);
        }

        public bool MatchJoinNode<T>(Action<MemoryNode<T>> callback)
            where T : class
        {
            if (_alphaNodes.Count == 0)
                return false;

            if (typeof (T).IsGenericType && typeof (T).GetGenericTypeDefinition() == typeof (Tuple<,>))
            {
                Type[] arguments = typeof (T).GetGenericArguments();

                var tupleNode = (RuleNodeSelector) Activator.CreateInstance(
                    typeof (TupleRuleNodeSelector<,>).MakeGenericType(arguments[0], arguments[1]),
                    _configurator, this);

                return tupleNode.Select(callback);
            }

            var done = new HashSet<int>();

            MemoryNode<T> left = null;
            for (int index = 0; index < _alphaNodes.Count; index++)
            {
                if (done.Contains(index))
                    continue;

                if (_alphaNodes[index].Select<T>(alpha =>
                    {
                        done.Add(index);
                        left = alpha;

                        for (int i = index + 1; i < _alphaNodes.Count; i++)
                        {
                            if (done.Contains(i))
                                continue;

                            _alphaNodes[i].Select<T>(right =>
                                {
                                    done.Add(i);
                                    _configurator.MatchJoinNode(left, right, join => { left = join; });
                                });
                        }
                    }))
                    break;
            }

            if (left != null)
            {
                if (left is AlphaNode<T>)
                {
                    _configurator.MatchJoinNode(left, join => { left = join; });
                }

                callback(left);
                return true;
            }

            return false;
        }

        void AddCompareCondition<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression,
            CompareNode<T, TProperty> compareNode)
            where T : class
        {
            var conditionFactory = new ConditionAlphaNodeSelectorFactory(_configurator, node => _alphaNodes.Add(node));

            var alphaFactory = new AlphaNodeSelectorFactory(conditionFactory, _configurator);

            var compareFactory = new CompareNodeSelectorFactory<TProperty>(alphaFactory, _configurator,
                compareNode.Comparator,
                compareNode.Value);

            Compile(propertyExpression, compareFactory);
        }

        void Compile<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression,
            Func<NodeSelectorFactory, NodeSelectorFactory> selectorConfigurator)
            where T : class
        {
            var conditionFactory = new ConditionAlphaNodeSelectorFactory(_configurator, node => _alphaNodes.Add(node));

            var alphaFactory = new AlphaNodeSelectorFactory(conditionFactory, _configurator);

            NodeSelectorFactory factory = selectorConfigurator(alphaFactory);

            Compile(propertyExpression, factory);
        }

        void Compile<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression,
            NodeSelectorFactory selectorFactory)
            where T : class
        {
            new PropertyExpressionVisitor<T>(_configurator, selectorFactory)
                .CreateSelector(propertyExpression.Body)
                .Select();
        }
    }
}