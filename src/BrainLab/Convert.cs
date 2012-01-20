using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BrainLab.Studio
{
	class Convert
	{
		private void ConvertMatlabMtxFiles(string fullPath)
		{
			//string[] events = Directory.GetFiles(fullPath);

			//int subCounter = 0;

			//foreach (var evt in events)
			//{
			//    var fileName = System.IO.Path.GetFileName(evt);
			//    var subjId = fileName.Split('_')[0];

			//    Subject subj = null;
			//    if (!subjects.TryGetValue(subjId, out subj))
			//        continue;

			//    Graph fMRI = null;
			//    if (!subj.Graphs.ContainsKey("fMRI"))
			//    {
			//        fMRI = new Graph(90, 90);
			//        subj.Graphs["fMRI"] = fMRI;
			//    }
			//    else
			//        fMRI = subj.Graphs["fMRI"];

			//    using (StreamWriter sw = new StreamWriter(@"C:\Users\Brent\Desktop\BrentExport\adj\" + subjId + "_fMRI_adj-mtx.txt"))
			//    {
			//        MatFileReader mtx = new MatFileReader(evt);
			//        foreach (var mla in mtx.Data)
			//        {
			//            var itms = (MLDouble)mla;

			//            for (int m = 0; m < 90; m++)
			//            {
			//                for (int n = 90; n < 180; n += 1)
			//                {
			//                    var val = itms.Get(m, n);
			//                    fMRI.AdjMatrix[m, n - 90] = val;
			//                    sw.Write(val.ToString() + " ");
			//                }

			//                sw.WriteLine();
			//            }
			//        }
			//    }
			//}
		}

		private void ConvertTrackerMtxFiles(string fullPath)
		{
			//string[] eventDirs = Directory.GetDirectories(fullPath);
			//string subDir = "";
			//string file = @"tensor\AAL116_tracker_adj_20100805.txt";

			//string[] possibleSubDirs = new string[] { "dti_30_VB15_PA_FORWARD", "dti-30_forward" };

			//int subCounter = 0;

			//foreach (var evtDir in eventDirs)
			//{
			//    foreach (var possibleSubDir in possibleSubDirs)
			//    {
			//        if (Directory.Exists(System.IO.Path.Combine(evtDir, possibleSubDir)))
			//        {
			//            subDir = possibleSubDir;
			//            break;
			//        }
			//    }

			//    if (!string.IsNullOrEmpty(subDir))
			//    {
			//        var filePath = System.IO.Path.Combine(evtDir, subDir, file);
			//        var subjId = System.IO.Path.GetFileName(evtDir);

			//        Subject subj = null;
			//        if (!subjects.TryGetValue(subjId, out subj))
			//            continue;

			//        Graph g = null;
			//        if (!subj.Graphs.ContainsKey("DTI"))
			//        {
			//            g = new Graph(90, 90);
			//            subj.Graphs["DTI"] = g;
			//        }
			//        else
			//            g = subj.Graphs["DTI"];

			//        using (StreamWriter sw = new StreamWriter(@"C:\Users\Brent\Desktop\BrentExport\adj\" + subjId + "_DTI_adj-mtx.txt"))
			//        {
			//            using (StreamReader sr = new StreamReader(filePath))
			//            {
			//                var line = "";
			//                var lineCount = 0;

			//                while ((line = sr.ReadLine()) != null)
			//                {
			//                    var columns = line.Split(' ');

			//                    for (int i = 0; i < columns.Length; i++)
			//                    {
			//                        double dVal = Double.Parse(columns[i]);

			//                        if (lineCount < 90 && i < 90)
			//                        {

			//                            if (i > lineCount)
			//                            {
			//                                g.AdjMatrix[lineCount, i] = dVal;
			//                                g.AdjMatrix[i, lineCount] = dVal;
			//                            }

			//                            //if (i == lineCount)
			//                            //	g.AdjMatrix[lineCount, i] = Double.NaN;
			//                        }
			//                    }

			//                    lineCount++;
			//                }
			//            }

			//            for (int i = 0; i < 90; i++)
			//            {
			//                for (int j = 0; j < 90; j++)
			//                {
			//                    sw.Write(g.AdjMatrix[i, j].ToString() + " ");
			//                }
			//                sw.WriteLine();
			//            }
			//        }
			//    }
			//}
		}
	}
}
