using PolygonEditor.Definitions;
using PolygonEditor.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditor.Modules
{
    /// <summary>
    /// Contains drwaing functions for Polygnos. Converts vector to raster graphics.
    /// </summary>
    public class DrawingPolygonModule
    {
        private Bitmap drawArea;
        private PictureBox drawAreaBox;
        public bool IsDuringTransaction { get; private set; } = false;

        public DrawingPolygonModule(Bitmap drawArea, PictureBox drawAreaBox)
        {
            this.drawArea = drawArea;
            this.drawAreaBox = drawAreaBox;
        }

        #region drawing transaction
        /// <summary>
        /// Begins the drawing transaction. After the transaction should end, call <see cref="FinishDrawingTransaction"/>.
        /// </summary>
        /// <returns>Is Transaction successfully beggined.</returns>
        /// <remarks>Use Drawing Transaction while invoking subsequent drawing functions that should result one visible result. It increases the performance and prevents from flickering.</remarks>
        /// <remarks></remarks>
        public bool BeginDrawingTransaction()
        {
            if (IsDuringTransaction)
                return false;
            IsDuringTransaction = true;
            return true;
        }

        /// <summary>
        /// Finishes the drawing transaction. Before the transaction should begin, call <see cref="BeginDrawingTransaction"/>.
        /// </summary>
        /// <returns>Is Transaction successfully finished.</returns>
        /// <remarks>Use Drawing Transaction while invoking subsequent drawing functions that should result one visible result. It increases the performance and prevents from flickering.</remarks>
        public bool FinishDrawingTransaction()
        {
            if (!IsDuringTransaction)
                return false;
            IsDuringTransaction = false;
            drawAreaBox.Refresh();
            return true;
        }
        #endregion

        #region drawing 
        public void DrawPolygonVertices(params VerticePoint[] vertices)
        {
            #region CheckArgs
            if (vertices == null || vertices.Length==0)
                return;
            #endregion 

            using (var g = Graphics.FromImage(drawArea))
            {
                Array.ForEach(vertices, v => v.DrawOnGraphics(g));
                if (!IsDuringTransaction)
                    drawAreaBox.Refresh();
            }
        }

        public void DrawPolygonEdges(params Line[] edges)
        {
            #region CheckArgs
            if (edges == null || edges.Length==0)
                return;
            #endregion 

            using (var g = Graphics.FromImage(drawArea))
            {
                Array.ForEach(edges, e=> e.DrawOnGraphics(g));
                if (!IsDuringTransaction)
                    drawAreaBox.Refresh();
            }
        }

        public void DrawPolygonBorders (Polygon p)
        {
            #region CheckArgs
            if (p == null)
                return;
            #endregion

            using (var g = Graphics.FromImage(drawArea))
            {
                p.GetVerticesList().ForEach( v => v.DrawOnGraphics(g));
                p.GetEdgesList().ForEach(e => e.DrawOnGraphics(g));
                if (!IsDuringTransaction)
                    drawAreaBox.Refresh();
            }
        }

        public void FillPolygon(Polygon p)
        {
            #region CheckArgs
            if (p == null)
                return;
            #endregion

            using (var b = new SolidBrush(Color.FromArgb(p.FillOpacity, p.FillColor)))
            {
                using (var g = Graphics.FromImage(drawArea))
                {
                    g.FillPolygon(b, p.GetVerticesList().Select(v => new PointF((float)v.X, (float)v.Y)).ToArray());
                    if (!IsDuringTransaction)
                        drawAreaBox.Refresh();
                }
            }
        }
        #endregion

        #region clearing
        public void ClearPolygonVertices(params VerticePoint[] vertices)
        {
            #region CheckArgs
            if (vertices == null || vertices.Length==0)
                return;
            #endregion 

            using (var g = Graphics.FromImage(drawArea))
            {
                Array.ForEach(vertices, v=> v.ClearFromGraphics(g));
                if(!IsDuringTransaction)
                    drawAreaBox.Refresh();
            }
        }

        public void ClearPolygonEdges(params Line[] edges)
        {
            #region CheckArgs
            if (edges == null || edges.Length==0)
                return;
            #endregion 

            using (var g = Graphics.FromImage(drawArea))
            {
                Array.ForEach(edges, e => e.ClearFromGraphics(g));
                if (!IsDuringTransaction)
                    drawAreaBox.Refresh();
            } 
        }

        public void ClearPolygonBorders(Polygon p, bool refreshDrawArea = true)
        {
            #region CheckArgs
            if (p == null)
                return;
            #endregion

            using (var g = Graphics.FromImage(drawArea))
            {
                p.GetVerticesList().ForEach(v => v.ClearFromGraphics(g));
                p.GetEdgesList().ForEach(e => e.ClearFromGraphics(g));
                if (!IsDuringTransaction)
                    drawAreaBox.Refresh();
            }
        }

        public void ClearPolygonFilling(Polygon p)
        {
            #region CheckArgs
            if (p == null)
                return;
            #endregion

            using (var b = new SolidBrush(p.BackgroudColor))
            {
                using (var g = Graphics.FromImage(drawArea))
                {
                    g.FillPolygon(b, p.GetVerticesList().Select(v => new PointF((float)v.X, (float)v.Y)).ToArray());
                    if (!IsDuringTransaction)
                        drawAreaBox.Refresh();
                }
            }
        }

        #endregion
    }
}
