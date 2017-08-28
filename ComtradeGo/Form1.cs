using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using static ComtradeGo.Universal;

namespace ComtradeGo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TipDropFilesHere();
        }

        private List<string> FilterFilePaths(Array array)
        {
            List<string> paths = new List<string> { };
            for (int i = 0; i < array.Length; i++)
            {
                string[] pathStrs = array.GetValue(i).ToString().Split('.');
                string formatStr = pathStrs[pathStrs.Length - 1].ToLower();
                if ((formatStr != "cfg") && (formatStr != "dat"))
                {
                    continue;
                }
                if (paths.Contains(pathStrs[0]))
                {
                    continue;
                }
                if (!CheckComtradeFilesExist(pathStrs[0]))
                {
                    continue;
                }
                paths.Add(pathStrs[0]);
            }
            return paths;
        }

        private void TipDropFilesHere()
        {
            TipPicBox.Image = ComtradeGO.Properties.Resources.DropHere;
        }

        private void TipConvertingNow()
        {
            TipPicBox.Image = ComtradeGO.Properties.Resources.converting;
        }

        private void TipHaveConverted()
        {
            TipPicBox.Image = ComtradeGO.Properties.Resources.Done;
        }

        private void TipNoFileToConvert()
        {
            TipPicBox.Image = ComtradeGO.Properties.Resources.NoFile;
        }

        private void TipReleaseHere()
        {
            TipPicBox.Image = ComtradeGO.Properties.Resources.ReleaseHere;
        }

        private void TipCSVFileOpened(string path)
        {
            Console.WriteLine(path + ".csv has been opened by other application, please close it!");
        }

        private bool CheckComtradeFilesExist(string path)
        {
            if(!File.Exists(path + ".cfg"))
            {
                return false;
            }
            if (!File.Exists(path + ".dat"))
            {
                return false;
            }
            return true;
        }

        private void CreatCSVFile(string path)
        {
            FileInfo info = new FileInfo(path);
            if(!info.Directory.Exists)
            {
                info.Directory.Create();
            }
        }

        private string MakeCSVHeaderFrom(List<AnalogData> analogList)
        {
            string header = "Time";
            foreach(AnalogData data in analogList)
            {
                header += ("," + data.Name);
            }
            return header;
        }

        private string MakeRowStringFrom(List<AnalogData> analogList, int rowIndex)
        {
            string rowString = "";
            for (int column = 0; column < analogList.Count; column++)
            {
                rowString += ("," + analogList[column].Data[rowIndex].ToString());
            }
            return rowString;
        }

        private bool SaveCsvFileFrom(ParseWave wave)
        {
            String csvPath = wave.FilePath + ".csv";
            CreatCSVFile(csvPath);
            FileStream fs;
            try
            {
                fs = new FileStream(csvPath, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                return false;
            }
                
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(MakeCSVHeaderFrom(wave.Analog));
            double deltaT = StampToValue(wave.Stamp[1]) - StampToValue(wave.Stamp[0]);
            for (int row = 0; row < wave.Stamp.Count; row++)
            {
                sw.WriteLine((deltaT * row).ToString() + MakeRowStringFrom(wave.Analog, row));
            }
            sw.Close();
            fs.Close();
            return true;
        }

        private void Comtrades2CSVsFrom(List<string> filePaths)
        {
            if (filePaths.Count() == 0)
            {
                TipNoFileToConvert();
                TipDropFilesHere();
                return;
            }
            TipConvertingNow();
            new Thread(() =>
            {
                Thread.Sleep(300);
                ParseWave parse;
                foreach (string path in filePaths)
                {
                    parse = new ParseWave(path);
                    if (!SaveCsvFileFrom(parse))
                    {
                        TipCSVFileOpened(path);
                    }
                }
                parse = null;
                TipHaveConverted();
                Thread.Sleep(1000);
                TipDropFilesHere();
                GC.Collect();
            }).Start();
        }

        private void FileDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                TipReleaseHere();
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void FileDragDrop(object sender, DragEventArgs e)
        {
            Array filePathArray = (Array)e.Data.GetData(DataFormats.FileDrop);
            List<string> paths = FilterFilePaths(filePathArray);
            Comtrades2CSVsFrom(paths);
            GC.Collect();
        }

        private void FileDragLeave(object sender, EventArgs e)
        {
            TipDropFilesHere();
        }
    }
}
