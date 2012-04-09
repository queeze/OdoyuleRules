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
namespace OdoyuleRules.Configuration.RulesEngineConfigurators
{
    using System.Linq;
    using Models.RuntimeModel;

    public class ConvertNodeLocator<TInput, TOutput> :
        RuntimeModelVisitorImpl
        where TInput : class, TOutput
        where TOutput : class
    {
        readonly MemoryNode<TInput> _input;
        readonly MemoryNode<TOutput> _output;
        ConvertNode<TInput, TOutput> _node;

        public ConvertNodeLocator(MemoryNode<TInput> input,
            MemoryNode<TOutput> output)
        {
            _input = input;
            _output = output;
        }

        public void Find()
        {
            if (_node == null)
            {
                ConvertNode<TInput, TOutput> convertNode = _input.Successors
                    .OfType<ConvertNode<TInput, TOutput>>()
                    .FirstOrDefault();

                if (convertNode != null)
                    _node = convertNode;
                else
                {
                    _node = new ConvertNode<TInput, TOutput>(_output);
                    _input.AddActivation(_node);
                }
            }
        }
    }
}