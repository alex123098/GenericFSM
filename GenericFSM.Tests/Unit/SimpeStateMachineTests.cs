using System;
using System.Linq;
using GenericFSM.Exceptions;
using GenericFSM.Machines;
using GenericFSM.Tests.Infrastructure;
using Xunit;
using Xunit.Extensions;

namespace GenericFSM.Tests.Unit
{
	public class SimpeStateMachineTests
	{
		[Fact]
		public void Ctor_WillAcceptInitialStateAndStateEnumeration() {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				null);
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				initialState.MakeEnumerable());
		}

		[Fact]
		public void Ctor_WillThrowIfInitialStateDoesNotExistInStateEnumeration() {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				null);
			Assert.Throws<InvalidOperationException>(() =>
				new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
					initialState,
					Enumerable.Empty<StateMachine<TrafficLightState, TrafficLightCommand>.StateObject>()));
		}

		[Fact]
		public void Start_WillInvokeEnterActionOnInitialState() {
			var enteringInvoked = false;
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				ctx => { enteringInvoked = true; },
				null);
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				initialState.MakeEnumerable());

			stateMachine.Start();

			Assert.True(enteringInvoked, "InitialState.Enter() must be invoked.");
		}

		[Theory]
		[InlineData(TrafficLightState.Green)]
		[InlineData(TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow)]
		public void Start_WillSetCurrentStateToInitial(TrafficLightState state) {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				state,
				null,
				null);
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				initialState.MakeEnumerable());

			Assert.Throws<InvalidOperationException>(() => stateMachine.CurrentState);
			stateMachine.Start();

			Assert.Equal(state, stateMachine.CurrentState);
		}

		[Fact]
		public void Start_WillFailIfInvokedTwice() {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				null);
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				initialState.MakeEnumerable());

			stateMachine.Start();

			Assert.Throws<InvalidOperationException>(() => stateMachine.Start());
		}

		[Fact]
		public void TriggerCommand_WillCheckCommandsGuardCondition() {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				null);
			var finalState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Yellow,
				null,
				null);
			var guardInvoked = false;
			Func<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext, bool> guardCondition = ctx => {
				guardInvoked = true;
				return true;
			};
			initialState.FillCommands(
				new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
					TrafficLightCommand.SwitchNext,
					guardCondition,
					finalState).MakeEnumerable());
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				new[] { initialState, finalState });

			stateMachine.Start();
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);

			Assert.True(guardInvoked, "Command's guard condition should be evaluated on transition");
		}

		[Fact]
		public void TriggerCommand_WillThrowIfCommandIsNotSupportedInCurrentState() {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null, null);
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				initialState.MakeEnumerable());

			stateMachine.Start();

			Assert.Throws<CommandNotSupportedException>(() => stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext));
		}

		[Fact]
		public void TriggerCommand_WillInvokeExitingActionForCurrentState() {
			var exitingInvoked = false;
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				ctx => { exitingInvoked = true; });
			var finalState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Yellow,
				null,
				null);
			initialState.FillCommands(
				new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
					TrafficLightCommand.SwitchNext,
					null,
					finalState).MakeEnumerable());
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				new[] { initialState, finalState });

			stateMachine.Start();
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);

			Assert.True(exitingInvoked, "Exiting action should be invoked.");
		}

		[Fact]
		public void TriggerCommand_GuardConditionShouldInvokeAfterExitingFromCurrentState() {
			var guardInvoked = false;
			Func<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext, bool> guardCondition = ctx => guardInvoked = true;
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				ctx => { throw new Exception(); });
			var finalState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Yellow,
				null,
				null);
			initialState.FillCommands(
				new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
					TrafficLightCommand.SwitchNext,
					guardCondition,
					finalState).MakeEnumerable());
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				new[] { initialState, finalState });

			stateMachine.Start();
			Assert.Throws<Exception>(() => stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext));

			Assert.False(guardInvoked, "Guard condition should be invoked after exiting action");
		}

		[Theory]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.Reset, TrafficLightState.Red)]
		[InlineData(TrafficLightState.Red, TrafficLightCommand.Reset, TrafficLightState.Yellow)]
		[InlineData(TrafficLightState.Yellow, TrafficLightCommand.SwitchNext, TrafficLightState.Red)]
		[InlineData(TrafficLightState.Red, TrafficLightCommand.SwitchNext, TrafficLightState.Green)]
		public void TriggerCommand_ShouldSetNewStateIfSuccess(
			TrafficLightState startState, 
			TrafficLightCommand command, 
			TrafficLightState finalState) {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				startState,
				null,
				null);
			var endState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				finalState,
				null,
				null);
			initialState.FillCommands(
				new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
					command,
					null,
					endState).MakeEnumerable());
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				new[] { initialState, endState });
			
			stateMachine.Start();
			Assert.Equal(startState, stateMachine.CurrentState);
			stateMachine.TriggerCommand(command);

			Assert.Equal(finalState, stateMachine.CurrentState);
		}

		[Fact]
		public void TriggerCommand_WillFailIfGuardConditionForCommandEvaluatesToFalse() {
			var initialState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Green,
				null,
				null);
			var endState = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				TrafficLightState.Red, 
				null,
				null);
			initialState.FillCommands(
				new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
					TrafficLightCommand.SwitchNext, 
					ctx => false,
					endState).MakeEnumerable());
			var stateMachine = new SimplePassiveStateMachine<TrafficLightState, TrafficLightCommand>(
				initialState,
				new[] { initialState, endState });

			stateMachine.Start();
			
			Assert.Throws<CommandNotSupportedException>(() => stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext));
		}
	}
}
