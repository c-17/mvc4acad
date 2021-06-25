using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using LiteDB;

namespace MVC4ACAD.Core{
    internal abstract class Document{
        #region PROPERTIES
        internal abstract String AppName{get;}

        internal abstract BsonMapper BsonMapper{get;}

        private String Buffer = String.Empty;

        private MemoryStream MemoryStream;

        protected LiteDatabase LiteDatabase{
            get{
                LiteDatabase LiteDatabase = new LiteDatabase(MemoryStream = MyMemoryStream, BsonMapper);

                LiteDatabase.OnDisposing += new EventHandler(OnDisposing);

                return LiteDatabase;
                }
            }

        private MemoryStream MyMemoryStream{
            get{
                MemoryStream MemoryStream = new MemoryStream();

                try{
                    if(this.Buffer.Length > 0){
                        MemoryStream = new MemoryStream();
                        
                        Byte[] Buffer = Convert.FromBase64String(this.Buffer);

                        MemoryStream.Write(Buffer, 0, Buffer.Length);
                        }
                    else{
                        using(Application.DocumentManager.MdiActiveDocument.LockDocument()){
                            using(Transaction Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()){
                                RegAppTable RegAppTable = Transaction.GetObject(HostApplicationServices.WorkingDatabase.RegAppTableId, OpenMode.ForWrite) as RegAppTable;

                                RegAppTableRecord RegAppTableRecord;

                                if(RegAppTable.Has(AppName)){
                                    RegAppTableRecord = Transaction.GetObject(RegAppTable[AppName], OpenMode.ForWrite) as RegAppTableRecord;

                                    if(HostApplicationServices.WorkingDatabase.TryGetObjectId(RegAppTableRecord.ExtensionDictionary.Handle, out Autodesk.AutoCAD.DatabaseServices.ObjectId ObjectId)){
                                        DBDictionary DBDictionary = Transaction.GetObject(ObjectId, OpenMode.ForWrite) as DBDictionary;
                        
                                        if(DBDictionary.Contains("Database")){
                                            Xrecord Xrecord = Transaction.GetObject(DBDictionary.GetAt("Database"), OpenMode.ForRead) as Xrecord;
                                    
                                            MemoryStream = new MemoryStream();

                                            this.Buffer = String.Join("", Xrecord.Data.AsArray().ToList().Select(TypedValue => Convert.ToString(TypedValue.Value)).ToList());

                                            Byte[] Buffer = Convert.FromBase64String(this.Buffer);

                                            MemoryStream.Write(Buffer, 0, Buffer.Length);
                                            }
                                        else{
                                            RegAppTableRecord = new RegAppTableRecord();

                                            RegAppTableRecord.Name = AppName;

                                            RegAppTable.Add(RegAppTableRecord);
                                            Transaction.AddNewlyCreatedDBObject(RegAppTableRecord, true);
                            
                                            RegAppTableRecord.CreateExtensionDictionary();

                                            //DBDictionary DBDictionary = Transaction.GetObject(RegAppTableRecord.ExtensionDictionary, OpenMode.ForWrite) as DBDictionary;
                        
                                            Xrecord Xrecord =  new Xrecord{Data = new ResultBuffer(Initialize(out MemoryStream))};
                    
                                            DBDictionary.SetAt("Database", Xrecord);
                
                                            Transaction.AddNewlyCreatedDBObject(Xrecord, true);
                                            }
                                        }
                                    }
                                else{
                                    RegAppTableRecord = new RegAppTableRecord();

                                    RegAppTableRecord.Name = AppName;

                                    RegAppTable.Add(RegAppTableRecord);
                                    Transaction.AddNewlyCreatedDBObject(RegAppTableRecord, true);
                            
                                    RegAppTableRecord.CreateExtensionDictionary();

                                    DBDictionary DBDictionary = Transaction.GetObject(RegAppTableRecord.ExtensionDictionary, OpenMode.ForWrite) as DBDictionary;
                        
                                    Xrecord Xrecord =  new Xrecord{Data = new ResultBuffer(Initialize(out MemoryStream))};
                    
                                    DBDictionary.SetAt("Database", Xrecord);
                
                                    Transaction.AddNewlyCreatedDBObject(Xrecord, true);
                                    }

                                Transaction.Commit();
                                }
                            }
                        }
                    }
                catch(Autodesk.AutoCAD.Runtime.Exception Exception){
                    Editor.WriteMessage("Exception: "+Exception.Message+" => "+Exception.StackTrace);
                    //Internet.SendException(Assembly.GetExecutingAssembly(), Exception, this);
                    }

                return MemoryStream;
                }
            }

        protected Autodesk.AutoCAD.ApplicationServices.Document ACADDocument{get;}

        internal Database Database => ACADDocument.Database;

        internal Editor Editor => ACADDocument.Editor;

        internal DocumentLock LockDocument => ACADDocument.LockDocument();
        #endregion
        
        #region CONSTRUCTORS
        internal Document(Autodesk.AutoCAD.ApplicationServices.Document ACADDocument){
            this.ACADDocument = ACADDocument;

            ACADDocument.Editor.CurrentUserCoordinateSystem = Matrix3d.Identity;

            ACADDocument.Database.LineWeightDisplay = true;

            //LiteDatabase = new LiteDatabase(MemoryStream = MyMemoryStream, BsonMapper);

            ACADDocument.Database.BeginSave += new DatabaseIOEventHandler(DatabaseIOEventHandler);
            }
        #endregion
        
