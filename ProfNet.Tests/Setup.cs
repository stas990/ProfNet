using System;
using Moq;
using NUnit.Framework;
using ProfNet.Logging;

namespace ProfNet.Tests
{
	[SetUpFixture]
	public class Setup
	{
		[SetUp]
		public void Startup()
		{
			Mock<IErrorHandler> mock = new Mock<IErrorHandler>();
			mock.Setup(x => x.Handling(It.IsAny<string>())).Callback<string>(Console.WriteLine);
			mock.Setup(x => x.Handling(It.IsAny<string>(), It.IsAny<Exception>())).Callback<string, Exception>((msg, ex) => Console.WriteLine("{0}. Exception: {1}", msg, ex));

			App.ErrorHandler = mock.Object;
		}
	}
}