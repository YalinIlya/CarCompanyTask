using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.IO.Compression;

namespace ApplicationForLeave
{
    class GenerateAppForLeave
    {
        public delegate void GenerateAppForLeaveEnd();
        public event GenerateAppForLeaveEnd OnGenerateAppForLeaveEnd;
        public delegate void WriteLog(string logText);
        public event WriteLog OnWriteLog;
        public delegate void RecordProcessed();
        public event RecordProcessed OnRecordProcessed;

        private Excel.Application ObjExcel=null;
        private Excel.Workbook xlWorkbook = null;
        private Excel.Worksheet xlWorksheet = null;
        private Excel.Range xlRange = null;

        private Word.Application app = null;
        private Word.Document doc = null;

        private bool isClose = false;

        public void Close()
        {
            isClose = true;
            //cleanup
            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            if (xlWorkbook != null)
            {
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);
            }

            //quit and release
            if (ObjExcel != null)
            {
                ObjExcel.Quit();
                Marshal.ReleaseComObject(ObjExcel);
            }
        }
        public void Generate(string excelFileName, string wordFileName, string folderPath)
        {
            bool needReturn = false;
            if (string.IsNullOrEmpty(excelFileName))
            {
                OnWriteLog?.Invoke("Укажите имя Excel файла.\r\n");
                needReturn = true;
            }
            if (string.IsNullOrEmpty(wordFileName))
            {
                OnWriteLog?.Invoke("Укажите имя файла шаблона Word.\r\n");
                needReturn = true;
            }
            if (string.IsNullOrEmpty(folderPath))
            {
                OnWriteLog?.Invoke("Укажите выходную папку для заявлений на отпуск.\r\n");
                needReturn = true;
            }
            if (needReturn)
            {
                OnGenerateAppForLeaveEnd?.Invoke();
                return;
            }
            string[] parametrs = new string[3];
            parametrs[0] = excelFileName;
            parametrs[1] = wordFileName;
            parametrs[2] = folderPath;
            Task GenExcelTask = new Task(GenAppForLeave, parametrs);
            GenExcelTask.Start();
        }

