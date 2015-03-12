using System;
using System.Runtime.Serialization;

namespace GenericFSM.Exceptions
{
	[Serializable]
	public class InvalidFsmConfigurationException : Exception
	{
		public InvalidFsmConfigurationException() { }
		public InvalidFsmConfigurationException(string message) : base(message) { }
		public InvalidFsmConfigurationException(string message, Exception innerException) : base(message, innerException) { }
		protected InvalidFsmConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
