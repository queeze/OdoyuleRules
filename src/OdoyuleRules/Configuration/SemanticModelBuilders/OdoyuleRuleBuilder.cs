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
namespace OdoyuleRules.Configuration.SemanticModelBuilders
{
    using System.Collections.Generic;
    using SemanticModel;


    public class OdoyuleRuleBuilder :
        RuleBuilder
    {
        readonly IList<RuleCondition> _conditions;
        readonly IList<RuleConsequence> _consequences;
        string _ruleName;

        public OdoyuleRuleBuilder()
        {
            _conditions = new List<RuleCondition>();
            _consequences = new List<RuleConsequence>();
        }

        public void SetName(string name)
        {
            _ruleName = name;
        }

        public void AddCondition<T>(RuleCondition<T> condition)
            where T : class
        {
            _conditions.Add(condition);
        }

        public void AddConsequence<T>(RuleConsequence<T> consequence)
            where T : class
        {
            _consequences.Add(consequence);
        }

        public Rule Build()
        {
            if (string.IsNullOrEmpty(_ruleName))
                throw new RulesEngineConfigurationException("Rule name was not set by configurator");

            return new OdoyuleRule(_ruleName, _conditions, _consequences);
        }
    }
}