using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using GenericFSM.Exceptions;

namespace GenericFSM.Machines
{
	public class SimplePassiveStateMachine<TState, TCommand> : StateMachine<TState, TCommand>
		where TState : struct, IComparable, IConvertible, IFormattable
		where TCommand : struct, IComparable, IConvertible, IFormattable
	{
		private readonly StateObject _initialState;
		private TState? _previousState;

		public SimplePassiveStateMachine(StateObject initialState, IEnumerable<StateObject> states) {
			Contract.Requires(initialState != null);
			Contract.Requires(states != null);

			if (!states.Any(state => state.Equals(initialState))) {
				throw new InvalidOperationException("Initial state must be presented in states enumeration.");
			}
			_initialState = initialState;
		}

		protected override void DoStart() {
			EnterState(_initialState);
		}

		public override void TriggerCommand(TCommand command) {
		    var ctx = CreateContext(command);
            _currentState.Exit(ctx);
            var commandObject = _currentState.FindCommand(command, ctx);
			if (commandObject == null) {
				throw new CommandNotSupportedException();
			}
			EnterState(commandObject.TargetState, command);
		}

		[ContractInvariantMethod]
		private void ContractInvariants() {
			Contract.Invariant(_initialState != null);
		}

		private StateMachineContext CreateContext(TCommand? command) {
			return new StateMachineContext(command, _currentState, _previousState, _executionData);
		}

		private void EnterState(StateObject state, TCommand? command = null) {
			Contract.Requires<ArgumentNullException>(state != null);

			if (_currentState != null) {
				_previousState = _currentState;
			}
			_currentState = state;
			_currentState.Enter(CreateContext(command));
		}
	}
}
