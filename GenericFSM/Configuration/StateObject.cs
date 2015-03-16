using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;

namespace GenericFSM
{
	public partial class StateMachine<TState, TCommand>
	{
		public class StateObject
		{
			private readonly TState _state;
			private readonly Action _enterAction;
			private readonly Action _exitAction;
			private readonly List<CommandObject> _commands = new List<CommandObject>(); 

			public TState State { get { return _state; } }

			public IEnumerable<CommandObject> Commands {
				get {
					Contract.Ensures(Contract.Result<IEnumerable<CommandObject>>() != null);
					return _commands;
				}
			}

			internal StateObject(TState state, Action enterAction, Action exitAction) {
				_state = state;
				_enterAction = enterAction;
				_exitAction = exitAction;
			}

			internal void FillCommands(IEnumerable<CommandObject> commands) {
				Contract.Requires<ArgumentNullException>(commands != null);
				_commands.AddRange(commands);
			}

			public void Enter() {
				if (_enterAction != null) {
					_enterAction();
				}
			}

			public void Exit() {
				if (_exitAction != null) {
					_exitAction();
				}
			}

			public CommandObject FindCommand(TCommand command) {
				return _commands.FirstOrDefault(cmd => cmd.Command.CompareTo(command) == 0 && cmd.CheckGuard());
			}

			public static implicit operator TState(StateObject stateObject) {
				Contract.Requires<ArgumentNullException>(stateObject != null);
				return stateObject.State;
			}

			[ContractInvariantMethod]
			private void ContractInvariants() {
				Contract.Invariant(_commands != null);
			}
		}
	}
}
