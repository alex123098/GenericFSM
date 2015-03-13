using System.Collections.Generic;
using System.Diagnostics.Contracts;
using GenericFSM.Exceptions;

namespace GenericFSM.Machines
{
	public class SimpleFsmBuilder<TState, TCommand> : FsmBuilder<TState, TCommand>
		where TState : struct
		where TCommand : struct
	{
		private readonly Dictionary<TState, StateConfiguration> _stateConfigurations = new Dictionary<TState, StateConfiguration>();

		public override StateMachine<TState, TCommand> CreateStateMachine() {
			if (_stateConfigurations.Count == 0) {
				throw new InvalidFsmConfigurationException("There's no states configured.");
			}
			if (_initialConfiguration == null) {
				throw new InvalidFsmConfigurationException("Initial state was not configured.");
			}
			return new SimplePassiveStateMachine<TState, TCommand>();
		}

		public override StateConfiguration FromState(TState state) {
			StateConfiguration config;
			if (!_stateConfigurations.TryGetValue(state, out config)) {
				config = new StateConfiguration(this, state);
				_stateConfigurations.Add(state, config);
			}
			return config;
		}

		[ContractInvariantMethod]
		private void ContractInvariants() {
			Contract.Invariant(_stateConfigurations != null);
		}
	}
}
