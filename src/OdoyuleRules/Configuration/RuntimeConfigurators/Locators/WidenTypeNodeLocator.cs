// Copyright 2011-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed 
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Configuration.RuntimeConfigurators.Locators
{
    using System.Linq;
    using RuntimeModel;
    using RuntimeModel.Nodes;


    public class WidenTypeNodeLocator<TInput, TOutput> :
        RuntimeVisitorBase,
        NodeLocator
        where TInput : class, TOutput
        where TOutput : class
    {
        readonly MemoryNode<TInput> _input;
        readonly MemoryNode<TOutput> _output;
        WidenTypeNode<TInput, TOutput> _node;

        public WidenTypeNodeLocator(MemoryNode<TInput> input, MemoryNode<TOutput> output)
        {
            _input = input;
            _output = output;
        }

        public bool Find()
        {
            if (_node != null)
                return true;

            WidenTypeNode<TInput, TOutput> widenTypeNode = _input.Successors
                .OfType<WidenTypeNode<TInput, TOutput>>()
                .FirstOrDefault();

            if (widenTypeNode != null)
                _node = widenTypeNode;
            else
            {
                _node = new WidenTypeNode<TInput, TOutput>(_output);
                _input.AddActivation(_node);
            }

            return _node != null;
        }
    }
}