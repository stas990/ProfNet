using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using ProfNet.Model;
using ProfNet.Model.Profiling.Operations;
using ProfNet.Model.Profiling.ProcessModel;

namespace ProfNet.Tests.Profiling
{
	[TestFixture]
	public class ProfilingProcessTests
	{
		private const string MockResult =
			"[{\"Name\":\"System.AppDomain.SetupDomain\",\"Duration\":\"7.53609\",\"StartTime\":\"114.965\",\"MethodId\":\"1913816\",\"ThreadId\":\"008FDDF0\",\"Childs\":[{\"Name\":\"System.AppDomain.SetupFusionStore\",\"Duration\":\"6.61345\",\"StartTime\":\"115.885\",\"MethodId\":\"1913756\",\"ThreadId\":\"008FDDF0\",\"Childs\":[{\"Name\":\"System.AppDomainSetup.SetupFusionContext\",\"Duration\":\"4.36758\",\"StartTime\":\"118.125\",\"MethodId\":\"1917328\",\"ThreadId\":\"008FDDF0\"}]}]},{\"Name\":\"System.AppDomain.SetDefaultDomainManager\",\"Duration\":\"3.23141\",\"StartTime\":\"122.754\",\"MethodId\":\"1912320\",\"ThreadId\":\"008FDDF0\",\"Childs\":[{\"Name\":\"System.AppDomain.SetDomainManager\",\"Duration\":\"1.49735\",\"StartTime\":\"124.475\",\"MethodId\":\"1912332\",\"ThreadId\":\"008FDDF0\"}]},{\"Name\":\"System.Runtime.Remoting.RemotingServices..cctor\",\"Duration\":\"2.27334\",\"StartTime\":\"128.961\",\"MethodId\":\"1940680\",\"ThreadId\":\"008FDDF0\"}]";

		private readonly Mock<IIOFileSystemService> _fileSystem = new Mock<IIOFileSystemService>();

		[TestFixtureSetUp]
		public void Setup()
		{
			string fileName = "D:\\SomeFolder\\FileName.exe";
			_fileSystem.Setup(x => x.OpenFileDialog(It.IsAny<string>())).Returns(fileName);
		}

		[Test(Description = "Environment variables test")]
		public void ExecutableOperationTest1()
		{
			Mock<IProcess> process = new Mock<IProcess>();
			Mock<IProcessProvider> processProvider = new Mock<IProcessProvider>();
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processProvider.Setup(x => x.Start(It.IsAny<ProcessStartInfo>())).Returns(process.Object).Callback<ProcessStartInfo>(x => processStartInfo = x);

			ExecutableProfilingOperation executableProfiling = new ExecutableProfilingOperation(processProvider.Object);
			executableProfiling.FileSystem = _fileSystem.Object;
			executableProfiling.StartProfiling();
			executableProfiling.DetachProfiler();

			Assert.AreEqual(_fileSystem.Object.OpenFileDialog(string.Empty), processStartInfo.FileName);
			Assert.IsTrue(processStartInfo.EnvironmentVariables.ContainsKey(Constants.Cor_Enable_Profiling));
			Assert.IsTrue(processStartInfo.EnvironmentVariables.ContainsKey(Constants.Cor_Profiler));
			Assert.IsTrue(processStartInfo.EnvironmentVariables.ContainsKey(Constants.Threshold));
			Assert.IsTrue(processStartInfo.EnvironmentVariables.ContainsKey(Constants.TmpFolderName));
		}

		[Test]
		public void ExecutableOperationTest2()
		{
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
			_fileSystem.Setup(x => x.OpenFileRead(It.IsAny<string>())).Returns(new MemoryStream(encoding.GetBytes(MockResult)));
			_fileSystem.Setup(x => x.FileExist(It.IsAny<string>())).Returns(true);

			Mock<IProcess> process = new Mock<IProcess>();
			Mock<IProcessProvider> processProvider = new Mock<IProcessProvider>();
			ProcessStartInfo processStartInfo;
			processProvider.Setup(x => x.Start(It.IsAny<ProcessStartInfo>())).Returns(process.Object).Callback<ProcessStartInfo>(x => processStartInfo = x);

			ExecutableProfilingOperation executableProfiling = new ExecutableProfilingOperation(processProvider.Object);
			executableProfiling.FileSystem = _fileSystem.Object;
			bool profilingFinishedRaised = false;
			executableProfiling.ProfilingFinished += () => profilingFinishedRaised = true;
			executableProfiling.StartProfiling();

			process.Raise(x => x.Exited += null, process, new EventArgs());
			Assert.IsNotNull(executableProfiling.Results.MethodInfos);
			Assert.IsTrue(profilingFinishedRaised);
		}
	}
}