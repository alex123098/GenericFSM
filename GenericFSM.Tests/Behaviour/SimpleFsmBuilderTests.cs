using GenericFSM.Exceptions;
using GenericFSM.Machines;
using GenericFSM.Tests.Infrastructure;
using Moq;
using Xunit;

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
	}
}
