using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ApplicationForLeave
{
    class GenerateExcel
    {
        public delegate void GenerateExcelEnd();
        public event GenerateExcelEnd OnGenerateExcelEnd;

        public void Generate()
        {
            Task GenExcelTask = new Task(GenExcel);
            GenExcelTask.Start();
        }

        private void GenExcel()
        {
            const int employeesCount = 20000;
            const int chiefsCount = 5000;
            const int lowRange = 100000;
            int[] perm = Enumerable.Range(0, employeesCount + chiefsCount).ToArray();

            Random r = new Random();
            for (int i = employeesCount + chiefsCount - 1; i >= 1; i--)
            {
                int j = r.Next(i + 1);
                // exchange perm[j] and perm[i]
                int temp = perm[j];
                perm[j] = perm[i];
                perm[i] = temp;
            }

            var lsChiefs = new List<Chief>(chiefsCount);
            int k = 1;
            for (int i = employeesCount; i < employeesCount + chiefsCount; i++)
            {
                lsChiefs.Add(new Chief { ID = perm[i] + lowRange, Fio = "FIO Chief " + k.ToString() });
                k++;
            }

            var lsEmployees = new List<Employee>(employeesCount);
            for (int i = 0; i < employeesCount; i++)
            {
                int ind = r.Next(chiefsCount);
                lsEmployees.Add(new Employee
                {
                    ID = perm[i] + lowRange,
                    Fio = "FIO Employee " + (i + 1).ToString(),
                    Chief = lsChiefs[ind]
                });
            }

            var excelApp = new Excel.Application();
            // Make the object visible.
            excelApp.Visible = true;

            // Create a new, empty workbook and add it to the collection returned 
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template. 
            // Because no argument is sent in this example, Add creates a new workbook. 
            excelApp.Workbooks.Add();

            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            // Establish column headings in cells A1 and B1.
            workSheet.Cells[1, "A"] = "Таб № Работника";
            workSheet.Cells[1, "B"] = "ФИО Работника";
            workSheet.Cells[1, "C"] = "Таб № Руководителя";
            workSheet.Cells[1, "D"] = "ФИО Руководителя";

            try
            {
                var row = 1;
                foreach (var employee in lsEmployees)
                {
                    row++;
                    workSheet.Cells[row, "A"] = employee.ID;
                    workSheet.Cells[row, "B"] = employee.Fio;
                    workSheet.Cells[row, "C"] = employee.Chief.ID;
                    workSheet.Cells[row, "D"] = employee.Chief.Fio;
                }

                workSheet.Columns[1].AutoFit();
                workSheet.Columns[2].AutoFit();
                workSheet.Columns[3].AutoFit();
                workSheet.Columns[4].AutoFit();
            }
            catch { }

            OnGenerateExcelEnd?.Invoke();
        }
    }
}
