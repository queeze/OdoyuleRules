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
namespace OdoyuleRules.Tests.Execution
{
    using System;
    using Configuration.RuntimeConfigurators;
    using NUnit.Framework;
    using RuntimeModel;
    using RuntimeModel.JoinNodes;
    using RuntimeModel.Nodes;


    [TestFixture]
    public class When_matching_two_separate_types_into_a_single_token
    {
        [Test]
        public void Should_perform_an_outer_join()
        {
            Assert.IsNotNull(_called);
        }

        Tuple<A, B> _called;

        [TestFixtureSetUp]
        public void Setup()
        {
            _called = null;

            var configurator = new OdoyuleRuntimeConfigurator();

            var productionNode = new DelegateProductionNode<Tuple<A,B>>(16, (session, x) => _called = x);

            var constantNode = new ConstantNode<A>(42);

            JoinNode<A> joinNodeA = configurator.CreateNode(id => new JoinNode<A>(id, constantNode));

            var constantNode2 = new ConstantNode<B>(27);

            JoinNode<B> joinNodeB = configurator.CreateNode(id => new JoinNode<B>(id, constantNode2));

            OuterJoinNode<A, B> outerJoinNode = configurator.CreateNode(id => new OuterJoinNode<A, B>(id, joinNodeB));
            outerJoinNode.AddActivation(productionNode);

            joinNodeA.AddActivation(outerJoinNode);

            var engine = new OdoyuleRulesEngine(configurator);

            AlphaNode<A> alphaNode = engine.GetAlphaNode<A>();
            alphaNode.AddActivation(joinNodeA);

            AlphaNode<B> alphaNodeB = engine.GetAlphaNode<B>();
            alphaNodeB.AddActivation(joinNodeB);

            using (Session session = engine.CreateSession())
            {
                session.Add(new A());
                session.Add(new B());
                session.Run();
            }
        }

        class A
        {
        }

        class B
        {
        }
    }
}