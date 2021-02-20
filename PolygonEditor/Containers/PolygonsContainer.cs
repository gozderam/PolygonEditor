using PolygonEditor.Definitions;
using PolygonEditor.Mock_data;
using PolygonEditor.Modules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using System.Windows.Forms;
using Polygon = PolygonEditor.Definitions.Polygon;
namespace PolygonEditor.Containers
{
    /// <summary>
    /// Aggregates the logic for the schema of polygons (vector graphics) with drawing functions (raster graphics). 
    /// </summary>
    public class PolygonsContainer
    {
        public List<Polygon> Polygons { get; set; } = new List<Polygon>();

        private List<VerticePoint> cachedVertices = new List<VerticePoint>();
        private List<Line> cachedEdges = new List<Line>();
        private readonly DrawingPolygonModule drawingModule;

        private MovingVerticeState movingVerticeState = new MovingVerticeState(null, new PointF(0, 0));
        private MovingEdgeState movingEdgeState = new MovingEdgeState(null, new Point(0, 0));
        private MovingPolygonState movingPolygonState = new MovingPolygonState(null, new PointF(0, 0));
        private AddingEdgeConstraintOrVerticeState addingEdgeConstraintOrVerticeState = new AddingEdgeConstraintOrVerticeState(null, new PointF(0,0));
        private DeletingVerticeState deletingVerticeState = new DeletingVerticeState(null);
        private DeletingPolygonState deletingPolygonState = new DeletingPolygonState(null);

        public PolygonsContainer(Bitmap drawArea, PictureBox drawAreaBox)
        {
            drawingModule = new DrawingPolygonModule(drawArea, drawAreaBox);
        }

        #region building      
        public void BuildNextPolygonPart(double x, double y)
        {
            // frist vertice
            if(cachedVertices.Count ==0)
            {
                var v = new VerticePoint(x, y);
                cachedVertices.Add(v);
            }

            // drawing last cached vertice
            drawingModule.DrawPolygonVertices(cachedVertices.Last());

            // subsequent vertice
            var nextV = new VerticePoint(x, y);
            cachedVertices.Add(nextV);

            // new edge between last cached and newly created
            var e = new Line(cachedVertices[cachedVertices.Count - 2], cachedVertices.Last());
            cachedEdges.Add(e);

            // draw edge (for now the edge has only one visible vertice)
            drawingModule.DrawPolygonEdges(e);

            // start moving last vertice
            movingVerticeState.Clear();
            movingVerticeState.selectedVertice = cachedVertices.Last();
            movingVerticeState.GetMoveVectorAndUpdateHitPoint(x, y);
        }

        public void UpdateNewEdgeEnd(double x, double y)
        {
            if(cachedVertices.Count>0)
            {
                drawingModule.BeginDrawingTransaction();
                // remove last edge from previous position
                drawingModule.ClearPolygonEdges(cachedEdges.Last());

                // update last cached vertice position
                var vector = movingVerticeState.GetMoveVectorAndUpdateHitPoint(x, y);
                cachedVertices.Last().Move(vector);

                // draw edge in new position
                Redraw();
                drawingModule.FinishDrawingTransaction();
            }
        }

        public void FinishPolygonBuilding(double x, double y)
        {
            RollbackDoubleClickEffect();

            if (cachedVertices.Count <= 2)
            {
                DeleteCurrentlyBuildedPolygon();
                return;
            }

            // last edge
            var lastEdge = new Line(cachedVertices.Last(), cachedVertices.First());
            cachedEdges.Add(lastEdge);

            drawingModule.BeginDrawingTransaction();
            drawingModule.DrawPolygonEdges(cachedEdges.Last());

            // finally create the polygon
            var newPolygon = new Polygon(cachedVertices.ConvertAll(v => v), cachedEdges.ConvertAll(edge => edge));
            Polygons.Add(newPolygon);
            drawingModule.FillPolygon(newPolygon);
            drawingModule.FinishDrawingTransaction();

            // clear cache
            cachedVertices.Clear();
            cachedEdges.Clear();
            movingVerticeState.Clear();
        }
        #endregion

