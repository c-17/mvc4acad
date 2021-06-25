using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;

namespace MVC4ACAD{
    public class App: IExtensionApplication{
        #region PROPERTIES
        internal const String Name = "MVC-4-ACAD";

        internal static List<Document> Documents = new List<Document>();

        internal static Document CurrentDocument => App.Documents.Find(D => D.Equals(Application.DocumentManager.MdiActiveDocument));
        #endregion

        #region CONSTRUCTORS
        public void Initialize(){
            LoadDocument(Application.DocumentManager.MdiActiveDocument);

            Application.DocumentManager.DocumentCreated += new DocumentCollectionEventHandler(DocumentCreated);

            Application.DocumentManager.DocumentToBeDestroyed += new DocumentCollectionEventHandler(DocumentToBeDestroyed);

            Application.DocumentManager.DocumentToBeDestroyed += new DocumentCollectionEventHandler(DocumentToBeDestroyed);

            Autodesk.AutoCAD.Ribbon.RibbonServices.RibbonPaletteSetCreated += new EventHandler(delegate{
                Autodesk.AutoCAD.Ribbon.RibbonServices.RibbonPaletteSet.Load += new Autodesk.AutoCAD.Windows.PalettePersistEventHandler(delegate{
                    //Ribbon.CreateRibbon(true);
                    });
                });

            Application.Idle += new EventHandler(LoadDocument);

            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("MVC4ACAD is Running...");
            }

        public void Terminate(){}
        #endregion

        #region FUNCTIONS
        private void LoadDocument(Autodesk.AutoCAD.ApplicationServices.Document Document){
            if(Document != null){
                if(!Documents.Exists(D => D.Equals(Document)))
                    Documents.Add(new Document(Document));
                }
            }
        #endregion

        #region EVENTS
        private void DocumentCreated(Object Object, DocumentCollectionEventArgs DocumentCollectionEventArgs) => LoadDocument(DocumentCollectionEventArgs.Document);
        
        private void DocumentToBeDestroyed(Object Object, DocumentCollectionEventArgs DocumentCollectionEventArgs) => Documents.Remove(Documents.Find(D => D.Equals(DocumentCollectionEventArgs.Document)));

        private void LoadDocument(Object Object, EventArgs EventArgs){
            Application.Idle -= LoadDocument;

            LoadDocument(Application.DocumentManager.MdiActiveDocument);
            }
        #endregion
        }
    }
