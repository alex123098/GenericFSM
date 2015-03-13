using System;

namespace GenericFSM.Exceptions
{
	[Serializable]
	public class CommandRegistrationException : Exception
	{
		public CommandRegistrationException() : this("Specified command already registered for this state without guard condition.") { }
		public CommandRegistrationException(string message) : base(message) { }
		public CommandRegistrationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
