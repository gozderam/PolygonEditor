using PolygonEditor.Definitions;
using PolygonEditor.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Line = PolygonEditor.Definitions.Line;
using Polygon = PolygonEditor.Definitions.Polygon;

namespace PolygonEditor.Utils
{
    public static class DrawingUtils
    { 
        #region Simple Shape Drawing
        /// <summary>
        /// Draws a point from the center.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        public static void DrawPoint(this Graphics g, VerticePoint p)
        {
            int drawingStartX = (int)(p.X - p.R);
            int drawingStartY = (int)(p.Y - p.R);
            g.FillEllipse(p.Brush, drawingStartX, drawingStartY, (int)(2*p.R), (int)(2*p.R));
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="l"></param>
        public static void DrawBresenhamLine(this Graphics g, Line l)
        {
            l.LinePoints.ForEach(p =>
            {
                g.DrawRectangle(l.Pen, p.X, p.Y, 1, 1);
            });
            DrawEdgeConstraintIcon(g, l);
            DrawEdgeConstraintText(g, l);
        }
        /// <summary>
        /// Clears a line.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="l"></param>
        public static void ClearBresenhamLine(this Graphics g, Line l)
        {
            using (var pen = new Pen(l.BackgroudColor, l.Pen.Width))
            l.LinePoints.ForEach(p =>
            {
                g.DrawRectangle(pen, p.X, p.Y, 1, 1);
            });
            ClearEdgeConstraintIcon(g, l);
            ClearEdgeConstraintText(g, l);
        }

        private static void DrawEdgeConstraintIcon(this Graphics g, Line l)
        {
            Point p = l.LinePoints[l.LinePoints.Count / 2];
            p.X += 4;
            p.Y += 4;
            if(l.Constraint!=null && l.Constraint.Icon!=null)
                g.DrawImage(l.Constraint.Icon, p.X, p.Y, l.Constraint.PrefferedIconSize.width, l.Constraint.PrefferedIconSize.height);
        }

        private static void ClearEdgeConstraintIcon(this Graphics g, Line l)
        {
            Point p = l.LinePoints[l.LinePoints.Count / 2];
            p.X += 4;
            p.Y += 4;
            using (var b = new SolidBrush(l.BackgroudColor))
            {
                if (l.Constraint != null && l.Constraint.Icon != null)
                    g.FillRectangle(b, p.X, p.Y, l.Constraint.PrefferedIconSize.width, l.Constraint.PrefferedIconSize.height);
            }
        }

        private static void DrawEdgeConstraintText(this Graphics g, Line l)
        {
            Point p = l.LinePoints[l.LinePoints.Count / 2];
            p.X += 4;
            p.Y += 4;
           
            using (var brush = new SolidBrush(Color.Beige))
            {
                using (var font = new Font("Calibri", 10))
                {
                    if (l.Constraint != null && l.Constraint.ConstraintText != null && l.Constraint.ConstraintText != "")
                    {
                        var textSize = TextRenderer.MeasureText(l.Constraint.ConstraintText, font);
                        g.FillRectangle(brush, p.X, p.Y, textSize.Width, textSize.Height);
                        g.DrawString(l.Constraint.ConstraintText, new Font("Calibri", 10), Brushes.Red, new Rectangle(p.X, p.Y, textSize.Width, textSize.Height));
                    }
                }
            }
        }

        private static void ClearEdgeConstraintText(this Graphics g, Line l)
        {
            Point p = l.LinePoints[l.LinePoints.Count / 2];
            p.X += 4;
            p.Y += 4;

            using (var brush = new SolidBrush(l.BackgroudColor))
            {
                using (var font = new Font("Calibri", 10))
                {
                    if (l.Constraint != null && l.Constraint.ConstraintText != null && l.Constraint.ConstraintText != "")
                    {
                        var textSize = TextRenderer.MeasureText(l.Constraint.ConstraintText, font);
                        g.FillRectangle(brush, p.X, p.Y, textSize.Width, textSize.Height);
                    }
                }
            }
        }
        #endregion
    }
}
