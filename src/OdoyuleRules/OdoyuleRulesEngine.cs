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
namespace OdoyuleRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration.RuntimeModelConfigurators;
    using Internals.Caching;
    using Models.RuntimeModel;


    public class OdoyuleRulesEngine :
        RulesEngine
    {
        readonly RuntimeConfigurator _configurator;
        readonly Cache<Type, AlphaNodeInitializer> _initializers;
        readonly Cache<Type, ActivationTypeProxy> _objectCache;
        readonly Cache<Type, Activation> _types;

        public OdoyuleRulesEngine(RuntimeConfigurator configurator)
        {
            _initializers = new GenericTypeCache<AlphaNodeInitializer>(typeof (AlphaNodeInitializerImpl<>));
            _objectCache = new GenericTypeCache<ActivationTypeProxy>(typeof (ActivationTypeProxyImpl<>));
            _types = new GenericTypeCache<Activation>(typeof (AlphaNode<>));

            _configurator = configurator;
        }

        public IEnumerable<Activation> Activations
        {
            get { return _types; }
        }

        public void Activate<T>(ActivationContext<T> context) where T : class
        {
            _types.Get(typeof (T), CreateMissingAlphaNode<T>).Activate(context);
        }

        public Session CreateSession()
        {
            return new RuntimeSession(this, _objectCache);
        }

        public bool Accept(RuntimeModelVisitor visitor)
        {
            return visitor.Visit(this, next => _types.All(activation => activation.Accept(next)));
        }

        Activation CreateMissingAlphaNode<T>(Type type)
            where T : class
        {
            AlphaNode<T> alphaNode = _configurator.CreateNode(id => new AlphaNode<T>(id));

            foreach (Type nestedType in GetActivationTypes(typeof (T)))
                _initializers[nestedType].AddActivation(this, alphaNode);

            return alphaNode;
        }

        public bool HasAlphaNode(Type type)
        {
            return _types.Has(type);
        }

        public AlphaNode<T> GetAlphaNode<T>()
            where T : class
        {
            var value = _types.Get(typeof (T), CreateMissingAlphaNode<T>) as AlphaNode<T>;
            if (value != null)
                return value;

            throw new InvalidOperationException("The activation for " + typeof (T).Name + " is not an Alpha node");
        }

        static IEnumerable<Type> GetActivationTypes(Type type)
        {
            IEnumerable<Type> excludedInterfaces = Enumerable.Empty<Type>();
            Type baseType = type.BaseType;
            if ((baseType != null) && IsAllowedActivationType(baseType))
            {
                yield return baseType;

                excludedInterfaces = baseType.GetInterfaces();
            }

            IEnumerable<Type> interfaces = type
                .GetInterfaces()
                .Except(excludedInterfaces)
                .Where(IsAllowedActivationType);

            foreach (Type interfaceType in interfaces)
                yield return interfaceType;
        }

        static bool IsAllowedActivationType(Type type)
        {
            if (type.Namespace == null)
                return false;

            if (type.Assembly == typeof (object).Assembly)
                return false;

            if (type.Namespace == "System")
                return false;

            if (type.Namespace.StartsWith("System."))
                return false;

            return true;
        }
    }
}