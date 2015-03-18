using System;
using System.Diagnostics.Contracts;

namespace GenericFSM
{
	public static class Command
	{
		public static int GetHashCode<T>(T command, Delegate guardCondition = null) where T : struct {
			return command.GetHashCode() + (guardCondition != null ? guardCondition.Method.GetHashCode() * 11 : 0);
		}
	}

	public partial class StateMachine<TState, TCommand>
	{
		public class CommandObject
		{
			private readonly TCommand _command;
			private readonly Func<StateMachineContext, bool> _guardCondition;
			private readonly StateObject _targetState;

			internal CommandObject(TCommand command, Func<StateMachineContext, bool> guardCondition, StateObject targetState) {
				Contract.Requires<ArgumentNullException>(targetState != null);

				_command = command;
				_guardCondition = guardCondition;
				_targetState = targetState;
			}

			[Pure]
			public TCommand Command { get { return _command; } }
			
			public StateObject TargetState {
				get {
					Contract.Ensures(Contract.Result<StateObject>() != null);
					return _targetState;
				}
			}

			public override int GetHashCode() {
				return GenericFSM.Command.GetHashCode(_command, _guardCondition);
			}

			[Pure]
			internal bool CheckGuard(StateMachineContext ctx) {
				return _guardCondition == null || _guardCondition(ctx);
			}

			public static implicit operator TCommand(CommandObject commandObject) {
				Contract.Requires<ArgumentNullException>(commandObject != null);

				return commandObject.Command;
			}

			public override string ToString() {
				return string.Format("{{Command: {0}, State: {1}}}", _command, _targetState.State);
			}

			[ContractInvariantMethod]
			private void ContractInvariants() {
				Contract.Invariant(_targetState != null);
			}
		}
	}
}
