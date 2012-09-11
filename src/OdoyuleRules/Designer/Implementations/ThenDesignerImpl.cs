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
    using Configuration.SemanticModelConfigurators;


    public class ThenDesignerImpl<T> :
        ThenDesigner<T>
        where T : class
    {
        readonly RuleConfigurator _configurator;

        public ThenDesignerImpl(RuleConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void AddConfigurator(RuleBuilderConfigurator configurator)
        {
            _configurator.AddConfigurator(configurator);
        }
    }

    public class ThenDesignerImpl<TLeft, TRight> :
        ThenDesigner<TLeft, TRight>
        where TRight : class
        where TLeft : class
    {
        readonly RuleConfigurator _configurator;

        public ThenDesignerImpl(RuleConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void AddConfigurator(RuleBuilderConfigurator configurator)
        {
            _configurator.AddConfigurator(configurator);
        }
    }
}