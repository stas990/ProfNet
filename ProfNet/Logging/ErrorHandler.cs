using System;
using System.Windows;

namespace ProfNet.Logging
{
	public class ErrorHandler : IErrorHandler
	{
		public void Handling(string message)
		{
			MessageBox.Show(message);
		}

		public void Handling(string message, Exception exception)
		{
			MessageBox.Show(string.Format("{0}. Exception:{1}", message, exception.Message));
		}
	}
}