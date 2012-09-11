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
namespace OdoyuleRules.Configuration.RuntimeConfigurators.PropertySelectors
{
    using System;


    public class ReferenceTypePropertySelector<T> :
        PropertySelector<T, T>
        where T : class
    {
        public Type PropertyType
        {
            get { return typeof (T); }
        }

        public Type ValueType
        {
            get { return typeof (T); }
        }

        public bool TryGetValue(T property, out T value)
        {
            value = property;
            return true;
        }
    }
}