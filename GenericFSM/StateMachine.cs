using System;
using System.Diagnostics.Contracts;

namespace GenericFSM
{
	public abstract partial class StateMachine<TState, TCommand>
		where TState : struct, IComparable, IConvertible, IFormattable
		where TCommand : struct, IComparable, IConvertible, IFormattable
	{
		protected StateObject _currentState;
		protected bool _started;

		public virtual void Start() {
			if (_started) {
				throw new InvalidOperationException("State machine has already started.");
			}
			_started = true;
			DoStart();
		}

		public abstract void TriggerCommand(TCommand command);

		[Pure]
		public TState CurrentState {
			get {
				if (!_started) {
					throw new InvalidOperationException("State machine is not started");
				}
				return _currentState;
			}
		}

		protected abstract void DoStart();
	}
}
