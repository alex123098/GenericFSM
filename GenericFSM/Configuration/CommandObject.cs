using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericFSM
{
	public static class Command
	{
		public static int GetHashCode<T>(T command, Func<bool> guardCondition = null) where T : struct {
			return command.GetHashCode() + (guardCondition != null ? guardCondition.Method.GetHashCode() * 11 : 0);
		}
	}

	public partial class StateMachine<TState, TCommand>
	{
		public class CommandObject
		{
			private readonly TCommand _command;
			private readonly Func<bool> _guardCondition;
			private readonly StateObject _targetState;

			internal CommandObject(TCommand command, Func<bool> guardCondition, StateObject targetState) {
				Contract.Requires<ArgumentNullException>(targetState != null);

				_command = command;
				_guardCondition = guardCondition;
				_targetState = targetState;
			}

			public override int GetHashCode() {
				return Command.GetHashCode(_command, _guardCondition);
			}
		}
	}
}
