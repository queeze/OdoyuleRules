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
namespace OdoyuleRules.Tests.Execution
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration.Designer;
    using NUnit.Framework;


    [TestFixture]
    public class Requesting_a_base_interface_of_a_fact
    {
        [Test]
        public void Should_return_instances_that_implement_said_fact()
        {
            RulesEngine engine = RulesEngineFactory.New(x =>
                {
                    x.Rules(r => r.Add<FirstRule>());
                });
            using (Session session = engine.CreateSession())
            {
                session.Add((IA)new A {Name = "Chris"});

                session.Run();

                IEnumerable<FactHandle<IA>> ass = session.Facts<IA>();

                Assert.AreEqual(1, ass.Count());
                Assert.AreEqual("Chris", ass.First().Fact.Name);
            }
        }


        class FirstRule :
            RuleDefinition
        {
            public FirstRule()
            {
                Fact<IA>();
            }
        }


        class A :
            IA
        {
            public string Name { get; set; }
        }
    }


    interface IA
    {
        string Name { get; }
    }
}