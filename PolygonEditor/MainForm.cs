using PolygonEditor.Containers;
using PolygonEditor.CustomControls;
using PolygonEditor.Definitions;
using PolygonEditor.Modules;
using PolygonEditor.Notifications;
using PolygonEditor.Utils;
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Line = PolygonEditor.Definitions.Line;
using Polygon = PolygonEditor.Definitions.Polygon;

namespace PolygonEditor
{
    public partial class MainForm : Form
    {
        ClickMode clickMode = ClickMode.MovingElement;
        private readonly PolygonsContainer polygonsContainer;
        public MainForm()
        {
            InitializeComponent();

            var drawArea = AppUtils.InitializeDrawArea(CONSTS.areaMaxWidth, CONSTS.areaMaxHeight, drawAreaBox);
            polygonsContainer = new PolygonsContainer(drawArea, drawAreaBox);

            polygonsContainer.AddMockedPolygon();
        }

        private void newPolygonButton_Click(object sender, EventArgs e)
        {
            switch(clickMode)
            {
                case ClickMode.ChoosingRectanglePoints:
                    polygonsContainer.DeleteCurrentlyBuildedPolygon();
                    break;
                default:
                    clickMode = ClickMode.ChoosingRectanglePoints;
                    break;
            }
        }

        private void drawAreaBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                switch(clickMode)
                {
                    case ClickMode.ChoosingRectanglePoints:
                        polygonsContainer.BuildNextPolygonPart(e.X, e.Y);
                        break;
                    case ClickMode.MovingElement:
                        polygonsContainer.StartMovingElementFrom(e.X, e.Y);
                        break;                           
                }
            else if (e.Button==MouseButtons.Right)
            {

                var resVertice = polygonsContainer.StartVerticeDeleting(e.X, e.Y);
                if(resVertice)
                {
                    ShowVerticeContextMenu(e.X, e.Y);
                    return;
                }
                    
                var resEdge = polygonsContainer.StartAddingEdgeConstraintOrVertice(e.X, e.Y);
                if (resEdge.isEdgeUnderHit)
                {
                    ShowEdgeContextMenu(resEdge.constraint, e.X, e.Y);
                    return;
                }

                var resPolygon = polygonsContainer.StartPolygonDeleting(e.X, e.Y);
                if (resPolygon)
                    ShowPolygonContextMenu(e.X, e.Y);

            }
        }

