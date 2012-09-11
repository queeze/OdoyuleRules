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
namespace OdoyuleRules.Configuration
{
    using System;
    using RuntimeModel.Values;


    public static class Conditionals
    {
        public static Value<T> Constant<T>(T value)
        {
            Type underlyingType = Nullable.GetUnderlyingType(typeof (T));
            if (underlyingType != null)
                return NullableConstantValue.New(underlyingType, value);

            if (typeof (T).IsValueType)
                return ConstantValue.New(typeof (T), value);

            return ReferenceConstantValue.New(typeof (T), value);
        }

        public static TokenValueFactory<T, TProperty> Property<T, TProperty>()
            where T : class
        {
            Type underlyingType = Nullable.GetUnderlyingType(typeof (TProperty));
            if (underlyingType != null)
            {
                Type constantType = typeof (NullableTokenValueFactory<,>).MakeGenericType(typeof (T), typeof (TProperty));
                return (TokenValueFactory<T, TProperty>) Activator.CreateInstance(constantType);
            }

            if (typeof (TProperty).IsValueType)
            {
                Type constantType = typeof (ValueTypeTokenValueFactory<,>).MakeGenericType(typeof (T),
                    typeof (TProperty));
                return (TokenValueFactory<T, TProperty>) Activator.CreateInstance(constantType);
            }

            Type referenceType = typeof (ReferenceTypeTokenValueFactory<,>).MakeGenericType(typeof (T),
                typeof (TProperty));
            return (TokenValueFactory<T, TProperty>) Activator.CreateInstance(referenceType);
        }
    }
}