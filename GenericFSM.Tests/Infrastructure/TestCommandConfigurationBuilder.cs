using System;

namespace GenericFSM.Tests.Infrastructure
{
	internal class TestCommandConfigurationBuilder<TState, TCommand>
		where TState : struct, IComparable, IConvertible, IFormattable
		where TCommand : struct, IComparable, IConvertible, IFormattable
	{
		private readonly TestStateConfigurationBuilder<TState, TCommand> _stateConfigurationBuilder = new TestStateConfigurationBuilder<TState, TCommand>();
		private Func<StateMachine<TState, TCommand>.StateMachineContext, bool> _guardCondition;
		private TCommand _command;

		internal TestStateConfigurationBuilder<TState, TCommand> State { get { return _stateConfigurationBuilder; } }

		internal TestCommandConfigurationBuilder<TState, TCommand> WithGuard(Func<StateMachine<TState, TCommand>.StateMachineContext, bool> guard) {
			_guardCondition = guard;
			return this;
		}

		internal TestCommandConfigurationBuilder<TState, TCommand> WithCommand(TCommand command) {
			_command = command;
			return this;
		}

		internal FsmBuilder<TState, TCommand>.CommandConfiguration Build() {
			var stateConfig = _stateConfigurationBuilder.Build();
			if (_guardCondition != null) {
				return new FsmBuilder<TState, TCommand>.CommandConfiguration(stateConfig, _command, _guardCondition);
			}
			return new FsmBuilder<TState, TCommand>.CommandConfiguration(stateConfig, _command);
		}
	}
}
