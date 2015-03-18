namespace GenericFSM
{
	public partial class StateMachine<TState, TCommand>
	{
		public struct StateMachineContext
		{
			private readonly TCommand? _command;
			private readonly TState? _currentState;
			private readonly object _data;
			private readonly TState? _previousState;

			public TCommand? CurrentCommand {
				get { return _command; }
			}

			public TState? CurrentState {
				get { return _currentState; }
			}

			public TState? PreviousState {
				get { return _previousState; }
			}

			public object Data { get { return _data; } }

			internal StateMachineContext(TCommand? command, TState? currentState, TState? previousState, object data) {
				_command = command;
				_currentState = currentState;
				_data = data;
				_previousState = previousState;
			}
		}
	}
}
