using System;
using System.Diagnostics.Contracts;

namespace GenericFSM
{
	public partial class FsmBuilder<TState>
	{
		public sealed class StateConfiguration
		{
			private readonly FsmBuilder<TState> _fsmBuilder;
			private readonly TState _state;

			public StateConfiguration(FsmBuilder<TState> fsmBuilder, TState state) {
				Contract.Requires<ArgumentNullException>(fsmBuilder != null);
				_fsmBuilder = fsmBuilder;
				_state = state;
			}

			[ContractInvariantMethod]
			private void ContractInvariants() {
				Contract.Invariant(_fsmBuilder != null);
			}

			public StateConfiguration AsInitialState() {
				Contract.Ensures(Contract.Result<StateConfiguration>() != null);
				_fsmBuilder.SetInitialStateConfiguration(this);
				return this;
			}
		}
	}
}
