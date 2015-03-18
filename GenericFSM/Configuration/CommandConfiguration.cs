using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace GenericFSM
{
	public partial class FsmBuilder<TState, TCommand>
	{
		public sealed class CommandConfiguration
		{
			#region Fields

			private readonly TCommand _command;
			private readonly Func<StateMachine<TState, TCommand>.StateMachineContext, bool> _guardCondition;
			private readonly StateConfiguration _fromStateConfiguration;
			private StateConfiguration _targetStateConfiguration;
			private StateMachine<TState, TCommand>.CommandObject _cachedCommandObject;

			#endregion
			
			#region Constructor

			public CommandConfiguration(StateConfiguration fromStateConfiguration, TCommand command) {
				Contract.Requires<ArgumentException>(fromStateConfiguration != null);

				_fromStateConfiguration = fromStateConfiguration;
				_command = command;
			}

			public CommandConfiguration(
				StateConfiguration fromStateConfiguration,
				TCommand command, Func<StateMachine<TState, TCommand>.StateMachineContext, bool> guardCondition)

				: this(fromStateConfiguration, command) {
				Contract.Requires<ArgumentException>(fromStateConfiguration != null);
				Contract.Requires<ArgumentNullException>(guardCondition != null);

				_guardCondition = guardCondition;
			}

			#endregion

			#region Internal Members

			internal TCommand Command {
				get { return _command; }
			}

			internal Func<StateMachine<TState, TCommand>.StateMachineContext, bool> GuardCondition {
				[Pure]
				get { return _guardCondition; }
			}

			internal StateMachine<TState, TCommand>.CommandObject CreateCommandObject() {
				Contract.Assume(_targetStateConfiguration != null);
				return _cachedCommandObject ?? 
					  (_cachedCommandObject = new StateMachine<TState, TCommand>.CommandObject(
						_command,
						_guardCondition,
						_targetStateConfiguration.CreateState()));
			}

			#endregion

			#region Interface

			public StateConfiguration SetState(TState state) {
				Contract.Ensures(Contract.Result<StateConfiguration>() != null);

				_targetStateConfiguration = _fromStateConfiguration.GetFsmBuilder().FromState(state);
				return _fromStateConfiguration;
			}

			#endregion

			#region Private Methods

			[ContractInvariantMethod]
			private void ContractInvariants() {
				Contract.Invariant(_fromStateConfiguration != null);
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
