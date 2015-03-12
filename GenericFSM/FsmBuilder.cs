using System;
using System.Diagnostics.Contracts;
using GenericFSM.Exceptions;

namespace GenericFSM
{
	[ContractClass(typeof(FsmBuilderContract<>))]
	public abstract partial class FsmBuilder<TState> where TState : struct
	{
		private StateConfiguration _initialConfiguration;

		protected FsmBuilder() {
			if (!typeof(TState).IsEnum) {
				throw new InvalidStateTypeException();
			}
		} 

		public abstract void CreateStateMachine();

		public abstract StateConfiguration FromState(TState state);

		protected virtual void SetInitialStateConfiguration(StateConfiguration configuration) {
			Contract.Requires<ArgumentNullException>(configuration != null);
			if (_initialConfiguration != null) {
				throw new InvalidFsmConfigurationException("Initial state has already been set.");
			}

			_initialConfiguration = configuration;
		}
	}

	[ContractClassFor(typeof(FsmBuilder<>))]
	public abstract class FsmBuilderContract<T> : FsmBuilder<T> where T:struct 
	{
		public override void CreateStateMachine() {
			throw new NotImplementedException();
		}

		public override StateConfiguration FromState(T state) {
			Contract.Ensures(Contract.Result<StateConfiguration>() != null);
			throw new NotImplementedException();
		}
	}
}
