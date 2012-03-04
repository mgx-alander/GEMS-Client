﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Gems.UIWPF
{
[ValueConversion( typeof( object ), typeof( int ) )]
	public class NumberToPolarValueConverter : IValueConverter
	{
		public object Convert(
			object value, Type targetType,
			object parameter, CultureInfo culture )
		{
			double number = (double)System.Convert.ChangeType( value, typeof(double) );

			if( number == -1 )
				return -1;//red

			if( number == 1 )
				return +1; //green

			return 0;//normal
		}

		public object ConvertBack(
			object value, Type targetType,
			object parameter, CultureInfo culture )
		{
			throw new NotSupportedException( "ConvertBack not supported" );
		}
	}
}