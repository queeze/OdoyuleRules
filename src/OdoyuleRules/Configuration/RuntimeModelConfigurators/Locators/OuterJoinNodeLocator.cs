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
namespace OdoyuleRules.Configuration.RuntimeModelConfigurators.Locators
{
    using System;
    using System.Linq;
    using OdoyuleRules.Models.RuntimeModel;


    public class OuterJoinNodeLocator<T1, T2> :
        RuntimeModelVisitorBase
        where T1 : class
        where T2 : class
    {
        readonly RuntimeConfigurator _configurator;
        readonly MemoryNode<T1> _left;
        readonly Func<RightActivation<T2>, bool> _matchRight;
        readonly MemoryNode<T2> _right;
        readonly Func<RightActivation<T2>> _rightActivation;
        OuterJoinNode<T1, T2> _node;

        public OuterJoinNodeLocator(RuntimeConfigurator runtimeConfigurator, MemoryNode<T1> left, MemoryNode<T2> right)
        {
            _configurator = runtimeConfigurator;
            _left = left;
            _right = right;
            _rightActivation = () => right as RightActivation<T2>;

            _matchRight = MatchNode;
        }

        public void Find(Action<OuterJoinNode<T1, T2>> callback)
        {
            if (_node == null)
            {
                OuterJoinNode<T1, T2> joinNode = _left.Successors
                    .OfType<OuterJoinNode<T1, T2>>()
                    .Where(node => _matchRight(node.RightActivation))
                    .FirstOrDefault();

                if (joinNode != null)
                    _node = joinNode;
                else
                {
                    RightActivation<T2> rightActivation = _rightActivation();
                    _node = _configurator.Outer<T1, T2>(rightActivation);
                    _left.AddActivation(_node);
                }
            }

            if (_node != null)
                callback(_node);
        }

        bool MatchNode(RightActivation<T2> node)
        {
            RightActivation<T2> rightActivation = _rightActivation();
            if (rightActivation == null)
                throw new InternalRulesEngineException("Unexpected null right activation cast");

            return rightActivation.Equals(node);
        }
    }
}