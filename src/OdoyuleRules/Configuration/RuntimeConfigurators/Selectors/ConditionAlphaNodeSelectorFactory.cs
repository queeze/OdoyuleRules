// Copyright 2011-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed 
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Configuration.RuntimeConfigurators.Selectors
{
    using System;
    using Compilation;


    public class ConditionAlphaNodeSelectorFactory :
        NodeSelectorFactory
    {
        readonly RuntimeConfigurator _configurator;
        readonly Action<RuleNodeSelector> _nodeCallback;

        public ConditionAlphaNodeSelectorFactory(RuntimeConfigurator configurator,
            Action<RuleNodeSelector> nodeCallback)
        {
            _configurator = configurator;
            _nodeCallback = nodeCallback;
        }

        public NodeSelector Create<T>()
            where T : class
        {
            var selector = new ConditionAlphaNodeSelector<T>(_configurator, _nodeCallback);

            return selector;
        }
    }
}