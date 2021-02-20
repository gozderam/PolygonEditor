using PolygonEditor.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Line = PolygonEditor.Definitions.Line;

namespace PolygonEditor.Definitions
{
    //TODO IPolygon - ograniczenia
    /// <summary>
    /// Reprsents a single polygon.
    /// </summary>
    /// <remarks>The order of Vertices and Eges is important! </remarks>
    public class Polygon //: IShape
    {
        public string Id { get; set; }
        public Color FillColor { get; } = Color.Blue;
        public int FillOpacity { get; } = 50;
        public Color BackgroudColor { get; set; } = Color.White;
        public int VerticesCount
        {
            get
            {
                return vertices.Count;
            }
        }

        public int EdgesCount
        {
            get
            {
                return edges.Count;
            }
        }

        /// <summary>
        /// List of subsequent vertices: v_0, v_1, ..., v_n.
        /// </summary>
        private List<VerticePoint> vertices = new List<VerticePoint>();
        /// <summary>
        /// List of subsequent edges: v_0v_1, v_1v_2, ..., v_(n-1)v_n. 
        /// </summary>
        private List<Line> edges = new List<Line>();

        public Polygon(List<VerticePoint> vertices, List<Line> edges)
        {
            Id = AppUtils.GeneratePolygonId();
            this.vertices = vertices;
            this.edges = edges;
            SetSelfAsParent();
        }

        #region getters
        /// <summary>
        /// Deep copy: copies the list of vertices and returns it (changes in the list won't be reflected in Polygon vertices list, but changes in elements of the list will).
        /// </summary>
        public List<VerticePoint> GetVerticesList()
        {
            return vertices.ConvertAll(v => v);
        }

        /// <summary>
        /// Deep copy: copies the list of edges and returns it (changes in the list won't be reflected in Polygon edges list, but changes in elements of the list will).
        /// </summary>
        public List<Line> GetEdgesList()
        {
            return edges.ConvertAll(e => e);
        }

        public int GetEdgeIndex(Line e)
        {
            return edges.IndexOf(e);
        }
        #endregion

        #region deleting vertice
        public (Line firstDeletedEdge, Line secondDeletedEdge, Line addedEdge) DeleteVertice(VerticePoint v)
        {
            #region CheckArgs
            if (v == null)
                return (null, null, null);
            if(vertices.Count==0)
                return (null, null, null);
            if (edges.Count == 0)
                return (null, null, null);
            #endregion

            int verticeIndex = vertices.IndexOf(v);
            if (verticeIndex == -1)
                return (null, null, null);

            // polygon must have at least 3 vertices and edges
            int newEdgeIndex = verticeIndex - 1 >=0 ?  verticeIndex - 1 : edges.Count-2; // insert new edge at the end if deleting first vertice

            // delete vertice
            vertices.Remove(v);

            // delete incident edges
            edges.Remove(v.FirstIncidentEdge);
            edges.Remove(v.SecondIncidentEdge);

            // crete and add new Edge
            // TODO default color
            var newEdge = new Line(v.FirstIncidentEdge.start, v.SecondIncidentEdge.end, this, v.FirstIncidentEdge.Pen.Color, (int)v.FirstIncidentEdge.Pen.Width);
            edges.Insert(newEdgeIndex, newEdge);

            return (v.FirstIncidentEdge, v.SecondIncidentEdge, newEdge);
        }
        #endregion

        #region adding vertice
        public (Line deletedEdge, Line firstAddedEdge, Line secondAddedEdge, VerticePoint addedVertice) AddVerticeOnEdge(Line e)
        {
            #region CheckArgs
            if (e == null)
                return (null, null, null, null);
            if (vertices.Count == 0)
                return (null, null, null, null);
            if (edges.Count == 0)
                return (null, null, null, null);
            #endregion

            int edgeIndex = edges.IndexOf(e);
            if (edgeIndex == -1)
                return (null, null, null, null);

            // point for new vertice 
            var point = e.LinePoints[e.LinePoints.Count / 2];

            // ddelete old edge
            edges.RemoveAt(edgeIndex);

            // new vertce
            var newVertice = new VerticePoint(point.X, point.Y, this);
            vertices.Insert(edgeIndex + 1, newVertice);

            // new Edges 
            var firsNewEgde = new Line(e.start, newVertice, this);
            var secondNewEdge = new Line(newVertice, e.end, this);
            edges.Insert(edgeIndex, firsNewEgde);
            edges.Insert(edgeIndex + 1, secondNewEdge);

            return (e, firsNewEgde, secondNewEdge, newVertice);
        }


        #endregion

        #region moving
        public void Move((double x, double y) vector)
        {
            // all constrains are fullfilled - no need to check 
            vertices.ForEach(v => { v.IncreaseX(vector.x); v.IncreaseY(vector.y); });
        }
        #endregion

        #region private methods
        private void SetSelfAsParent()
        {
            vertices.ForEach(v => v.Parent = this);
            edges.ForEach(e => e.Parent = this);
        }
        #endregion
    }
}
