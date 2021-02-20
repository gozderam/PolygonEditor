using PolygonEditor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Definitions
{
    /// <summary>
    /// Represents a shape a polygon can contain. 
    /// </summary>
    public interface ISimpleShape
    {
        string Id { get; set; }
        Color BackgroudColor { get; set; }
        Polygon Parent { get; set; }
        void DrawOnGraphics(Graphics g);
        void ClearFromGraphics(Graphics g);
        bool IsPointInHitArea(double x, double y);
    }
}
