// Copyright 2011 Chris Patterson
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
namespace OdoyuleRules.Dsl.Parsing
{
    using System.Collections.Generic;
    using System.Linq;


    public class RuleDefinition
    {
        public RuleDefinition(string name, IEnumerable<RuleConditionImpl> conditions)
        {
            Name = name;
            Conditions = conditions.ToArray();
        }

        public string Name { get; set; }

        public RuleConditionImpl[] Conditions { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n{1}", Name, string.Join(", ", Conditions.Select(x => x.ToString())));
        }
    }
}