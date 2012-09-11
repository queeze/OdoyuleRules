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
namespace OdoyuleRules.RuntimeModel.JoinNodes
{
    using System.Reflection;


    public interface PropertySelectorFactory
    {
        /// <summary>
        /// Attempts to create a PropertySelector for the property
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        bool TryGetPropertySelector<TProperty>(PropertyInfo propertyInfo,
            out PropertySelector<TProperty> propertySelector);

        /// <summary>
        /// Non-generic version
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        bool TryGetPropertySelector(PropertyInfo propertyInfo, out PropertySelector propertySelector);
    }
}