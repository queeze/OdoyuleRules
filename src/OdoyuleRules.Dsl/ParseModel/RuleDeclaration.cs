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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public class RuleDeclaration :
        Declaration
    {
        readonly IList<ConditionDeclaration> _conditions;
        readonly IList<ConsequenceDeclaration> _consequences;
        readonly string _name;
        readonly IEnumerable<NamespaceDeclaration> _namespaces;

        public RuleDeclaration(string ruleName, IEnumerable<NamespaceDeclaration> namespaces,
            IEnumerable<ConditionDeclaration> conditions, IEnumerable<ConsequenceDeclaration> consequences)
            : base(DeclarationType.Rule)
        {
            _namespaces = namespaces;
            _name = ruleName;
            _conditions = conditions.ToList();
            _consequences = consequences.ToList();
        }

        public IEnumerable<ConditionDeclaration> Conditions
        {
            get { return _conditions; }
        }

        public IEnumerable<ConsequenceDeclaration> Consequences
        {
            get { return _consequences; }
        }

        public IEnumerable<NamespaceDeclaration> Namespaces
        {
            get { return _namespaces; }
        }

        public string Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("rule {0}", _name).AppendLine();
            foreach (NamespaceDeclaration ns in _namespaces)
            {
                sb.Append(' ', 4).AppendLine(ns.ToString());
            }
            sb.Append(' ', 4).AppendLine("when");
            foreach (ConditionDeclaration condition in _conditions)
            {
                sb.Append(' ', 8).AppendLine(condition.ToString());
            }
            sb.Append(' ', 4).AppendLine("then");
            foreach (ConsequenceDeclaration consequence in _consequences)
            {
                sb.Append(' ', 8).AppendLine(consequence.ToString());
            }
            sb.AppendLine("end");

            return sb.ToString();
        }
    }
}