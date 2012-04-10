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
namespace OdoyuleRules.Configuration.Designer
{
    using System;
    using System.Linq.Expressions;
    using SemanticModelConfigurators;


    public class MultipleFactBinding<TLeft, TRight> :
        Binding<TLeft, TRight>
        where TLeft : class
        where TRight : class
    {
        readonly RuleConfigurator _configurator;

        public MultipleFactBinding(RuleConfigurator configurator)
        {
            _configurator = configurator;
        }

        public Binding<TLeft, TRight> When(Expression<Func<TLeft, bool>> expression)
        {
            var configurator = new RuleConditionConfiguratorImpl<TLeft>(expression);
            _configurator.AddConfigurator(configurator);

            return this;
        }

        public Binding<TLeft, TRight> When(Expression<Func<TRight, bool>> expression)
        {
            var configurator = new RuleConditionConfiguratorImpl<TRight>(expression);
            _configurator.AddConfigurator(configurator);

            return this;
        }

        public Binding<TLeft, TRight> When(Expression<Func<TLeft, TRight, bool>> expression)
        {
//            IEnumerable<RuleConditionConfiguratorImpl<Token<TLeft,TRight>>> conditions = expression.ParseConditions()
//                .Select(x => new RuleConditionConfiguratorImpl<Token<TLeft,TRight>>(x));
//
//            foreach (var condition in conditions)
//            {
//                _configurator.AddConfigurator(condition);
//            }

            return this;
        }

        public Binding<TLeft, TRight> Then(Action<ThenConfigurator<TLeft>> leftAction)
        {
            var thenConfigurator = new ThenConfiguratorImpl<TLeft>(_configurator);

            leftAction(thenConfigurator);

            return this;
        }

        public Binding<TLeft, TRight> Then(Action<ThenConfigurator<TRight>> rightAction)
        {
            var thenConfigurator = new ThenConfiguratorImpl<TRight>(_configurator);

            rightAction(thenConfigurator);

            return this;
        }

        public Binding<TLeft, TRight> Then(Action<ThenConfigurator<TLeft, TRight>> configureCallback)
        {
            var thenConfigurator = new ThenConfiguratorImpl<TLeft, TRight>(_configurator);

            configureCallback(thenConfigurator);

            return this;
        }
    }
}