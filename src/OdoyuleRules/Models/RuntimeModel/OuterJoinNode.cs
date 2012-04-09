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
namespace OdoyuleRules.Models.RuntimeModel
{
    using System.Linq;

    public class OuterJoinNode<TLeft, TRight> :
        MemoryNodeImpl<Token<TLeft, TRight>>,
        Activation<TLeft>,
        MemoryNode<Token<TLeft, TRight>>
        where TLeft : class
        where TRight : class
    {
        readonly RightActivation<TRight> _rightActivation;

        public OuterJoinNode(int id, RightActivation<TRight> rightActivation)
            : base(id)
        {
            _rightActivation = rightActivation;
        }

        public void Activate(ActivationContext<TLeft> context)
        {
            _rightActivation.RightActivate(context, right =>
                {
                    ActivationContext<Token<TLeft, TRight>> joinContext =
                        context.CreateContext(new Token<TLeft, TRight>(context, right.Fact));

                    base.Activate(joinContext);

                    return true;
                });
        }

        public bool Accept(RuntimeModelVisitor visitor)
        {
            return visitor.Visit(this, next => _rightActivation.Accept(next)
                                               && Successors.All(activation => activation.Accept(next)));
        }
    }
}