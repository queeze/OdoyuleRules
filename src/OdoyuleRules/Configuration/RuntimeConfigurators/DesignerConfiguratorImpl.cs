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
namespace OdoyuleRules.Configuration.RuntimeConfigurators
{
    using System;
    using System.Collections.Generic;
    using OdoyuleRules.Configuration.Builders;
    using OdoyuleRules.Configuration.Configurators;
    using OdoyuleRules.SemanticModel;
    using OdoyuleRules.Configuration.SemanticModelBuilders;
    using OdoyuleRules.Configuration.SemanticModelConfigurators;


    public class DesignerConfiguratorImpl :
        DesignerRuleConfigurator,
        EngineBuilderConfigurator
    {
        readonly IList<RuleBuilderConfigurator> _configurators;

        public DesignerConfiguratorImpl()
        {
            _configurators = new List<RuleBuilderConfigurator>();
        }

        public void Add<T>()
            where T : RuleBuilderConfigurator, new()
        {
            Add(() => new T());
        }

        public void Add<T>(Func<T> ruleFactory)
            where T : RuleBuilderConfigurator
        {
            T configurator = ruleFactory();

            _configurators.Add(configurator);
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            foreach (RuleBuilderConfigurator configurator in _configurators)
            {
                foreach (ValidationResult result in configurator.ValidateConfiguration())
                {
                    yield return result;
                }
            }
        }

        public EngineBuilder Configure(EngineBuilder builder)
        {
            foreach (RuleBuilderConfigurator configurator in _configurators)
            {
                var ruleBuilder = new OdoyuleRuleBuilder();

                configurator.Configure(ruleBuilder);

                Rule rule = ruleBuilder.Build();

                builder.AddRule(rule);
            }

            return builder;
        }
    }
}