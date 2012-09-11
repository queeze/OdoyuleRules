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
namespace OdoyuleRules.Tests.InternalDSL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Configuration.Designer;
    using Models.RuntimeModel;
    using NUnit.Framework;
    using Visualizer;


    [TestFixture]
    public class When_dealing_with_nullable_facts
    {
        [Test]
        public void Should_be_able_to_handle_matching_value()
        {
            var value = new ValueImpl<string>(true, "123");
            var component = new ComponentImpl(value);
            var segment = new SegmentImpl(component);

            using (Session session = _engine.CreateSession())
            {
                session.Add<Segment>(segment);
                session.Run();

                List<FactHandle<Route>> routes = session.Facts<Route>().ToList();

                Assert.AreEqual(1, routes.Count);
            }
        }

        [Test]
        public void Should_be_able_to_handle_null_value()
        {
            var value = new ValueImpl<string>(false, null);
            var component = new ComponentImpl(value);
            var segment = new SegmentImpl(component);

            using (Session session = _engine.CreateSession())
            {
                session.Add<Segment>(segment);
                session.Run();

                List<FactHandle<Route>> routes = session.Facts<Route>().ToList();

                Assert.AreEqual(0, routes.Count);
            }
        }

        [Test]
        public void Should_be_able_to_handle_null_higher_value()
        {
            var segment = new SegmentImpl();

            using (Session session = _engine.CreateSession())
            {
                session.Add<Segment>(segment);
                session.Run();

                List<FactHandle<Route>> routes = session.Facts<Route>().ToList();

                Assert.AreEqual(0, routes.Count);
            }
        }

        [Test]
        public void Should_be_able_to_handle_a_nullable_value_type()
        {
            using (Session session = _engine.CreateSession())
            {
                session.Add(new Subject {Count = 27});
                session.Run();

                List<FactHandle<Route>> routes = session.Facts<Route>().ToList();

                Assert.AreEqual(1, routes.Count);
            }
        }


        [Test]
        [Explicit]
        public void Show_me()
        {
            _engine.ShowVisualizer();
        }

        RulesEngine _engine;

        [TestFixtureSetUp]
        public void Setup()
        {
            _engine = RulesEngineFactory.New(x =>
                {
                    x.RegisterPropertySelector(new ValuePropertySelectorFactory());

                    x.Rule<NestedValueRule>();
                    x.Rule<NullableIntRule>();
                });
        }


        class ValuePropertySelectorFactory :
            PropertySelectorFactory
        {
            public bool TryGetPropertySelector<TProperty>(PropertyInfo propertyInfo, out PropertySelector<TProperty> propertySelector)
            {
                PropertySelector result;
                if(TryGetPropertySelector(propertyInfo, out result))
                {
                    propertySelector = (PropertySelector<TProperty>) result;
                    return true;
                }

                propertySelector = null;
                return false;
            }

            public bool TryGetPropertySelector(PropertyInfo propertyInfo, out PropertySelector propertySelector)
            {
                var propertyType = propertyInfo.PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Value<>))
                {
                    Type valueType = propertyType.GetGenericArguments()[0];

                    Type type = typeof(ValuePropertySelector<>).MakeGenericType(valueType);

                    propertySelector = (PropertySelector)Activator.CreateInstance(type);
                    return true;
                }

                var declaringType = propertyInfo.DeclaringType;
                if (declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(Value<>))
                {
                    Type valueType = declaringType.GetGenericArguments()[0];

                    Type type = typeof(ValuePropertySelector<>).MakeGenericType(valueType);

                    propertySelector = (PropertySelector)Activator.CreateInstance(type);
                    return true;
                }

                propertySelector = null;
                return false;
            }
        }

        class ValuePropertySelector<T> :
            PropertySelector<Value<T>, T>
        {
            public bool TryGetValue(Value<T> property, out T value)
            {
                if(property.HasValue)
                {
                    value = property.Value;
                    return true;
                }

                value = default(T);
                return false;
            }

            public Type PropertyType
            {
                get { return typeof (Value<T>); }
            }

            public Type ValueType
            {
                get { return typeof(T); }
            }
        }


        interface Component
        {
            Value<string> Identifier { get; }
        }


        class ComponentImpl : Component
        {
            Value<string> _identifier;

            public ComponentImpl(Value<string> identifier)
            {
                _identifier = identifier;
            }

            public Value<string> Identifier
            {
                get { return _identifier; }
            }
        }


        interface Segment
        {
            Value<Component> SendingSystem { get; }
        }


        class SegmentImpl : Segment
        {
            readonly Value<Component> _componentValue;

            public SegmentImpl()
            {
                _componentValue = new ValueImpl<Component>(false, default(Component));
            }

            public SegmentImpl(Component component)
            {
                _componentValue = new ValueImpl<Component>(component != null, component);
            }

            public Value<Component> SendingSystem
            {
                get { return _componentValue; }
            }
        }


        interface Value<T>
        {
            bool HasValue { get; }

            T Value { get; }
        }


        class ValueImpl<T> : Value<T>
        {
            bool _hasValue;
            T _value;

            public ValueImpl(bool hasValue, T value)
            {
                _hasValue = hasValue;
                _value = value;
            }

            public bool HasValue
            {
                get { return _hasValue; }
            }

            public T Value
            {
                get { return _value; }
            }
        }


        class NestedValueRule :
            RuleDefinition
        {
            public NestedValueRule()
            {
                Fact<Segment>()
                    .When(x => x.SendingSystem.Value.Identifier.Value == "123")
                    .Add(() => new Route());
            }
        }

        class NullableIntRule :
            RuleDefinition
        {
            public NullableIntRule()
            {
                Fact<Subject>()
                    .When(x => x.Count == 27)
                    .Add(() => new Route());
            }
        }

        class Subject
        {
            public int? Count { get; set; }
        }


        class Route
        {
        }
    }
}