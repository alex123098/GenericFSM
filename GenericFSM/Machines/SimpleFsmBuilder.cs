using System.Collections.Generic;
using System.Diagnostics.Contracts;
using GenericFSM.Exceptions;

namespace GenericFSM.Machines
{
	public class SimpleFsmBuilder<TState> : FsmBuilder<TState> where TState: struct
	{
		private readonly Dictionary<TState, StateConfiguration> _stateConfigurations = new Dictionary<TState, StateConfiguration>(); 

		public override void CreateStateMachine() {
			throw new InvalidFsmConfigurationException();
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
