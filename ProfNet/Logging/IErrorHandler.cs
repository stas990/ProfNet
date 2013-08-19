using System;

namespace ProfNet.Logging
{
	public interface IErrorHandler
	{
		void Handling(string message);
		void Handling(string message, Exception exception);
	}
}