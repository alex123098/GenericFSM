using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using GenericFSM.Exceptions;

namespace GenericFSM.Machines
{
	public class SimpleFsmBuilder<TState, TCommand> : FsmBuilder<TState, TCommand>
		where TState : struct, IComparable, IConvertible, IFormattable
		where TCommand : struct, IComparable, IConvertible, IFormattable
	{
		private readonly Dictionary<TState, StateConfiguration> _stateConfigurations = new Dictionary<TState, StateConfiguration>();

		public override StateMachine<TState, TCommand> CreateStateMachine() {
			if (_stateConfigurations.Count == 0) {
				throw new InvalidFsmConfigurationException("There's no states configured.");
			}
			if (_initialStateConfiguration == null) {
				throw new InvalidFsmConfigurationException("Initial state was not configured.");
			}
			return new SimplePassiveStateMachine<TState, TCommand>(
				_initialStateConfiguration.CreateState(),
				_stateConfigurations.Values.Select(
					stateConfig => stateConfig.CreateState()));
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
