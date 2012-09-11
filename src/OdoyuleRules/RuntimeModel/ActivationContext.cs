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
    using System;


    /// <summary>
    /// Encapsulates the activation of a fact as it moves from left to right
    /// in the network
    /// </summary>
    public interface ActivationContext
    {
        /// <summary>
        /// Create a new context linked to this context
        /// </summary>
        /// <typeparam name="TContext">The context type</typeparam>
        /// <param name="fact">The fact</param>
        /// <returns>The newly created context</returns>
        ActivationContext<TContext> CreateContext<TContext>(TContext fact)
            where TContext : class;

        /// <summary>
        /// Provides access to memory storage for nodes
        /// </summary>
        /// <typeparam name="TMemory">The type of memory to access</typeparam>
        /// <param name="id">The identifier for the node</param>
        /// <param name="callback">The callback to access the memory</param>
        void Access<TMemory>(int id, Action<ContextMemory<TMemory>> callback)
            where TMemory : class;

        /// <summary>
        /// Schedule an operation on the agenda for this session
        /// </summary>
        /// <param name="operation">The operation to invoke</param>
        /// <param name="priority">The priority of the operation, should be zero</param>
        void Schedule(Action<Session> operation, int priority = 0);
    }


    /// <summary>
    /// Encapsulates the activation of a fact, including the fact type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ActivationContext<out T> :
        ActivationContext
        where T : class
    {
        /// <summary>
        /// The fact itself
        /// </summary>
        T Fact { get; }

        /// <summary>
        /// Converts the activation context to the requested type, invoking the 
        /// callback
        /// </summary>
        /// <typeparam name="TOutput">The output type for the callback</typeparam>
        /// <param name="callback">The callback to invoke with the context</param>
        void Convert<TOutput>(Action<ActivationContext<TOutput>> callback)
            where TOutput : class;
    }
}