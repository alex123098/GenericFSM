using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericFSM
{
	public abstract partial class StateMachine<TState, TCommand>
		where TState : struct
		where TCommand : struct
	{
		public abstract void Start();
	}
}
