namespace OdoyuleRules
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class RuntimeConfigurationException :
        OdoyuleRulesException
    {
        public RuntimeConfigurationException()
        {
        }

        public RuntimeConfigurationException(string message)
            : base(message)
        {
        }

        public RuntimeConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RuntimeConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}