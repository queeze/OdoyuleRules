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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using RuntimeModel.JoinNodes;


    class OdoyuleRulesEngineConfigurator :
        RulesEngineConfigurator,
        Configurator
    {
        readonly IList<RulesEngineBuilderConfigurator> _ruleConfigurators;
        readonly IList<Action<RuntimeConfigurator>> _runtimeConfiguratorActions;


        public OdoyuleRulesEngineConfigurator()
        {
            _ruleConfigurators = new List<RulesEngineBuilderConfigurator>();
            _runtimeConfiguratorActions = new List<Action<RuntimeConfigurator>>();
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            return _ruleConfigurators.SelectMany(x => x.ValidateConfiguration());
        }

        public void Add(RulesEngineBuilderConfigurator configurator)
        {
            _ruleConfigurators.Add(configurator);
        }

        public void RegisterPropertySelector(PropertySelectorFactory propertySelectorFactory)
        {
            _runtimeConfiguratorActions.Add(x => x.AddPropertySelectorFactory(propertySelectorFactory));
        }

        public RulesEngine Create()
        {
            RulesEngineBuilder builder = new OdoyuleRulesEngineBuilder();

            foreach (var configuratorAction in _runtimeConfiguratorActions)
            {
                builder.AddRuntimeConfiguratorAction(configuratorAction);
            }
            
            builder = _ruleConfigurators.Aggregate(builder, (b, x) => x.Configure(b));

            return builder.Build();
        }
    }
}