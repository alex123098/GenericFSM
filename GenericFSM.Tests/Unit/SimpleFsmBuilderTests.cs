using GenericFSM.Exceptions;
using GenericFSM.Machines;
using GenericFSM.Tests.Infrastructure;
using Xunit;

namespace GenericFSM.Tests.Unit
{
	public class SimpleFsmBuilderTests
	{
		[Fact]
		public void UnableToCreateStateMachine_WithoutConfiguration() {
			var fsmBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();
			
			Assert.Throws<InvalidFsmConfigurationException>(() => fsmBuilder.CreateStateMachine());
		}

		[Fact]
		public void Ctor_WillThrowIfStateTypeIsNotEnum() {
			Assert.Throws<InvalidStateTypeException>(() => new SimpleFsmBuilder<int, TrafficLightCommand>());
		}

		[Fact]
		public void Ctor_WillThrowIfCommandTypeIsNotEnum() {
			Assert.Throws<InvalidCommandTypeException>(() => new SimpleFsmBuilder<TrafficLightState, int>());
		}
		
		[Fact]
		public void FromState_WillReturnStateConfiguration() {
			var fsmBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();

			var stateConfiguration = fsmBuilder.FromState(TrafficLightState.Green);

			Assert.NotNull(stateConfiguration);
		}

		[Fact]
		public void WillThrowOnAttemptToSetInitialStateTwice() {
			var fsmBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();

			fsmBuilder.FromState(TrafficLightState.Green).AsInitialState();
			
			Assert.Throws<InvalidFsmConfigurationException>(() => fsmBuilder.FromState(TrafficLightState.Red).AsInitialState());
		}

		[Fact]
		public void FromState_WillReturnOneConfigurationForEachPossibleState() {
			var fsmBuilder = new SimpleFsmBuilder<TrafficLightState, TrafficLightCommand>();

			var stateConfig1 = fsmBuilder.FromState(TrafficLightState.Green);
			var stateConfig2 = fsmBuilder.FromState(TrafficLightState.Green);
			var stateConfigRed = fsmBuilder.FromState(TrafficLightState.Red);

			Assert.Same(stateConfig1, stateConfig2);
			Assert.NotSame(stateConfig1, stateConfigRed);
		}
	}
}
