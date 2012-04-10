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
namespace OdoyuleRules.Configuration.SemanticModelConfigurators
{
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using Designer;
    using SemanticModelBuilders;


    public class OdoyuleRuleConfigurator :
        RuleConfigurator,
        RuleBuilderConfigurator
    {
        readonly List<RuleBuilderConfigurator> _configurators;
        string _name;

        public OdoyuleRuleConfigurator()
        {
            _configurators = new List<RuleBuilderConfigurator>();
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(_name))
                yield return this.Failure("The rule name must be specified");

            foreach (ValidationResult result
                in _configurators.SelectMany(configurator => configurator.ValidateConfiguration()))
                yield return result;
        }

        public void Configure(RuleBuilder builder)
        {
            builder.SetName(_name);

            foreach (RuleBuilderConfigurator configurator in _configurators)
            {
                configurator.Configure(builder);
            }
        }

        public void AddConfigurator(RuleBuilderConfigurator configurator)
        {
            _configurators.Add(configurator);
        }

        public RuleDefinitionConfigurator<T> Fact<T>()
            where T : class
        {
            return new FactRuleDefinitionConfigurator<T>(this);
        }

        public void SetName(string ruleName)
        {
            _name = ruleName;
        }
    }
}