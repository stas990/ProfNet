﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProfNet
{
	public class AutoGreyableImage : Image
	{
		static AutoGreyableImage()
		{
			IsEnabledProperty.OverrideMetadata(typeof(AutoGreyableImage), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoGreyScaleImageIsEnabledPropertyChanged)));
		}

		private static void OnAutoGreyScaleImageIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
		{
			var autoGreyScaleImg = source as AutoGreyableImage;
			var isEnable = Convert.ToBoolean(args.NewValue);

			if (autoGreyScaleImg != null)
			{

				if (!isEnable)
				{
					var bitmapImage = new BitmapImage(new Uri(autoGreyScaleImg.Source.ToString()));
					autoGreyScaleImg.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
					autoGreyScaleImg.OpacityMask = new ImageBrush(bitmapImage);
				}
				else
				{
					autoGreyScaleImg.Source = ((FormatConvertedBitmap)autoGreyScaleImg.Source).Source;
					autoGreyScaleImg.OpacityMask = null;
				}
			}
		}
	}
}