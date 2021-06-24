using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using LiteDB;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using MVC4ACAD.Core;

using MVC4ACAD.Models;

namespace MVC4ACAD{
    internal class Document:MyDocument{
        #region CONSTANTES
        #endregion

        #region PROPIEDADES
        internal override String AppName => App.Name;

        internal override BsonMapper BsonMapper{
            get{
                BsonMapper BsonMapper = new BsonMapper();
                
                //BsonMapper.IncludeFields = true;

                BsonMapper.EmptyStringToNull = false;

                BsonMapper.IncludeNonPublic = true;
                
                BsonMapper.Entity<MyCircle>()
                    .Id(T => T.Id);
                
                return BsonMapper;
                }
            }
        #endregion

        #region CONSTRUCTORS
        internal Document(Autodesk.AutoCAD.ApplicationServices.Document Document):base(Document){
            }
        #endregion

        #region FUNCTIONS
        internal MyCircle CreateMyCircle(Point3d Center, Double Radius){
            MyCircle MyCircle = new MyCircle(Center, Radius);

            try{
                using(LiteDatabase LiteDatabase = this.LiteDatabase){
                    LiteDatabase.GetCollection<MyCircle>("MyCircles").Insert(MyCircle);
                    }
                }
            catch(Exception Exception){
                Editor.WriteMessage("Exception: "+Exception.Message+" => "+Exception.StackTrace);
                }

            return MyCircle;
            }
        #endregion

        #region PROMPT
        internal PromptSelectionResult SeleccionaTrazos(String Mensaje, String MensajeDeReintento){
            PromptSelectionOptions PromptSelectionOptions = new PromptSelectionOptions();
            PromptSelectionOptions.AllowDuplicates = false;
            PromptSelectionOptions.AllowSubSelections = false;
            PromptSelectionOptions.ForceSubSelections = false;
            PromptSelectionOptions.MessageForAdding = Mensaje;
            PromptSelectionOptions.MessageForRemoval = MensajeDeReintento;
            PromptSelectionOptions.PrepareOptionalDetails = false;
            PromptSelectionOptions.RejectObjectsFromNonCurrentSpace = true;
            PromptSelectionOptions.RejectObjectsOnLockedLayers = true;
            PromptSelectionOptions.RejectPaperspaceViewport = true;
            PromptSelectionOptions.SelectEverythingInAperture = false;
            PromptSelectionOptions.SingleOnly = false;
            PromptSelectionOptions.SinglePickInSpace = false;

            return Document.Editor.GetSelection(PromptSelectionOptions, new SelectionFilter(new TypedValue[2]{new TypedValue(Convert.ToInt32(DxfCode.Start), "LINE"), new TypedValue(Convert.ToInt32(DxfCode.LayerName), "HTPTrazos")}));
            }
        #endregion
        }
    }