        #region FUNCTIONS
        private TypedValue[] Initialize(out MemoryStream MemoryStream){
            MemoryStream = new MemoryStream();

            new LiteDatabase(MemoryStream, BsonMapper);

            Buffer = Convert.ToBase64String(MemoryStream.ToArray());

            List<String> Buffers = Buffer.Split(32000).ToList();

            return Buffers.Select(B => new TypedValue(Convert.ToInt32(DxfCode.ExtendedDataAsciiString), B)).ToArray();
            }
        
        internal Boolean Equals(Autodesk.AutoCAD.ApplicationServices.Document ACADDocument) => this.ACADDocument == ACADDocument;

        internal void EraseEntity(Entity Entity){
            if(Entity.IsErased)
                return;

            using(Transaction Transaction = ACADDocument.Database.TransactionManager.StartTransaction()){
                Transaction.GetObject(Entity.ObjectId, OpenMode.ForWrite).Erase(true);

                Transaction.Commit();
                }
            }

        internal void Backup(String File){
            using(LiteDatabase LiteDatabase = this.LiteDatabase){
                MemoryStream MemoryStream = new MemoryStream(this.MemoryStream.ToArray());

                using(FileStream FileStream = new FileStream(File, FileMode.OpenOrCreate)){
                    MemoryStream.CopyTo(FileStream);

                    FileStream.Flush();
                    }
                }
            }
        #endregion
        
        #region EVENTS
        private void OnDisposing(Object Object, EventArgs EventArgs){
            Buffer = Convert.ToBase64String(MemoryStream.ToArray());

            (Object as LiteDatabase).OnDisposing -= OnDisposing;
            }

        private void DatabaseIOEventHandler(Object Object, DatabaseIOEventArgs DatabaseIOEventArgs){
            Editor.WriteMessage("DatabaseIOEventHandler");

            List<String> Buffers = Convert.ToBase64String(MemoryStream.ToArray()).Split(32000).ToList();
            
            using(ACADDocument.LockDocument()){
                using(Transaction Transaction = Database.TransactionManager.StartTransaction()){
                    RegAppTable RegAppTable = Transaction.GetObject(Database.RegAppTableId, OpenMode.ForWrite) as RegAppTable;
                    RegAppTableRecord RegAppTableRecord = Transaction.GetObject(RegAppTable[AppName], OpenMode.ForWrite) as RegAppTableRecord;

                    if(Database.TryGetObjectId(RegAppTableRecord.ExtensionDictionary.Handle, out Autodesk.AutoCAD.DatabaseServices.ObjectId ObjectId)){
                        DBDictionary DBDictionary = Transaction.GetObject(ObjectId, OpenMode.ForWrite) as DBDictionary;
                        
                        if(DBDictionary.Contains("Database"))
                            Transaction.GetObject(DBDictionary.GetAt("Database"), OpenMode.ForWrite).Erase(true);

                        Xrecord Xrecord = new Xrecord{Data = new ResultBuffer(Buffers.Select(B => new TypedValue(Convert.ToInt32(DxfCode.ExtendedDataAsciiString), B)).ToArray())};
                    
                        DBDictionary.SetAt("Database", Xrecord);
                
                        Transaction.AddNewlyCreatedDBObject(Xrecord, true);
                        }

                    Transaction.Commit();
                    }
                }
            }
        #endregion

        #region PROMPT
        internal PromptPointResult IndicaPunto(String Mensaje, Boolean Desplazamiento = false, String[] PalabrasClave = null, String PalabraClavePredeterminada = ""){
            PromptPointOptions PromptPointOptions = new PromptPointOptions("\n"+Mensaje);
            PromptPointOptions.AllowArbitraryInput = false;
            PromptPointOptions.LimitsChecked = false;
            PromptPointOptions.AllowNone = false;
            PromptPointOptions.UseBasePoint = false;
            PromptPointOptions.UseDashedLine = Desplazamiento;

            if(PalabrasClave != null){
                PromptPointOptions.AppendKeywordsToMessage = true;

                foreach(String PalabraClave in PalabrasClave){
                    PromptPointOptions.Keywords.Add(PalabraClave);
                    }

                if(!PalabraClavePredeterminada.Equals(String.Empty))
                    PromptPointOptions.Keywords.Default = PalabraClavePredeterminada;

                PromptPointOptions.AllowNone = true;
                }

            return ACADDocument.Editor.GetPoint(PromptPointOptions);
            }

        internal PromptPointResult IndicaPunto(String Mensaje, Point3d PuntoBase, Boolean Desplazamiento = false, String[] PalabrasClave = null, String PalabraClavePredeterminada = ""){
            PromptPointOptions PromptPointOptions = new PromptPointOptions("\n"+Mensaje);
            PromptPointOptions.AllowArbitraryInput = false;
            PromptPointOptions.LimitsChecked = false;
            PromptPointOptions.AllowNone = false;
            PromptPointOptions.BasePoint = PuntoBase;
            PromptPointOptions.UseBasePoint = true;
            PromptPointOptions.UseDashedLine = Desplazamiento;

            if(PalabrasClave != null){
                PromptPointOptions.AppendKeywordsToMessage = true;

                foreach(String PalabraClave in PalabrasClave){
                    PromptPointOptions.Keywords.Add(PalabraClave);
                    }

                if(!PalabraClavePredeterminada.Equals(String.Empty))
                    PromptPointOptions.Keywords.Default = PalabraClavePredeterminada;

                PromptPointOptions.AllowNone = true;
                }

            return ACADDocument.Editor.GetPoint(PromptPointOptions);
            }
        #endregion
        }
    }
