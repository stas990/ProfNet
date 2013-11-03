using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ProfNet.Model;
using ProfNet.Tests.Mocks;

namespace ProfNet.Tests.Profiling
{
	[TestFixture]
	public class BaseProfilingFunctionTests
	{
		private void RunPipeServer(object message)
		{
			using (var server = new NamedPipeServerStream(NamedPipeMessanger.PipeName))
			{
				server.WaitForConnection();
				using (var streamWriter = new StreamWriter(server))
				{
					streamWriter.WriteLine(message);
					streamWriter.Flush();
				}
			}
		}

		[Test]
		public void ProcessIdMessageTests()
		{
			string processId = "ProcessId 1234";

			var mockProfiling = new MockProfilingOperation();
			var autoResetEvent = new AutoResetEvent(false);
			Task.Factory.StartNew(RunPipeServer, processId);
			
			mockProfiling.ProfilingFinished += () =>
			                                   	{
			                                   		autoResetEvent.Set();
													Console.WriteLine("AutoReset event setted");
			                                   	};

			mockProfiling.StartProfiling();
			autoResetEvent.WaitOne(1000);
			mockProfiling.DetachProfiler();
			Console.WriteLine("Assert ProcessId={0}", mockProfiling.PublicProcessId);
			Assert.AreEqual(1234, mockProfiling.PublicProcessId);
		}

		[Test]
		public void FailedMessageTests()
		{
			string failed = "Failed SomeMessage";

			var mockProfiling = new MockProfilingOperation();
			Task.Factory.StartNew(RunPipeServer, failed);
			var autoResetEvent = new AutoResetEvent(false);
			mockProfiling.ProfilingFinished += () =>
				autoResetEvent.Set();

			mockProfiling.StartProfiling();
			autoResetEvent.WaitOne(2000);
			Assert.IsTrue(mockProfiling.Stoped);
		}
	}
}