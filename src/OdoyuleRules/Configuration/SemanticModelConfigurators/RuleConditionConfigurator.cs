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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Configurators;
    using Designer.Interpreters;
    using Models.SemanticModel;
    using SemanticModelBuilders;
    using Visualization;


    public class RuleConditionConfigurator<T> :
        RuleBuilderConfigurator
        where T : class
    {
        readonly IList<RuleCondition<T>> _conditions;
        readonly Expression<Func<T, bool>> _expression;

        public RuleConditionConfigurator(Expression<Func<T, bool>> expression)
        {
            _conditions = new List<RuleCondition<T>>();

            _expression = expression;
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            RuleCondition[] conditions = _expression.ParseConditions().ToArray();

            foreach (RuleCondition condition in conditions)
            {
                var ruleCondition = condition as RuleCondition<T>;
                if (ruleCondition != null)
                    _conditions.Add(ruleCondition);
                else
                    yield return this.Failure("Condition", "Must match expression type: " + typeof (T).GetShortName());
            }
        }

        public void Configure(RuleBuilder builder)
        {
            foreach (var condition in _conditions)
            {
                builder.AddCondition(condition);
            }
        }
    }
}