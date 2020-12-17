using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using AcRx = Autodesk.AutoCAD.Runtime;
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Colors;

namespace AutoCAD_test
{
    public class Test1Command
    {
        public int inputNumber = 0;

        [CommandMethod("Test1")]
        public void Test1()
        {
            // autoCAD file data
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            //var json = "";
            //ed.WriteMessage("hello world\n");

            // 数値の入力
            PromptStringOptions pStrOpts = new PromptStringOptions("Test1：数字を入力してください。\n");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = doc.Editor.GetString(pStrOpts);
            if (pStrRes.StringResult != (""))
            {
                inputNumber = int.Parse(pStrRes.StringResult);
            }

            ed.WriteMessage("Test1：入力されたのは" + inputNumber.ToString() + "\n");

            using (var tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                var opts = new PromptSelectionOptions();
                //ed.SelectionAdded += new SelectionAddedEventHandler(OnCurveSelectionAdded);
                var res = ed.GetSelection(opts);
                if (res.Status == PromptStatus.OK)
                {
                    List<Polyline> extPolys = new List<Polyline>();
                    List<Polyline> intPolys = new List<Polyline>();

                    var selset = res.Value;
                    foreach (var val in selset)
                    {
                        ed.WriteMessage("選択したオブジェクト:" + val.GetType().ToString() + "\n");

                    }
                }
                else
                {
                    ed.WriteMessage("部材が選択されていません。\n");
                }

                tr.Commit();

                //// terminate filter
                //ed.SelectionAdded -= new SelectionAddedEventHandler(OnCurveSelectionAdded);
            }




        }




    }



}