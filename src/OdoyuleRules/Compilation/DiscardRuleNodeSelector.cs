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
    using Configuration.RuntimeModelConfigurators;
    using Internal;
    using Models.RuntimeModel;


    public class DiscardRuleNodeSelector<T, TDiscard> :
        RuleNodeSelector,
        LeftJoinRuleNodeSelector
        where T : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly LeftJoinRuleNodeSelector _left;
        LeftJoinNode<T, TDiscard> _node;
        RuleNodeSelector _parent;

        public DiscardRuleNodeSelector(RuntimeConfigurator configurator, LeftJoinRuleNodeSelector left)
        {
            _configurator = configurator;
            _left = left;
        }

        public void Match<TT, TTDiscard>(Action<LeftJoinNode<TT, TTDiscard>> callback)
            where TT : class
        {
            this.CallAs<DiscardRuleNodeSelector<Token<TT, TTDiscard>, TDiscard>>(self =>
                {
                    if (_node == null)
                        _left.Match<T, TDiscard>(leftJoin => { _node = leftJoin; });

                    _configurator.MatchLeftJoinNode(self._node, callback);
                });
        }

        public bool Select<TSelect>(Action<MemoryNode<TSelect>> callback)
            where TSelect : class
        {
            if (typeof (TSelect) == typeof (T) && _node == null)
            {
                _left.Match<T, TDiscard>(n => _node = n);
            }

            var node = _node as MemoryNode<TSelect>;
            if (node != null)
            {
                callback(node);
                return true;
            }

            if (_parent == null)
            {
                if (!typeof (T).IsGenericType || typeof (T).GetGenericTypeDefinition() != typeof (Token<,>))
                    return false;

                Type[] arguments = typeof (T).GetGenericArguments();

                _parent = (RuleNodeSelector) Activator.CreateInstance(
                    typeof (DiscardRuleNodeSelector<,>).MakeGenericType(arguments),
                    _configurator, this);
            }

            return _parent.Select(callback);
        }
    }
}