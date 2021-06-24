using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace MVC4ACAD.Controllers{
    public sealed class MyCircles{
        internal const String MVC4ACAD1 = "MVC4ACAD1";
        internal const String MVC4ACAD2 = "MVC4ACAD2";
        internal const String MVC4ACAD3 = "MVC4ACAD3";
        internal const String MVC4ACAD4 = "MVC4ACAD4";
        
        [CommandMethod(MVC4ACAD1)]
        public static void MVC4ACAD1_F(){
            Document Document = App.CurrentDocument;
            
            for(Int32 i=0;i<100000;i++)
                Document.CreateMyCircle(new Point3d(i, i, 0), (i+1));
            }
        }
    }
