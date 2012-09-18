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
namespace OdoyuleRules.Dsl.ParseModel
{
    using System.Linq;


    public class TypeConditionDeclaration :
        ConditionDeclaration
    {
        public TypeConditionDeclaration(string typeName, params ConditionDeclaration[] conditions)
            : base(DeclarationType.TypeCondition)
        {
            TypeName = typeName;
            Conditions = conditions;
        }

        public string TypeName { get; private set; }
        public ConditionDeclaration[] Conditions { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", TypeName, string.Join(", ", Conditions.Select(x => x.ToString())));
        }
    }
}