        #region deleting polygon
        public void DeleteCurrentlyBuildedPolygon()
        {
            // remove from drawArea
            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonVertices(cachedVertices.ToArray());
            drawingModule.ClearPolygonEdges(cachedEdges.ToArray());
            cachedVertices.Clear();
            cachedEdges.Clear();
            Redraw();
            drawingModule.FinishDrawingTransaction();

            // clear cache
            cachedVertices.Clear();
            cachedEdges.Clear();
        }

        public bool StartPolygonDeleting(double x, double y)
        {
            var p = GetPolygonFromHit(x, y);
            if (p != null)
            {
                deletingPolygonState.selectedPolygon = p;
                return true;
            }
            return false;
        }

        public void DeleteSelectedPolygon()
        {
            DeletePolygon(deletingPolygonState.selectedPolygon);
            deletingPolygonState.Clear();
        }

        public void DeletePolygon(Polygon p)
        {
            #region checkArgs
            if (p== null)
                return;
            #endregion

            if (Polygons.Remove(p))
            {
                drawingModule.BeginDrawingTransaction();
                drawingModule.ClearPolygonVertices(p.GetVerticesList().ToArray());
                drawingModule.ClearPolygonEdges(p.GetEdgesList().ToArray());
                drawingModule.ClearPolygonFilling(p);
                Redraw();
                drawingModule.FinishDrawingTransaction();
            }
            deletingPolygonState.Clear();
        }

        #endregion

        #region deleting vertice
        public bool StartVerticeDeleting(double x, double y)
        {
            var v = GetVerticeFromHit(x, y);
            if (v != null)
            {
                deletingVerticeState.selectedVertice = v;
                return true;
            }
            return false;
        }

        public void DeleteSelectedVertice()
        {
            #region checkArgs
            if (deletingVerticeState.selectedVertice == null)
                return;
            #endregion

            var v = deletingVerticeState.selectedVertice;
            var polygon = v.Parent;

            // if polygon ha only 3 vertices delete the polygon
            if (polygon.VerticesCount == 3)
            {
                DeletePolygon(polygon);
                return;
            }

            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonFilling(v.Parent);
            var (firstOldEdge, secondOldEdge, newEdge) = polygon.DeleteVertice(v);
            drawingModule.ClearPolygonVertices(v);
            drawingModule.ClearPolygonEdges(new[] { firstOldEdge, secondOldEdge });
            drawingModule.DrawPolygonEdges(newEdge);
            Redraw();
            drawingModule.FinishDrawingTransaction();

            deletingVerticeState.Clear();
        }
        #endregion

