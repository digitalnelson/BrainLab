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
using System.Windows.Shapes;

namespace BrainLab.Studio
{
	/// <summary>
	/// Interaction logic for Report.xaml
	/// </summary>
	public partial class Report : Window
	{
		public Report()
		{
			InitializeComponent();
		}

		public void SetData(string textData)
		{
			_tbReport.Text = textData;
		}
	}
}
