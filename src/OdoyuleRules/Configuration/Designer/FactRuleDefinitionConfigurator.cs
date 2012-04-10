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


    public class FactRuleDefinitionConfigurator<T> :
        RuleDefinitionConfigurator<T>
        where T : class
    {
        readonly RuleConfigurator _configurator;

        public FactRuleDefinitionConfigurator(RuleConfigurator configurator)
        {
            _configurator = configurator;
        }

        public RuleDefinitionConfigurator<T> When(Expression<Func<T, bool>> expression)
        {
            var configurator = new RuleConditionConfiguratorImpl<T>(expression);
            _configurator.AddConfigurator(configurator);

            return this;
        }

        public Binding<T, TRight> Join<TRight>()
            where TRight : class
        {
            return new MultipleFactBinding<T, TRight>(_configurator);
        }

        public RuleDefinitionConfigurator<T> Then(Action<ThenConfigurator<T>> configureCallback)
        {
            var thenConfigurator = new ThenConfiguratorImpl<T>(_configurator);

            configureCallback(thenConfigurator);

            return this;
        }

    }
}