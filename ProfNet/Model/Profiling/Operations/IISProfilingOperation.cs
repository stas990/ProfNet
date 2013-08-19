using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;
using ProfNet.Helpers;

namespace ProfNet.Model.Profiling.Operations
{
	public class IISProfilingOperation : BaseProfilingOperation
	{
		[DllImport("Advapi32.dll")]
		private static extern bool LookupAccountName(string machineName, string accountName, byte[] sid,
								 ref int sidLen, StringBuilder domainName, ref int domainNameLen, out int peUse);

		[DllImport("Advapi32.dll")]
		private static extern bool ConvertSidToStringSidW(byte[] sid, out IntPtr stringSid);

		[DllImport("Kernel32.dll")]
		private static extern bool LocalFree(IntPtr ptr);

		protected override bool StartProfilingInternal(IEnumerable<KeyValuePair<string, string>> environmentVariables)
		{
			UrlWindow newNoteWindow = new UrlWindow();
			bool? showDialogResult = newNoteWindow.ShowDialog();
			if (!showDialogResult.HasValue || !showDialogResult.Value)
				return false;

			if (!StopIIS())
			    return false;

			string[] profilerEnvironment = environmentVariables.Select(x => string.Format("{0}={1}", x.Key, x.Value)).ToArray();
			PrepareEnvironment(profilerEnvironment);

			Process.Start(newNoteWindow.Uri);

			return true;
		}

		protected override void DetachProfilingInternal()
		{
			
		}

		private static bool StopIIS()
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
			if (Environment.OSVersion.Version.Major >= 6/*Vista*/)
				processStartInfo.Arguments = "/c net stop was /y";
			else
				processStartInfo.Arguments = "/c net stop iisadmin /y";
			Process process = Process.Start(processStartInfo);

			Workspace.Instance.Status.Status = "Stoping iis";

			while (!process.HasExited)
			{
				Workspace.Instance.Status.Status += ".";
				Thread.Sleep(100);
			}

			if (process.ExitCode != 0)
			{
				Workspace.Instance.Status.Status = string.Format(" Error {0} occurred", process.ExitCode);
				return false;
			}

			Workspace.Instance.Status.Status = "IIS stopped";
			return true;
		}

		private static bool StartIIS()
		{
			Workspace.Instance.Status.Status = "Starting IIS ";
			ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe") { Arguments = "/c net start w3svc" };
			Process process = Process.Start(processStartInfo);
			while (!process.HasExited)
			{
				Workspace.Instance.Status.Status += ".";
				Thread.Sleep(100);
			}
			if (process.ExitCode != 0)
			{
				Workspace.Instance.Status.Status += string.Format(" Error {0} occurred", process.ExitCode);
				return false;
			}
			Workspace.Instance.Status.Status = "IIS running";
			return true;
		}

