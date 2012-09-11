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
namespace OdoyuleRules.RuntimeModel
{
    using System;
    using Configuration.RuntimeModelConfigurators;
    using Configuration.RuntimeModelConfigurators.Locators;
    using FactNodes;
    using Visualization;


    class AlphaNodeInitializerImpl<T> :
        AlphaNodeInitializer
        where T : class
    {
        public void AddActivation<TParent>(OdoyuleRulesEngine rulesEngine, AlphaNode<TParent> activation)
            where TParent : class
        {
            AlphaNode<T> alphaNode = rulesEngine.GetAlphaNode<T>();

            RunLocator(activation, alphaNode);
        }

        void RunLocator<TParent>(AlphaNode<TParent> activation, AlphaNode<T> alphaNode)
            where TParent : class
        {
            Type type = typeof (WidenTypeNodeLocator<,>).MakeGenericType(typeof (TParent), typeof (T));
            var locator = (NodeLocator) Activator.CreateInstance(type, activation, alphaNode);

            bool found = locator.Find();
            if (!found)
                throw new InternalRulesEngineException("Unable to widen type "
                                                       + typeof (TParent).GetShortName()
                                                       + " to type "
                                                       + typeof (T).GetShortName());
        }
    }
}