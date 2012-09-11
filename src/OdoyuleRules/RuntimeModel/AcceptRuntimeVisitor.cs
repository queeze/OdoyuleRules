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
    /// <summary>
    /// A node that can be visited by a visitor should implement this interface
    /// </summary>
    public interface AcceptRuntimeVisitor
    {
        /// <summary>
        /// Implemented by nodes that can be visited by the visitor.
        /// </summary>
        /// <param name="visitor">The visitor visiting the node</param>
        /// <returns>True to continue visitation, otherwise false</returns>
        bool Accept(RuntimeVisitor visitor);
    }
}