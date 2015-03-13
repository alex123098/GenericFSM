using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using GenericFSM.Exceptions;

namespace GenericFSM
{
	public partial class FsmBuilder<TState, TCommand>
	{
		public sealed class StateConfiguration
		{
			#region Fields

			private readonly FsmBuilder<TState, TCommand> _fsmBuilder;
			private readonly TState _state;
			private Action _enteringAction;
			private Action _exitingAction;

			private readonly Dictionary<int, CommandConfiguration> _commandConfigurations =
				new Dictionary<int, CommandConfiguration>();

			#endregion

			#region Contructor

			public StateConfiguration(FsmBuilder<TState, TCommand> fsmBuilder, TState state) {
				Contract.Requires<ArgumentNullException>(fsmBuilder != null);

				_fsmBuilder = fsmBuilder;
				_state = state;
			}

			#endregion

			#region Internal Members

			internal TState State {
				get { return _state; }
			}

			[Pure]
			internal Action GetEnteringAction() {
				return _enteringAction;
			}

			[Pure]
			internal Action GetExitingAction() {
				return _exitingAction;
			}

			[Pure]
			internal FsmBuilder<TState, TCommand> GetFsmBuilder() {
				Contract.Ensures(Contract.Result<FsmBuilder<TState, TCommand>>() != null);

				return _fsmBuilder;
			}

			[Pure]
			internal StateMachine<TState, TCommand>.StateObject CreateState() {
				Contract.Ensures(Contract.Result<StateMachine<TState, TCommand>.StateObject>() != null);

				var stateObject = new StateMachine<TState, TCommand>.StateObject(_state, _enteringAction, _exitingAction);
				stateObject.FillCommands(_commandConfigurations.Select(command => command.Value.CreateCommandObject()));
				return stateObject;
			}

			#endregion

			#region Interface

			public StateConfiguration AsInitialState() {
				Contract.Ensures(Contract.Result<StateConfiguration>() != null);

				_fsmBuilder.SetInitialStateConfiguration(this);
				return this;
			}

			public StateConfiguration WithExitingAction(Action exitingAction) {
				Contract.Requires<ArgumentNullException>(exitingAction != null);
				Contract.Ensures(Contract.Result<StateConfiguration>() != null);

				_exitingAction = exitingAction;
				return this;
			}

			public StateConfiguration WithEnteringAction(Action enteringAction) {
				Contract.Requires<ArgumentNullException>(enteringAction != null);
				Contract.Ensures(Contract.Result<StateConfiguration>() != null);

				_enteringAction = enteringAction;
				return this;
			}

			public CommandConfiguration OnCommand(TCommand command) {
				Contract.Ensures(Contract.Result<CommandConfiguration>() != null);

				var commandKey = CalculateCommandKey(command);
				if (_commandConfigurations.ContainsKey(commandKey)) {
					throw new CommandRegistrationException();
				}
				var commandConfig = new CommandConfiguration(this, command);
				_commandConfigurations.Add(commandKey, commandConfig);
				return commandConfig;
			}

			public CommandConfiguration OnCommand(TCommand command, Func<bool> guardCondition) {
				Contract.Requires<ArgumentNullException>(guardCondition != null);
				Contract.Ensures(Contract.Result<CommandConfiguration>() != null);

				var commandKeyWithoutGuard = CalculateCommandKey(command);
				var commandKeyWithGuard = CalculateCommandKey(command, guardCondition);
				CommandConfiguration commandConfig;
				if (_commandConfigurations.TryGetValue(commandKeyWithoutGuard, out commandConfig) &&
				    commandConfig.GuardCondition == null) {
					throw new CommandRegistrationException();
				}
				if (_commandConfigurations.ContainsKey(commandKeyWithGuard)) {
					throw new CommandRegistrationException(
						"The specified command with the specified guard has already been registered for this state.");
				}

				commandConfig = new CommandConfiguration(this, command, guardCondition);
				_commandConfigurations.Add(commandKeyWithGuard, commandConfig);
				return commandConfig;
			}

			#endregion

			#region Private methods

			private int CalculateCommandKey(TCommand command, Func<bool> guard = null) {
				return Command.GetHashCode(command, guard);
			}

			[ContractInvariantMethod]
			private void ContractInvariants() {
				Contract.Invariant(_fsmBuilder != null);
				Contract.Invariant(_commandConfigurations != null);
			}

			#endregion

			#region Cosmetic

			// hide object methods from Intellisence

			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj) {
				// ReSharper disable once BaseObjectEqualsIsObjectEquals
				return base.Equals(obj);
			}

			// ReSharper disable once NonReadonlyMemberInGetHashCode
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode() {
				// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
				return base.GetHashCode();
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString() {
				return base.ToString();
			}

			#endregion

			
		}
	}
}
