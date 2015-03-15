﻿using GenericFSM.Tests.Infrastructure;
using Xunit;
using Xunit.Extensions;

namespace GenericFSM.Tests.Unit
{
	public class StateObjectTests
	{
		[Theory]
		[InlineData(TrafficLightState.Green)]
		[InlineData(TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow)]
		public void ImplicitConversionToState_WillReturnCorrectState(TrafficLightState state) {
			var stateObject = new StateMachine<TrafficLightState, TrafficLightCommand>.StateObject(
				state,
				null,
				null);

			Assert.Equal(state, stateObject);
		}
	}
}
