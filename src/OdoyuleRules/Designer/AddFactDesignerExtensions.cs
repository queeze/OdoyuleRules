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
namespace OdoyuleRules.Designer
{
    using System;
    using Configuration;
    using Configuration.SemanticModelConfigurators;
    using SemanticModel.Consequences;


    public static class AddFactDesignerExtensions
    {
        public static FactDesigner<T> Add<T, TAdd>(this FactDesigner<T> designer, Func<T, TAdd> factFactory)
            where T : class
            where TAdd : class
        {
            designer.Then(then =>
                {
                    AddFactConsequence<T, TAdd> consequence = Consequences.Add(factFactory);

                    var consequenceConfigurator = new RuleConsequenceConfigurator<T>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return designer;
        }

        public static FactDesigner<T> Add<T, TAdd>(this FactDesigner<T> designer, Func<TAdd> factFactory)
            where T : class
            where TAdd : class
        {
            designer.Then(then =>
                {
                    AddFactConsequence<T, TAdd> consequence = Consequences.Add<T, TAdd>(x => factFactory());

                    var consequenceConfigurator = new RuleConsequenceConfigurator<T>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return designer;
        }


        public static JoinDesigner<TLeft, TRight> Add<TLeft, TRight, T>(this JoinDesigner<TLeft, TRight> designer,
            Func<T> factFactory)
            where TLeft : class
            where TRight : class
            where T : class
        {
            designer.Then((ThenDesigner<TLeft, TRight> then) =>
                {
                    AddFactConsequence<Tuple<TLeft, TRight>, T> consequence =
                        Consequences.Add((Tuple<TLeft, TRight> t) => factFactory());

                    var consequenceConfigurator = new RuleConsequenceConfigurator<Tuple<TLeft, TRight>>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return designer;
        }

        public static JoinDesigner<TLeft, TRight> Add<TLeft, TRight, T>(this JoinDesigner<TLeft, TRight> designer,
            Func<Tuple<TLeft, TRight>, T> factFactory)
            where TLeft : class
            where TRight : class
            where T : class
        {
            designer.Then((ThenDesigner<TLeft, TRight> then) =>
                {
                    AddFactConsequence<Tuple<TLeft, TRight>, T> consequence = Consequences.Add(factFactory);

                    var consequenceConfigurator = new RuleConsequenceConfigurator<Tuple<TLeft, TRight>>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return designer;
        }

        public static JoinDesigner<TLeft, TRight> Add<TLeft, TRight, T>(this JoinDesigner<TLeft, TRight> designer,
            Func<TLeft, TRight, T> factFactory)
            where TLeft : class
            where TRight : class
            where T : class
        {
            designer.Then((ThenDesigner<TLeft, TRight> then) =>
                {
                    AddFactConsequence<Tuple<TLeft, TRight>, T> consequence =
                        Consequences.Add((Tuple<TLeft, TRight> t) => factFactory(t.Item1, t.Item2));

                    var consequenceConfigurator = new RuleConsequenceConfigurator<Tuple<TLeft, TRight>>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return designer;
        }
    }
}