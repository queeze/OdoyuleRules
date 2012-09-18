namespace OdoyuleRules.Tests.ExternalDSL
{
    using Dsl;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_rule_has_invalid_input_text
    {
        const string BogusRule = @"
rule Johnson
when
 bogus = Invalid(garbage)
then
end";
        [Test]
        public void Should_display_the_error_position()
        {
            var ruleParser = new OdoyuleRuleParser();

            var result = ruleParser.Parse(BogusRule);

        }
    }
}
