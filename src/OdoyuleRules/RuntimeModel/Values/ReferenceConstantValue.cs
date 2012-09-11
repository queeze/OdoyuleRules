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
namespace OdoyuleRules.RuntimeModel.Values
{
    using System;


    public static class ReferenceConstantValue
    {
        public static Value<T> New<T>(Type valueType, T value)
        {
            Type referenceConstantType = typeof (ReferenceConstantValue<>).MakeGenericType(valueType);
            return (Value<T>) Activator.CreateInstance(referenceConstantType, value);
        }
    }


    public class ReferenceConstantValue<T> :
        Value<T>
        where T : class
    {
        readonly T _value;

        public ReferenceConstantValue(T value)
        {
            _value = value;
        }

        public bool TryGetValue(out T value)
        {
            if (_value != null)
            {
                value = _value;
                return true;
            }

            value = default(T);
            return false;
        }

        public override string ToString()
        {
            return _value == null ? "(null)" : _value.ToString();
        }
    }
}