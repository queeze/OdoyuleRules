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
namespace OdoyuleRules.Configuration.RuntimeModelConfigurators
{
    using System;
    using Models.RuntimeModel;


    public interface RuntimeConfigurator
    {
        RulesEngine RulesEngine { get; }

        T CreateNode<T>(Func<int, T> nodeFactory);

        AlphaNode<T> GetAlphaNode<T>()
            where T : class;

        void MatchJoinNode<T>(MemoryNode<T> left, Action<JoinNode<T>> callback)
            where T : class;

        void MatchJoinNode<T>(MemoryNode<T> left, MemoryNode<T> right, Action<JoinNode<T>> action)
            where T : class;

        void MatchLeftJoinNode<T, TDiscard>(MemoryNode<Token<T, TDiscard>> start,
            Action<LeftJoinNode<T, TDiscard>> callback)
            where T : class;

        void MatchOuterJoinNode<T1, T2>(MemoryNode<T1> leftStart, MemoryNode<T2> rightStart,
            Action<OuterJoinNode<T1, T2>> callback)
            where T1 : class
            where T2 : class;
    }
}