        #region hit detecting  
        /// <summary>
        /// Detects if any vertice is under hit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Vertice under hit or null if there is no such a vertice.</returns>
        public VerticePoint GetVerticeFromHit(double x, double y)
        {
            for (int i = 0; i < Polygons.Count; i++)
            {
                var vertices = Polygons[i].GetVerticesList();
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (vertices[j].IsPointInHitArea(x, y))
                    {
                        return vertices[j];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Detects if any line is under hit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Line under hit or null if there is no such a line.</returns>
        public Line GetEdgeFromHit(double x, double y)
        {
            for (int i = 0; i < Polygons.Count; i++)
            {
                var edges = Polygons[i].GetEdgesList();
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edges[j].IsPointInHitArea(x, y))
                    {
                        return edges[j];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Detects if any polygon is under hit.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Polygon under hit or null if there is no such a polygon.</returns>
        public Polygon GetPolygonFromHit(double x, double y)
        {
            for (int i = Polygons.Count - 1; i >= 0; i--)
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddPolygon(Polygons[i].GetVerticesList().Select(v => new PointF((float)v.X, (float)v.Y)).ToArray());
                if (gp.IsVisible((float)x, (float)y))
                {
                    return Polygons[i];
                }
            }
            return null;
        }
        #endregion

        #region general moving
        public void StartMovingElementFrom(double x, double y)
        {
            if(!StartVerticeMoving(x, y))
                if (!StartEdgeMoving(x, y))
                    StartPolygonMoving(x, y);

        }

        public void MoveElement(double x, double y)
        {
            if (!MoveVertice(x, y))
                if (!MoveEdge(x, y))
                    MovePolygon(x, y);
        }

        public void FinishElementMoving()
        {
            if (!FinishVerticeMoving())
                if (!FinishEdgeMoving())
                    FinishPolygonMoving();
        }
        #endregion

        #region moving vertice
        public bool StartVerticeMoving(double x, double y)
        {
            var v = GetVerticeFromHit(x, y);
            if(v!=null)
            {
                movingVerticeState.selectedVertice = v;
                movingVerticeState.GetMoveVectorAndUpdateHitPoint(x, y);

                return true;
            }
            return false;
        }
        public bool MoveVertice(double x, double y)
        {
            if (!movingVerticeState.IsDuringMovement)
                return false;
            drawingModule.BeginDrawingTransaction();
            // clear old position
            drawingModule.ClearPolygonBorders(movingVerticeState.selectedVertice.Parent);
            drawingModule.ClearPolygonFilling(movingVerticeState.selectedVertice.Parent);

            // update positions
            var vector = movingVerticeState.GetMoveVectorAndUpdateHitPoint(x, y);
            movingVerticeState.selectedVertice.Move(vector);

            // draw with updated positions
            Redraw();
            drawingModule.FinishDrawingTransaction();
            return true;
        }

        public bool FinishVerticeMoving()
        {
            if (!movingVerticeState.IsDuringMovement)
                return false;

            movingVerticeState.Clear();
            return true;
        }
        #endregion

        #region moving edge
        public bool StartEdgeMoving(double x, double y)
        {
            var e = GetEdgeFromHit(x, y);
            if (e != null)
            {
                movingEdgeState.selectedEdge = e;
                movingEdgeState.GetMoveVectorAndUpdateHitPoint(x, y);
                return true;
            }
            return false;
        }

        public bool MoveEdge(double x, double y)
        {
            if (!movingEdgeState.IsDuringMovement)
                return false;
            drawingModule.BeginDrawingTransaction();
            // clear old position
            drawingModule.ClearPolygonBorders(movingEdgeState.selectedEdge.Parent);
            drawingModule.ClearPolygonFilling(movingEdgeState.selectedEdge.Parent);

            // update positions
            var vector = movingEdgeState.GetMoveVectorAndUpdateHitPoint(x, y);
            movingEdgeState.selectedEdge.Move(vector);

            // draw with updated positions
            Redraw();
            drawingModule.FinishDrawingTransaction();
            return true;
        }

        public bool FinishEdgeMoving()
        {
            if (!movingEdgeState.IsDuringMovement)
                return false;

            movingEdgeState.Clear();
            return true;
        }
        #endregion

        #region moving polygon
        public bool StartPolygonMoving(double x, double y)
        {
            var p = GetPolygonFromHit(x, y);
            if (p != null)
            {
                movingPolygonState.selectedPolygon = p;
                movingPolygonState.GetMoveVectorAndUpdateHitPoint(x, y);
                return true;
            }
            return false;
        }

        public bool MovePolygon(double x, double y)
        {
            if (!movingPolygonState.IsDuringMovement)
                return false;

            drawingModule.BeginDrawingTransaction();
            // clear old position
            drawingModule.ClearPolygonBorders(movingPolygonState.selectedPolygon);
            drawingModule.ClearPolygonFilling(movingPolygonState.selectedPolygon);

            // update positions
            var vector = movingPolygonState.GetMoveVectorAndUpdateHitPoint(x, y);
            movingPolygonState.selectedPolygon.Move(vector);

            // draw with updated positions
            Redraw();
            drawingModule.FinishDrawingTransaction();

            return true;
        }
        public bool FinishPolygonMoving()
        {
            if (!movingPolygonState.IsDuringMovement)
                return false;

            movingPolygonState.Clear();
            return true;
        }
        #endregion

        #region adding edges constraints or vertice
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>If there is any edge under the hit to add constrint or vertice to.</returns>
        public (bool isEdgeUnderHit, IEdgeConstraint constraint) StartAddingEdgeConstraintOrVertice(double x, double y)
        {
            addingEdgeConstraintOrVerticeState.selectedEdge  = GetEdgeFromHit(x, y);
            addingEdgeConstraintOrVerticeState.hitPoint = new PointF((float)x, (float)y);
            if (addingEdgeConstraintOrVerticeState.selectedEdge != null && GetVerticeFromHit(x, y) == null)
            {
                return (true, addingEdgeConstraintOrVerticeState.selectedEdge.Constraint);
            }
            return (false, null);
        }

        public void AddVerticeOnSelectedEdge()
        {
            #region checkArgs
            if (addingEdgeConstraintOrVerticeState.selectedEdge == null)
                return;
            #endregion

            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            var polygon = e.Parent;

            var (oldEdge, firstNewEdge, secondNewEdge, newVertice) = polygon.AddVerticeOnEdge(e);

            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonEdges(oldEdge);
            drawingModule.DrawPolygonEdges(new[] { firstNewEdge, secondNewEdge });
            drawingModule.DrawPolygonVertices(newVertice);
            Redraw();
            drawingModule.FinishDrawingTransaction();

            addingEdgeConstraintOrVerticeState.Clear();
        }

        public ConstraintOperationResult CanAddConstraintVerticalEdge()
        {
            var constraint = new VerticalEdgeConstraint();
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;

            // firstly checks with startX
            var canAccStart = e.CanAddConstraintAndApply(constraint, e.start.X, e.start.Y, e.start.X, e.end.Y);
            if (canAccStart.Success)
                return canAccStart;

            //then check accorindg to endX
            var canAccEnd = e.CanAddConstraintAndApply(constraint, e.end.X, e.start.Y, e.end.X, e.end.Y);
            if (canAccEnd.Success)
                return canAccStart;

            return new ConstraintOperationResult(false, ConstraintOperationKind.AddAndApplyWithCheck);
        }

        public ConstraintOperationResult FinishAddingConstraintVerticalEdge()
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            addingEdgeConstraintOrVerticeState.constraint = new VerticalEdgeConstraint();
            var constraint = addingEdgeConstraintOrVerticeState.constraint;

            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonBorders(e.Parent);
            drawingModule.ClearPolygonFilling(e.Parent);

            ConstraintOperationResult canAccStart, canAccEnd = null;
            // firstly checks with startX
             canAccStart = e.AddConstraintAndApplyWithCheck(constraint, e.start.X, e.start.Y, e.start.X, e.end.Y);
            if (!canAccStart.Success)
            {
                //then check accorindg to endX
                 canAccEnd = e.AddConstraintAndApplyWithCheck(constraint, e.end.X, e.start.Y, e.end.X, e.end.Y);
            }

            Redraw();
            drawingModule.FinishDrawingTransaction();
            addingEdgeConstraintOrVerticeState.Clear();
            if (canAccStart != null && canAccStart.Success)
                return canAccStart;
            if (canAccEnd != null && canAccEnd.Success)
                return canAccEnd;
            if (canAccStart != null)
                return canAccStart;
            if (canAccEnd != null)
                return canAccEnd;
            return new ConstraintOperationResult(false, ConstraintOperationKind.AddAndApplyWithCheck, TEXTS.UNKNOWN_ERROR);
        }

        public ConstraintOperationResult DeleteConstraintVerticalEdge()
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonEdges(e);
            var ret = e.DeleteConstraintWithCheck();
            Redraw();
            drawingModule.FinishDrawingTransaction();

            addingEdgeConstraintOrVerticeState.Clear();
            return ret;
        }

        public ConstraintOperationResult CanAddConstraintHorizontalEdge()
        {
            var constraint = new HorizontalEdgeConstraint();
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            // firstly checks with startY
            var canAccStart = e.CanAddConstraintAndApply(constraint, e.start.X, e.start.Y, e.end.X, e.start.Y);
            if (canAccStart.Success)
                return canAccStart;
            //then check accorindg to endX
            var canAccEnd = e.CanAddConstraintAndApply(constraint, e.start.X, e.end.Y, e.end.X, e.end.Y);
            if (canAccEnd.Success)
                return canAccStart;

            return new ConstraintOperationResult(false, ConstraintOperationKind.AddAndApplyWithCheck);
        }

        public ConstraintOperationResult FinishAddingConstraintHorizontalEdge()
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            addingEdgeConstraintOrVerticeState.constraint = new HorizontalEdgeConstraint();
            var constraint = addingEdgeConstraintOrVerticeState.constraint;

            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonBorders(e.Parent);
            drawingModule.ClearPolygonFilling(e.Parent);

            ConstraintOperationResult canAccStart, canAccEnd = null;
            // firstly checks with startX
            canAccStart = e.AddConstraintAndApplyWithCheck(constraint, e.start.X, e.start.Y, e.end.X, e.start.Y);
            if (!canAccStart.Success)
            {
                //then check accorindg to endX
                 canAccEnd = e.AddConstraintAndApplyWithCheck(constraint, e.start.X, e.end.Y, e.end.X, e.end.Y);
            }

            Redraw();
            drawingModule.FinishDrawingTransaction();

            addingEdgeConstraintOrVerticeState.Clear();

            if (canAccStart != null && canAccStart.Success)
                return canAccStart;
            if (canAccEnd != null && canAccEnd.Success)
                return canAccEnd;
            if (canAccStart != null)
                return canAccStart;
            if (canAccEnd != null)
                return canAccEnd;
            return new ConstraintOperationResult(false, ConstraintOperationKind.AddAndApplyWithCheck, TEXTS.UNKNOWN_ERROR);
        }

        public ConstraintOperationResult DeleteConstraintHorizontalEdge()
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            drawingModule.BeginDrawingTransaction();     
            drawingModule.ClearPolygonEdges(e);
            var ret = e.DeleteConstraintWithCheck();
            Redraw();
            drawingModule.FinishDrawingTransaction();

            addingEdgeConstraintOrVerticeState.Clear();

            return ret;
        }

