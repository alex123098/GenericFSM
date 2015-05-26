using System;
using GenericFSM.Exceptions;
using GenericFSM.Machines;
using GenericFSM.Tests.Infrastructure;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace GenericFSM.Tests.Behaviour
{
	public class SimpleFsmBuilderTests
	{
		[Fact]
		public void CreateStateMachine_WillCreateStateMachineForSimpleConfiguration() {
			var builder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();

			builder.FromState(TrafficLightState.Green).AsInitialState();

			var stateMachine = builder.CreateStateMachine();

			Assert.NotNull(stateMachine);
		}

		[Fact]
		public void CreateStateMachine_WillThrowIfInitialStateIsNotConfigured() {
			var builder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();

			builder.FromState(TrafficLightState.Green);

			Assert.Throws<InvalidFsmConfigurationException>(() => builder.CreateStateMachine());
		}

		[Fact]
		public void CreateStateMachine_WithArgumentEqualToTrue_CreatesStartedStateMachine() {
			var builder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			builder.FromState(TrafficLightState.Green).AsInitialState();

			var stateMachine = builder.CreateStateMachine(createStarted: true);

			Assert.NotNull(stateMachine);
		}

		[Fact]
		public void CreateStateMachine_WithStartedFlag_WillInvokeStartMethod() {
			var stateMachineMock = Mock.Of<StateMachine<TrafficLightState, TrafficLightCommand>>();
			var builder = Mock.Of<FsmBuilder<TrafficLightState, TrafficLightCommand>>(
				b => b.CreateStateMachine() == stateMachineMock &&
				// ReSharper disable once PossibleUnintendedReferenceComparison
				b.FromState(TrafficLightState.Green) == new FsmBuilder<TrafficLightState, TrafficLightCommand>.StateConfiguration(b, TrafficLightState.Green));
			Mock.Get(builder).Setup(b => b.CreateStateMachine(It.IsAny<bool>())).CallBase();
			
			builder.FromState(TrafficLightState.Green).AsInitialState();
			var stateMachine = builder.CreateStateMachine(createStarted: true);
			
			Assert.NotNull(stateMachine);
			Mock.Get(stateMachine).Verify(sm => sm.Start());
		}

		[Theory]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext, TrafficLightState.Yellow)]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext, TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow, TrafficLightCommand.SwitchNext, TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow, TrafficLightCommand.Reset, TrafficLightState.Green)]
		public void StateMachineConfiguredForTwoStatesWillLeadToFinalState(
			TrafficLightState initialState, 
			TrafficLightCommand command, 
			TrafficLightState finalState) {

			var stateMachineBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			stateMachineBuilder
				.FromState(initialState)
				.AsInitialState()
				.OnCommand(command)
				.SetState(finalState);

			var stateMachine = stateMachineBuilder.CreateStateMachine(true);
			Assert.Equal(initialState, stateMachine.CurrentState);
			stateMachine.TriggerCommand(command);

			Assert.Equal(finalState, stateMachine.CurrentState);
		}

		[Theory]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext, TrafficLightState.Red, TrafficLightState.Yellow)]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext, TrafficLightState.Yellow, TrafficLightState.Red)]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.Reset, TrafficLightState.Red, TrafficLightState.Yellow)]
		[InlineData(TrafficLightState.Yellow, TrafficLightCommand.SwitchNext, TrafficLightState.Green, TrafficLightState.Red)]
		[InlineData(TrafficLightState.Red, TrafficLightCommand.Reset, TrafficLightState.Red, TrafficLightState.Green)]
		public void StateMachineConfiguredForTwoStatesWithGuardedCommandsWillSelectCorrectFinalState(
			TrafficLightState initialState, 
			TrafficLightCommand command,
			TrafficLightState stateToSkip,
			TrafficLightState stateToSet) {

			var stateMachineBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			stateMachineBuilder
				.FromState(initialState)
				.AsInitialState()
				.OnCommand(command, ctx => false)
				.SetState(stateToSkip)
				.OnCommand(command, ctx => true)
				.SetState(stateToSet);
			var stateMachine = stateMachineBuilder.CreateStateMachine(true);

			Assert.Equal(initialState, stateMachine.CurrentState);
			stateMachine.TriggerCommand(command);

			Assert.Equal(stateToSet, stateMachine.CurrentState);
		}

		[Fact]
		public void StateMachineConfiguredForThreeStatesWithTwoCommands() {
			var stateMachineBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			stateMachineBuilder
				.FromState(TrafficLightState.Green)
				.AsInitialState()
				.OnCommand(TrafficLightCommand.SwitchNext)
				.SetState(TrafficLightState.Yellow)
				.OnCommand(TrafficLightCommand.Reset)
				.SetState(TrafficLightState.Green);
			stateMachineBuilder
				.FromState(TrafficLightState.Yellow)
				.OnCommand(TrafficLightCommand.SwitchNext, ctx => ctx.PreviousState == TrafficLightState.Green)
				.SetState(TrafficLightState.Red)
				.OnCommand(TrafficLightCommand.SwitchNext, ctx => ctx.PreviousState == TrafficLightState.Red)
				.SetState(TrafficLightState.Green)
				.OnCommand(TrafficLightCommand.Reset)
				.SetState(TrafficLightState.Green);
			stateMachineBuilder
				.FromState(TrafficLightState.Red)
				.OnCommand(TrafficLightCommand.SwitchNext)
				.SetState(TrafficLightState.Yellow)
				.OnCommand(TrafficLightCommand.Reset)
				.SetState(TrafficLightState.Green);

			var stateMachine = stateMachineBuilder.CreateStateMachine(true);

			Assert.Equal(TrafficLightState.Green, stateMachine.CurrentState);
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);
			Assert.Equal(TrafficLightState.Yellow, stateMachine.CurrentState);
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);
			Assert.Equal(TrafficLightState.Red, stateMachine.CurrentState);
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);
			Assert.Equal(TrafficLightState.Yellow, stateMachine.CurrentState);
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);
			Assert.Equal(TrafficLightState.Green, stateMachine.CurrentState);
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);
			stateMachine.TriggerCommand(TrafficLightCommand.Reset);
			Assert.Equal(TrafficLightState.Green, stateMachine.CurrentState);
		}

		[Fact]
		public void UserWillBeAbleToProvideTheExecutionData() {
			const string data = "sample data";
			Action<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext> action =
				ctx => {
					var stringData = Assert.IsAssignableFrom<string>(ctx.Data);
					Assert.Equal(data, stringData);
					
				};
			Func<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext, bool> guard =
				ctx => {
					var stringData = Assert.IsAssignableFrom<string>(ctx.Data);
					Assert.Equal(data, stringData);
					return true;
				};
			var stateMachineBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			stateMachineBuilder
				.FromState(TrafficLightState.Green)
				.AsInitialState()
				.WithEnteringAction(action)
				.WithExitingAction(action)
				.OnCommand(TrafficLightCommand.SwitchNext, guard)
				.SetState(TrafficLightState.Yellow);

			var stateMachine = stateMachineBuilder.CreateStateMachine();
			stateMachine.SetData(data);
			stateMachine.Start();
			stateMachine.TriggerCommand(TrafficLightCommand.SwitchNext);
		}
	}
}
