using System;

namespace GenericFSM.Exceptions
{
	[Serializable]
	public class CommandNotSupportedException : Exception
	{
		public CommandNotSupportedException() : this("Command is not supported in current state."){ }
		public CommandNotSupportedException(string message) : base(message) { }
		public CommandNotSupportedException(string message, Exception innerException) : base(message, innerException) { }
	}
}