        public (ConstraintOperationResult canAdd, double edgeLength, double xHit, double yHit) CanAddConstraintFixedLength()
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            var hitPoint = addingEdgeConstraintOrVerticeState.hitPoint;
            var constraint = new FixedLengthConstraint(e.Length);

            // TODO calculate coordinates 
            var canApplyRes = e.CanAddConstraintAndApply(constraint, e.start.X, e.start.Y, e.end.X, e.end.Y);
            return canApplyRes.Success ? (canApplyRes, e.Length, hitPoint.X, hitPoint.Y) : (canApplyRes, -1, -1, -1);
        }

        public ConstraintOperationResult FinishAddingConstraintFixedLength(double length)
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            addingEdgeConstraintOrVerticeState.constraint = new FixedLengthConstraint(length);
            var constraint = addingEdgeConstraintOrVerticeState.constraint;

            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonBorders(e.Parent);
            drawingModule.ClearPolygonFilling(e.Parent);

            ConstraintOperationResult canAccStart, canAccEnd = null;


            // firstly checks with startX
            (double x_end, double y_end) = (e.end.X, e.end.Y);
            // if lenght from prompt is different than edge's lenght - calculate new coordinates
            if (e.Length!=length)
                ( x_end, y_end) = CalculateLocationForFixedLengthFromStart(e, length);
            canAccStart = e.AddConstraintAndApplyWithCheck(constraint, e.start.X, e.start.Y, x_end, y_end);
            if (!canAccStart.Success)
            {
                //then check accorindg to endX
                // if length from prompt is different than edge's lenght - calculate new coordinates
                (double x_start, double y_start) = (e.start.X, e.start.Y);
                if (e.Length != length)
                    ( x_start,  y_start) = CalculateLocationForFixedLengthFromEnd(e, length);
                canAccEnd = e.AddConstraintAndApplyWithCheck(constraint, x_start, y_start, e.end.X, e.end.Y);
            }
           
