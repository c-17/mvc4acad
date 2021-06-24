using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace MVC4ACAD.Controllers{
    public sealed class Soporte{
        
        [CommandMethod("MVC4ACAD_DB")]
        public static void HTPDU_DB(){
            App.CurrentDocument.Dump("C:\\mvc4acad.db");
            }
        }
    }
