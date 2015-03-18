using System;
using System.Diagnostics.Contracts;
using GenericFSM.Exceptions;

namespace GenericFSM
{
	[ContractClass(typeof(FsmBuilderContract<,>))]
	public abstract partial class FsmBuilder<TState, TCommand>
		where TState : struct, IComparable, IConvertible, IFormattable
		where TCommand : struct, IComparable, IConvertible, IFormattable
	{
		protected StateConfiguration _initialStateConfiguration;

		protected FsmBuilder() {
			if (!typeof(TState).IsEnum) {
				throw new InvalidStateTypeException();
			}
			if (!typeof(TCommand).IsEnum) {
				throw new InvalidCommandTypeException();
			}
		} 

		public abstract StateMachine<TState, TCommand> CreateStateMachine();

		public virtual StateMachine<TState, TCommand> CreateStateMachine(bool createStarted) {
			Contract.Ensures(Contract.Result<StateMachine<TState, TCommand>>() != null);
			var machine = CreateStateMachine();
			if (createStarted) {
				machine.Start();
			}
			return machine;
		}

		public abstract StateConfiguration FromState(TState state);

		protected virtual void SetInitialStateConfiguration(StateConfiguration configuration) {
			Contract.Requires<ArgumentNullException>(configuration != null);
			if (_initialStateConfiguration != null) {
				throw new InvalidFsmConfigurationException("Initial state has already been set.");
			}

			_initialStateConfiguration = configuration;
		}
	}

	[ContractClassFor(typeof(FsmBuilder<,>))]
	internal abstract class FsmBuilderContract<T1,T2> : FsmBuilder<T1,T2>
		where T1 : struct, IComparable, IConvertible, IFormattable
		where T2 : struct, IComparable, IConvertible, IFormattable
	{
		public override StateMachine<T1, T2> CreateStateMachine() {
			Contract.Ensures(Contract.Result<StateMachine<T1, T2>>() != null);
			throw new NotImplementedException();
		}

		public override StateConfiguration FromState(T1 state) {
			Contract.Ensures(Contract.Result<StateConfiguration>() != null);
			throw new NotImplementedException();
		}
	}
}
