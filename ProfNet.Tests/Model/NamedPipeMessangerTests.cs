using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ProfNet.Model;

namespace ProfNet.Tests.Model
{
	[TestFixture]
	public class NamedPipeMessangerTests
	{
		private void RunPipeServer(object message)
		{
			using (NamedPipeServerStream server = new NamedPipeServerStream(NamedPipeMessanger.PipeName))
			{
				server.WaitForConnection();
				using(StreamWriter streamWriter = new StreamWriter(server))
				{
					streamWriter.WriteLine(message);
					streamWriter.Flush();
				}
			}
		}

		[Test]
		public void Test()
		{
			const string message = "Hello!!!";
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			NamedPipeMessanger messanger = new NamedPipeMessanger();
			messanger.MessageRecived += s =>
			                            	{
			                            		Assert.AreEqual(message, s.Trim());
			                            		autoResetEvent.Set();
			                            	};

			Task.Factory.StartNew(RunPipeServer, message);
			messanger.StartListen();
			autoResetEvent.WaitOne(1500);
			messanger.StopListen();
		}

		[Test]
		public void TestSpecificMessage()
		{
			const string message = "ProcessId 1320";
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			NamedPipeMessanger messanger = new NamedPipeMessanger();
			messanger.SpecificMessageRecived += (id, m) =>
			                                    	{
			                                    		Assert.AreEqual(NamedPipeMessanger.SpecificMessageId.ProcessId, id);
														Assert.AreEqual("1320", m.Trim());
			                                    	};

			Task.Factory.StartNew(RunPipeServer, message);
			messanger.StartListen();
			autoResetEvent.WaitOne(1500);
			messanger.StopListen();
		}

		[Test]
		public void AbortMessangerTest()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			NamedPipeMessanger messanger = new NamedPipeMessanger();
			bool aborted = false;

			messanger.Aborted += () =>
			                     	{
			                     		aborted = true;
			                     		autoResetEvent.Set();
			                     	};

			messanger.StartListen();
			messanger.StopListen();

			autoResetEvent.WaitOne(1500);
			Assert.IsTrue(aborted);
		}
	}
}