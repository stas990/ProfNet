using System.Runtime.InteropServices;

namespace ProfNet.Model
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct Config
	{
		public double ThresholdValue;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string WorkFolder;
	}
}