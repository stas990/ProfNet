using System.Collections.Generic;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ProfNet.Model.Profiling
{
	public class MethodInfosViewModel : DocumentViewModel, IDragSource
	{
		public IEnumerable<MethodInfo> Methods { set; get; }

		public MethodInfo SelectedMethod { set; get; }

		public ProfilingResults Parent { set; get; }

		public override string Title
		{
			get { return "Methods"; }
		}

		public void StartDrag(DragInfo dragInfo)
		{
			dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
			dragInfo.Data = SelectedMethod;
		}
	}
}