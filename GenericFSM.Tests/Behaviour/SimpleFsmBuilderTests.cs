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
		public void CreateStateMachine_WillCanCreateAlreadyStartedStateMachine() {
			var builder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			builder.FromState(TrafficLightState.Green).AsInitialState();

			var stateMachine = builder.CreateStateMachine(createStarted: true);

			Assert.NotNull(stateMachine);
		}

		[Fact]
		public void CreateStateMachine_WithStartedFlagWillInvokeStartMethod() {
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
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext, TrafficLightState.Red, TrafficLightState.Yellow)]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext, TrafficLightState.Red, TrafficLightState.Yellow)]
		public void StateMachineConfiguredForTwoStatesWithGuardedCommandsWillSelectCorrectFinalState(
			TrafficLightState initialState, 
			TrafficLightCommand command,
			TrafficLightState stateToSkip,
			TrafficLightState stateToSet) {

			var stateMachineBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			stateMachineBuilder
				.FromState(initialState)
				.AsInitialState()
				.OnCommand(command, () => false)
				.SetState(stateToSkip)
				.OnCommand(command, () => true)
				.SetState(stateToSet);
			var stateMachine = stateMachineBuilder.CreateStateMachine(true);

			Assert.Equal(initialState, stateMachine.CurrentState);
			stateMachine.TriggerCommand(command);

			Assert.Equal(stateToSet, stateMachine.CurrentState);
		}
	}
}
