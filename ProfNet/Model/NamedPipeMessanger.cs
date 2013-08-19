using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProfNet.Model
{
	public class NamedPipeMessanger
	{
		internal const string PipeName = "Profiler";
		private NamedPipeClientStream _pipe;
		private bool _aborted;
		private static string[] _enums;

		internal event Action Aborted = delegate {};
		public event Action<string> MessageRecived = delegate {};
		public event Action<SpecificMessageId, string> SpecificMessageRecived = delegate {};

		public bool CanStartListen{get { return _pipe == null; }}

		public void StartListen()
		{
			Task.Factory.StartNew(WaitMessage);
		}

		public void StopListen()
		{
			_aborted = true;
		}

		private void WaitMessage()
		{
			while (true)
			{
				using (_pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut))
				{
					//I do not know how abort Connect method
					try
					{
						_pipe.Connect(1000);
						using (StreamReader reader = new StreamReader(_pipe))
						{
							string message = reader.ReadToEnd();
							string enumString = Enums.FirstOrDefault(message.StartsWith);
							
							if(!string.IsNullOrEmpty(enumString))
							{
								message = message.Substring(enumString.Length);
								SpecificMessageRecived((SpecificMessageId)Enum.Parse(typeof(SpecificMessageId), enumString), message);
							}
							else
								MessageRecived(message);
						}
					}
					catch (TimeoutException)
					{
						Debug.WriteLine("Pipe client timeout exception");
					}
				}

				if(_aborted)
				{
					_pipe = null;

					_aborted = false;
					Aborted();
					break;
				}
			}
		}

		private static string[] Enums
		{
			get
			{
				if (_enums == null)
					_enums = Enum.GetNames(typeof(SpecificMessageId));

				return _enums;
			}
		}

		public enum SpecificMessageId
		{
			ProcessId,
			Failed
		}
	}
}