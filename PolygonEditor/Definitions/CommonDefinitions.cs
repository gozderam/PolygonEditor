using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Definitions
{
    public enum ClickMode
    {
        None,
        ChoosingRectanglePoints,
        MovingElement,
    }

    public struct MovingVerticeState
    {
        public VerticePoint selectedVertice;
        public PointF hitPoint;

        public MovingVerticeState(VerticePoint currentlyMovingVertice, PointF hitPoint)
        {
            this.selectedVertice = currentlyMovingVertice;
            this.hitPoint = hitPoint;
        }

        public bool IsDuringMovement { 
            get
            {
                return selectedVertice != null;
            } 
        }

        public void Clear()
        {
            selectedVertice = null; 
        }

        public (double x, double y) GetMoveVectorAndUpdateHitPoint(double x, double y)
        {
            double ret_x = x - hitPoint.X;
            double ret_y = y - hitPoint.Y;

            hitPoint.X = (float)x;
            hitPoint.Y = (float)y;
            return (ret_x, ret_y);
        }
    }

    public struct MovingEdgeState
    {
        public Line selectedEdge;
        public PointF hitPoint;

        public MovingEdgeState(Line currentlyMovingEdge, PointF hitPoint)
        {
            this.selectedEdge = currentlyMovingEdge;
            this.hitPoint = hitPoint;
        }

        public bool IsDuringMovement
        {
            get
            {
                return selectedEdge != null;
            }
        }

        public void Clear()
        {
            selectedEdge = null;
        }

        public (double x, double y) GetMoveVectorAndUpdateHitPoint(double x, double y)
        {
            double ret_x = x - hitPoint.X;
            double ret_y = y - hitPoint.Y;

            hitPoint.X = (float)x;
            hitPoint.Y = (float)y;
            return (ret_x, ret_y);
        }
    }

    public struct MovingPolygonState
    {
        public Polygon selectedPolygon;
        public PointF hitPoint;

        public MovingPolygonState(Polygon currentlyMovingPolygon, PointF hitPoint)
        {
            this.selectedPolygon = currentlyMovingPolygon;
            this.hitPoint = hitPoint;
        }

        public bool IsDuringMovement
        {
            get
            {
                return selectedPolygon != null;
            }
        }

        public void Clear()
        {
            selectedPolygon = null;
        }

        public (double x, double y) GetMoveVectorAndUpdateHitPoint(double x, double y)
        {
            double ret_x = x - hitPoint.X;
            double ret_y = y - hitPoint.Y;

            hitPoint.X = (float)x;
            hitPoint.Y = (float)y;
            return (ret_x, ret_y);
        }

    }

    public struct AddingEdgeConstraintOrVerticeState
    {
        public Line selectedEdge;
        public IEdgeConstraint constraint;
        public PointF hitPoint;

        public AddingEdgeConstraintOrVerticeState(Line currentlyMovingEdge, PointF hitPoint)
        {
            this.selectedEdge = currentlyMovingEdge;
            constraint = null;
            this.hitPoint = hitPoint;
        }

        public bool IsDuringMovement
        {
            get
            {
                return selectedEdge != null;
            }
        }

        public void Clear()
        {
            selectedEdge = null;
        }
    }

    public struct DeletingVerticeState
    {
        public VerticePoint selectedVertice;

        public DeletingVerticeState(VerticePoint currentlyDeletingVertice)
        {
            this.selectedVertice = currentlyDeletingVertice;
        }

        public bool IsDuringMovement
        {
            get
            {
                return selectedVertice != null;
            }
        }

        public void Clear()
        {
            selectedVertice = null;
        }
    }

    public struct DeletingPolygonState
    {
        public Polygon selectedPolygon;

        public DeletingPolygonState(Polygon selectedPolygon)
        {
            this.selectedPolygon = selectedPolygon;
         }

        public bool IsDuringMovement
        {
            get
            {
                return selectedPolygon != null;
            }
        }

        public void Clear()
        {
            selectedPolygon = null;
        }
    }
}
