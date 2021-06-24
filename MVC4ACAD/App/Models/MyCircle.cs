using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;

using MVC4ACAD.Core;

using LiteDB;

namespace MVC4ACAD.Models{
    internal class MyCircle{

        [BsonIgnore]
        private Circle Circle;

        #region PROPERTIES
        [BsonId]
        internal Int64 Id{get; set;}

        internal Int64 Handle{get; set;}

        [BsonIgnore]
        internal Double Radius => Circle.Radius;
        
        internal Point3d Center => Circle.Center;
        #endregion

        #region CONSTRUCTORS
        [BsonCtor]
        public MyCircle(){}

        internal MyCircle(Point3d Center, Double Radius){
            using(Transaction Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()){
                BlockTable BlockTable = Transaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BlockTableRecord = Transaction.GetObject(BlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Circle = new Circle(Center, Vector3d.ZAxis, Radius);

                BlockTableRecord.AppendEntity(Circle);
                Transaction.AddNewlyCreatedDBObject(Circle, true);

                Id = Handle = Circle.Handle.Value;

                Transaction.Commit();
                }
            }
        #endregion

        #region FUNCTIONS
        /*internal static ILiteCollection<Pozo> IncludeAll(LiteDatabase LiteDatabase){
            return LiteDatabase.GetCollection<Pozo>("Pozos")
                .Include(P => P.Red)
                
                .Include(P => P.Red.EstiloDePozos)
                .Include(P => P.Red.EstiloDePozosEtiquetas)
                .Include(P => P.Red.EstiloDePozosNumeracion)
                
                .Include(P => P.Red.EstiloDeTramos)
                .Include(P => P.Red.EstiloDeTramosEtiquetas)
                .Include(P => P.Red.EstiloDeTramosSentidoDeFlujo)
                .Include(P => P.Red.EstiloDeTramosCaidasEtiquetas)
                    
                .Include(P => P.Red.EstiloDeSuperficiesAhora)
                .Include(P => P.Red.EstiloDeSuperficiesEtiquetasAhora)
                .Include(P => P.Red.EstiloDeSuperficiesCaucesEnTerrenoAhora)
                .Include(P => P.Red.EstiloDeSuperficiesCaucesNaturalesAhora)
                    
                .Include(P => P.Red.EstiloDeSuperficiesAntes)
                .Include(P => P.Red.EstiloDeSuperficiesEtiquetasAntes)
                .Include(P => P.Red.EstiloDeSuperficiesCaucesEnTerrenoAntes)
                .Include(P => P.Red.EstiloDeSuperficiesCaucesNaturalesAntes)
                    
                .Include(P => P.Red.EstiloDeTextoDePozosEtiquetas)
                .Include(P => P.Red.EstiloDeTextoDePozosNumeracion)
                    
                .Include(P => P.Red.EstiloDeTextoDeTramosEtiquetas)
                .Include(P => P.Red.EstiloDeTextoDeTramosCaidasEtiquetas)
                    
                .Include(P => P.Red.EstiloDeTextoDeSuperficiesEtiquetasAhora)
                    
                .Include(P => P.Red.EstiloDeTextoDeSuperficiesEtiquetasAntes)
                
                .Include(P => P.Red.Tramos)

                .Include(P => P.Red.Pozos)

                .Include(P => P.Tramos);
            }

        internal static ILiteCollection<Pozo> Include(LiteDatabase LiteDatabase){
            return LiteDatabase.GetCollection<Pozo>("Pozos")
                .Include(P => P.Red)
                .Include(P => P.Tramos);
            }

        internal Pozo Inicializar(Documento Documento){
            Red.Inicializar(Documento);

            return this;
            }

        internal Pozo Inicializar(Red Red){
            this.Red = Red;
            
            try{
                using(Transaction Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()){
                    BlockTable BlockTable = Transaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord BlockTableRecord = Transaction.GetObject(BlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    if(HostApplicationServices.WorkingDatabase.TryGetObjectId(new Handle(Handle), out Autodesk.AutoCAD.DatabaseServices.ObjectId ObjectId) && !ObjectId.IsErased){
                        Circle = Transaction.GetObject(ObjectId, OpenMode.ForWrite) as Circle;
                    
                        Circle.Layer = Red.Layer.Name;
                        }
                    else{
                        Circle = new Circle(Centro, Vector3d.ZAxis, Red.RadioDePozos);

                        Circle.Layer = Red.Layer.Name;
                    
                        Red.EstiloDePozos.Aplicar(Circle);

                        BlockTableRecord.AppendEntity(Circle);
                        Transaction.AddNewlyCreatedDBObject(Circle, true);

                        Handle = Circle.Handle.Value;
                        }

                    Transaction.Commit();
                    }
                }
            catch(Autodesk.AutoCAD.Runtime.Exception Exception){
                Internet.SendException(Assembly.GetExecutingAssembly(), Exception, this);
                }
            
            EtiquetaDeIdentificacion = new EtiquetaDeIdentificacionClass(this);
            
            EtiquetaDeTipo = new EtiquetaDeTipoClass(this);

            EtiquetaDePropiedades = new EtiquetaDePropiedadesClass(this);

            Red.Documento.ActualizarPozo(this);

            return this;
            }

        internal Pozo Actualizar(Boolean ConservarCambiosDeUsuario = true, Boolean Reposicionar = false){
            try{
                using(Transaction Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()){
                    Circle = Transaction.GetObject(Circle.ObjectId, OpenMode.ForWrite) as Circle;

                    Circle.Layer = Red.Layer.Name;

                    if(Red.Layer.IsOn){
                        if(ConservarCambiosDeUsuario){
                            if(!Red.EstiloDePozos.TieneCambiosDeUsuario(Circle)){
                                Red.EstiloDePozos.Aplicar(Circle);

                                Circle.Radius = Red.RadioDePozos;
                                }
                            }
                        else{
                            Red.EstiloDePozos.Aplicar(Circle);

                            Circle.Radius = Red.RadioDePozos;
                            }
                        }

                    //if(TienePozoDeRegulacion)
                    //    Circle.Visible = false;

                    Transaction.Commit();
                    }
                }
            catch(Autodesk.AutoCAD.Runtime.Exception Exception){
                Internet.SendException(Assembly.GetExecutingAssembly(), Exception, this);
                }

            EtiquetaDeIdentificacion.Actualizar(ConservarCambiosDeUsuario, Reposicionar);
            
            EtiquetaDeTipo.Actualizar(ConservarCambiosDeUsuario, Reposicionar);
            
            EtiquetaDePropiedades.Actualizar(ConservarCambiosDeUsuario, Reposicionar);

            return this;
            }

        internal Boolean Contiene(Int64 Handle) => (this.Handle == Handle || EtiquetaDeIdentificacionHandle == Handle || EtiquetaDeTipoHandle == Handle || EtiquetaDePropiedadesMLeaderHandle == Handle || EtiquetaDePropiedadesLineHandle == Handle || EtiquetaDePropiedadesMTextElevacionHandle == Handle || EtiquetaDePropiedadesMTextProfundidadHandle == Handle || EtiquetaDePropiedadesMTextArrastreHandle == Handle);*/
        #endregion
        }
    }
