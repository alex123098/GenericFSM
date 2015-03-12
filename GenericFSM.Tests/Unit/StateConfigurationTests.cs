using GenericFSM.Tests.Infrastructure;
using Moq;
using Xunit;

namespace GenericFSM.Tests.Unit
{
	public class StateConfigurationTests
	{
		[Fact]
		public void Ctor_WillAcceptFsmBuilder() {
			var fsmBuilder = Mock.Of<FsmBuilder<TrafficLightState>>();
			var stateConfiguration = new FsmBuilder<TrafficLightState>.StateConfiguration(fsmBuilder, TrafficLightState.Green);
		}

		[Fact]
		public void AsInitialState_WillNotReturnNull() {
			var fsmBuilder = Mock.Of<FsmBuilder<TrafficLightState>>();
			var stateConfiguration = new FsmBuilder<TrafficLightState>.StateConfiguration(fsmBuilder, TrafficLightState.Green);
			
			var otherStateVar = stateConfiguration.AsInitialState();

			Assert.NotNull(otherStateVar);
		}

		[Fact]
		public void AsInitialState_WillReturnSameInstance() {
			var fsmBuilder = Mock.Of<FsmBuilder<TrafficLightState>>();
			var stateConfiguration = new FsmBuilder<TrafficLightState>.StateConfiguration(fsmBuilder, TrafficLightState.Green);

			var otherStateVar = stateConfiguration.AsInitialState();

			Assert.Same(stateConfiguration, otherStateVar);
		}
	}
}
