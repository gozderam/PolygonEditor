using PolygonEditor.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditor.Definitions
{
    /// <summary>
    /// Represents a point - vertice of rectangle.
    /// Changes are automatically synchronized with incident lines. 
    /// </summary>
    public class VerticePoint : ISimpleShape
    {
        private double x;
        private double y;

        public static Color DefaultVerticeColor { get; set; } = Color.Black;
        public static int DefaultVerticeRadius { get; set; } = 4;
        public string Id { get; set; }
        public Color BackgroudColor { get; set; } = Color.White;
        public Polygon Parent { get; set; }

        public double X
        {
            get
            {
                return x;
            }
            private set
            {
                 x = value;
                 SynchronizeIncidentEdges();

            }
        }
        public double Y
        {
            get
            {
                return y;
            }
            private set
            {

                 y = value;
                 SynchronizeIncidentEdges();
            }
        }
        public double R { get; }
        public SolidBrush Brush { get; }

        /// <summary>
        /// Edge v_(i-1)v_i for vertice v_i in polygon.
        /// </summary>
        /// <remarks>Note that if the verticePoint isn't the part of a line, FirstIncidentEdge is null.</remarks>
        public Line FirstIncidentEdge { get; set; } = null;

        /// <summary>
        /// Edge v_(i)v_(i+1) for vertice v_i in polygon.
        /// </summary>
        /// <remarks>Note that if the verticePoint isn't the part of a line, SecondIncidentEdge is null. </remarks>
        public Line SecondIncidentEdge { get; set; } = null;


        public VerticePoint(double x, double y, Polygon parent = null, Color? color = null, double? r = null)
        {
            Id = Id = AppUtils.GenerateVerticeId();
            this.X = x;
            this.Y = y;
            this.R = r == null ? DefaultVerticeRadius : (int)r;
            this.Brush = new SolidBrush(color==null? DefaultVerticeColor : (Color)color);
            Parent = parent; 
        }

        #region ISimpleShape
        public void DrawOnGraphics(Graphics g)
        {
            g.DrawPoint(this);
        }

        public void ClearFromGraphics(Graphics g)
        {
            var color = this.Brush.Color;
            this.Brush.Color = BackgroudColor;
            g.DrawPoint(this);
            this.Brush.Color = color;
        }

        public bool IsPointInHitArea(double x, double y)
        {
            return new Rectangle((int)(X - R), (int)(Y - R), (int)(2 * R), (int)(2 * R)).Contains((int)x, (int)y);
        }
        #endregion

        #region moving
        /// <summary>
        /// Moves the vertice to (<paramref name="x"/>, <paramref name="y"/>) and checks incident edges constraints.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <remarks>Vertice does not have to be moved in (<paramref name="x"/>, <paramref name="y"/>), because of the constraints. The destination point can differ. </remarks>


        public void Move((double x, double y) vector)
        {
            this.x += vector.x;
            this.y += vector.y;

            // polygon is being builded
            if (!IsPolygonPart())
            {
                FirstIncidentEdge?.UpdateLinePosition();
                return;
            }

            AlgorithmsUtils.ApplyPolygonConstraitnsAfterVerticeMove(this, vector);
        }

        //public void Move(double x, double y)
        //{

        //if(FirstIncidentEdge != null && !(FirstIncidentEdge.Constraint is FixedLengthConstraint))
        //{
        //    if (CheckIncidentEdgesConstraint(x, Y).Success)
        //        this.x = x;
        //    if (CheckIncidentEdgesConstraint(X, y).Success)
        //        this.y = y;
        //    SynchronizeIncidentEdges();
        //}

        //if (FirstIncidentEdge != null && FirstIncidentEdge.Constraint is FixedLengthConstraint)
        //{
        //    var ret = C_c(FirstIncidentEdge, x, y);
        //    this.x = ret.x;
        //    this.y = ret.y;
        //    SynchronizeIncidentEdges();
        //}

        //}

        /// <summary>
        /// Increases X coordinate by <paramref name="addX"/>.
        /// </summary>
        /// <param name="addX"></param>
        /// <remarks>This function does not checks edges constraints. Use <see cref="Move(double, double)"/> to check edges' constraints before moving. </remarks>
        public void IncreaseX(double addX)
        {
            X += addX;
        }
        /// <summary>
        /// Icreases Y coordinate by <paramref name="addY"/>.
        /// </summary>
        /// <param name="addY"></param>
        /// <remarks>This function does not checks edges constraints. Use <see cref="Move(double, double)"/> to check edges' constraints before moving. </remarks>

        public void IncreaseY(double addY)
        {
            Y += addY;
        }
        #endregion

        #region private methods
        private void SynchronizeIncidentEdges()
        {
            FirstIncidentEdge?.UpdateLinePosition();
            SecondIncidentEdge?.UpdateLinePosition();
        }
        private ConstraintOperationResult CheckIncidentEdgesConstraint(double newX, double newY)
        {
            if (FirstIncidentEdge != null)
            {
                var checkRes = FirstIncidentEdge.CheckConstraint(FirstIncidentEdge.start.X, FirstIncidentEdge.end.Y, newX, newY);
                if (!checkRes.Success)
                    return checkRes;
            }
            if (SecondIncidentEdge != null)
            {
                var checkRes = SecondIncidentEdge.CheckConstraint(newX, newY, SecondIncidentEdge.end.X, SecondIncidentEdge.end.Y);
                if (!checkRes.Success)
                    return checkRes;
            }

            return new ConstraintOperationResult(true, ConstraintOperationKind.Check);
        }
        public bool IsPolygonPart()
        {
            if (SecondIncidentEdge == null || FirstIncidentEdge == null || SecondIncidentEdge.start==null 
                || SecondIncidentEdge.end==null || FirstIncidentEdge.start==null || FirstIncidentEdge.end==null)
                return false;
            return true;
        }

        private (double x, double y) C_c(Line e, double mouseX, double mouseY)
        {
            // assume from start to end
            var edgeL = (e.start.X - e.end.X) * (e.start.X - e.end.X) + (e.start.Y - e.end.Y) * (e.start.Y - e.end.Y);
            var nL = (e.start.X - mouseX) * (e.start.X - mouseX) + (e.start.Y - mouseY) * (e.start.Y - mouseY);
            var p = Math.Sqrt(edgeL / nL);
            var x = (p * (mouseX - e.start.X) + e.start.X);
            var y = (p * (mouseY - e.start.Y) + e.start.Y);
            return (x,y);
        }
        #endregion

    }
}
