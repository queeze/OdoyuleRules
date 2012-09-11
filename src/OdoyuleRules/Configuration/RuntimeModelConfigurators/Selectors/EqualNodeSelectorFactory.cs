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
namespace OdoyuleRules.Configuration.RuntimeModelConfigurators.Selectors
{
    using System;
    using RuntimeModel;


    public class EqualNodeSelectorFactory :
        NodeSelectorFactory
    {
        readonly NodeSelectorFactory _nextFactory;
        readonly object _value;
        RuntimeConfigurator _configurator;

        public EqualNodeSelectorFactory(NodeSelectorFactory nextFactory, RuntimeConfigurator configurator, object value)
        {
            _nextFactory = nextFactory;
            _value = value;
            _configurator = configurator;
        }

        public NodeSelector Create<T>()
            where T : class
        {
            if (!typeof (T).IsGenericType || typeof (T).GetGenericTypeDefinition() != typeof (Token<,>))
                throw new ArgumentException("Type was not a token type: " + typeof (T).FullName);

            return EqualNodeSelector.Create(_configurator, typeof (T), _nextFactory, _value);
        }
    }
}