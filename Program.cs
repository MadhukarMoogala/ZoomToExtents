using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomToExtents
{
    public static class Extension
{
    public static void ZoomExtents(this Database db)
    {
        if (db == null)
            throw new ArgumentNullException("db");

        db.TileMode = true;
        db.UpdateExt(false);
        using (var tr = new OpenCloseTransaction())
        {
            var view = (ViewportTableRecord)tr.GetObject(db.CurrentViewportTableRecordId, OpenMode.ForWrite);
            var ratio = view.Width / view.Height;
            var ext = new Extents3d(db.Extmin, db.Extmax);
            ext.TransformBy(view.WorldToEye());
            var lg = ext.MaxPoint.X - ext.MinPoint.X;
            var ht = ext.MaxPoint.Y - ext.MinPoint.Y;
            if (lg / ht < ratio)
            {
                view.Height = ht;
                view.Width = ht * ratio;
            }
            else
            {
                view.Height = lg / ratio;
                view.Width = lg;
            }
            view.CenterPoint = new Point2d(
                (ext.MaxPoint.X + ext.MinPoint.X) / 2.0,
                (ext.MaxPoint.Y + ext.MinPoint.Y) / 2.0);
            tr.Commit();
        }
    }

    public static Matrix3d WorldToEye(this AbstractViewTableRecord view)
    {
        if (view == null)
            throw new ArgumentNullException("view");
        return
            Matrix3d.WorldToPlane(view.ViewDirection) *
            Matrix3d.Displacement(view.Target.GetAsVector().Negate()) *
            Matrix3d.Rotation(view.ViewTwist, view.ViewDirection, view.Target);
    }
}
    internal class Program
    {
        
        static void Main(string[] args)
        {
        }
    }
}
