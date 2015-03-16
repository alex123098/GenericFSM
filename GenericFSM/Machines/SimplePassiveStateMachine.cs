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
			_currentState.Exit();
			var commandObject = _initialState.FindCommand(command);
			if (commandObject == null) {
				throw new CommandNotSupportedException();
			}
			EnterState(commandObject.TargetState);
		}

		[ContractInvariantMethod]
		private void ContractInvariants() {
			Contract.Invariant(_initialState != null);
		}

		private void EnterState(StateObject state) {
			Contract.Requires<ArgumentNullException>(state != null);

			_currentState = state;
			_currentState.Enter();
		}
	}
}
