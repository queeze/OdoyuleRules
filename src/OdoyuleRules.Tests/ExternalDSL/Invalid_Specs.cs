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
    using System.Linq;
    using Dsl;
    using Dsl.ParseModel;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_rule_has_invalid_input_text
    {
        [Test]
        public void Should_display_the_error_position()
        {
            var ruleParser = new OdoyuleRuleParser();

            var ex =
                Assert.Throws<RuleParserException>(
                    () => { RuleDeclaration rule = ruleParser.Parse(BogusRule).Single(); });

            Assert.IsTrue(ex.Message.Contains("bogus"));
        }

        const string BogusRule = @"
rule Johnson
when
 bogus = Invalid(garbage)
then
end";
    }
}