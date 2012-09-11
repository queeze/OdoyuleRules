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
    using System;
    using Configuration.SemanticModelConfigurators;


    public static class DelegateDesignerExtensions
    {
        /// <summary>
        /// Specify a delegate to invoke if the rule conditions are met
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callback"></param>
        public static void Delegate<T>(this ThenDesigner<T> configurator, Action<T> callback)
            where T : class
        {
            var consequenceConfigurator = new DelegateRuleConsequenceConfigurator<T>(callback);

            configurator.AddConfigurator(consequenceConfigurator);
        }

        /// <summary>
        /// Specify a delegate to invoke if the rule conditions are met
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callback"></param>
        public static void Delegate<T>(this ThenDesigner<T> configurator, Action<Session, T> callback)
            where T : class
        {
            var consequenceConfigurator = new DelegateRuleConsequenceConfigurator<T>(callback);

            configurator.AddConfigurator(consequenceConfigurator);
        }
    }
}