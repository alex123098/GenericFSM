using System;
using Moq;

namespace GenericFSM.Tests.Infrastructure
{
	internal class TestStateConfigurationBuilder<TState, TCommand>
		where TState : struct, IComparable, IConvertible, IFormattable
		where TCommand : struct, IComparable, IConvertible, IFormattable
	{
		private FsmBuilder<TState, TCommand> _fsmBuilder;
		private TState _state;

		internal TestStateConfigurationBuilder() {
			_fsmBuilder = Mock.Of<FsmBuilder<TState, TCommand>>();
		}

		internal TestStateConfigurationBuilder<TState, TCommand> WithFsmBuilder(FsmBuilder<TState, TCommand> fsmBuilder) {
			_fsmBuilder = fsmBuilder;
			return this;
		}

		internal TestStateConfigurationBuilder<TState, TCommand> ForState(TState state) {
			_state = state;
			return this;
		}

		internal FsmBuilder<TState, TCommand>.StateConfiguration Build() {
			return new FsmBuilder<TState, TCommand>.StateConfiguration(_fsmBuilder, _state);
		}
	}
}
