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
    internal class Document:Core.Document{
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
                    .Id(E => E.Id);
                
                return BsonMapper;
                }
            }
        #endregion

        #region CONSTRUCTORS
        internal Document(Autodesk.AutoCAD.ApplicationServices.Document Document):base(Document){}
        #endregion

        #region FUNCTIONS
        internal MyCircle CreateMyCircle(MyCircle MyCircle){
            try{
                using(LiteDatabase LiteDatabase = this.LiteDatabase)
                    LiteDatabase.GetCollection<MyCircle>("MyCircles").Insert(MyCircle);
                }
            catch(Exception Exception){
                Editor.WriteMessage("Exception: "+Exception.Message+" => "+Exception.StackTrace);
                }

            return MyCircle;
            }

        private MyCircle ReadMyCircle(Int64 Handle){
            try{
                using(LiteDatabase LiteDatabase = this.LiteDatabase){
                    MyCircle MyCircle = LiteDatabase.GetCollection<MyCircle>("MyCircles").Query().Where(E => E.Id == Handle).FirstOrDefault();

                    if(MyCircle != null)
                        MyCircle.Init();

                    return MyCircle;
                    }
                }
            catch(Exception Exception){
                Editor.WriteMessage("Exception: "+Exception.Message+" => "+Exception.StackTrace);
                }

            return null;
            }

        internal MyCircle UpdateMyCircle(MyCircle MyCircle){
            try{
                using(LiteDatabase LiteDatabase = this.LiteDatabase)
                    LiteDatabase.GetCollection<MyCircle>("MyCircles").Update(MyCircle);
                }
            catch(Exception Exception){
                Editor.WriteMessage("Exception: "+Exception.Message+" => "+Exception.StackTrace);
                }

            return MyCircle;
            }

        internal Boolean DeleteMyCircle(MyCircle MyCircle){
            try{
                using(LiteDatabase LiteDatabase = this.LiteDatabase)
                    return LiteDatabase.GetCollection<MyCircle>("MyCircles").Delete(new BsonValue(MyCircle.Id));
                }
            catch(Exception Exception){
                Editor.WriteMessage("Exception: "+Exception.Message+" => "+Exception.StackTrace);
                }

            return false;
            }
        #endregion

        #region PROMPT
        internal MyCircle SelectMyCircle(String Message, String RejectMessage, String[] Words = null, String Word = null){
            PromptEntityOptions PromptEntityOptions = new PromptEntityOptions("\n"+Message);
            PromptEntityOptions.AllowObjectOnLockedLayer = true;
            PromptEntityOptions.AllowNone = true;
            PromptEntityOptions.SetRejectMessage("\n"+RejectMessage);

            if(Words != null){
                PromptEntityOptions.AppendKeywordsToMessage = true;
                
                Words.ToList().ForEach(K => PromptEntityOptions.Keywords.Add(K));

                if(Word != null)
                    PromptEntityOptions.Keywords.Default = Word;
                }

            PromptEntityResult PromptEntityResult = Editor.GetEntity(PromptEntityOptions);

            return ReadMyCircle(PromptEntityResult.ObjectId.Handle.Value);
            }
        #endregion
        }
    }