        private void GenAppForLeave(object parametrs)
        {
            Random gen = new Random();
            string[] paramsData = (string[])parametrs;
            string filename = paramsData[0];
            string templateFileName = paramsData[1];
            string newPdfFilePath = paramsData[2];
            string tempDirectory = "temp";
            ObjExcel = new Excel.Application();
            //Открываем книгу.
            try
            {
                xlWorkbook = ObjExcel.Workbooks.Open(filename, 0, true, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            }
            catch(Exception ex)
            {
                OnWriteLog?.Invoke(ex.Message + "\r\n");
            }
            if (xlWorkbook == null)
            {
                OnWriteLog?.Invoke("Не удалось открыть файл " + filename + "\r\n");
                OnGenerateAppForLeaveEnd?.Invoke();
                return;
            }
            //Выбираем таблицу(лист).
            ObjExcel.Visible = false;
            xlWorksheet = (Excel.Worksheet)xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;
            
            //Создаём новый Word.Application
            app = new Word.Application();
            //Загружаем документ
            object wordFileName = templateFileName;
            object falseValue = false;
            object trueValue = true;
            object missing = Type.Missing;

            try
            {
                doc = app.Documents.Open(ref wordFileName, ref missing, ref trueValue,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing);
            }
            catch (Exception ex)
            {
                OnWriteLog?.Invoke(ex.Message + "\r\n");
            }
            if (doc == null)
            {
                OnWriteLog?.Invoke("Не удалось открыть файл " + wordFileName + "\r\n");
                app.Quit();
                OnGenerateAppForLeaveEnd?.Invoke();
                return;
            }

            int rowCount = xlRange.Rows.Count;
            //int colCount = 4;
            Directory.CreateDirectory(newPdfFilePath + "\\" + tempDirectory);
            for (int i = 2; i <= rowCount; i++)
            {
                if (isClose)
                    break;
                object times = 1;
                while (doc.Undo(ref times))
                { }

                string chiefNumber = xlRange.Cells[i, "C"].Value2.ToString();
                string chiefFIO = xlRange.Cells[i, "D"].Value2.ToString();
                string employeeFIO = xlRange.Cells[i, "B"].Value2.ToString();
                string employeeNumber = xlRange.Cells[i, "A"].Value2.ToString();

                string pdfFileName = newPdfFilePath +"\\"+ tempDirectory + "\\" + employeeNumber + "_" + chiefNumber + ".pdf";
                OnWriteLog?.Invoke("Save file " + pdfFileName);
                //Console.Write("Save file " + pdfFileName);

                //Очищаем параметры поиска
                app.Selection.Find.ClearFormatting();
                app.Selection.Find.Replacement.ClearFormatting();

                //Задаём параметры замены и выполняем замену.
                object findText = "<ФИО Руководителя>";
                object replaceWith = chiefFIO;
                object replace = Word.WdReplace.wdReplaceAll;

                app.Selection.Find.Execute(ref findText, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                ref replace, ref missing, ref missing, ref missing, ref missing);

                findText = "<ФИО Сотрудника>";
                replaceWith = employeeFIO;

                app.Selection.Find.Execute(ref findText, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                ref replace, ref missing, ref missing, ref missing, ref missing);


                //Генерация даты отпуска
                DateTime DateStart = new DateTime(2018, 1, 1);
                DateTime DateEnd = new DateTime(2019, 1, 1);
                int range = (DateEnd - DateStart).Days;
                DateTime LeaveDate = DateStart.AddDays(gen.Next(range));

                findText = "<Дата начала отпуска>";
                replaceWith = LeaveDate.ToShortDateString();

                app.Selection.Find.Execute(ref findText, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                ref replace, ref missing, ref missing, ref missing, ref missing);

                //Количество дней отпуска
                int DaysCount = gen.Next(40);
                findText = "<N>";
                replaceWith = DaysCount.ToString();

                app.Selection.Find.Execute(ref findText, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                ref replace, ref missing, ref missing, ref missing, ref missing);

                //Генерация даты написания заявления
                if (LeaveDate.AddDays(-14) <= DateStart)
                    DateStart = DateStart.AddDays(-14);
                range = (LeaveDate.AddDays(-14) - DateStart).Days;
                DateTime SignDate = DateStart.AddDays(gen.Next(range));
                findText = "<Дата написания заявления>";
                replaceWith = SignDate.ToShortDateString();

                app.Selection.Find.Execute(ref findText, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                ref replace, ref missing, ref missing, ref missing, ref missing);

                findText = "<Таб.№ работника>";
                replaceWith = employeeNumber;

                app.Selection.Find.Execute(ref findText, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref replaceWith,
                ref replace, ref missing, ref missing, ref missing, ref missing);

                app.Visible = false;
                //Сохраняем pdf
                object fileName2 = pdfFileName;
                object saveFormat = Word.WdSaveFormat.wdFormatPDF;
                try
                {
                    app.ActiveDocument.SaveAs2(ref fileName2,
                    ref saveFormat, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing);
                    
                    //Создаём архив и добавляем в него файл
                    string zipPath = newPdfFilePath + "\\" + chiefNumber + ".zip";
                    if (!File.Exists(zipPath))
                    {
                        string directoryPath = newPdfFilePath + "\\" + chiefNumber;
                        Directory.CreateDirectory(directoryPath);
                        ZipFile.CreateFromDirectory(directoryPath, zipPath);
                        Directory.Delete(directoryPath);
                    }
                    using (ZipArchive modFile = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                    {
                        modFile.CreateEntryFromFile((string)fileName2, employeeNumber + "_" + chiefNumber + ".pdf");
                    }
                }
                catch (Exception ex)
                {
                    OnWriteLog?.Invoke("\r\n"+ex.Message+"\r\n");
                    break;
                }
                File.Delete((string)fileName2);

                OnWriteLog?.Invoke(" OK\r\n");
                OnRecordProcessed?.Invoke();
            }

            Directory.Delete(newPdfFilePath + "\\" + tempDirectory);
            object saveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
            doc.Close(ref saveChanges, ref missing, ref missing);
            doc = null;
            app.Quit();

            OnGenerateAppForLeaveEnd?.Invoke();
        }
    }
}
