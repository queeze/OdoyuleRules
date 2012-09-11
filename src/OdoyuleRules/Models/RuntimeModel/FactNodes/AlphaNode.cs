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
namespace OdoyuleRules.Models.RuntimeModel
{
    using System.Linq;
    using Internals.Extensions;


    public class AlphaNode<T> :
        MemoryNodeImpl<T>,
        MemoryNode<T>,
        Activation
        where T : class
    {
        public AlphaNode(int id)
            : base(id)
        {
        }

        public void Activate<TActivation>(ActivationContext<TActivation> context)
            where TActivation : class
        {
            this.CastAs<Activation<TActivation>>().Activate(context);
        }

        public bool Accept(RuntimeModelVisitor visitor)
        {
            return visitor.Visit(this, next => Successors.All(activation => activation.Accept(next)));
        }
    }
}