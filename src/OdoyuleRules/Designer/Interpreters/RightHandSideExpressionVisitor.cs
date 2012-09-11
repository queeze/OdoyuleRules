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
namespace OdoyuleRules.Designer.Interpreters
{
    using System;
    using System.Linq.Expressions;


    /// <summary>
    /// Inspects the right side of a binary expression
    /// </summary>
    public class RightHandSideExpressionVisitor :
        ExpressionVisitor
    {
        Func<object> _value;

        public RightHandSideExpressionVisitor()
        {
            _value = () => { throw new InvalidOperationException("The right hand value was not found"); };
        }

        public object Value
        {
            get { return _value(); }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _value = () => node.Value;
            return node;
        }
    }
}