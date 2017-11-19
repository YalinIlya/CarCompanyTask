using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace ApplicationForLeave
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GenerateAppForLeave GenAppForLeave = null;
        int countRecords;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BGenerateExcel_Click(object sender, RoutedEventArgs e)
        {
            BGenerateExcel.IsEnabled = false;
            BGeneratePdf.IsEnabled = false;

            GenerateExcel GenExcel = new GenerateExcel();
            GenExcel.OnGenerateExcelEnd += OnGenerateExcelEnd;
            GenExcel.Generate();           
        }
        private void BGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            countRecords = 0;
            BGenerateExcel.IsEnabled = false;
            BGeneratePdf.IsEnabled = false;

            GenAppForLeave = new GenerateAppForLeave();
            GenAppForLeave.OnGenerateAppForLeaveEnd += OnGenerateAppForLeaveEnd;
            GenAppForLeave.OnWriteLog += OnWriteLog;
            GenAppForLeave.OnRecordProcessed += OnRecordProcessed;
            string fileName = TbxExcelFileName.Text;
            string wordFileName = TbxWordFileName.Text;
            string folderPath = TbxPdfFolderPath.Text;
            GenAppForLeave.Generate(fileName, wordFileName, folderPath);
        }

        private void BSelectExcelFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".xlsx";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                TbxExcelFileName.Text = filename;
            }
        }
        private void BSelectWordFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                TbxWordFileName.Text = filename;
            }
        }
        private void BSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();
            TbxPdfFolderPath.Text = folderBrowser.SelectedPath;
        }

        void OnWriteLog(string logtext)
        {
            SetText(logtext);
        }
        void OnGenerateExcelEnd()
        {
            BGenerateExcel_IsEnabled(true);
            BGeneratePdf_IsEnabled(true);
        }
        void OnGenerateAppForLeaveEnd()
        {
            BGeneratePdf_IsEnabled(true);
            BGenerateExcel_IsEnabled(true);
        }
        void OnRecordProcessed()
        {
            countRecords++;
            ShowCountRecords(countRecords.ToString());
        }

        delegate void IsBtnEnabledCallBack(bool isEnabled);
        void BGenerateExcel_IsEnabled(bool isEnabled)
        {
            if (!BGenerateExcel.Dispatcher.CheckAccess())
            {
                IsBtnEnabledCallBack d = new IsBtnEnabledCallBack(BGenerateExcel_IsEnabled);
                BGenerateExcel.Dispatcher.BeginInvoke(d, new object[] { isEnabled });
            }
            else
            {
                BGenerateExcel.IsEnabled = isEnabled;
            }
        }
        void BGeneratePdf_IsEnabled(bool isEnabled)
        {
            if (!BGeneratePdf.Dispatcher.CheckAccess())
            {
                IsBtnEnabledCallBack d = new IsBtnEnabledCallBack(BGeneratePdf_IsEnabled);
                BGeneratePdf.Dispatcher.BeginInvoke(d, new object[] { isEnabled });
            }
            else
            {
                BGeneratePdf.IsEnabled = isEnabled;
            }
        }
        delegate void SetTextCallback(string text);
        void SetText(string text)
        {
            if (!TbxLog.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(SetText);
                TbxLog.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                TbxLog.AppendText(text);
                TbxLog.ScrollToEnd();
            }
        }
        void ShowCountRecords(string str)
        {
            if (!lblRecordsCount.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(ShowCountRecords);
                lblRecordsCount.Dispatcher.BeginInvoke(d, new object[] { str });
            }
            else
            {
                lblRecordsCount.Content = str;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GenAppForLeave != null)
                GenAppForLeave.Close();
        }
    }
}
