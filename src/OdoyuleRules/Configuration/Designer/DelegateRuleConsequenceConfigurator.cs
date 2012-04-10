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
namespace OdoyuleRules.Configuration.Designer
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Models.SemanticModel;
    using SemanticModelBuilders;
    using SemanticModelConfigurators;


    public class DelegateRuleConsequenceConfigurator<T> :
        RuleBuilderConfigurator
        where T : class
    {
        readonly Action<Session, T> _callback;

        public DelegateRuleConsequenceConfigurator(Action<Session, T> callback)
        {
            _callback = callback;
        }

        public DelegateRuleConsequenceConfigurator(Action<T> callback)
        {
            _callback = (session, fact) => callback(fact);
        }

        public IEnumerable<ValidationResult> ValidateConfiguration()
        {
            if (_callback == null)
                yield return this.Failure("Delegate", "must not be null");
        }

        public void Configure(RuleBuilder builder)
        {
            var consequence = new DelegateConsequence<T>(_callback);

            builder.AddConsequence(consequence);
        }
    }
}