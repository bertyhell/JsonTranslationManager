using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Tmc.WinUI.Common;

namespace JsonTranslationManager
{
    public class Int2ColorConverter : IValueConverter
    {
        private static readonly ColorPercent[] PERCENT_COLORS =
        {
	        new ColorPercent{Percent = 0, Red = 240, Green = 128, Blue = 128}, 
	        new ColorPercent{Percent = 0.66, Red = 255, Green = 215, Blue = 0}, 
	        new ColorPercent{Percent = 1, Red = 144, Green = 238, Blue = 144}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (targetType == typeof(Brush) && value is double)
			{
				return ConvertDouble2Brush((double) value);
			}
			return null;
        }

	    public static SolidColorBrush ConvertDouble2Brush(double percentage)
	    {
			double percent = percentage;
			if (percent < 0 || percent > 1)
			{
				return null;
			}
			return new SolidColorBrush(Percent2Color(percent));	    
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Color Percent2Color(double percent)
        {
            for (var I = 0; I < PERCENT_COLORS.Length; I++)
            {
                if (percent <= PERCENT_COLORS[I].Percent)
                {
                    ColorPercent lower = I-1 < 0 ? new ColorPercent{Percent = 0.1, Red = 0, Green = 0, Blue = 0} : PERCENT_COLORS[I - 1];
                    var upper = PERCENT_COLORS[I];
                    var range = upper.Percent - lower.Percent;
                    var rangePct = (percent - lower.Percent) / range;
                    var pctLower = 1 - rangePct;
                    var pctUpper = rangePct;
                    byte red = (byte)Math.Round(lower.Red * pctLower + upper.Red * pctUpper);
                    byte green = (byte)Math.Round(lower.Green * pctLower + upper.Green * pctUpper);
                    byte blue = (byte)Math.Round(lower.Blue * pctLower + upper.Blue * pctUpper);
                    return Color.FromRgb(red, green, blue);
                }
            }
            return Colors.DarkGray;
        }
    }
}
