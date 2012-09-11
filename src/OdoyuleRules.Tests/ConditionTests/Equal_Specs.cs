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
namespace OdoyuleRules.Tests.ConditionTests
{
    using System.Collections.Generic;
    using Configuration;
    using NUnit.Framework;
    using SemanticModel;


    [TestFixture]
    public class Conditions_using_equal
    {
        [Test]
        public void Should_be_equatable()
        {
            var left = Conditions.Equal((Order o) => o.Name, "123");
            var right = Conditions.Equal((Order o) => o.Name, "123");

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Should_be_idempotent()
        {
            HashSet<RuleCondition> conditions = new HashSet<RuleCondition>();

            conditions.Add(Conditions.Equal((Order o) => o.Name, "123"));
            conditions.Add(Conditions.Equal((Order o) => o.Name, "123"));

            Assert.AreEqual(1, conditions.Count);
        }

        [Test]
        public void Should_match_equal_values()
        {
            _result = null;

            using (Session session = _engine.CreateSession())
            {
                session.Add(new Order {Name = "JOE"});
                session.Run();
            }

            Assert.IsNotNull(_result);
        }

        [Test]
        public void Should_not_match_inequal_values()
        {
            _result = null;

            using (Session session = _engine.CreateSession())
            {
                session.Add(new Order {Name = "BOB"});
                session.Run();
            }

            Assert.IsNull(_result);
        }

        Order _result;
        RulesEngine _engine;

        [TestFixtureSetUp]
        public void Define_rule()
        {
            var conditions = new[]
                {
                    Conditions.Equal((Order x) => x.Name, "JOE"),
                };

            var consequences = new[]
                {
                    Consequences.Delegate((Order o) => _result = o),
                };

            Rule rule = new OdoyuleRule("RuleA", conditions, consequences);

            _engine = RulesEngineFactory.New(x => x.Add(rule));
        }


        class Order
        {
            public string Name { get; set; }
        }
    }
}