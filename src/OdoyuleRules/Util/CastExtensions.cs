// Copyright 2011 Chris Patterson
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
namespace OdoyuleRules.Util
{
    using System;

    static class CastExtensions
    {
        internal static T CastAs<T>(this object obj)
            where T : class
        {
            var self = obj as T;
            if (self == null)
            {
                string message = string.Format("Failed to cast {0} to {1}",
                    obj.GetType().FullName, typeof (T).FullName);
                throw new InternalRulesEngineException(message);
            }

            return self;
        }

        internal static void CastAs<T>(this object obj, Action<T> callback)
            where T : class
        {
            var self = obj as T;
            if (self == null)
            {
                string message = string.Format("Failed to cast {0} to {1}",
                    obj.GetType().FullName, typeof (T).FullName);
                throw new InternalRulesEngineException(message);
            }

            callback(self);
        }
    }
}