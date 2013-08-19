using System.Collections.Generic;
using ProfNet.Model.Profiling.Operations;

namespace ProfNet.Tests.Mocks
{
	public class MockProfilingOperation : BaseProfilingOperation
	{
		public bool Stoped { private set; get; }

		public MockProfilingOperation()
		{
			ProfilingFinished += () => Stoped = true;
		}

		internal int PublicProcessId
		{
			get { return ProcessId; }
		}

		protected override bool StartProfilingInternal(IEnumerable<KeyValuePair<string, string>> environmentVariables)
		{
			return true;
		}

		protected override void DetachProfilingInternal()
		{
			
		}
	}
}