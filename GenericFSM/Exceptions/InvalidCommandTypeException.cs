using System;

namespace GenericFSM.Exceptions
{
	[Serializable]
	public class InvalidCommandTypeException : Exception
	{
		public InvalidCommandTypeException() : this("The Command type should be enum") { }
		public InvalidCommandTypeException(string message) : base(message) { }
		public InvalidCommandTypeException(string message, Exception innerException) : base(message, innerException) { }
	}
}
