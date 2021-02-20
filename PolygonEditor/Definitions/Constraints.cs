using PolygonEditor.Properties;
using PolygonEditor.Utils;
using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace PolygonEditor.Definitions
{
    #region constraint kind
    public enum EdgeConstraintKind
    {
        VertcialEdge,
        HorizontalEdge,
        FixedLength, 
    }
    #endregion

    #region abstract
    public interface IEdgeConstraint
    {
        EdgeConstraintKind ConstraintKind { get; }
        ConstraintOperationResult CanAddConstraintAndCheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY);
        ConstraintOperationResult CheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY);
        Image Icon { get; }
        (int width, int height) PrefferedIconSize { get; }
        string ConstraintText { get; }

    }
    #endregion

    #region constraint operation result
    public class ConstraintOperationResult
    {
        public bool Success { get; }
        public string Message { get; }
        public ConstraintOperationKind OperationKind { get; }
        public ConstraintOperationResult(bool success, ConstraintOperationKind operationKind, string message = "")
        {
            Success = success;
            OperationKind = this.OperationKind;
            Message = message;
        }
    }

    public enum ConstraintOperationKind
    {
        CanAddAndApply,
        CanDelete,
        Check,
        AddAndApplyWithCheck,
        ApplyWithCheck,
        DeleteWithCheck
    }
    #endregion

    #region vertical constraint
    public class VerticalEdgeConstraint : IEdgeConstraint
    {
        public EdgeConstraintKind ConstraintKind { get; } = EdgeConstraintKind.VertcialEdge;
        public ConstraintOperationResult CanAddConstraintAndCheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            // can Add
            if (edge.Constraint != null) return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.ONE_CONSTRAINT_NOT);
            if (edge.start.FirstIncidentEdge.Constraint?.ConstraintKind == ConstraintKind
                || edge.end.SecondIncidentEdge.Constraint?.ConstraintKind == ConstraintKind) 
                return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.INCIDENT_EDGES_VERTICAL_NOT);

            //can Apply
            if (edge.Parent == null)
                return new ConstraintOperationResult(true, ConstraintOperationKind.CanAddAndApply);

            if (!Helper.CanApplyOneVerticeChangeToEdge(edge, newStartX, newStartY, newEndX, newEndY))
                return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.CANNOT_ADD_OTHER_EDGES);
            return new ConstraintOperationResult(true, ConstraintOperationKind.CanAddAndApply);
        }

        public ConstraintOperationResult CheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            return newStartX == newEndX ? new ConstraintOperationResult(true, ConstraintOperationKind.Check) :
                new ConstraintOperationResult(false, ConstraintOperationKind.Check, TEXTS.VERTICAL_COORDINATES_NOT);
        }
        public Image Icon
        {
            get
            {
                return Resources.vertical_icon;
            }
        }
        public (int width, int height) PrefferedIconSize { get; } = (15, 15);
        public string ConstraintText { get; } = null;

    }
    #endregion

    #region horizontal constraint
    public class HorizontalEdgeConstraint : IEdgeConstraint
    {
        public EdgeConstraintKind ConstraintKind { get; } = EdgeConstraintKind.HorizontalEdge;

        public ConstraintOperationResult CanAddConstraintAndCheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            // can Add
            if (edge.Constraint != null) return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.ONE_CONSTRAINT_NOT);
            if (edge.start.FirstIncidentEdge.Constraint?.ConstraintKind == ConstraintKind
                || edge.end.SecondIncidentEdge.Constraint?.ConstraintKind == ConstraintKind)
                return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.INCIDENT_EDGES_HORIZONTAL_NOT);

            //can Apply
            if (edge.Parent == null)
                return new ConstraintOperationResult(true, ConstraintOperationKind.CanAddAndApply);

            if (!Helper.CanApplyOneVerticeChangeToEdge(edge, newStartX, newStartY, newEndX, newEndY))
                return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.CANNOT_ADD_OTHER_EDGES);
            return new ConstraintOperationResult(true, ConstraintOperationKind.CanAddAndApply);
        }

        public ConstraintOperationResult CheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            return newStartY == newEndY ? new ConstraintOperationResult(true, ConstraintOperationKind.Check) :
                new ConstraintOperationResult(false, ConstraintOperationKind.Check, TEXTS.HORIZONTAL_COORDINATES_NOT);
        }

        public Image Icon
        {
            get
            {
                return Resources.horizontal_icon;
            }
        }
        public (int width, int height) PrefferedIconSize { get; } = (15, 15);
        public string ConstraintText { get; } = null;


    }
    #endregion

    #region fixed length constraint
    public class FixedLengthConstraint : IEdgeConstraint
    {
        public EdgeConstraintKind ConstraintKind { get; } = EdgeConstraintKind.FixedLength;
        public double Length { get; private set; } = 0;
        private double eps = 0.001;

        public FixedLengthConstraint(double length)
        {
            this.Length = length;
        }

        public ConstraintOperationResult CanAddConstraintAndCheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            // can Add
            if (edge.Constraint != null) return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.ONE_CONSTRAINT_NOT);

            //can Apply
            if (edge.Parent == null)
                return new ConstraintOperationResult(true, ConstraintOperationKind.CanAddAndApply);
  
            if (!Helper.CanApplyOneVerticeChangeToEdge(edge, newStartX, newStartY, newEndX,  newEndY))
                return new ConstraintOperationResult(false, ConstraintOperationKind.CanAddAndApply, TEXTS.CANNOT_ADD_OTHER_EDGES);
            return new ConstraintOperationResult(true, ConstraintOperationKind.CanAddAndApply);
        }

        public ConstraintOperationResult CheckOf(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            var l1 = ((newEndX - newStartX) * (newEndX - newStartX) + (newEndY - newStartY) * (newEndY - newStartY));
            var l2 = Length * Length;
            if ( Math.Abs(l1 - l2) < eps )
            {
                return new ConstraintOperationResult(true, ConstraintOperationKind.Check);
            }

            return
                new ConstraintOperationResult(false, ConstraintOperationKind.Check, TEXTS.LENGTH_CANNOT_CHANGE);
        }

        public Image Icon
        {
            get
            {
                return null;
            }
        }
        public (int width, int height) PrefferedIconSize { get; } = (15, 15);
        public string ConstraintText
        {
            get
            {
                return Math.Round(Length, 2).ToString();
            }
        }

        #region private methods
        private double Len(double startX, double startY, double endX, double endY)
        {
            return Math.Sqrt((startX - endX) * (startX - endX) + (startY - endY) * (startY - endY));
        }
        #endregion
    }
    #endregion

    #region Helper
    internal static class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="newStartX"></param>
        /// <param name="newStartY"></param>
        /// <param name="newEndX"></param>
        /// <param name="newEndY"></param>
        /// <returns></returns>
        /// <remarks>Only one vertice should differ from edge vertices!</remarks>
        public static bool CanApplyOneVerticeChangeToEdge(Line edge, double newStartX, double newStartY, double newEndX, double newEndY)
        {
            (double x, double y) startVector = (newStartX - edge.start.X, newStartY - edge.start.Y);
            (double x, double y) endVector = (newEndX - edge.end.X, newEndY - edge.end.Y);

            double eps = 0.0001;
            // assum that one vertice is different from the edge
            (double x, double y) vector = (!(Math.Abs(startVector.x) < eps) || !(Math.Abs(startVector.y) < eps)) ? startVector : endVector;
            if (Math.Abs(vector.x)<eps && Math.Abs(vector.y)<eps) return true;

            return check(true) && check(false);

            bool check(bool isX)
            {
                var vectorCoord = isX ? vector.x : vector.y;
                var neededConstr = isX ? EdgeConstraintKind.HorizontalEdge : EdgeConstraintKind.VertcialEdge;
                if (vectorCoord != 0)
                {
                    var e = edge.end.SecondIncidentEdge;
                    for (; e != edge; e = e.end.SecondIncidentEdge)
                    {
                        if (e.Constraint == null || e.Constraint.ConstraintKind == neededConstr)
                            break;
                    }
                    if (e == edge)
                        return false;
                }
                return true;
            }
         
        }
    }
    #endregion
}
