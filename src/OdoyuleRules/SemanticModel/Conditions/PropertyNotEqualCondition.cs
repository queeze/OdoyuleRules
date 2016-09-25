// Copyright 2011 Chris Patterson
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
namespace OdoyuleRules.SemanticModel
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    public class PropertyNotEqualCondition<T, TProperty> :
        PropertyCondition<T, TProperty>,
        RuleCondition<T>,
        IEquatable<PropertyNotEqualCondition<T, TProperty>>
		where T : class
		where TProperty : IComparable<TProperty>
    {
        readonly TProperty _value;

        public PropertyNotEqualCondition(PropertyInfo propertyInfo,
                                         Expression<Func<T, TProperty>> propertyExpression,
                                         TProperty value)
            : base(propertyInfo, propertyExpression)
        {
            _value = value;
        }

        public TProperty Value
        {
            get { return _value; }
        }

        public bool Equals(PropertyNotEqualCondition<T, TProperty> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._value, _value);
        }

        public bool Accept(SemanticVisitor visitor)
        {
            return visitor.Visit(this, x => true);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PropertyNotEqualCondition<T, TProperty>);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ _value.GetHashCode();
            }
        }
    }
}