using System.Collections.Generic;

namespace GenericFSM.Tests.Infrastructure
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> MakeEnumerable<T>(this T value) {
			yield return value;
		}
	}
}
