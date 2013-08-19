using System;
using Microsoft.Win32;

namespace ProfNet.Helpers
{
	public class ProfilerRegister
	{
		private static readonly Guid _profilerGuid = new Guid("626409CA-BF10-4F44-A020-B4C99D1628EE");

		private static bool IsX64
		{
			get { return 8 == IntPtr.Size; }
		}

		public static Guid ProfilerGuid { get { return _profilerGuid; } }

		public static void RegisterProfilerIfNeeded()
		{
			if(!IsProfilerRegistered())
				RegisterProfiler();
		}

		public static void RegisterProfiler()
		{
			RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(string.Format("CLSID\\{{{0}}}", _profilerGuid));
			registryKey.SetValue("", "Profiler.ProfNet.1");

			registryKey = Registry.ClassesRoot.CreateSubKey(string.Format("CLSID\\{{{0}}}\\InprocServer32", _profilerGuid));
			registryKey.SetValue("", string.Format("{0}\\ProfNet.Preformance.dll", Environment.CurrentDirectory));
			registryKey.SetValue("ThreadingModel", "Apartment");

			registryKey = Registry.ClassesRoot.CreateSubKey(string.Format("CLSID\\{{{0}}}\\ProgID", _profilerGuid));
			registryKey.SetValue("", "Profiler.ProfNet.1");
		}

		public static bool IsProfilerRegistered()
		{
			RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(string.Format("CLSID\\{{{0}}}\\", _profilerGuid));

			if (registryKey == null)
				return false;

			if(registryKey.GetValue(null) == null)
				return false;

			return true;
		}
	}
}