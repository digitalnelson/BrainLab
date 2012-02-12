using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra.IO;

namespace BrainLabFileConvert
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void _ConvertFiles(object sender, RoutedEventArgs e)
		{
			MatlabMatrixReader<double> reader = new MatlabMatrixReader<double>(@"C:\Users\Brent\Desktop\wave_corr_mowb\17671_timecourse_regress-MO-WB_wavecor.mat");

			var mtxs = reader.ReadMatrices();
		}
	}
}
