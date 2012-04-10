namespace OdoyuleRules.Configuration
{
    using System;
    using Designer;
    using Models.SemanticModel;


    public static class AddFactConsequenceExtensions
    {
        public static void Add<T, TFact>(this RuleDefinitionConfigurator<T> binding, Func<T, TFact> factFactory)
            where T : class
            where TFact : class
        {
            binding.Then(then =>
                {
                    var consequence = new AddFactConsequence<T, TFact>(factFactory);

                    var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<T>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });
        }

        public static void Add<T, TFact>(this RuleDefinitionConfigurator<T> binding, Func<TFact> factFactory)
            where T : class
            where TFact : class
        {
            binding.Then(then =>
                {
                    AddFactConsequence<T, TFact> consequence = Consequences.Add<T, TFact>(x => factFactory());

                    var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<T>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });
        }


        public static Binding<TLeft, TRight> Add<TLeft, TRight, T>(this Binding<TLeft, TRight> binding,
            Func<T> factFactory)
            where TLeft : class
            where TRight : class
            where T : class
        {
            binding.Then((ThenConfigurator<TLeft, TRight> then) =>
                {
                    AddFactConsequence<Tuple<TLeft, TRight>, T> consequence =
                        Consequences.Add((Tuple<TLeft, TRight> t) => factFactory());

                    var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<Tuple<TLeft, TRight>>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return binding;
        }
    }
}