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
namespace OdoyuleRules
{
    using System;
    using System.Collections.Generic;
    using Configuration.Configurators;
    using Configuration.RuntimeModelConfigurators;
    using Configuration.SemanticModelBuilders;
    using Configuration.SemanticModelConfigurators;
    using Models.SemanticModel;


    public static class DesignerExtensions
    {
        public static void Rule<TRule>(this RulesEngineConfigurator configurator)
            where TRule : RuleBuilderConfigurator, new()
        {
            Rule(configurator, () => new TRule());
        }

        public static void Rule<TRule>(this RulesEngineConfigurator configurator, Func<TRule> ruleFactory)
            where TRule : RuleBuilderConfigurator
        {
            TRule designer = ruleFactory();

            IEnumerable<ValidationResult> results = designer.ValidateConfiguration();
            ConfigurationResultImpl.CompileResults(results);

            var builder = new OdoyuleRuleBuilder();

            designer.Configure(builder);

            Rule rule = builder.Build();

            configurator.Add(rule);
        }

        public static void Rules(this RulesEngineConfigurator configurator,
            Action<DesignerRuleConfigurator> configureCallback)
        {
            var designerConfigurator = new DesignerRuleConfiguratorImpl();

            configureCallback(designerConfigurator);

            configurator.Add(designerConfigurator);
        }

        public static void Add(this RulesEngineConfigurator configurator, params Rule[] rules)
        {
            foreach (Rule rule in rules)
            {
                var semanticModelRuleConfigurator = new SemanticModelRuleConfigurator(rule);

                configurator.Add(semanticModelRuleConfigurator);
            }
        }
    }
}