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
    using Configuration.RuntimeConfigurators;
    using Configuration.SemanticModelConfigurators;
    using SemanticModel;


    public static class RulesEngineConfiguratorExtensions
    {
        [Obsolete("Use the nested Rules() for multiple rules")]
        public static void Rule<TRule>(this EngineConfigurator configurator)
            where TRule : RuleBuilderConfigurator, new()
        {
            Rule(configurator, () => new TRule());
        }

        [Obsolete("Use the nested Rules() for multiple rules")]
        public static void Rule<TRule>(this EngineConfigurator configurator, Func<TRule> ruleFactory)
            where TRule : RuleBuilderConfigurator
        {
            configurator.Rules(x => x.Add(ruleFactory));
        }

        public static void Rules(this EngineConfigurator configurator,
            Action<DesignerRuleConfigurator> configureCallback)
        {
            var designerConfigurator = new DesignerConfiguratorImpl();

            configureCallback(designerConfigurator);

            configurator.Add(designerConfigurator);
        }

        public static void Add(this EngineConfigurator configurator, params Rule[] rules)
        {
            foreach (Rule rule in rules)
            {
                var semanticModelRuleConfigurator = new SemanticModelConfigurator(rule);

                configurator.Add(semanticModelRuleConfigurator);
            }
        }
    }
}