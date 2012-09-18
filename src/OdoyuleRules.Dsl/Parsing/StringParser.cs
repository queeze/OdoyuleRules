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
namespace OdoyuleRules.Dsl.Parsing
{
    using System;
    using System.Linq;


    public abstract class StringParser
    {
        protected Parser<string, char> Char()
        {
            return input => input.Length > 0
                                ? (Result<char>)new Success<char>(input[0], input.Substring(1))
                                : (Result<char>)null;
        }

        protected Parser<string, char> Char(char ch)
        {
            return from c in Char() where c == ch select c;
        }

        protected Parser<string, char> Char(Predicate<char> pred)
        {
            return from c in Char() where pred(c) select c;
        }

        public Parser<string, TValue> Succeed<TValue>(TValue value)
        {
            return input => new Success<TValue>(value, input);
        }

        public Parser<string, TValue[]> ZeroOrMore<TValue>(Parser<string, TValue> parser)
        {
            return OneOrMore(parser).Or(Succeed(new TValue[0]));
        }

        public Parser<string, TValue[]> OneOrMore<TValue>(Parser<string, TValue> parser)
        {
            return from x in parser
                   from xs in ZeroOrMore(parser)
                   select (new[] {x}).Concat(xs).ToArray();
        }
    }
}