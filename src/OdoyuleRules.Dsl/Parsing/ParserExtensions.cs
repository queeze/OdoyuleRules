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


    public static class ParserExtensions
    {
        public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)
        {
            return input =>
                {
                    Result<T> result = parser(input);
                    var failure = result as Failure<T>;
                    if (failure != null)
                        return failure;

                    var success = result as Success<T>;
                    if (success == null)
                        return new FailureImpl<T>(input);

                    if (!predicate(success.Value))
                        return new FailureImpl<T>(input);

                    return result;
                };
        }


        public static Parser<TSelect> Select<T, TSelect>(this Parser<T> parser, Func<T, TSelect> selector)
        {
            return input =>
                {
                    Result<T> result = parser(input);
                    var failure = result as Failure<T>;
                    if (failure != null)
                        return new FailureImpl<TSelect>(failure.FailedInput);

                    var success = result as Success<T>;
                    if (success == null)
                        return new FailureImpl<TSelect>(input);

                    return new SuccessImpl<TSelect>(selector(success.Value), success.Rest);
                };
        }

        public static Parser<TSelect> SelectMany<T, TIntermediate, TSelect>(this Parser<T> parser,
            Func<T, Parser<TIntermediate>> selector, Func<T, TIntermediate, TSelect> projector)
        {
            return input =>
                {
                    Result<T> result = parser(input);
                    var failure = result as Failure<T>;
                    if (failure != null)
                        return new FailureImpl<TSelect>(failure.FailedInput);

                    var success = result as Success<T>;
                    if (success == null)
                        return new FailureImpl<TSelect>(input);

                    T value = success.Value;
                    Result<TIntermediate> nextResult = selector(value)(success.Rest);
                    var nextFailure = nextResult as Failure<TIntermediate>;
                    if (nextFailure != null)
                        return new FailureImpl<TSelect>(nextFailure.FailedInput);

                    var nextSuccess = nextResult as Success<TIntermediate>;
                    if (nextSuccess == null)
                        return new FailureImpl<TSelect>(input);

                    return new SuccessImpl<TSelect>(projector(value, nextSuccess.Value), nextSuccess.Rest);
                };
        }

        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            return input =>
                {
                    Result<T> result = first(input);
                    if (result is Success<T>)
                        return result;

                    return second(input);
                };
        }

        public static Parser<TSecond> And<TFirst, TSecond>(this Parser<TFirst> first, Parser<TSecond> second)
        {
            return input =>
                {
                    Result<TFirst> firstResult = first(input);
                    var failure = firstResult as Failure<TFirst>;
                    if (failure != null)
                        return new FailureImpl<TSecond>(failure.FailedInput);

                    var success = firstResult as Success<TFirst>;
                    if (success == null)
                        return new FailureImpl<TSecond>(input);

                    return second(success.Rest);
                };
        }
    }
}