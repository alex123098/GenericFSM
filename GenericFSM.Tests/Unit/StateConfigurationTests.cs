using System;
using System.Linq;
using GenericFSM.Tests.Infrastructure;
using Xunit;
using Xunit.Extensions;
using GenericFSM.Exceptions;
using Moq;

namespace GenericFSM.Tests.Unit
{
	public class StateConfigurationTests
	{
		[Fact]
		public void Ctor_WillAcceptFsmBuilder() {
			var configBuilder = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>();
			configBuilder.Build();
		}

		[Theory]
		[InlineData(TrafficLightState.Green)]
		[InlineData(TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow)]
		public void Ctor_WillAcceptState(TrafficLightState state) {
			var stateConfig = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(state)
				.Build();
			Assert.Equal(state, stateConfig.State);
		}

		[Fact]
		public void AsInitialState_WillNotReturnNull() {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();
			
			var otherStateVar = stateConfiguration.AsInitialState();

			Assert.NotNull(otherStateVar);
		}

		[Fact]
		public void AsInitialState_WillReturnSameInstance() {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			var otherStateVar = stateConfiguration.AsInitialState();

			Assert.Same(stateConfiguration, otherStateVar);
		}

		[Fact]
		public void GetEnteringAction_ReturnsPresettedAction() {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			Assert.Null(stateConfiguration.GetEnteringAction());
			Action<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext> enteringAction = ctx => { };
			stateConfiguration.WithEnteringAction(enteringAction);
			
			Assert.Same(enteringAction, stateConfiguration.GetEnteringAction());
		}

		[Fact]
		public void GetExitingAction_ReturnsPresettedAction() {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			Assert.Null(stateConfiguration.GetExitingAction());
			Action<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext> exitingAction = ctx => { };
			stateConfiguration.WithExitingAction(exitingAction);

			Assert.Same(exitingAction, stateConfiguration.GetExitingAction());
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void OnCommand_WillReturnCorrectCommandConfiguration(TrafficLightCommand command) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			var commandConfiguration = stateConfiguration.OnCommand(command);

			Assert.NotNull(commandConfiguration);
			Assert.Equal(command, commandConfiguration.Command);
		}

		[Fact]
		public void OnCommand_CanSpecifyGuardCondition() {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();
			Func<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext, bool> guardCondition = ctx => true;

			var commandConfiguration = stateConfiguration.OnCommand(TrafficLightCommand.Reset, guardCondition);

			Assert.NotNull(commandConfiguration);
			Assert.Same(guardCondition, commandConfiguration.GuardCondition);
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void OnCommand_FailsIfAlreadySpecifiedUnconditionedCommand(TrafficLightCommand command) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			stateConfiguration.OnCommand(command);

			Assert.Throws<CommandRegistrationException>(() => stateConfiguration.OnCommand(command));
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void OnCommand_WillNotFailIfInvokedManyTimesWithGuardCondition(TrafficLightCommand command) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			stateConfiguration.OnCommand(command, ctx => true);

			stateConfiguration.OnCommand(command, ctx => false);
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void OnCommand_WithGuardConditionWillThrowIfAlreadyConfiguredWithoutGuardCondition(TrafficLightCommand command) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();

			stateConfiguration.OnCommand(command);

			Assert.Throws<CommandRegistrationException>(() => stateConfiguration.OnCommand(command, ctx => true));
		}

		[Theory]
		[InlineData(TrafficLightCommand.Reset)]
		[InlineData(TrafficLightCommand.SwitchNext)]
		public void OnCommand_WillFailIfInvokedForSameCommandWithSameGuardConditionTwice(TrafficLightCommand command) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>().Build();
			Func<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext, bool> guard = ctx => true;

			stateConfiguration.OnCommand(command, guard);

			Assert.Throws<CommandRegistrationException>(() => stateConfiguration.OnCommand(command, guard));
		}

		[Theory]
		[InlineData(TrafficLightState.Green)]
		[InlineData(TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow)]
		public void CreateState_WillReturnCorrectStateObject(TrafficLightState state) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(state)
				.Build();

			var stateObject = stateConfiguration.CreateState();

			Assert.Equal(state, stateObject.State);
		}

		[Theory]
		[InlineData(TrafficLightState.Green)]
		[InlineData(TrafficLightState.Red)]
		[InlineData(TrafficLightState.Yellow)]
		public void CreateState_WillReturnCorrectStateWithEnterAndExitActions(TrafficLightState state) {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(state)
				.Build();
			var enterInvoked = false;
			var exitInvoked = false;
			Action<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext> enterAction = 
				ctx => { enterInvoked = true; };
			Action<StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext> exitAction = 
				ctx => { exitInvoked = true; };
			
			var stateObject = stateConfiguration
				.WithEnteringAction(enterAction)
				.WithExitingAction(exitAction)
				.CreateState();
			var simpleContext = new StateMachine<TrafficLightState, TrafficLightCommand>.StateMachineContext(null, null, null, null);
			stateObject.Enter(simpleContext);
			stateObject.Exit(simpleContext);

			Assert.Equal(state, stateObject.State);
			Assert.True(enterInvoked, "State.Enter() must invoke enter action.");
			Assert.True(exitInvoked, "State.Exit() must invoke exit action.");
		}

		[Theory]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.Reset)]
		[InlineData(TrafficLightState.Red, TrafficLightCommand.Reset)]
		[InlineData(TrafficLightState.Yellow, TrafficLightCommand.Reset)]
		[InlineData(TrafficLightState.Yellow, TrafficLightCommand.SwitchNext)]
		[InlineData(TrafficLightState.Green, TrafficLightCommand.SwitchNext)]
		public void CreateState_WillReturnCorrectStateWithExpectedCommands(TrafficLightState state, TrafficLightCommand command) {
			var fsmStub = Mock.Of<FsmBuilder<TrafficLightState, TrafficLightCommand>>();
			var finiteState = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.ForState(state)
				.WithFsmBuilder(fsmStub)
				.Build();
			Mock.Get(fsmStub)
				.Setup(builder => builder.FromState(state))
				.Returns(finiteState);
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.WithFsmBuilder(fsmStub)
				.ForState(state)
				.Build();
			stateConfiguration.OnCommand(command).SetState(state);

			var stateObject = stateConfiguration.CreateState();

			Assert.Equal(state, stateObject.State);
			var expectedCommand = stateObject.Commands.Single();
			Assert.Equal(Command.GetHashCode(command), expectedCommand.GetHashCode());
		}

		[Fact]
		public void CreateState_WillReuseCreatedStateObject() {
			var stateConfiguration = new TestStateConfigurationBuilder<TrafficLightState, TrafficLightCommand>()
				.Build();
			
			var stateObject = stateConfiguration.CreateState();

			Assert.Same(stateObject, stateConfiguration.CreateState());
		}
	}
}