        private void drawAreaBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                if (clickMode == ClickMode.ChoosingRectanglePoints)
                {
                    clickMode = ClickMode.MovingElement;
                    polygonsContainer.FinishPolygonBuilding(e.X, e.Y);
                }
        }

        private void drawAreaBox_MouseMove(object sender, MouseEventArgs e)
        {
            switch (clickMode)
            {
                case ClickMode.ChoosingRectanglePoints:
                    polygonsContainer.UpdateNewEdgeEnd(e.X, e.Y);
                    break;
                case ClickMode.MovingElement:
                    polygonsContainer.MoveElement(e.X, e.Y);
                    break;
            }
        }

        private void drawAreaBox_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                switch (clickMode)
                {
                    case ClickMode.MovingElement:
                        polygonsContainer.FinishElementMoving();
                        break;
                }
        }


        #region notification
        private void ShowErrorNotification(string message)
        {
            NotificationError notification = new NotificationError();
            notification.Show(message, Screen.FromControl(this));
        }
        #endregion

        #region edge contex menu
        private void ShowEdgeContextMenu(IEdgeConstraint constraint, int x, int y)
        {
            // initially all unchecked
            ((ToolStripMenuItem)edgeContextMenu.Items["verticalEdgeContextMenu"]).Checked = false;
            ((ToolStripMenuItem)edgeContextMenu.Items["horizontalEdgeContextMenu"]).Checked = false;
            ((ToolStripMenuItem)edgeContextMenu.Items["fixedLengthEdgeContextMenu"]).Checked = false;

            if (constraint!=null)
                switch(constraint.ConstraintKind)
                {
                    case EdgeConstraintKind.VertcialEdge:
                        ((ToolStripMenuItem)edgeContextMenu.Items["verticalEdgeContextMenu"]).Checked = true;
                        break;
                    case EdgeConstraintKind.HorizontalEdge:
                        ((ToolStripMenuItem)edgeContextMenu.Items["horizontalEdgeContextMenu"]).Checked = true;
                        break;
                    case EdgeConstraintKind.FixedLength:
                        ((ToolStripMenuItem)edgeContextMenu.Items["fixedLengthEdgeContextMenu"]).Checked = true;
                        break;
                }
            edgeContextMenu.Show(drawAreaBox, x, y);
        }


        private void verticalEdgeContextMenu_Click(object sender, EventArgs e)
        {
            // deleting
            if (((ToolStripMenuItem)sender).Checked)
            {
                var cadDelRes = polygonsContainer.DeleteConstraintVerticalEdge();
                if (!cadDelRes.Success)
                {
                    ShowErrorNotification(cadDelRes.Message);
                }
                return;
            }

            // adding
            var addConstrRes = polygonsContainer.FinishAddingConstraintVerticalEdge();
            if (!addConstrRes.Success)
            {
                ShowErrorNotification(addConstrRes.Message);
            }

        }

        private void horizontalEdgeContextMenu_Click(object sender, EventArgs e)
        {
            // deleting
            if (((ToolStripMenuItem)sender).Checked)
            {
                var cadDelRes = polygonsContainer.DeleteConstraintHorizontalEdge();
                if (!cadDelRes.Success)
                {
                    ShowErrorNotification(cadDelRes.Message);
                }
                return;
            }

            // adding
            var addConstrRes = polygonsContainer.FinishAddingConstraintHorizontalEdge();
            if (!addConstrRes.Success)
            {
                ShowErrorNotification(addConstrRes.Message);
            }
        }
        private void fixedLengthEdgeContextMenu_Click(object sender, EventArgs e)
        {
            // deleting
            if (((ToolStripMenuItem)sender).Checked)
            {
                var cadDelRes = polygonsContainer.DeleteConstraintFixedLength();
                if (!cadDelRes.Success)
                {
                    ShowErrorNotification(cadDelRes.Message);
                }
                return;
            }

            // adding
            var res = polygonsContainer.CanAddConstraintFixedLength();
            if (!res.canAdd.Success)
            {
                ShowErrorNotification(res.canAdd.Message);
                return;
            }

            (double renderX, double renderY) = AppUtils.MapPointFromPictueBoxToForm(this, drawAreaBox, res.xHit, res.yHit);

            string promptValue = EdgeLengthDialogPrompt.ShowDialog(this, "Edge's length:", "Length Constraint", renderX, renderY, res.edgeLength);
            if (promptValue != null && (double.TryParse(promptValue, NumberStyles.Any, new CultureInfo("en-US"), out double doubleValue)))
            {
                var addConstrRes = polygonsContainer.FinishAddingConstraintFixedLength(doubleValue);
                if (!addConstrRes.Success)
                {
                    ShowErrorNotification(addConstrRes.Message);

                }
            }
        }

        private void addVerticeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygonsContainer.AddVerticeOnSelectedEdge();
        }
        #endregion


        #region vertice contex menu
        private void ShowVerticeContextMenu(int x, int y)
        {
            verticeContextMenu.Show(drawAreaBox, x, y);
        }

        private void deleteVerticeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygonsContainer.DeleteSelectedVertice();
        }

        #endregion

        #region polygon contex menu
        private void ShowPolygonContextMenu(int x, int y)
        {
            polygonContextMenu.Show(drawAreaBox, x, y);
        }
        #endregion
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygonsContainer.DeleteSelectedPolygon();
        }
    }
}
