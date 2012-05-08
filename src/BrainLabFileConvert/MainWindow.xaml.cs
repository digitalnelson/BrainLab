using System;
using System.Collections.Generic;
using System.IO;
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
using csmatio;
using csmatio.io;
using csmatio.types;

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
			ConvertMatlabFiles();
		}

		private void ConvertMatlabFiles()
		{
			string[] events = Directory.GetFiles(@"C:\Users\Brent\Desktop\Neuro Imaging\BrentExport\sz_fmri");

			foreach (var evt in events)
			{
				var fileName = System.IO.Path.GetFileName(evt);
				var subjId = fileName.Split('_')[0];

				using (StreamWriter sw = new StreamWriter(@"C:\Users\Brent\Desktop\tmp\" + subjId + "_fMRI_mo-adj-mtx.txt"))
				{
					MatFileReader mtx = new MatFileReader(evt);
					foreach (var mla in mtx.Data)
					{
						var itms = (MLDouble)mla;

						for (int m = 0; m < 90; m++)
						{
							for (int n = 90; n < 180; n += 1)
							{
								var val = itms.Get(m, n);
								if (n == 90)
									sw.Write(val.ToString());
								else
									sw.Write("\t" + val.ToString());
							}
							sw.WriteLine();
						}
					}
				}
			}
		}

		private void ConvertTrackerFiles()
		{
			string[] eventDirs = Directory.GetDirectories(@"C:\Users\Brent\Desktop\Neuro Imaging\BrentExport\sz_dti");
			string subDir = "";
			string file = @"tensor\AAL116_tracker_adj_20100805.txt";

			string[] possibleSubDirs = new string[] { "dti_30_VB15_PA_FORWARD", "dti-30_forward" };

			foreach (var evtDir in eventDirs)
			{
				foreach (var possibleSubDir in possibleSubDirs)
				{
					if (Directory.Exists(System.IO.Path.Combine(evtDir, possibleSubDir)))
					{
						subDir = possibleSubDir;
						break;
					}
				}

				if (!string.IsNullOrEmpty(subDir))
				{
					var filePath = System.IO.Path.Combine(evtDir, subDir, file);
					var subjId = System.IO.Path.GetFileName(evtDir);

					using (StreamWriter sw = new StreamWriter(@"C:\Users\Brent\Desktop\convert\" + subjId + "_DTI_adj-mtx.txt"))
					{
						using (StreamReader sr = new StreamReader(filePath))
						{
							var line = "";

							while ((line = sr.ReadLine()) != null)
							{
								var columns = line.Split(' ');

								for (int i = 0; i < columns.Length; i++)
								{
									double val = Double.Parse(columns[i]);

									if (i == 0)
										sw.Write(val.ToString());
									else
										sw.Write("\t" + val.ToString());
								}

								sw.WriteLine();
							}
						}
					}
				}
			}
		}
	}
}
