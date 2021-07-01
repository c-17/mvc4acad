using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using MVC4ACAD.Models;

namespace MVC4ACAD.Controllers{
    public sealed class MyCircles{
        internal const String MVC4ACAD_C = "MVC4ACAD_C";
        internal const String MVC4ACAD_R = "MVC4ACAD_R";
        internal const String MVC4ACAD_U = "MVC4ACAD_U";
        internal const String MVC4ACAD_D = "MVC4ACAD_D";
        
        [CommandMethod(MVC4ACAD_C)]
        public static void MVC4ACAD_C_F(){
            Document Document = App.CurrentDocument;
            
            for(Int32 i=0;i<100;i++){
                MyCircle MyCircle = new MyCircle(new Point3d(i, i, 0), (i+1));

                Document.CreateMyCircle(MyCircle);
                }
            }
        
        [CommandMethod(MVC4ACAD_R)]
        public static void MVC4ACAD_R_F(){
            Document Document = App.CurrentDocument;

            MyCircle MyCircle = Document.SelectMyCircle("Select a MyCircle", "Must select a MyCircle");

            if(MyCircle != null)
                Document.Editor.WriteMessage("MyCircle Location: "+MyCircle.Center.ToString());
            }
        
        [CommandMethod(MVC4ACAD_U)]
        public static void MVC4ACAD_U_F(){
            Document Document = App.CurrentDocument;

            MyCircle MyCircle = Document.SelectMyCircle("Select a MyCircle", "Must select a MyCircle");
            }
        
        [CommandMethod(MVC4ACAD_D)]
        public static void MVC4ACAD_D_F(){
            Document Document = App.CurrentDocument;
            }
        }
    }
