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
namespace OdoyuleRules.Designer
{
    using System;
    using System.Linq.Expressions;
    using Configuration.SemanticModelConfigurators;


    public class JoinDesignerImpl<TLeft, TRight> :
        JoinDesigner<TLeft, TRight>
        where TLeft : class
        where TRight : class
    {
        readonly RuleConfigurator _configurator;

        public JoinDesignerImpl(RuleConfigurator configurator)
        {
            _configurator = configurator;
        }

        public JoinDesigner<TLeft, TRight> When(Expression<Func<TLeft, bool>> expression)
        {
            var configurator = new RuleConditionConfigurator<TLeft>(expression);
            _configurator.AddConfigurator(configurator);

            return this;
        }

        public JoinDesigner<TLeft, TRight> When(Expression<Func<TRight, bool>> expression)
        {
            var configurator = new RuleConditionConfigurator<TRight>(expression);
            _configurator.AddConfigurator(configurator);

            return this;
        }

        public JoinDesigner<TLeft, TRight> When(Expression<Func<TLeft, TRight, bool>> expression)
        {
            throw new NotImplementedException("beta network joins are not done yet");
//            IEnumerable<RuleConditionConfiguratorImpl<Token<TLeft,TRight>>> conditions = expression.ParseConditions()
//                .Select(x => new RuleConditionConfiguratorImpl<Token<TLeft,TRight>>(x));
//
//            foreach (var condition in conditions)
//            {
//                _configurator.AddConfigurator(condition);
//            }

            return this;
        }

        public JoinDesigner<TLeft, TRight> Then(Action<ThenDesigner<TLeft>> leftAction)
        {
            var thenConfigurator = new ThenDesignerImpl<TLeft>(_configurator);

            leftAction(thenConfigurator);

            return this;
        }

        public JoinDesigner<TLeft, TRight> Then(Action<ThenDesigner<TRight>> rightAction)
        {
            var thenConfigurator = new ThenDesignerImpl<TRight>(_configurator);

            rightAction(thenConfigurator);

            return this;
        }

        public JoinDesigner<TLeft, TRight> Then(Action<ThenDesigner<TLeft, TRight>> configureCallback)
        {
            var thenConfigurator = new ThenDesignerImpl<TLeft, TRight>(_configurator);

            configureCallback(thenConfigurator);

            return this;
        }
    }
}