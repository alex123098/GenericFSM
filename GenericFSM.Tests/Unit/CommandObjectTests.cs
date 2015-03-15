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
	}
}