            Redraw();
            drawingModule.FinishDrawingTransaction();

            addingEdgeConstraintOrVerticeState.Clear();

            if (canAccStart != null && canAccStart.Success)
                return canAccStart;
            if (canAccEnd != null && canAccEnd.Success)
                return canAccEnd;
            if (canAccStart != null)
                return canAccStart;
            if (canAccEnd != null)
                return canAccEnd;
            return new ConstraintOperationResult(false, ConstraintOperationKind.AddAndApplyWithCheck, TEXTS.UNKNOWN_ERROR);
        }

        public ConstraintOperationResult DeleteConstraintFixedLength()
        {
            var e = addingEdgeConstraintOrVerticeState.selectedEdge;
            drawingModule.BeginDrawingTransaction();
            drawingModule.ClearPolygonEdges(e);
            var ret = e.DeleteConstraintWithCheck();
            Redraw();
            drawingModule.FinishDrawingTransaction();

            addingEdgeConstraintOrVerticeState.Clear();

            return ret;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Redraws all polygons on the draw area. Use after deleting/moving edge, vertice or polygon. 
        /// </summary>
        private void Redraw()
        {
            Polygons?.ForEach(p => drawingModule.ClearPolygonBorders(p));
            Polygons?.ForEach(p => drawingModule.ClearPolygonFilling(p));

            bool isInvokedInsideTransaction = drawingModule.IsDuringTransaction;
            if (!isInvokedInsideTransaction)
                drawingModule.BeginDrawingTransaction();

            if(cachedEdges.Count>0)
                drawingModule.DrawPolygonEdges(cachedEdges.ToArray());
            if(cachedVertices.Count>0)
                drawingModule.DrawPolygonVertices(cachedVertices.GetRange(0, cachedVertices.Count-1).ToArray());
            Polygons?.ForEach(p => drawingModule.FillPolygon(p));
            Polygons?.ForEach(p => drawingModule.DrawPolygonBorders(p));

            if (!isInvokedInsideTransaction)
                drawingModule.FinishDrawingTransaction();
        }

        /// <summary>
        /// Windows Forms interprets double mouse click as: (mouse click, mouse click, double mouse click).
        /// Hence, while building a polygon, there is one extra mouse click event and as a consequence one extra vertices and edges.
        /// This function is a rollback for this issue. 
        /// </summary>
        private void RollbackDoubleClickEffect()
        {
            if (cachedVertices.Count >= 2 && cachedEdges.Count >= 2)
            {
                // clear from drawArea (last vertice hasn't been drawn yet)
                drawingModule.BeginDrawingTransaction();
                drawingModule.ClearPolygonVertices(cachedVertices[cachedVertices.Count-2]);
                drawingModule.ClearPolygonEdges(new[] { cachedEdges[cachedEdges.Count - 1], cachedEdges[cachedEdges.Count - 2] });

                // clear from cache (two vertices and an edge)
                cachedVertices.RemoveAt(cachedVertices.Count - 1);
                cachedVertices.RemoveAt(cachedVertices.Count - 1);
                cachedEdges.RemoveAt(cachedEdges.Count - 1);
                cachedEdges.RemoveAt(cachedEdges.Count - 1);

                drawingModule.DrawPolygonVertices(cachedVertices.Last());
                drawingModule.FinishDrawingTransaction();
            }
        }

        private (double newX, double newY) CalculateLocationForFixedLengthFromStart(Line edge, double length)
        {
            var edgeLength = edge.Length;
            var p = length / edgeLength;
            double x = ((p * (edge.end.X - edge.start.X) + edge.start.X));
            double y = ((p * (edge.end.Y - edge.start.Y) + edge.start.Y));
            return (x, y);
        }

        private (double newX, double newY) CalculateLocationForFixedLengthFromEnd(Line edge, double length)
        {
            var edgeLength = edge.Length;
            var p = length / edgeLength;
            double x = ((p * (edge.start.X - edge.end.X) + edge.end.X));
            double y = ((p * (edge.start.Y - edge.end.Y) + edge.end.Y));
            return (x, y);
        }

        public void AddMockedPolygon()
        {
            var p = MockedPolygon.PreparePolygon();
            Polygons.Add(p);
            drawingModule.BeginDrawingTransaction();
            drawingModule.FillPolygon(p);
            drawingModule.DrawPolygonBorders(p);
            drawingModule.DrawPolygonEdges(p.GetEdgesList().ToArray());
            drawingModule.FinishDrawingTransaction();
        }
        #endregion

    }
}
