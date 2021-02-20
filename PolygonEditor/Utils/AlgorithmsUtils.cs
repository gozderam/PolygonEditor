using PolygonEditor.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Utils
{
    class AlgorithmsUtils
    {
        #region Bresenham algorithm
        public static IEnumerable<Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }

            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);

            int d = 2 * dy - dx; // initial d
            int dIncrE = 2 * dy;
            int dIncrNE = 2 * (dy - dx);

            int ystep = (y0 < y1) ? 1 : -1;

            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Point((steep ? y : x), (steep ? x : y));
                if (d < 0) // E
                {
                    d += dIncrE;
                }
                else // NE
                {
                    y += ystep;
                    d += dIncrNE;
                }
            }
            yield break;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vector"></param>
        /// <returns>If whole poygon needs to be moved.</returns>
        public static bool ApplyPolygonConstraitnsAfterVerticeMove(VerticePoint v, (double x, double y) vector)
        {
            // counterclockwise
            //how the last moved vertice was moved
            (double x, double y) lastVector = vector;
            var e = v.FirstIncidentEdge;
            bool allVerticesMoevd = false;
            int movedVerticesCount = 1;

            while (true)
            {
                if (e.start == v)
                {
                    allVerticesMoevd = true;
                    break;
                }

                if (e.CheckConstraint(e.start.X, e.start.Y, e.end.X, e.end.Y).Success)
                {
                    e.UpdateLinePosition();
                    break;
                }
                // apply regarding constraints
                if (e.Constraint.ConstraintKind == EdgeConstraintKind.HorizontalEdge)
                {
                    var tempV = (0, e.end.Y - e.start.Y);
                    if (e.ApplyWithConstraintCheck(e.start.X, e.end.Y, e.end.X, e.end.Y).Success)
                        lastVector = tempV;
                }
                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.VertcialEdge)
                {
                    var tempV = (e.end.X - e.start.X, 0);
                    if (e.ApplyWithConstraintCheck(e.end.X, e.start.Y, e.end.X, e.end.Y).Success)
                        lastVector = tempV;
                }
                else if (e.Constraint.ConstraintKind ==  EdgeConstraintKind.FixedLength)
                    e.ApplyWithConstraintCheck(e.start.X + lastVector.x, e.start.Y + lastVector.y, e.end.X, e.end.Y);
                e.UpdateLinePosition();
                e = e.start.FirstIncidentEdge;
                movedVerticesCount++;
            }
            
            // we didnt move all vertices - go the other direction with initial vector
            if (!allVerticesMoevd)
                lastVector = vector;
            // we moved all vertices - set up new vector in case of moving whole polygon
            // lastVector.x == 0 means that we were moving the vertices only by y coordinate. To move whole polygon - move only by x now.
            else if (lastVector.x == 0)
                lastVector = (vector.x, 0);
            // an analogy to above 
            else if (lastVector.y == 0)
                lastVector = (0, vector.y);

            // clockwise
            e = v.SecondIncidentEdge;
            while (true)
            {
                if (e.end == v)
                    break;
                if (e.CheckConstraint(e.start.X, e.start.Y, e.end.X, e.end.Y).Success)
                {
                    e.UpdateLinePosition();
                    break;
                }
                // apply regarding constraints
                if (e.Constraint.ConstraintKind == EdgeConstraintKind.HorizontalEdge)
                {
                    var tempV = (0, e.start.Y - e.end.Y);
                    if (e.ApplyWithConstraintCheck(e.start.X, e.start.Y, e.end.X, e.start.Y).Success)
                        lastVector = tempV;
                }
                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.VertcialEdge)
                {
                    var tempV = (e.start.X - e.end.X, 0);
                    if (e.ApplyWithConstraintCheck(e.start.X, e.start.Y, e.start.X, e.end.Y).Success)
                        lastVector = tempV;
                }

                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.FixedLength)
                    e.ApplyWithConstraintCheck(e.start.X, e.start.Y, e.end.X + lastVector.x, e.end.Y + lastVector.y);
                e.UpdateLinePosition();
                e = e.end.SecondIncidentEdge;
                movedVerticesCount++;
            }

            return movedVerticesCount>=e.Parent.VerticesCount;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="vector"></param>
        /// <returns>If whole poygon needs to be moved.</returns>
        public static bool ApplyPolygonConstraitnsAfterEdgeMove(Line edge, (double x, double y) vector)
        {
            //how the last moved vertice was moved
            (double x, double y) lastVector = vector;
            var e = edge.start.FirstIncidentEdge;
            bool allVerticesMoevd = false;
            int movedVerticesCount = 2;
 
            // counterclockwise
            while (true)
            {
                if (e.start == edge.end)
                {
                    allVerticesMoevd = true;
                    break;
                }

                if (e.CheckConstraint(e.start.X, e.start.Y, e.end.X, e.end.Y).Success)
                {
                    e.UpdateLinePosition();
                    break;
                }
                // apply regarding constraints
                if (e.Constraint.ConstraintKind == EdgeConstraintKind.HorizontalEdge)
                {
                    var tempV = (0, e.end.Y - e.start.Y);
                    if (e.ApplyWithConstraintCheck(e.start.X, e.end.Y, e.end.X, e.end.Y).Success)
                        lastVector = tempV;
                }
                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.VertcialEdge)
                {
                    var tempV = (e.end.X - e.start.X, 0);
                    if (e.ApplyWithConstraintCheck(e.end.X, e.start.Y, e.end.X, e.end.Y).Success)
                        lastVector = tempV;
                }

                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.FixedLength)
                    e.ApplyWithConstraintCheck(e.start.X + lastVector.x, e.start.Y + lastVector.y, e.end.X, e.end.Y);
                e.UpdateLinePosition();
                e = e.start.FirstIncidentEdge;
                movedVerticesCount++;
            }

            // we didnt move all vertices - go the other direction with initial vector
            if (!allVerticesMoevd)
                lastVector = vector;
            // we moved all vertices - set up new vector in case of moving whole polygon
            // lastVector.x == 0 means that we were moving the vertices only by y coordinate. To move whole polygon - move only by x now.
            else if (lastVector.x == 0)
                lastVector = (vector.x, 0);
            // an analogy to above 
            else if (lastVector.y == 0)
                lastVector = (0, vector.y);

            e = edge.end.SecondIncidentEdge;
            // clockwise
            while (true)
            {
                if (e.end == edge.start)
                    break;
                if (e.CheckConstraint(e.start.X, e.start.Y, e.end.X, e.end.Y).Success)
                {
                    e.UpdateLinePosition();
                    break;
                }
                // apply regarding constraints
                if (e.Constraint.ConstraintKind == EdgeConstraintKind.HorizontalEdge)
                {
                    var tempV = (0, e.start.Y - e.end.Y);
                    if (e.ApplyWithConstraintCheck(e.start.X, e.start.Y, e.end.X, e.start.Y).Success)
                        lastVector = tempV;
                }
                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.VertcialEdge)
                {
                    var tempV = (e.start.X - e.end.X, 0);
                    if (e.ApplyWithConstraintCheck(e.start.X, e.start.Y, e.start.X, e.end.Y).Success)
                        lastVector = tempV;
                }
                else if (e.Constraint.ConstraintKind == EdgeConstraintKind.FixedLength)
                    e.ApplyWithConstraintCheck(e.start.X, e.start.Y, e.end.X + lastVector.x, e.end.Y + lastVector.y);
                e.UpdateLinePosition();
                e = e.end.SecondIncidentEdge;
                movedVerticesCount++;
            }

            return movedVerticesCount >= e.Parent.VerticesCount;
        }
    }
}
