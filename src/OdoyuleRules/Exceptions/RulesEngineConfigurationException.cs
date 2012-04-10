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
    using System.Linq;
    using System.Runtime.Serialization;
    using Configuration.Configurators;


    [Serializable]
    public class RulesEngineConfigurationException :
        OdoyuleRulesException
    {
        public RulesEngineConfigurationException()
        {
        }

        public RulesEngineConfigurationException(ConfigurationResult result)
            : this(GetMessage(result))
        {
            Result = result;
        }

        public RulesEngineConfigurationException(ConfigurationResult result, Exception innerException)
            : this(GetMessage(result), innerException)
        {
            Result = result;
        }

        public RulesEngineConfigurationException(string message)
            : base(message)
        {
        }

        public RulesEngineConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RulesEngineConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Result = (ConfigurationResult) info.GetValue("Result", typeof (ConfigurationResultImpl));
        }

        public ConfigurationResult Result { get; private set; }

        static string GetMessage(ConfigurationResult result)
        {
            return "There were errors configuring the rules engine:" +
                   Environment.NewLine +
                   string.Join(Environment.NewLine, result.Results
                       .Select(x => string.Format("{0}: {1}", x.Key, x.Message)).ToArray());
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Result", Result);
        }
    }
}