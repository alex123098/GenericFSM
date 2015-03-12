using System;

namespace GenericFSM.Exceptions
{
	[Serializable]
	public class InvalidStateTypeException: Exception
	{
		public InvalidStateTypeException() : this ("The State type should be enum"){}
		public InvalidStateTypeException(string message) : base(message) {}
		public InvalidStateTypeException(string message, Exception innerException) : base(message, innerException) {}
	}
}
