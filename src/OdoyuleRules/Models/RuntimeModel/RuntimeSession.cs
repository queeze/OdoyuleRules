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
namespace OdoyuleRules.Models.RuntimeModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Internals.Caching;


    class RuntimeSession :
        Session,
        ActivationContext
    {
        readonly Agenda _agenda;
        readonly FactCache _facts;
        readonly Cache<int, ContextMemory> _memoryCache;
        readonly Cache<Type, ActivationTypeProxy> _objectCache;
        readonly RulesEngine _rulesEngine;

        bool _disposed;
        long _runTime;

        public RuntimeSession(RulesEngine rulesEngine, Cache<Type, ActivationTypeProxy> objectCache)
        {
            _rulesEngine = rulesEngine;
            _objectCache = objectCache;
            _memoryCache = new ConcurrentCache<int, ContextMemory>();
            _facts = new FactCache();
            _agenda = new AgendaImpl();
        }

        public void Access<T>(int id, Action<ContextMemory<T>> callback)
            where T : class
        {
            ContextMemory contextMemory = _memoryCache.Get(id, key => new ContextMemory<T>());

            contextMemory.Access(callback);
        }

        public void Schedule(Action<Session> operation, int priority = 0)
        {
            _agenda.Schedule(() => operation(this), priority);
        }


        public ActivationContext<T> CreateContext<T>(T fact)
            where T : class
        {
            var context = new SessionActivationContext<T>(this, fact);

            return context;
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                if (Stopwatch.IsHighResolution)
                {
                    double ticks = _runTime;
                    ticks *= Stopwatch.Frequency;

                    return TimeSpan.FromTicks(unchecked((long) ticks));
                }

                return TimeSpan.FromTicks(_runTime);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public FactHandle<T> Add<T>(T fact)
            where T : class
        {
            ActivationContext<T> context = CreateContext(fact);

            _rulesEngine.Activate(context);

            return _facts.Add(context);
        }

        public FactHandle Add(object fact)
        {
            if (fact == null)
                throw new ArgumentNullException("fact");

            Type factType = fact.GetType();
            if (factType.IsValueType || factType == typeof (string))
                throw new ArgumentException("Facts must be reference types", "fact");

            return _objectCache[factType].Activate(_rulesEngine, this, _facts, fact);
        }

        public void Run()
        {
            long startedAt = Stopwatch.GetTimestamp();

            // let's schedule an executor to support timeouts if necessary and run as a background worker using TPL?
            while (_agenda.Run())
            {
            }

            long endedAt = Stopwatch.GetTimestamp();

            _runTime += (endedAt - startedAt);
        }

        public IEnumerable<FactHandle<T>> Facts<T>()
            where T : class
        {
            return _facts.Facts<T>();
        }

        ~RuntimeSession()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _facts.Clear();
                _memoryCache.Clear();
            }

            _disposed = true;
        }
    }
}