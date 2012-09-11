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
namespace OdoyuleRules.Tests.Usage
{
    using System.Linq;
    using Configuration;
    using Designer;
    using NUnit.Framework;


    [TestFixture]
    public class Usage_of_the_rules_engine
    {
        [Test]
        public void Should_support_a_single_fact_rule()
        {
            var engine = RulesEngineFactory.New(x =>
                {
                    x.Rules(r =>
                        {
                            r.Add<FirstRule>();
                        });
                });


            FactHandle<Destination>[] destinations;
            using(var session = engine.CreateSession())
            {
                session.Add(new FirstSegmentImpl{SourceId = "public"});

                session.Run();

                destinations = session.Facts<Destination>().ToArray();
            }

            Assert.AreEqual(1, destinations.Length);
        }

        [Test]
        public void Should_support_double_fact_rules()
        {
            var engine = RulesEngineFactory.New(x =>
                {
                    x.Rule<SecondRule>();
                });


            FactHandle<Destination>[] destinations;
            using(var session = engine.CreateSession())
            {
                session.Add(new FirstSegmentImpl{SourceId = "public"});
                session.Add(new SecondSegmentImpl{Amount = 10001.0m});

                session.Run();

                destinations = session.Facts<Destination>().ToArray();
            }

            Assert.AreEqual(1, destinations.Length);
        }

        [Test]
        public void Should_optimize_rules_of_multiple_types()
        {
            var engine = RulesEngineFactory.New(x =>
                {
                    x.Rule<FirstRule>();
                    x.Rule<SecondRule>();
                });


            FactHandle<Destination>[] destinations;
            using(var session = engine.CreateSession())
            {
                object first = new FirstSegmentImpl {SourceId = "public"};
                session.Add(first);
                object second = new SecondSegmentImpl {Amount = 10001.0m};
                session.Add(second);

                session.Run();

                destinations = session.Facts<Destination>().ToArray();
            }

            Assert.AreEqual(2, destinations.Length);

            Assert.AreEqual("90210", destinations[0].Fact.Address);
            Assert.AreEqual("74011", destinations[1].Fact.Address);
        }

        class FirstRule :
            RuleDefinition
        {
            public FirstRule()
            {
                Fact<FirstSegment>()
                    .When(x => x.SourceId == "public")
                    .Add(() => new Destination("90210"));
            }
        }

        class SecondRule :
            RuleDefinition
        {
            public SecondRule()
            {
                Fact<FirstSegment>()
                    .Join<SecondSegment>()
                    .When(first => first.SourceId == "public")
                    .When(second => second.Amount > 10000.0m)
                    .Add(() => new Destination("74011"));
            }
        }

        class Destination
        {
            readonly string _address;

            public string Address
            {
                get { return _address; }
            }

            public Destination(string address)
            {
                _address = address;
            }
        }

        interface Segment
        {
        }

        interface FirstSegment :
            Segment
        {
            string SourceId { get; }
        }

        class FirstSegmentImpl :
            FirstSegment
        {
            public string SourceId { get; set; }
        }

        interface SecondSegment :
            Segment
        {
            decimal Amount { get; }
        }

        class SecondSegmentImpl :
            SecondSegment
        {
            public decimal Amount { get; set; }
        }
    }
}