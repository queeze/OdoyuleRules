namespace OdoyuleRules.Tests.ConditionTests
{
    using System.Collections.Generic;
    using Configuration;
    using NUnit.Framework;
    using SemanticModel;


    [TestFixture]
    public class Conditions_using_not_equal
    {
        [Test]
        public void Should_be_equatable()
        {
            var left = Conditions.NotEqual((Order o) => o.Name, "123");
            var right = Conditions.NotEqual((Order o) => o.Name, "123");

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Should_be_idempotent()
        {
            HashSet<RuleCondition> conditions = new HashSet<RuleCondition>();

            conditions.Add(Conditions.NotEqual((Order o) => o.Name, "123"));
            conditions.Add(Conditions.NotEqual((Order o) => o.Name, "123"));

            Assert.AreEqual(1, conditions.Count);
        }

        [Test, Explicit("Not yet implemented, but soon")]
        public void Should_match_not_equal_values()
        {
            _result = null;

            using (Session session = _engine.CreateSession())
            {
                session.Add(new Order {Name = "JOE"});
                session.Run();
            }

            Assert.IsNotNull(_result);
        }

        [Test, Explicit("Not yet implemented, but soon")]
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
                    Conditions.NotEqual((Order x) => x.Name, "BOB"),
                };

            var consequences = new[]
                {
                    Consequences.Delegate((Order o) => _result = o),
                };

            Rule rule = new SemanticRule("RuleA", conditions, consequences);

            _engine = RulesEngineFactory.New(x => x.Add(rule));
        }


        class Order
        {
            public string Name { get; set; }
        }
    }
}