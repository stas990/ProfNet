using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ProfNet.Helpers
{
	internal static class ProcessHelper
	{
		[DllImport("kernel32.dll")]
		public static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags, StringBuilder lpExeName, out int size);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("Kernel32.dll")]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hHandle);

		[DllImport("Advapi32.dll")]
		public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, ref IntPtr TokenHandle);

		[DllImport("UserEnv.dll")]
		private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment,IntPtr hToken,bool bInherit);

		[Flags]
		public enum ProcessAccessFlags : uint
		{
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VMOperation = 0x00000008,
			VMRead = 0x00000010,
			VMWrite = 0x00000020,
			DupHandle = 0x00000040,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			Synchronize = 0x00100000
		}

		public static string[] GetProcessEnvironment(Process process)
		{
			IntPtr processHandle = OpenProcess(0x20400, false, process.Id);
			if (processHandle == IntPtr.Zero)
				return new string[0];
			IntPtr tokenHandle = IntPtr.Zero;
			if (!OpenProcessToken(processHandle, 0x20008, ref tokenHandle))
				return new string[0];
			IntPtr environmentPtr = IntPtr.Zero;
			if (!CreateEnvironmentBlock(out environmentPtr, tokenHandle, false))
				return new String[0];
			unsafe
			{
				string[] envStrings = null;
				// rather than duplicate the code that walks over the environment, 
				// we have this funny loop where the first iteration just counts the strings,
				// and the second iteration fills in the strings
				for (int i = 0; i < 2; i++)
				{
					char* env = (char*)environmentPtr.ToPointer();
					int count = 0;
					while (true)
					{
						int len = wcslen(env);
						if (len == 0)
							break;
						if (envStrings != null)
							envStrings[count] = new String(env);
						count++;
						env += len + 1;
					}
					if (envStrings == null)
						envStrings = new string[count];
				}
				return envStrings;
			}
		}

		private static unsafe int wcslen(char* s)
		{
			char* e;
			for (e = s; *e != '\0'; e++);
			return (int)(e - s);
		}
	}
}