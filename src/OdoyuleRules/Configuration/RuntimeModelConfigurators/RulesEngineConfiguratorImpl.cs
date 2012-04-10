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
namespace OdoyuleRules.Configuration.RuntimeModelConfigurators
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;


    class RulesEngineConfiguratorImpl :
        RulesEngineConfigurator,
        Configurator
    {
        IList<RulesEngineBuilderConfigurator> _ruleConfigurators;

        public RulesEngineConfiguratorImpl()
        {
            _ruleConfigurators = new List<RulesEngineBuilderConfigurator>();
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            foreach (RulesEngineBuilderConfigurator configurator in _ruleConfigurators)
            {
                foreach (ValidationResult result in configurator.ValidateConfiguration())
                {
                    yield return result;
                }
            }
        }

        public void Add(RulesEngineBuilderConfigurator configurator)
        {
            _ruleConfigurators.Add(configurator);
        }

        public RulesEngine Create()
        {
            RulesEngineBuilder builder = new OdoyuleRulesEngineBuilder();

            builder = _ruleConfigurators.Aggregate(builder, (current, configurator) => configurator.Configure(current));

            return builder.Build();
        }
    }
}