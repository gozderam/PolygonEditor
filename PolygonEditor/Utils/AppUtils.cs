using PolygonEditor.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolygonEditor.Utils
{
    public static class AppUtils
    {
        #region initialization
        /// <summary>
        /// Initializes draw area with areaMaxWidth and AreaMaxHeight and sets it as an image of pictureBox.
        /// </summary>
        /// <param name="areaMaxWidth"></param>
        /// <param name="areaMaxheight"></param>
        /// <param name="pictureBox"></param>
        /// <returns></returns>
        /// <remarks>Note, that the size of bitmap seen in the window will be equal to pictureBox size.</remarks>
        public static Bitmap InitializeDrawArea(int areaMaxWidth, int areaMaxheight, PictureBox pictureBox)
        {
            var drawArea = new Bitmap(areaMaxWidth, areaMaxheight);
            pictureBox.Image = drawArea;
            return drawArea;
        }
        #endregion

        #region caluculating positions
        /// <summary>
        /// Maps position of a point on the pictureBox to the point on the form.
        /// </summary>
        public static (double formX, double formY) MapPointFromPictueBoxToForm(Form parentForm, PictureBox drawAreaBox, double drawAreaBoxX, double drawAreaBoxY)
        {
            Rectangle screenRectangle = parentForm.RectangleToScreen(parentForm.ClientRectangle);
            double titleHeight = screenRectangle.Top - parentForm.Top;
            double x = (drawAreaBoxX + parentForm.Left + drawAreaBox.Left);
            double y = (drawAreaBoxY + parentForm.Top + drawAreaBox.Top + titleHeight);
            return (x, y);
        }
        #endregion

        #region id generating mechanism
        private static long NextIdNumberForPolygons = 0;
        private static long NextIdNumberForEdges = 0;
        private static long NextIdNumberForVertices = 0;

        public static string GeneratePolygonId()
        {
            return "P_" + (NextIdNumberForPolygons++).ToString();
        }
        public static string GenerateEdgeId()
        {
            return "E_" + (NextIdNumberForEdges++).ToString();
        }
        public static string GenerateVerticeId()
        {
            return "V_" + (NextIdNumberForVertices++).ToString();
        }
        #endregion
   
    }

}
