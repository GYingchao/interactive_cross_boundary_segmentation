using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace CrossBoundaryBrushes
{
	class ColorInterpolater
	{
		public ColorInterpolater()
		{

		}
		// --- 5 interpolating colors,---
		private Color color_1 = Color.Blue; //Color.FromArgb(255, 250, 120);Color.FromArgb(255, 250, 120);
		private Color color_2 = Color.Cyan; //Color.FromArgb(255, 255, 70);	//Color.FromArgb(210, 230, 73);
		private Color color_3 = Color.Lime; //Color.FromArgb(168, 255, 81);	//Color.FromArgb(154, 203, 0);
		private Color color_4 = Color.Yellow; //Color.FromArgb(255, 128, 128);	//Color.FromArgb(211, 159, 58);
		private Color color_5 = Color.Red;// FromArgb(255, 128, 128); //Color.FromArgb(255, 0, 0);		//Color.FromArgb(254, 82, 80);

		public Color Color_1
		{
			get { return color_1; }
			set { color_1 = value; }
		}
		public Color Color_2
		{
			get { return color_2; }
			set { color_2 = value; }
		}
		public Color Color_3
		{
			get { return color_3; }
			set { color_3 = value; }
		}
		public Color Color_4
		{
			get { return color_4; }
			set { color_4 = value; }
		}
		public Color Color_5
		{
			get { return color_5; }
			set { color_5 = value; }
		}

		// --- divid the range into five intervals ---
		private double cut_value_1 = 0.0;
		private double cut_value_2 = 0.0;
		private double cut_value_3 = 0.0;
		private double cut_value_4 = 0.0;
		private double cut_value_5 = 0.0;
		// --- we assume the data is smoothly distributed ---
		private void FindIntervals(double[] datas)
		{
			List<double> arr = new List<double>(); // -- sort the data
			for (int i = 0; i < datas.Length; ++i)
			{
				arr.Add(datas[i]);
			}
			arr.Sort();

			int L = datas.Length; ;

			int ii1 = (int)(0.25 * L);
			int ii2 = (int)(0.50 * L);
			int ii3 = (int)(0.75 * L);
			
			this.cut_value_1 = arr[0];
			this.cut_value_2 = arr[ii1];
			this.cut_value_3 = arr[ii2];
			this.cut_value_4 = arr[ii3];
			this.cut_value_5 = arr[L - 1];
		}
		public Color[] InterpolateColors(double[] datas)
		{
			int n = datas.Length;

			Color[] clrs = new Color[n];

			FindIntervals(datas);

			int dR1 = color_2.R - color_1.R;
			int dR2 = color_3.R - color_2.R;
			int dR3 = color_4.R - color_3.R;
			int dR4 = color_5.R - color_4.R;

			int dG1 = color_2.G - color_1.G;
			int dG2 = color_3.G - color_2.G;
			int dG3 = color_4.G - color_3.G;
			int dG4 = color_5.G - color_4.G;

			int dB1 = color_2.B - color_1.B;
			int dB2 = color_3.B - color_2.B;
			int dB3 = color_4.B - color_3.B;
			int dB4 = color_5.B - color_4.B;

			double epsilon = 1e-6;
			
			double dv1 = cut_value_2 - cut_value_1 + epsilon;
			double dv2 = cut_value_3 - cut_value_2 + epsilon;
			double dv3 = cut_value_4 - cut_value_3 + epsilon;
			double dv4 = cut_value_5 - cut_value_4 + epsilon;

			
			for (int i = 0; i < n; ++i)
			{
				double d = datas[i];
				byte R = 255, G = 255, B = 255;

				if (d <= cut_value_2)
				{
					double r = (d - cut_value_1) / (dv1);
					//r = r * r * r;
					R = (byte)(color_1.R + (dR1 * r));
					G = (byte)(color_1.G + (dG1 * r));
					B = (byte)(color_1.B + (dB1 * r));
				}
				else if (d > cut_value_2 && d <= cut_value_3)
				{
					double r = (d - cut_value_2) / (dv2);
					//r = r * r * r;
					R = (byte)(color_2.R + (dR2 * r));
					G = (byte)(color_2.G + (dG2 * r));
					B = (byte)(color_2.B + (dB2 * r));
				}
				else if (d > cut_value_3 && d <= cut_value_4)
				{
					double r = (d - cut_value_3) / (dv3);
					//r = r * r * r;
					R = (byte)(color_3.R + (dR3 * r));
					G = (byte)(color_3.G + (dG3 * r));
					B = (byte)(color_3.B + (dB3 * r));
				}
				else if (d > cut_value_4 && d <= cut_value_5)
				{
					double r = (d - cut_value_4) / (dv4);
					//r = r * r * r;
					R = (byte)(color_4.R + (dR4 * r));
					G = (byte)(color_4.G + (dG4 * r));
					B = (byte)(color_4.B + (dB4 * r));
				}
				clrs[i] = Color.FromArgb(R, G, B);
			}

			return clrs;
		}
	}
}
