using GenericFSM.Tests.Infrastructure;
using Xunit;
using Xunit.Extensions;

namespace GenericFSM.Tests.Unit
{
	public class CommandObjectTests
	{
		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void ImplicitConversionToCommand_WillReturnCommandValue(TrafficLightCommand command) {
			var commandObject = new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
				command, 
				null,
				new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
					TrafficLightState.Green, 
					null, 
					null));

			Assert.Equal(command, commandObject);
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset, TrafficLightState.Green)]
		[InlineData(TrafficLightCommand.Reset, TrafficLightState.Red)]
		[InlineData(TrafficLightCommand.Reset, TrafficLightState.Yellow)]
		[InlineData(TrafficLightCommand.SwitchNext, TrafficLightState.Green)]
		[InlineData(TrafficLightCommand.SwitchNext, TrafficLightState.Red)]
		[InlineData(TrafficLightCommand.SwitchNext, TrafficLightState.Yellow)]
		public void ToString_WillReturnCorrectStringResult(TrafficLightCommand command, TrafficLightState state) {
			var commandObject = new StateMachine<TrafficLightState, TrafficLightCommand>.CommandObject(
				command,
				null,
				new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
					state,
					null,
					null));

			var expected = string.Format("{{Command: {0}, State: {1}}}", command, state);
			Assert.Equal(expected, commandObject.ToString());
		}
	}
}
