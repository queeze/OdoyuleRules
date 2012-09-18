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
    using System.Collections.Generic;


    public abstract class InputParser
    {
        protected Parser<char> Char()
        {
            return input => input.IsEnd
                                ? (Result<char>)new FailureImpl<char>(input)
                                : (Result<char>)new SuccessImpl<char>(input.Char, input.Next());
        }

        protected Parser<char> Char(char ch)
        {
            return from c in Char() where c == ch select c;
        }

        protected Parser<char> Char(Predicate<char> pred)
        {
            return from c in Char() where pred(c) select c;
        }

        protected Parser<string> Keyword(string text)
        {
            return input =>
                {
                    Input rest = input;

                    foreach (char c in text)
                    {
                        if (rest.IsEnd)
                            return new FailureImpl<string>(input);

                        char r = rest.Char;
                        if (r != c)
                            return new FailureImpl<string>(input);

                        rest = rest.Next();
                    }

                    return new SuccessImpl<string>(text, rest);
                };
        }

        public Parser<T> Succeed<T>(T value)
        {
            return input => new SuccessImpl<T>(value, input);
        }

        public Parser<T[]> ZeroOrMore<T>(Parser<T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return input =>
                {
                    Input rest = input;
                    var results = new List<T>();
                    Result<T> r = parser(input);
                    while (r is Success<T>)
                    {
                        var success = r as Success<T>;
                        if (rest == success.Rest)
                            break;

                        results.Add(success.Value);
                        rest = success.Rest;
                        r = parser(rest);
                    }

                    return new SuccessImpl<T[]>(results.ToArray(), rest);
                };
        }

        public Parser<T[]> OneOrMore<T>(Parser<T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return input =>
                {
                    Input rest = input;
                    var results = new List<T>();

                    Result<T> result = parser(input);
                    var failure = result as Failure<T>;
                    if (failure != null)
                        return new FailureImpl<T[]>(failure.FailedInput);

                    while (result is Success<T>)
                    {
                        var success = result as Success<T>;
                        if (rest == success.Rest)
                            break;

                        results.Add(success.Value);
                        rest = success.Rest;
                        result = parser(rest);
                    }

                    return new SuccessImpl<T[]>(results.ToArray(), rest);
                };
        }
    }
}