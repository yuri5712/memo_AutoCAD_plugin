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
    public class Test2Command
    {
        public int inputNumber = 0;

        [CommandMethod("Test2")]
        public void Test2()
        {
            // autoCAD file data
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            //var json = "";
            //ed.WriteMessage("hello world\n");

            // 数値の入力
            PromptStringOptions pStrOpts = new PromptStringOptions("Test2：数字を入力してください。\n");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = doc.Editor.GetString(pStrOpts);
            if (pStrRes.StringResult != (""))
            {
                inputNumber = int.Parse(pStrRes.StringResult);
            }

            ed.WriteMessage("Test2：入力されたのは" + inputNumber.ToString() + "\n");

            using (var tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                var opts = new PromptSelectionOptions();
                ed.SelectionAdded += new SelectionAddedEventHandler(OnCurveSelectionAdded);
                var res = ed.GetSelection(opts);
                if (res.Status == PromptStatus.OK)
                {
                    List<Polyline> extPolys = new List<Polyline>();
                    List<Polyline> intPolys = new List<Polyline>();

                    var selset = res.Value;
                    foreach (var val in selset)
                    {
                        var selobj = val as SelectedObject;
                        var objid = selobj.ObjectId;
                        var crv = tr.GetObject(objid, OpenMode.ForWrite) as Curve;

                        if (crv.Color.IsByLayer)
                        {
                            var layer = tr.GetObject(crv.LayerId, OpenMode.ForRead) as LayerTableRecord;
                            ed.WriteMessage("選択したオブジェクトはレイヤー色になっています。" + inputNumber.ToString() + "\n");
                        }
                        else
                        {
                            ed.WriteMessage("選択したオブジェクトはレイヤー色になっていません。" + inputNumber.ToString() + "\n");

                        }
                    }
                }
                else
                {
                    ed.WriteMessage("部材が選択されていません。\n");
                }

                tr.Commit();

                // terminate filter
                ed.SelectionAdded -= new SelectionAddedEventHandler(OnCurveSelectionAdded);
            }




            void OnCurveSelectionAdded(object sender, SelectionAddedEventArgs e)
            {
                try
                {
                    ObjectId[] ids = e.AddedObjects.GetObjectIds();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        if (!ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Curve))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Leader))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Line))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Ray))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Xline))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Arc))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Polyline))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Polyline2d))) &&
                            !ids[i].ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Polyline3d)))
                            )
                        {
                            e.Remove(i);
                        }
                        else if (ids[i] == null)
                        {
                            continue;
                        }
                        else
                        {
                            bool tooSmall = false;
                            if (tooSmall)
                            {
                                ed.WriteMessage("切削不可能な部材を除外しました。\n");
                                e.Remove(i);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    //ed.WriteMessage("Emarf: 部材が選択されていません。\n");
                }

            }

        }




    }



}