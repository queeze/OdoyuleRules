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
    using System.Reflection;
    using OdoyuleRules.RuntimeModel.JoinNodes;


    public class DefaultPropertySelectorFactory :
        PropertySelectorFactory
    {
        public bool TryGetPropertySelector<TProperty>(PropertyInfo propertyInfo, out PropertySelector<TProperty> propertySelector)
        {
            PropertySelector result;
            if (TryGetPropertySelector(propertyInfo, out result))
            {
                propertySelector = (PropertySelector<TProperty>)result;
                return true;
            }

            propertySelector = null;
            return false;
        }

        public bool TryGetPropertySelector(PropertyInfo propertyInfo,
            out PropertySelector propertySelector)
        {
            Type underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (underlyingType != null)
            {
                Type selectorType = typeof (NullableTypePropertySelector<>).MakeGenericType(underlyingType);
                propertySelector = (PropertySelector) Activator.CreateInstance(selectorType);
                return true;
            }

            if (propertyInfo.PropertyType.IsEnum)
            {
                Type selectorType = typeof(EnumPropertySelector<,>).MakeGenericType(propertyInfo.PropertyType, typeof(int));
                propertySelector = (PropertySelector) Activator.CreateInstance(selectorType);
                return true;
            }

            if (propertyInfo.PropertyType.IsValueType)
            {
                Type selectorType = typeof(ValueTypePropertySelector<>).MakeGenericType(propertyInfo.PropertyType);
                propertySelector = (PropertySelector) Activator.CreateInstance(selectorType);
                return true;
            }

            Type genericType = typeof(ReferenceTypePropertySelector<>).MakeGenericType(propertyInfo.PropertyType);
            propertySelector = (PropertySelector) Activator.CreateInstance(genericType);
            return true;

//			if (typeof(IEnumerable<>).IsAssignableFrom(type))
//				return CreateEnumerableSerializer(type);
//
//			if (type.IsInterface)
//				return CreateObjectSerializerForInterface(type);
        }
    }
}