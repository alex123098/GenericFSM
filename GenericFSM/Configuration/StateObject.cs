using System;
using System.Collections.Generic;

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
			public IEnumerable<CommandObject> Commands { get { return _commands; } }

			internal StateObject(TState state, Action enterAction, Action exitAction) {
				_state = state;
				_enterAction = enterAction;
				_exitAction = exitAction;
			}

			internal void FillCommands(IEnumerable<CommandObject> commands) {
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
		}
	}
}
