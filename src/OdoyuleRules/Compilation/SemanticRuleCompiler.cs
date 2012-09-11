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
    using Configuration.RuntimeConfigurators;
    using SemanticModel;


    public class SemanticRuleCompiler :
        RuleCompiler
    {
        readonly RuntimeConfigurator _configurator;

        public SemanticRuleCompiler(RuntimeConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Compile(Rule rule)
        {
            CompileRule(rule);
        }

        void CompileRule(Rule rule)
        {
            RuleConditionCompiler conditionCompiler = new SemanticRuleConditionCompiler(_configurator);

            foreach (RuleCondition condition in rule.Conditions)
            {
                condition.Accept(conditionCompiler);
            }

            RuleConsequenceCompiler consequenceCompiler = new OdoyuleRuleConsequenceCompiler(_configurator,
                conditionCompiler);

            foreach (RuleConsequence consequence in rule.Consequences)
            {
                consequence.Accept(consequenceCompiler);
            }
        }
    }
}