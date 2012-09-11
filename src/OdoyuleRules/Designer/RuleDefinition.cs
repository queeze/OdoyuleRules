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
    using System.Collections.Generic;
    using Configuration.Configurators;
    using Configuration.SemanticModelBuilders;
    using Configuration.SemanticModelConfigurators;


    /// <summary>
    /// A RuleDefinition is the base class used to create a class that uses
    /// the internal rule DSL to define a rule. Once defined, the RuleDefinition
    /// builds the semantic model for the rule, which can be added to the rules
    /// engine.
    /// </summary>
    public abstract class RuleDefinition :
        RuleBuilderConfigurator
    {
        readonly OdoyuleRuleConfigurator _ruleConfigurator;

        protected RuleDefinition()
        {
            _ruleConfigurator = new OdoyuleRuleConfigurator();
            _ruleConfigurator.SetName(GenerateDefaultRuleName());
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            return _ruleConfigurator.ValidateConfiguration();
        }

        public void Configure(RuleBuilder builder)
        {
            _ruleConfigurator.Configure(builder);
        }

        protected void Name(string name)
        {
            _ruleConfigurator.SetName(name);
        }

        protected FactDesigner<T> Fact<T>()
            where T : class
        {
            return _ruleConfigurator.Fact<T>();
        }

        string GenerateDefaultRuleName()
        {
            return GetType().Name.Replace("Rule", "");
        }
    }
}