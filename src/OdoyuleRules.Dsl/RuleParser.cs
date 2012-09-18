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
namespace OdoyuleRules.Dsl
{
    using System.Linq;
    using ParseModel;
    using Parsing;
    using Parsing.Comparators;
    using Parsing.Operators;


    public abstract class RuleParser :
        InputParser
    {
        protected readonly Parser<string> DottedId;

        protected readonly Parser<char> FirstSymbolChar;
        protected readonly Parser<char> IdSeparator;

        protected readonly Parser<ConditionDeclaration[]> MatchConditions;
        protected readonly Parser<NamespaceDeclaration> Namespace;
        protected readonly Parser<ConditionDeclaration> NextCondition;
        protected readonly Parser<char> SymbolChar;
        protected readonly Parser<TypeConditionDeclaration> TypeMatch;
        protected readonly Parser<VariableDeclaration> Variable;
        protected readonly Parser<VariableTypeConditionDeclaration> VariableTypeCondition;

        public RuleParser()
        {
            Whitespace = ZeroOrMore(Char(' ').Or(Char('\t').Or(Char('\n')).Or(Char('\r'))));
            NewLine = ZeroOrMore(Char('\r').And(Char('\n')).Or(Char('\n')));
            Printable = ZeroOrMore(Char(char.IsLetterOrDigit).Or(Char(char.IsWhiteSpace)).Or(Char('.')).Or(Char(',')));

            FirstSymbolChar = from c in Char(char.IsLetter).Or(Char('_')) select c;
            SymbolChar = from c in Char(char.IsLetterOrDigit).Or(Char('_')) select c;

            Id = from w in Whitespace
                 from c in FirstSymbolChar
                 from cs in ZeroOrMore(SymbolChar)
                 select new string(new[] {c}.Concat(cs).ToArray());

            IdSeparator = from w in Whitespace
                          from c in Char('.')
                          select c;

            DottedId = from id in Id
                       from nextId in ZeroOrMore(
                           from c in IdSeparator
                           from idn in Id
                           select idn)
                       select string.Join(".", id, string.Join(".", nextId));

            QuotedString = from w in Whitespace
                           from enq in Char('"').Or(Char('\''))
                           from text in Printable
                           from deq in Char(enq)
                           select new string(text);

            ListSeparator = from w in Whitespace
                            from ch in Char(',')
                            select new ListSeparator();

            Minus = from w in Whitespace
                    from op in Char('-')
                    select (Operator)new MinusOperator();

            Plus = from w in Whitespace
                   from op in Char('+')
                   select (Operator)new PlusOperator();

            Multiply = from w in Whitespace
                       from op in Char('*')
                       select (Operator)new MultiplyOperator();

            Divide = from w in Whitespace
                     from op in Char('/')
                     select (Operator)new DivideOperator();

            Equal = from w in Whitespace
                    from op in Char('=').And(Char('='))
                    select (Comparator)new EqualComparator();

            NotEqual = from w in Whitespace
                       from op in Char('!').And(Char('='))
                       select (Comparator)new NotEqualComparator();

            GreaterThanOrEqual = from w in Whitespace
                                 from op in Char('>').And(Char('='))
                                 select (Comparator)new GreaterThanOrEqualComparator();

            LessThanOrEqual = from w in Whitespace
                              from op in Char('<').And(Char('='))
                              select (Comparator)new LessThanOrEqualComparator();

            LessThan = from w in Whitespace
                       from op in Char('<')
                       select (Comparator)new LessThanComparator();

            GreaterThan = from w in Whitespace
                          from op in Char('<')
                          select (Comparator)new GreaterThanComparator();

            Operators = Multiply.Or(Divide).Or(Plus).Or(Minus);

            Comparators = NotEqual.Or(GreaterThanOrEqual).Or(LessThanOrEqual).Or(Equal).Or(GreaterThan).Or(LessThan);

            Condition = from w in Whitespace
                        from id in Id
                        from op in Comparators
                        from value in QuotedString
                        select new CompareConditionDeclaration(id, op, value);

            NextCondition = from sep in ListSeparator
                            from cond in Condition
                            select cond;

            MatchConditions = from first in Condition
                              from rest in ZeroOrMore(NextCondition)
                              select new[] {first}.Concat(rest).ToArray();

            TypeMatch = from w in Whitespace
                        from className in Id
                        from open in Char('(')
                        from conditions in MatchConditions
                        from close in Char(')')
                        select new TypeConditionDeclaration(className, conditions);

            Namespace = from w in Whitespace
                        from prefix in Keyword("using")
                        from ns in DottedId
                        select new NamespaceDeclaration(ns);


            Variable = from w in Whitespace
                       from flag in Char('$')
                       from id in Id
                       select new VariableDeclaration(id);

            VariableTypeCondition = from v in Variable
                                    from w in Whitespace
                                    from c in Char(':')
                                    from t in TypeMatch
                                    select new VariableTypeConditionDeclaration(v, t);

            Rule = from bws in Whitespace
                   from open in Keyword("rule")
                   from name in QuotedString.Or(Id)
                   from nss in ZeroOrMore(Namespace)
                   from ws in Whitespace
                   from when in Keyword("when")
                   from conditions in
                       ZeroOrMore(VariableTypeCondition.Or<ConditionDeclaration>(TypeMatch).Or(Condition))
                   from ws2 in Whitespace
                   from then in Keyword("then")
                   from ws3 in Whitespace
                   from theEnd in Keyword("end")
                   from ews in Whitespace
                   select new RuleDeclaration(name, nss, conditions, Enumerable.Empty<ConsequenceDeclaration>());
        }


        protected Parser<ListSeparator> ListSeparator { get; private set; }

        protected Parser<Comparator> Equal { get; private set; }
        protected Parser<Comparator> NotEqual { get; private set; }
        protected Parser<Comparator> GreaterThan { get; private set; }
        protected Parser<Comparator> GreaterThanOrEqual { get; private set; }
        protected Parser<Comparator> LessThan { get; private set; }
        protected Parser<Comparator> LessThanOrEqual { get; private set; }
        protected Parser<Comparator> Comparators { get; private set; }

        protected Parser<Operator> Multiply { get; private set; }
        protected Parser<Operator> Divide { get; private set; }
        protected Parser<Operator> Plus { get; private set; }
        protected Parser<Operator> Minus { get; private set; }
        protected Parser<Operator> Operators { get; private set; }

        protected Parser<char[]> Whitespace { get; private set; }

        protected Parser<char[]> NewLine { get; private set; }

        protected Parser<string> Id { get; private set; }

        protected Parser<string> QuotedString { get; private set; }

        protected Parser<char[]> Printable { get; private set; }

        protected Parser<RuleDeclaration> Rule { get; private set; }

        protected Parser<ConditionDeclaration> Condition { get; private set; }
    }
}