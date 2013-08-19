using System;
using System.Runtime.Serialization;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ProfNet.Model.Notes
{
	[DataContract]
	public class Note : DocumentViewModel, IDropTarget
	{
		private object _tag;
		private string _text;
		private string _header;

		public Note()
		{
			CreationDate = DateTime.Now;
		}

		public DateTime CreationDate { set; get; }
		
		[DataMember]
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				RaisePropertyChanged("Text");
			}
		}

		[DataMember]
		public string Header
		{
			set
			{
				_header = value;
				RaisePropertyChanged("Header");
			}
			get { return _header; }
		}

		[DataMember]
		public object Tag
		{
			set
			{
				_tag = value;
				RaisePropertyChanged("Tag");
			}
			get { return _tag; }
		}

		public override string Title
		{
			get { return Header; }
		}

		public void DragOver(DropInfo dropInfo)
		{
			dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
			dropInfo.Effects = DragDropEffects.Move;
		}

		public void Drop(DropInfo dropInfo)
		{
			Tag = dropInfo.Data;
		}

		public override string ContentId
		{
			get
			{
				return Constants.NoteContentId;
			}
		}
	}
}