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
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Models.RuntimeModel;


    /// <summary>
    /// Visits a property expression, and creates the appropriate chain of node selectors
    /// that will build out the property chain in the alpha network
    /// </summary>
    public interface PropertyExpressionVisitor
    {
        NodeSelector CreateSelector(Expression expression);
    }


    public class PropertyExpressionVisitor<T> :
        ExpressionVisitor,
        PropertyExpressionVisitor
        where T : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly NodeSelectorFactory _nodeSelectorFactory;
        NodeSelector _selector;

        public PropertyExpressionVisitor(RuntimeConfigurator configurator)
        {
            _configurator = configurator;
        }

        public PropertyExpressionVisitor(RuntimeConfigurator configurator, NodeSelectorFactory nodeSelectorFactory)
        {
            _nodeSelectorFactory = nodeSelectorFactory;
            _configurator = configurator;
        }

        public NodeSelector CreateSelector(Expression expression)
        {
            Visit(expression);

            return _selector;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType == MemberTypes.Property)
            {
                return VisitProperty(node);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex && node.Right.NodeType == ExpressionType.Constant)
            {
                var constantExpression = node.Right as ConstantExpression;
                if (constantExpression != null)
                {
                    Type factoryType = typeof (ArrayNodeSelectorFactory<>).MakeGenericType(node.Type);

                    var nodeSelectorFactory =
                        (NodeSelectorFactory)
                        Activator.CreateInstance(factoryType, _nodeSelectorFactory, constantExpression.Value);

                    Type visitorType = typeof (PropertyExpressionVisitor<>).MakeGenericType(node.Left.Type);
                    var visitor =
                        (PropertyExpressionVisitor)
                        Activator.CreateInstance(visitorType, _configurator, nodeSelectorFactory);
                    _selector = visitor.CreateSelector(node.Left);

                    return node;
                }
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var factory = new TypeNodeSelectorFactory(_nodeSelectorFactory, _configurator);

            _selector = factory.Create<T>();

            return base.VisitParameter(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsSpecialName && node.Method.Name.Equals("get_Item") && node.Object != null
                && node.Object.Type.IsGenericType)
            {
                Type typeDefinition = node.Object.Type.GetGenericTypeDefinition();
                if (typeDefinition == typeof (IList<>) || typeDefinition == typeof (List<>))
                {
                    var indexExpression = node.Arguments[0] as ConstantExpression;
                    if (indexExpression != null)
                    {
                        Type factoryType = typeof (ListNodeSelectorFactory<>).MakeGenericType(node.Type);

                        var nodeSelectorFactory =
                            (NodeSelectorFactory)
                            Activator.CreateInstance(factoryType, _nodeSelectorFactory, indexExpression.Value);

                        Type visitorType = typeof (PropertyExpressionVisitor<>).MakeGenericType(node.Object.Type);
                        var visitor =
                            (PropertyExpressionVisitor)
                            Activator.CreateInstance(visitorType, _configurator, nodeSelectorFactory);
                        _selector = visitor.CreateSelector(node.Object);

                        return node;
                    }
                }
            }

            return base.VisitMethodCall(node);
        }

        Expression VisitProperty(MemberExpression memberExpression)
        {
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Expected a property expression");

            PropertySelector propertySelector = _configurator.GetPropertySelector(propertyInfo);
            if (propertySelector.PropertyType != propertyInfo.PropertyType)
            {
                // we have to skip back one because we are on a property of a custom type
                if (propertySelector.PropertyType == memberExpression.Expression.Type)
                {
                    var propertyMemberExpression = memberExpression.Expression as MemberExpression;
                    if (propertyMemberExpression != null)
                    {
                        var propertyPropertyInfo = propertyMemberExpression.Member as PropertyInfo;
                        if (propertyPropertyInfo != null)
                        {
                            NodeSelectorFactory nextSelectorFactory = PropertyNodeSelectorFactory.Create(_configurator,
                                _nodeSelectorFactory, propertyPropertyInfo);

                            Type propertyVisitorType = typeof (PropertyExpressionVisitor<>)
                                .MakeGenericType(propertyMemberExpression.Expression.Type);
                            var propertyVisitor = (PropertyExpressionVisitor) Activator.CreateInstance(
                                propertyVisitorType, _configurator, nextSelectorFactory);
                            _selector = propertyVisitor.CreateSelector(propertyMemberExpression.Expression);

                            return memberExpression;
                        }
                    }
                }
            }

            NodeSelectorFactory nodeSelectorFactory = PropertyNodeSelectorFactory.Create(_configurator,
                _nodeSelectorFactory, propertyInfo);

            Type visitorType = typeof (PropertyExpressionVisitor<>).MakeGenericType(memberExpression.Expression.Type);
            var visitor =
                (PropertyExpressionVisitor) Activator.CreateInstance(visitorType, _configurator, nodeSelectorFactory);
            _selector = visitor.CreateSelector(memberExpression.Expression);

            return memberExpression;
        }
    }
}