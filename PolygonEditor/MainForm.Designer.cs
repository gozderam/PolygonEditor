namespace PolygonEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.newPolygonButton = new System.Windows.Forms.Button();
            this.edgeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.verticalEdgeContextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.horizontalEdgeContextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fixedLengthEdgeContextMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.addVerticeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawAreaBox = new System.Windows.Forms.PictureBox();
            this.verticeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteVerticeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawAreaBox)).BeginInit();
            this.verticeContextMenu.SuspendLayout();
            this.polygonContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // newPolygonButton
            // 
            this.newPolygonButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newPolygonButton.Location = new System.Drawing.Point(1028, 26);
            this.newPolygonButton.Name = "newPolygonButton";
            this.newPolygonButton.Size = new System.Drawing.Size(128, 41);
            this.newPolygonButton.TabIndex = 2;
            this.newPolygonButton.Text = "New Polygon";
            this.newPolygonButton.UseVisualStyleBackColor = true;
            this.newPolygonButton.Click += new System.EventHandler(this.newPolygonButton_Click);
            // 
            // edgeContextMenu
            // 
            this.edgeContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.edgeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verticalEdgeContextMenu,
            this.horizontalEdgeContextMenu,
            this.fixedLengthEdgeContextMenu,
            this.addVerticeToolStripMenuItem});
            this.edgeContextMenu.Name = "edgeContextMenu";
            this.edgeContextMenu.Size = new System.Drawing.Size(167, 108);
            // 
            // verticalEdgeContextMenu
            // 
            this.verticalEdgeContextMenu.Image = ((System.Drawing.Image)(resources.GetObject("verticalEdgeContextMenu.Image")));
            this.verticalEdgeContextMenu.Name = "verticalEdgeContextMenu";
            this.verticalEdgeContextMenu.Size = new System.Drawing.Size(166, 26);
            this.verticalEdgeContextMenu.Text = "Vertical ";
            this.verticalEdgeContextMenu.Click += new System.EventHandler(this.verticalEdgeContextMenu_Click);
            // 
            // horizontalEdgeContextMenu
            // 
            this.horizontalEdgeContextMenu.Image = ((System.Drawing.Image)(resources.GetObject("horizontalEdgeContextMenu.Image")));
            this.horizontalEdgeContextMenu.Name = "horizontalEdgeContextMenu";
            this.horizontalEdgeContextMenu.Size = new System.Drawing.Size(166, 26);
            this.horizontalEdgeContextMenu.Text = "Horizontal";
            this.horizontalEdgeContextMenu.Click += new System.EventHandler(this.horizontalEdgeContextMenu_Click);
            // 
            // fixedLengthEdgeContextMenu
            // 
            this.fixedLengthEdgeContextMenu.Image = global::PolygonEditor.Properties.Resources.length_icon;
            this.fixedLengthEdgeContextMenu.Name = "fixedLengthEdgeContextMenu";
            this.fixedLengthEdgeContextMenu.Size = new System.Drawing.Size(166, 26);
            this.fixedLengthEdgeContextMenu.Text = "Fixed Length";
            this.fixedLengthEdgeContextMenu.Click += new System.EventHandler(this.fixedLengthEdgeContextMenu_Click);
            // 
            // addVerticeToolStripMenuItem
            // 
            this.addVerticeToolStripMenuItem.Name = "addVerticeToolStripMenuItem";
            this.addVerticeToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.addVerticeToolStripMenuItem.Text = "Add Vertice";
            this.addVerticeToolStripMenuItem.Click += new System.EventHandler(this.addVerticeToolStripMenuItem_Click);
            // 
            // drawAreaBox
            // 
            this.drawAreaBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.drawAreaBox.BackColor = System.Drawing.Color.White;
            this.drawAreaBox.Location = new System.Drawing.Point(12, 12);
            this.drawAreaBox.Name = "drawAreaBox";
            this.drawAreaBox.Size = new System.Drawing.Size(1158, 720);
            this.drawAreaBox.TabIndex = 0;
            this.drawAreaBox.TabStop = false;
            this.drawAreaBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.drawAreaBox_MouseDoubleClick);
            this.drawAreaBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.drawAreaBox_MouseDown);
            this.drawAreaBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.drawAreaBox_MouseMove);
            this.drawAreaBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawAreaBox_MouseUp);
            // 
            // verticeContextMenu
            // 
            this.verticeContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.verticeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteVerticeToolStripMenuItem});
            this.verticeContextMenu.Name = "verticeContextMenu";
            this.verticeContextMenu.Size = new System.Drawing.Size(172, 28);
            // 
            // deleteVerticeToolStripMenuItem
            // 
            this.deleteVerticeToolStripMenuItem.Name = "deleteVerticeToolStripMenuItem";
            this.deleteVerticeToolStripMenuItem.Size = new System.Drawing.Size(171, 24);
            this.deleteVerticeToolStripMenuItem.Text = "Delete Vertice";
            this.deleteVerticeToolStripMenuItem.Click += new System.EventHandler(this.deleteVerticeToolStripMenuItem_Click);
            // 
            // polygonContextMenu
            // 
            this.polygonContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.polygonContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.polygonContextMenu.Name = "polygonMenuStrip";
            this.polygonContextMenu.Size = new System.Drawing.Size(180, 28);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.deleteToolStripMenuItem.Text = "Delete Polygon";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 753);
            this.Controls.Add(this.newPolygonButton);
            this.Controls.Add(this.drawAreaBox);
            this.Name = "MainForm";
            this.Text = "Polygons Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.edgeContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.drawAreaBox)).EndInit();
            this.verticeContextMenu.ResumeLayout(false);
            this.polygonContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox drawAreaBox;
        private System.Windows.Forms.Button newPolygonButton;
        private System.Windows.Forms.ContextMenuStrip edgeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem verticalEdgeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem horizontalEdgeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem fixedLengthEdgeContextMenu;
        private System.Windows.Forms.ContextMenuStrip verticeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteVerticeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip polygonContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addVerticeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}