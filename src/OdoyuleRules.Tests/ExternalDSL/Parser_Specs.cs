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
    using Dsl.Parsing;
    using NUnit.Framework;


    [TestFixture]
    public class Parser_specs
    {
        [Test]
        public void TestCase()
        {
            string text = " rule \"MyRule\" when then end";

            Console.WriteLine(text);

            var parser = new OdoyuleRuleParser();

            RuleDeclaration result = parser.Parse(text).SingleOrDefault();

            Assert.IsNotNull(result);
            Console.WriteLine("Result: " + result);
        }

        [Test]
        public void TestCase2()
        {
            string text = " rule YourRule when then end";

            Console.WriteLine(text);

            var parser = new OdoyuleRuleParser();

            RuleDeclaration result = parser.Parse(text).SingleOrDefault();

            Assert.IsNotNull(result);
            Console.WriteLine("Result: " + result);
        }

        [Test]
        public void TestCase3()
        {
            string text =
                @"
rule YourRule
    when 
        Name == 'Mary'
        City == 'Tulsa'
        ZipCode == '12345'
    then
end";

            Console.WriteLine(text);

            var parser = new OdoyuleRuleParser();

            RuleDeclaration result = parser.Parse(text).SingleOrDefault();

            Assert.IsNotNull(result);
            Console.WriteLine("Result: " + result);
        }


        [Test]
        public void TestCase4()
        {
            string text = @"
rule YourRule
    when 
        MyClass(Name == 'Mary', City == 'Tulsa')
    then
end
";

            Console.WriteLine(text);

            var parser = new OdoyuleRuleParser();

            RuleDeclaration result = parser.Parse(text).SingleOrDefault();

            Assert.IsNotNull(result);
            Console.WriteLine("Result: " + result);
        }

        [Test]
        public void A_rule_with_a_named_match()
        {
            string text =
                @"
rule YourRule
    when 
        $name : MyClass(Name == 'Mary', City == 'Tulsa')
    then
end";

            Console.WriteLine("Input:");
            Console.WriteLine(text);

            var parser = new OdoyuleRuleParser();

            RuleDeclaration result = parser.Parse(text).SingleOrDefault();

            Assert.IsNotNull(result);
            Console.WriteLine("Result: " + result);

            Console.WriteLine("Model:");
            Console.WriteLine(result.ToString());

            Assert.AreEqual("YourRule", result.Name);

            Assert.AreEqual(1, result.Conditions.Count());

            ConditionDeclaration condition = result.Conditions.First();
            Assert.IsNotNull(condition);

            var variableCondition = condition as VariableTypeConditionDeclaration;
            Assert.IsNotNull(variableCondition);

            Assert.AreEqual("name", variableCondition.Variable.Name);
        }
    }
}