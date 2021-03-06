﻿using System;
using GenericFSM.Tests.Infrastructure;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace GenericFSM.Tests.Unit
{
	public class CommandConfigurationTests
	{
		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void Ctor_WillAcceptTheStateConfigurationAndCommand(TrafficLightCommand command) {
			var commandConfig = new TestCommandConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.WithCommand(command)
				.Build();

			Assert.Equal(command, commandConfig.Command);
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void Ctor_WillAcceptGuardCondition(TrafficLightCommand command) {
			Func<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext, bool> guardCondition = ctx => true;
			var commandConfig = new TestCommandConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.WithCommand(command)
				.WithGuard(guardCondition)
				.Build();

			Assert.Equal(command, commandConfig.Command);
			Assert.NotNull(commandConfig.GuardCondition);
			Assert.Same(guardCondition, commandConfig.GuardCondition);
		}

		[Theory]
		[InlineData(TrafficLightState.Green)]
		[InlineData(TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow)]
		public void SetState_WillReturnStateConfiguration(TrafficLightState state) {
			var commandConfigBuilder = new TestCommandConfigurationBuilder<TrafficLightState, TrafficLightCommand>();
			var fsmStub = Mock.Of<FsmBuilder<TrafficLightState, TrafficLightCommand>>();
			var finiteState = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(state)
				.WithFsmBuilder(fsmStub)
				.Build();
			Mock.Get(fsmStub)
				.Setup(builder => builder.FromState(state))
				.Returns(finiteState);
			commandConfigBuilder
				.State
				.WithFsmBuilder(fsmStub);

			var commandConfig = commandConfigBuilder.Build();

			var stateConfig = commandConfig.SetState(state);

			Assert.NotNull(stateConfig);
			Mock.Get(fsmStub).Verify();
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset, TrafficLightState.Green)]
		[InlineData(TrafficLightCommand.Reset, TrafficLightState.Red)]
		[InlineData(TrafficLightCommand.SwitchNext, TrafficLightState.Green)]
		[InlineData(TrafficLightCommand.SwitchNext, TrafficLightState.Yellow)]
		public void CreateCommandObject_WillCreateAppropriateCommandObject(TrafficLightCommand command, TrafficLightState state) {
			var fsmStub = Mock.Of<FsmBuilder<TrafficLightState, TrafficLightCommand>>();
			var finiteState = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(state)
				.WithFsmBuilder(fsmStub)
				.Build();
			Mock.Get(fsmStub)
				.Setup(builder => builder.FromState(state))
				.Returns(finiteState);
			var commandConfigBuilder = new TestCommandConfigurationBuilder<TrafficLightState, TrafficLightCommand>();
			commandConfigBuilder
				.State
				.WithFsmBuilder(fsmStub);
			var commandConfig = commandConfigBuilder
				.WithCommand(command)
				.Build();
			commandConfig.SetState(state);

			var commandObject = commandConfig.CreateCommandObject();

			Assert.Equal(command, commandObject.Command);
			Assert.Equal(state, commandObject.TargetState);
		}

		[Fact]
		public void CreateCommandObject_WillReuseCreatedCommandObject() {
			var fsmStub = Mock.Of<FsmBuilder<TrafficLightState, TrafficLightCommand>>();
			var finiteState = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(TrafficLightState.Green)
				.WithFsmBuilder(fsmStub)
				.Build();
			Mock.Get(fsmStub)
				.Setup(builder => builder.FromState(TrafficLightState.Green))
				.Returns(finiteState);
			var commandConfigBuilder = new TestCommandConfigurationBuilder<TrafficLightState, TrafficLightCommand>();
			commandConfigBuilder
				.State
				.WithFsmBuilder(fsmStub);
			var commandConfig = commandConfigBuilder
				.Build();
			commandConfig.SetState(TrafficLightState.Green);

			var commandObject = commandConfig.CreateCommandObject();

			Assert.Same(commandObject, commandConfig.CreateCommandObject());
		}
	}
}
