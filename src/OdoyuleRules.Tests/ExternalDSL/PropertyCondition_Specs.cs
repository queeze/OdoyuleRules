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
namespace OdoyuleRules.Tests.ExternalDSL
{
    using System;
    using System.Linq;
    using Dsl;
    using Dsl.ParseModel;
    using NUnit.Framework;


    public class PropertyRuleSubject
    {
        public PropertyRuleSubject()
        {
            Name = "Joe";
        }

        public string Name { get; private set; }
    }


    [TestFixture]
    public class Creating_a_condition_on_a_rule_property
    {
        [Test]
        public void Should_properly_match_the_condition_expression()
        {
            var ruleParser = new OdoyuleRuleParser();

            RuleDeclaration rule = ruleParser.Parse(RuleText).SingleOrDefault();

            Assert.IsNotNull(rule);


            Console.WriteLine(rule);
        }

        const string RuleText =
            @"
rule PropertyValueRule
using OdoyuleRules.Tests.ExternalDSL
when
    PropertyRuleSubject(Name == 'Joe')
then 
end";
    }
}