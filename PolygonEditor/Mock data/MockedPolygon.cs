using PolygonEditor.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Mock_data
{
    public static class MockedPolygon
    {
        private static List<VerticePoint> vertices = new List<VerticePoint>
        { new VerticePoint(422, 106),
          new VerticePoint(914, 106),
          new VerticePoint(1039, 145),
          new VerticePoint(1115, 446),
          new VerticePoint(1115, 606),
          new VerticePoint(725, 606),
          new VerticePoint(476, 633),
          new VerticePoint(330, 623),
          new VerticePoint(370, 527),
          new VerticePoint(623, 413),
          new VerticePoint(638, 269)
        };

        private static List<Line> PrepareEdges()
        {
            var edges = new List<Line>();
            for(int i =0; i < vertices.Count; i++)
            {
                edges.Add(new Line(vertices[i], vertices[(i+1) % vertices.Count]));
            }
            return edges;
        }

        public static Polygon PreparePolygon()
        {
            var edges = PrepareEdges();
            var p = new Polygon(vertices, edges);
            edges[0].AddConstraintAndApplyWithCheck(new HorizontalEdgeConstraint(), edges[0].start.X,
             edges[0].start.Y, edges[0].end.X, edges[0].end.Y);
            edges[3].AddConstraintAndApplyWithCheck(new VerticalEdgeConstraint(), edges[3].start.X,
             edges[3].start.Y, edges[3].end.X, edges[3].end.Y);
            edges[4].AddConstraintAndApplyWithCheck(new HorizontalEdgeConstraint(), edges[4].start.X,
             edges[4].start.Y, edges[4].end.X, edges[4].end.Y);
            edges[5].AddConstraintAndApplyWithCheck(new FixedLengthConstraint(edges[5].Length), edges[5].start.X,
             edges[5].start.Y, edges[5].end.X, edges[5].end.Y);
            edges[7].AddConstraintAndApplyWithCheck(new FixedLengthConstraint(edges[7].Length), edges[7].start.X,
             edges[7].start.Y, edges[7].end.X, edges[7].end.Y);
            edges[8].AddConstraintAndApplyWithCheck(new FixedLengthConstraint(edges[8].Length), edges[8].start.X,
             edges[8].start.Y, edges[8].end.X, edges[8].end.Y);
            edges[9].AddConstraintAndApplyWithCheck(new FixedLengthConstraint(edges[9].Length), edges[9].start.X,
             edges[9].start.Y, edges[9].end.X, edges[9].end.Y);
            return p;     
        }
    }
}