		public static void PrepareEnvironment(string[] profilerEnvironments)
		{
			Process[] servicesProcesses = Process.GetProcessesByName("services");
			if (servicesProcesses.Length != 1)
			{
				servicesProcesses = Process.GetProcessesByName("services.exe");
				if (servicesProcesses.Length != 1)
					return;
			}

			string[] processEnvironment = ProcessHelper.GetProcessEnvironment(servicesProcesses[0]);
			string[] enumerable = processEnvironment.Union(profilerEnvironments).ToArray();

			string asp_netAccountSid = null;

			try
			{
				SetEnvironmentVariables("IISADMIN", enumerable);
				SetEnvironmentVariables("W3SVC", enumerable);
				SetEnvironmentVariables("WAS", enumerable);

				string asp_netAccountName = GetASP_NETAccountName();
				if (asp_netAccountName != null)
				{
					asp_netAccountSid = LookupAccountSid(asp_netAccountName);
					if (asp_netAccountSid != null)
						SetAccountEnvironment(asp_netAccountSid, enumerable);
				}

				if (StartIIS())
				{
					Workspace.Instance.Status.Status = "Waiting for ASP.NET worker process to start up";
					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				
			}
			finally
			{
				DeleteEnvironmentVariables("IISADMIN");
				DeleteEnvironmentVariables("W3SVC");
				DeleteEnvironmentVariables("WAS");

				if (asp_netAccountSid != null)
					ResetAccountEnvironment(asp_netAccountSid, processEnvironment);
			}
		}

		private static string LookupAccountSid(string accountName)
		{
			int sidLen = 0;
			byte[] sid = new byte[sidLen];
			int domainNameLen = 0;
			int peUse;
			StringBuilder domainName = new StringBuilder();
			LookupAccountName(Environment.MachineName, accountName, sid, ref sidLen, domainName, ref domainNameLen, out peUse);

			sid = new byte[sidLen];
			domainName = new StringBuilder(domainNameLen);
			string stringSid = null;
			if (LookupAccountName(Environment.MachineName, accountName, sid, ref sidLen, domainName, ref domainNameLen, out peUse))
			{
				IntPtr stringSidPtr;
				if (ConvertSidToStringSidW(sid, out stringSidPtr))
				{
					try
					{
						stringSid = Marshal.PtrToStringUni(stringSidPtr);
					}
					finally
					{
						LocalFree(stringSidPtr);
					}
				}
			}
			return stringSid;
		}

		private static void SetEnvironmentVariables(string serviceName, string[] environment)
		{
			Microsoft.Win32.RegistryKey key = GetServiceKey(serviceName);
			if (key != null)
				key.SetValue("Environment", environment);
		}

		private static Microsoft.Win32.RegistryKey GetServiceKey(string serviceName)
		{
			Microsoft.Win32.RegistryKey localMachine = Microsoft.Win32.Registry.LocalMachine;
			Microsoft.Win32.RegistryKey key = localMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName, true);
			return key;
		}

		private static void DeleteEnvironmentVariables(string serviceName)
		{
			Microsoft.Win32.RegistryKey key = GetServiceKey(serviceName);
			if (key != null)
				key.DeleteValue("Environment");
		}

		private static string GetASP_NETAccountName()
		{
			try
			{
				XmlDocument machineConfig = new XmlDocument();
				string runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
				string configPath = Path.Combine(runtimePath, @"CONFIG\machine.config");
				machineConfig.Load(configPath);
				XmlNodeList elemList = machineConfig.GetElementsByTagName("processModel");
				for (int i = 0; i < elemList.Count; i++)
				{
					XmlAttributeCollection attributes = elemList[i].Attributes;
					XmlAttribute userNameAttribute = attributes["userName"];
					if (userNameAttribute != null)
					{
						string userName = userNameAttribute.InnerText;
						if (userName == "machine")
							return "ASPNET";
						if (userName == "SYSTEM")
							return null;

						return userName;
					}
				}
			}
			catch
			{
				// swallow all exceptions here
			}
			return "ASPNET";
		}

		private static void SetAccountEnvironment(string serviceAccountSid, string[] profilerEnvironment)
		{
			Microsoft.Win32.RegistryKey key = GetAccountEnvironmentKey(serviceAccountSid);
			if (key != null)
			{
				foreach (string envVariable in profilerEnvironment)
				{
					key.SetValue(EnvKey(envVariable), EnvValue(envVariable));
				}
			}
		}

		private static void ResetAccountEnvironment(string serviceAccountSid, string[] profilerEnvironment)
		{
			Microsoft.Win32.RegistryKey key = GetAccountEnvironmentKey(serviceAccountSid);
			if (key != null)
			{
				foreach (string envVariable in profilerEnvironment)
				{
					key.DeleteValue(EnvKey(envVariable));
				}
			}
		}

		private static Microsoft.Win32.RegistryKey GetAccountEnvironmentKey(string serviceAccountSid)
		{
			Microsoft.Win32.RegistryKey users = Microsoft.Win32.Registry.Users;
			return users.OpenSubKey(serviceAccountSid + @"\Environment", true);
		}

		private static string EnvKey(string envVariable)
		{
			int index = envVariable.IndexOf('=');
			Debug.Assert(index >= 0);
			return envVariable.Substring(0, index);
		}

		private static string EnvValue(string envVariable)
		{
			int index = envVariable.IndexOf('=');
			Debug.Assert(index >= 0);
			return envVariable.Substring(index + 1);
		}
	}
}