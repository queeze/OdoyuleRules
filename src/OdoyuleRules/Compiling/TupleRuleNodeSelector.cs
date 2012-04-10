// Copyright 2011 Chris Patterson
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
namespace OdoyuleRules.Compiling
{
    using System;
    using Configuration.RuntimeModelConfigurators;
    using Models.RuntimeModel;

    public class TupleRuleNodeSelector<T1, T2> :
        RuleNodeSelector
        where T1 : class
        where T2 : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly RuleConditionCompiler _next;
        OuterJoinNode<T1, T2> _node;
        RuleNodeSelector _parent;

        public TupleRuleNodeSelector(RuntimeConfigurator configurator, RuleConditionCompiler next)
        {
            _configurator = configurator;
            _next = next;
        }

        public bool Select<TSelect>(Action<MemoryNode<TSelect>> callback)
            where TSelect : class
        {
            if (typeof (TSelect) == typeof (Tuple<T1, T2>) && _node == null)
            {
                _next.MatchJoinNode<T1>(t1 =>
                    {
                        _next.MatchJoinNode<T2>(t2 =>
                            {
                                _configurator.MatchOuterJoinNode(t1, t2, outerJoin =>
                                    {
                                        // finally!
                                        _node = outerJoin;
                                    });
                            });
                    });
            }

            var node = _node as MemoryNode<TSelect>;
            if (node != null)
            {
                callback(node);
                return true;
            }

            return false;
        }
    }
}