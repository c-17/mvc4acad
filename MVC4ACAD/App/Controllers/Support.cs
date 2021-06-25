using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using Autodesk.AutoCAD.Windows;

namespace MVC4ACAD.Controllers{
    public sealed class Soporte{
        
        [CommandMethod("MVC4ACAD_DB")]
        public static void HTPDU_DB(){
            //App.CurrentDocument.Dump("C:\\mvc4acad.db");

            Microsoft.Win32.SaveFileDialog SaveFileDialog = new Microsoft.Win32.SaveFileDialog();

            SaveFileDialog.FileName = "mvc4acad"; // Default file name
            SaveFileDialog.DefaultExt = ".db"; // Default file extension
            SaveFileDialog.Filter = "Text documents (.db)|*.db"; // Filter files by extension

            if((SaveFileDialog.ShowDialog() ?? false))
                App.CurrentDocument.Backup(SaveFileDialog.FileName);
            }
        }
    }
