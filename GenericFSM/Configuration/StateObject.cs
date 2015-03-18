using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace GenericFSM
{
	public partial class StateMachine<TState, TCommand>
	{
		public class StateObject
		{
			private readonly TState _state;
			private readonly Action<StateMachineContext> _enterAction;
			private readonly Action<StateMachineContext> _exitAction;
			private readonly List<CommandObject> _commands = new List<CommandObject>(); 

			public TState State { get { return _state; } }

			[Pure]
			public IEnumerable<CommandObject> Commands {
				get {
					Contract.Ensures(Contract.Result<IEnumerable<CommandObject>>() != null);
					return _commands;
				}
			}

			internal StateObject(TState state, Action<StateMachineContext> enterAction, Action<StateMachineContext> exitAction) {
				_state = state;
				_enterAction = enterAction;
				_exitAction = exitAction;
			}

			internal void FillCommands(IEnumerable<CommandObject> commands) {
				Contract.Requires<ArgumentNullException>(commands != null);
				_commands.AddRange(commands);
			}

			public void Enter(StateMachineContext ctx) {
				if (_enterAction != null) {
					_enterAction(ctx);
				}
			}

			public void Exit(StateMachineContext ctx) {
				if (_exitAction != null) {
					_exitAction(ctx);
				}
			}

			public CommandObject FindCommand(TCommand command, StateMachineContext ctx) {
				return _commands.FirstOrDefault(
					cmd => cmd.Command.CompareTo(command) == 0 && cmd.CheckGuard(ctx));
			}

			public static implicit operator TState(StateObject stateObject) {
				Contract.Requires<ArgumentNullException>(stateObject != null);
				return stateObject.State;
			}

			public override string ToString() {
				return string.Format("{{{0}}}", _state);
			}

			[ContractInvariantMethod]
			private void ContractInvariants() {
				Contract.Invariant(_commands != null);
			}
		}
	}
}
