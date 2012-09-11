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
    using Configuration.RuntimeConfigurators;
    using Internals.Extensions;
    using RuntimeModel;
    using RuntimeModel.Nodes;


    /// <summary>
    /// Select the output node of a rule condition/match/etc.
    /// </summary>
    public interface RuleNodeSelector
    {
        bool Select<TSelect>(Action<MemoryNode<TSelect>> callback)
            where TSelect : class;
    }


    public class RuleNodeSelector<T> :
        RuleNodeSelector,
        LeftJoinRuleNodeSelector
        where T : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly AlphaNode<T> _node;
        RuleNodeSelector _parent;

        public RuleNodeSelector(RuntimeConfigurator configurator, AlphaNode<T> node)
        {
            _node = node;
            _configurator = configurator;
        }

        public void Match<TT, TTDiscard>(Action<LeftJoinNode<TT, TTDiscard>> callback)
            where TT : class
        {
            var self = this.CastAs<RuleNodeSelector<Token<TT, TTDiscard>>>();
                
            _configurator.MatchLeftJoinNode(self._node, callback);
        }

        public bool Select<TSelect>(Action<MemoryNode<TSelect>> callback)
            where TSelect : class
        {
            var node = _node as MemoryNode<TSelect>;
            if (node != null)
            {
                callback(node);
                return true;
            }

            if (_parent == null)
            {
                if (!typeof (T).IsGenericType || typeof (T).GetGenericTypeDefinition() != typeof (Token<,>))
                    throw new OdoyuleRulesException("Unable to map " + typeof (T) + " to " + typeof (TSelect));

                Type[] arguments = typeof (T).GetGenericArguments();

                _parent = (RuleNodeSelector) Activator.CreateInstance(
                    typeof (DiscardRuleNodeSelector<,>).MakeGenericType(arguments),
                    _configurator, this);
            }

            return _parent.Select(callback);
        }
    }
}