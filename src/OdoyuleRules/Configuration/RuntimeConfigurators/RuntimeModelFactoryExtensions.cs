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
namespace OdoyuleRules.Configuration.RuntimeConfigurators
{
    using System;
    using RuntimeModel;
    using RuntimeModel.Nodes;


    public static class RuntimeModelFactoryExtensions
    {
        public static AlphaNode<T> Alpha<T>(this RuntimeConfigurator configurator)
            where T : class
        {
            return configurator.CreateNode(id => new AlphaNode<T>(id));
        }

        public static AlphaNode<Token<T1, T2>> Alpha<T1, T2>(this RuntimeConfigurator configurator)
            where T1 : class
        {
            return configurator.CreateNode(id => new AlphaNode<Token<T1, T2>>(id));
        }

        public static LeftJoinNode<T1, T2> Left<T1, T2>(this RuntimeConfigurator configurator,
            RightActivation<T1> rightActivation)
            where T1 : class
        {
            return configurator.CreateNode(id => new LeftJoinNode<T1, T2>(id, rightActivation));
        }

        public static JoinNode<T> Join<T>(this RuntimeConfigurator configurator, RightActivation<T> rightActivation)
            where T : class
        {
            return configurator.CreateNode(id => new JoinNode<T>(id, rightActivation));
        }

        public static OuterJoinNode<T1, T2> Outer<T1, T2>(this RuntimeConfigurator configurator,
            RightActivation<T2> rightActivation)
            where T1 : class
            where T2 : class
        {
            return configurator.CreateNode(id => new OuterJoinNode<T1, T2>(id, rightActivation));
        }

        public static DelegateProductionNode<T> Delegate<T>(this RuntimeConfigurator configurator,
            Action<Session, T> callback)
            where T : class
        {
            return configurator.CreateNode(id => new DelegateProductionNode<T>(id, callback));
        }

        public static AddFactNode<T, TFact> AddFact<T, TFact>(this RuntimeConfigurator configurator,
            Func<T, TFact> factFactory)
            where T : class
            where TFact : class
        {
            return configurator.CreateNode(id => new AddFactNode<T, TFact>(id, factFactory));
        }

        public static ConstantNode<T> Constant<T>(this RuntimeConfigurator configurator)
            where T : class
        {
            return configurator.CreateNode(id => new ConstantNode<T>(id));
        }
    }
}