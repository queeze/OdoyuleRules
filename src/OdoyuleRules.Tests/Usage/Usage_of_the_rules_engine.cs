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
    using Designer;
    using NUnit.Framework;

    [TestFixture]
    public class Usage_of_the_rules_engine
    {
        [Test]
        public void Should_support_required_features()
        {
        }

        class FirstRule :
            RuleDefinition
        {
            public FirstRule()
            {
                Fact<FirstSegment>()
                    .When(x => x.SourceId == "public")
                    .Then(x => x.Add(fact => new Destination("90210")));
            }
        }

        class SecondRule :
            RuleDefinition
        {
            public SecondRule()
            {
                Fact<FirstSegment>()
                    //.Join<SecondSegment>()
                    .When((FirstSegment first) => first.SourceId == "public")
                    //.When((SecondSegment second) => second.Amount > 10000.0m)
                    .Then(x => x.Add(() => new Destination("74011")));
            }
        }

        class Destination
        {
            readonly string _address;

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