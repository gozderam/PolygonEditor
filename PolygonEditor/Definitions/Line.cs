using PolygonEditor.CustomControls;
using PolygonEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Definitions
{
    /// <summary>
    /// Represents a line - edge of polygon.
    /// Changes are automatically synchronzied with incident vertices.
    /// </summary>
    public class Line : ISimpleShape
    {
        public static Color DefaultLineColor { get; set; } = Color.Black;
        public static int DefaultLineThickness { get; set; } = 1;

        public string Id { get; set; }
        public Color BackgroudColor { get; set; } = Color.White;
        public Polygon Parent { get; set; }
        public readonly VerticePoint start;
        public readonly VerticePoint end;
        public List<Point> LinePoints { get; private set;  }
        public Pen Pen { get; set; }

        public IEdgeConstraint Constraint { get; set; } = null;

        public double Length
        {
            get
            {
                return Math.Sqrt((start.X - end.X) * (start.X - end.X) + (start.Y - end.Y) * (start.Y - end.Y));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pen"></param>
        /// <param name="parent"></param>
        /// <remarks>Updates informationa about incident edges for <paramref name="start"/> nad <paramref name="end"./></remarks>
        public Line(VerticePoint start, VerticePoint end, Polygon parent = null, Color? color = null, int? thickness = null)
        {
            Id = AppUtils.GenerateEdgeId();
            this.start = start;
            this.end = end;
            SetSelfAsIncidentEdgeforVetrices();
            Pen = new Pen(color == null ? DefaultLineColor : (Color)color, thickness == null? DefaultLineThickness : (int)thickness);
            UpdateLinePosition();
            Parent = parent;
        }

        #region ISimpleShape
        public void DrawOnGraphics(Graphics g)
        {
            g.DrawBresenhamLine(this);
        }

        public void ClearFromGraphics(Graphics g)
        {
            g.ClearBresenhamLine(this);
        }
        public bool IsPointInHitArea(double x, double y)
        {
            //TODO hitarea for lines
            int d = 2;
            return LinePoints.Any(p => ((p.X - x)*(p.X-x) + (p.Y-y)*(p.Y-y))<=d*d /*p.X == x && p.Y == y*/);
        }
        #endregion

        #region moving
        public void Move((double x, double y) vector)
        {

            this.start.IncreaseX(vector.x);
            this.start.IncreaseY(vector.y);
            this.end.IncreaseX(vector.x);
            this.end.IncreaseY(vector.y);

            AlgorithmsUtils.ApplyPolygonConstraitnsAfterEdgeMove(this, vector);

        }
        #endregion

        #region updating line points
        public void UpdateLinePosition()
        {
            if (end != null && start != null)
                LinePoints = AlgorithmsUtils.GetPointsOnLine((int)start.X, (int)start.Y, (int)end.X, (int)end.Y).ToList();
        }
        #endregion

        #region constraints
        public ConstraintOperationResult CanAddConstraintAndApply(IEdgeConstraint constraint, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            return constraint.CanAddConstraintAndCheckOf(this, newStartX, newStartY, newEndX, newEndY);
        }
        public ConstraintOperationResult AddConstraintAndApplyWithCheck(IEdgeConstraint constraint, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            var canAddAndApply = CanAddConstraintAndApply(constraint, newStartX, newStartY, newEndX, newEndY);
            if(!canAddAndApply.Success)
            {
                return new ConstraintOperationResult(false, ConstraintOperationKind.AddAndApplyWithCheck, canAddAndApply.Message);
            }

            // add
            Constraint = constraint;

            // apply
            (double x, double y) startVector = (newStartX - start.X, newStartY - start.Y);
            (double x, double y) endVector = (newEndX - end.X, newEndY - end.Y);
            start.IncreaseX(startVector.x);
            start.IncreaseY(startVector.y);
            end.IncreaseX(endVector.x);
            end.IncreaseY(endVector.y);

            AlgorithmsUtils.ApplyPolygonConstraitnsAfterVerticeMove(this.start, startVector);
            AlgorithmsUtils.ApplyPolygonConstraitnsAfterVerticeMove(this.end, endVector);

            return new ConstraintOperationResult(true, ConstraintOperationKind.AddAndApplyWithCheck);
        }
        public ConstraintOperationResult CheckConstraint(double newStartX, double newStartY, double newEndX, double newEndY)
        {
            if (Constraint == null)
            {
                return new ConstraintOperationResult(true, ConstraintOperationKind.Check);
            }
            return Constraint.CheckOf(this, newStartX, newStartY, newEndX, newEndY);
        }
        public ConstraintOperationResult ApplyWithConstraintCheck(double newStartX, double newStartY, double newEndX, double newEndY)
        {
            var constraintCheck = CheckConstraint(newStartX, newStartY, newEndX, newEndY);
            if (!constraintCheck.Success)
            {
                return new ConstraintOperationResult(false, ConstraintOperationKind.ApplyWithCheck, constraintCheck.Message);
            }

            // apply
            //start.Move((newStartX - start.X, newStartY - start.Y));
            //start.Move((newEndX - end.X, newEndY - end.Y));
            start.IncreaseX(newStartX - start.X);
            start.IncreaseY(newStartY - start.Y);
            end.IncreaseX(newEndX - end.X);
            end.IncreaseY(newEndY - end.Y);

            return new ConstraintOperationResult(true, ConstraintOperationKind.ApplyWithCheck);
        }
        public ConstraintOperationResult CanDeleteConstraint()
        {
            if(Constraint==null)
            {
                return new ConstraintOperationResult(false, ConstraintOperationKind.CanDelete, TEXTS.EMPTY_CONSTRAINT_CANNOT_DELETE);
            }
            return new ConstraintOperationResult(true, ConstraintOperationKind.CanDelete);
        }
        public ConstraintOperationResult DeleteConstraintWithCheck()
        {
            var canDelete = CanDeleteConstraint();
            if(!canDelete.Success)
            {
                return new ConstraintOperationResult(false, ConstraintOperationKind.DeleteWithCheck, canDelete.Message);
            }
            // delete
            Constraint = null;
            return new ConstraintOperationResult(true, ConstraintOperationKind.DeleteWithCheck);
        }
        #endregion

        #region private methods
        


        private void SetSelfAsIncidentEdgeforVetrices()
        {
            if(start!=null)
                start.SecondIncidentEdge = this;
            if(end!=null)
                end.FirstIncidentEdge = this;
        }

        private ConstraintOperationResult CheckIncidentEdgesConstraints((double x, double y) vector)
        {
            if (start != null && start.FirstIncidentEdge != null)
            {
                var checkRes = start.FirstIncidentEdge.CheckConstraint(
                start.FirstIncidentEdge.start.X, start.FirstIncidentEdge.start.Y,
                start.FirstIncidentEdge.end.X + vector.x, start.FirstIncidentEdge.end.Y + vector.y);

                if (!checkRes.Success) return checkRes;
            }

            if (end != null && end.SecondIncidentEdge != null)
            {
                var checkRes = end.SecondIncidentEdge.CheckConstraint(
                end.SecondIncidentEdge.start.X + vector.x, end.SecondIncidentEdge.start.Y + vector.y,
                end.SecondIncidentEdge.end.X, end.SecondIncidentEdge.end.Y);

                if (!checkRes.Success) return checkRes;
            }

            return new ConstraintOperationResult(true, ConstraintOperationKind.Check);
        }

        #endregion
    }
}
