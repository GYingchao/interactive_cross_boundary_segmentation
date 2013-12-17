namespace CrossBoundaryBrushes
{
    partial class FormMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			MyGeometry.Matrix4d matrix4d1 = new MyGeometry.Matrix4d();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripSplitButtonOpen = new System.Windows.Forms.ToolStripSplitButton();
			this.openMeshFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openCameraFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.segmentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSplitButtonSaveFile = new System.Windows.Forms.ToolStripSplitButton();
			this.saveMeshFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveCameraFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.segmentationToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonViewingTool = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSketchTool = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.panelOperation = new System.Windows.Forms.Panel();
			this.switchModeButton = new System.Windows.Forms.Button();
			this.Undo = new System.Windows.Forms.Button();
			this.clear = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.buttonCloseTab = new System.Windows.Forms.Button();
			this.buttonShowHideProperty = new System.Windows.Forms.Button();
			this.tabControlModelList = new System.Windows.Forms.TabControl();
			this.meshView1 = new MeshView();
			this.buttonClearOutputText = new System.Windows.Forms.Button();
			this.textBoxOutput = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageModel = new System.Windows.Forms.TabPage();
			this.propertyGridModel = new System.Windows.Forms.PropertyGrid();
			this.tabPageDisplay = new System.Windows.Forms.TabPage();
			this.propertyGridDisplay = new System.Windows.Forms.PropertyGrid();
			this.tabPageTools = new System.Windows.Forms.TabPage();
			this.propertyGridTools = new System.Windows.Forms.PropertyGrid();
			this.tabPageOthers = new System.Windows.Forms.TabPage();
			this.propertyGridOthers = new System.Windows.Forms.PropertyGrid();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.buttonSkepSkeletonizer = new System.Windows.Forms.Button();
			this.timer2 = new System.Windows.Forms.Timer(this.components);
			this.toolStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.panelOperation.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageModel.SuspendLayout();
			this.tabPageDisplay.SuspendLayout();
			this.tabPageTools.SuspendLayout();
			this.tabPageOthers.SuspendLayout();
			this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButtonOpen,
            this.toolStripSplitButtonSaveFile,
            this.toolStripSeparator1,
            this.toolStripButtonViewingTool,
            this.toolStripButtonSketchTool,
            this.toolStripButton1,
            this.toolStripSeparator4});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(228, 39);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripSplitButtonOpen
			// 
			this.toolStripSplitButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripSplitButtonOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMeshFileToolStripMenuItem,
            this.openCameraFileToolStripMenuItem,
            this.segmentationToolStripMenuItem});
			this.toolStripSplitButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButtonOpen.Image")));
			this.toolStripSplitButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButtonOpen.Name = "toolStripSplitButtonOpen";
			this.toolStripSplitButtonOpen.Size = new System.Drawing.Size(48, 36);
			this.toolStripSplitButtonOpen.Text = "Open File";
			this.toolStripSplitButtonOpen.ButtonClick += new System.EventHandler(this.toolStripSplitButtonOpen_ButtonClick);
			// 
			// openMeshFileToolStripMenuItem
			// 
			this.openMeshFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openMeshFileToolStripMenuItem.Image")));
			this.openMeshFileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.openMeshFileToolStripMenuItem.Name = "openMeshFileToolStripMenuItem";
			this.openMeshFileToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.openMeshFileToolStripMenuItem.Text = "Mesh File...";
			this.openMeshFileToolStripMenuItem.Click += new System.EventHandler(this.openMeshFileToolStripMenuItem_Click);
			// 
			// openCameraFileToolStripMenuItem
			// 
			this.openCameraFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openCameraFileToolStripMenuItem.Image")));
			this.openCameraFileToolStripMenuItem.Name = "openCameraFileToolStripMenuItem";
			this.openCameraFileToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.openCameraFileToolStripMenuItem.Text = "Camera File...";
			this.openCameraFileToolStripMenuItem.Click += new System.EventHandler(this.openCameraFileToolStripMenuItem_Click);
			// 
			// segmentationToolStripMenuItem
			// 
			this.segmentationToolStripMenuItem.Name = "segmentationToolStripMenuItem";
			this.segmentationToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.segmentationToolStripMenuItem.Text = "Segmentation";
			this.segmentationToolStripMenuItem.Click += new System.EventHandler(this.segmentationToolStripMenuItem_Click);
			// 
			// toolStripSplitButtonSaveFile
			// 
			this.toolStripSplitButtonSaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripSplitButtonSaveFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMeshFileToolStripMenuItem,
            this.saveCameraFileToolStripMenuItem,
            this.segmentationToolStripMenuItem1});
			this.toolStripSplitButtonSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButtonSaveFile.Image")));
			this.toolStripSplitButtonSaveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButtonSaveFile.Name = "toolStripSplitButtonSaveFile";
			this.toolStripSplitButtonSaveFile.Size = new System.Drawing.Size(48, 36);
			this.toolStripSplitButtonSaveFile.Text = "Save File";
			// 
			// saveMeshFileToolStripMenuItem
			// 
			this.saveMeshFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveMeshFileToolStripMenuItem.Image")));
			this.saveMeshFileToolStripMenuItem.Name = "saveMeshFileToolStripMenuItem";
			this.saveMeshFileToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.saveMeshFileToolStripMenuItem.Text = "Mesh File...";
			this.saveMeshFileToolStripMenuItem.Click += new System.EventHandler(this.saveMeshFileToolStripMenuItem_Click);
			// 
			// saveCameraFileToolStripMenuItem
			// 
			this.saveCameraFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveCameraFileToolStripMenuItem.Image")));
			this.saveCameraFileToolStripMenuItem.Name = "saveCameraFileToolStripMenuItem";
			this.saveCameraFileToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.saveCameraFileToolStripMenuItem.Text = "Camera File...";
			this.saveCameraFileToolStripMenuItem.Click += new System.EventHandler(this.saveCameraFileToolStripMenuItem_Click);
			// 
			// segmentationToolStripMenuItem1
			// 
			this.segmentationToolStripMenuItem1.Name = "segmentationToolStripMenuItem1";
			this.segmentationToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
			this.segmentationToolStripMenuItem1.Text = "Segmentation";
			this.segmentationToolStripMenuItem1.Click += new System.EventHandler(this.segmentationToolStripMenuItem1_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
			// 
			// toolStripButtonViewingTool
			// 
			this.toolStripButtonViewingTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonViewingTool.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonViewingTool.Image")));
			this.toolStripButtonViewingTool.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonViewingTool.Name = "toolStripButtonViewingTool";
			this.toolStripButtonViewingTool.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonViewingTool.Text = "Viewing Tool";
			this.toolStripButtonViewingTool.Click += new System.EventHandler(this.toolStripButtonViewingTool_Click);
			// 
			// toolStripButtonSketchTool
			// 
			this.toolStripButtonSketchTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSketchTool.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSketchTool.Image")));
			this.toolStripButtonSketchTool.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSketchTool.Name = "toolStripButtonSketchTool";
			this.toolStripButtonSketchTool.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonSketchTool.Text = "Brush Tool";
			this.toolStripButtonSketchTool.ToolTipText = "Brush Tool (Keyboard \'S\')";
			this.toolStripButtonSketchTool.Click += new System.EventHandler(this.toolStripButtonSketchTool_Click);
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(36, 36);
			this.toolStripButton1.Text = "Multi-stroke mode";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click_1);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(816, 465);
			this.splitContainer1.SplitterDistance = 570;
			this.splitContainer1.TabIndex = 3;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.panelOperation);
			this.splitContainer2.Panel1.Controls.Add(this.button3);
			this.splitContainer2.Panel1.Controls.Add(this.buttonCloseTab);
			this.splitContainer2.Panel1.Controls.Add(this.buttonShowHideProperty);
			this.splitContainer2.Panel1.Controls.Add(this.tabControlModelList);
			this.splitContainer2.Panel1.Controls.Add(this.meshView1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.buttonClearOutputText);
			this.splitContainer2.Panel2.Controls.Add(this.textBoxOutput);
			this.splitContainer2.Panel2Collapsed = true;
			this.splitContainer2.Size = new System.Drawing.Size(570, 465);
			this.splitContainer2.SplitterDistance = 338;
			this.splitContainer2.TabIndex = 0;
			// 
			// panelOperation
			// 
			this.panelOperation.BackColor = System.Drawing.SystemColors.Control;
			this.panelOperation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelOperation.Controls.Add(this.switchModeButton);
			this.panelOperation.Controls.Add(this.Undo);
			this.panelOperation.Controls.Add(this.clear);
			this.panelOperation.Location = new System.Drawing.Point(2, 23);
			this.panelOperation.Margin = new System.Windows.Forms.Padding(2);
			this.panelOperation.Name = "panelOperation";
			this.panelOperation.Size = new System.Drawing.Size(77, 161);
			this.panelOperation.TabIndex = 12;
			// 
			// switchModeButton
			// 
			this.switchModeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.switchModeButton.BackColor = System.Drawing.Color.White;
			this.switchModeButton.Font = new System.Drawing.Font("Microsoft YaHei", 7.304348F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.switchModeButton.ForeColor = System.Drawing.Color.Red;
			this.switchModeButton.Location = new System.Drawing.Point(12, 9);
			this.switchModeButton.Margin = new System.Windows.Forms.Padding(2);
			this.switchModeButton.Name = "switchModeButton";
			this.switchModeButton.Size = new System.Drawing.Size(48, 43);
			this.switchModeButton.TabIndex = 12;
			this.switchModeButton.Text = "Part";
			this.switchModeButton.UseVisualStyleBackColor = false;
			this.switchModeButton.Click += new System.EventHandler(this.switchModeButton_Click_1);
			// 
			// Undo
			// 
			this.Undo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.Undo.BackColor = System.Drawing.Color.White;
			this.Undo.Font = new System.Drawing.Font("Microsoft YaHei", 7.304348F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Undo.Location = new System.Drawing.Point(12, 56);
			this.Undo.Margin = new System.Windows.Forms.Padding(2);
			this.Undo.Name = "Undo";
			this.Undo.Size = new System.Drawing.Size(49, 43);
			this.Undo.TabIndex = 15;
			this.Undo.Text = "Undo";
			this.Undo.UseVisualStyleBackColor = false;
			this.Undo.Click += new System.EventHandler(this.Undo_Click);
			// 
			// clear
			// 
			this.clear.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.clear.BackColor = System.Drawing.Color.White;
			this.clear.Font = new System.Drawing.Font("Microsoft YaHei", 7.304348F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.clear.Location = new System.Drawing.Point(12, 103);
			this.clear.Margin = new System.Windows.Forms.Padding(2);
			this.clear.Name = "clear";
			this.clear.Size = new System.Drawing.Size(50, 43);
			this.clear.TabIndex = 13;
			this.clear.Text = "Clear";
			this.clear.UseVisualStyleBackColor = false;
			this.clear.Click += new System.EventHandler(this.clear_Click);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.BackgroundImage = global::CrossBoundaryBrushes.Properties.Resources.down;
			this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.button3.Location = new System.Drawing.Point(531, 438);
			this.button3.Margin = new System.Windows.Forms.Padding(2);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(36, 22);
			this.button3.TabIndex = 8;
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// buttonCloseTab
			// 
			this.buttonCloseTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCloseTab.Image = ((System.Drawing.Image)(resources.GetObject("buttonCloseTab.Image")));
			this.buttonCloseTab.Location = new System.Drawing.Point(531, 23);
			this.buttonCloseTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonCloseTab.Name = "buttonCloseTab";
			this.buttonCloseTab.Size = new System.Drawing.Size(36, 22);
			this.buttonCloseTab.TabIndex = 4;
			this.buttonCloseTab.UseVisualStyleBackColor = true;
			this.buttonCloseTab.Click += new System.EventHandler(this.buttonCloseTab_Click);
			// 
			// buttonShowHideProperty
			// 
			this.buttonShowHideProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonShowHideProperty.Image = ((System.Drawing.Image)(resources.GetObject("buttonShowHideProperty.Image")));
			this.buttonShowHideProperty.Location = new System.Drawing.Point(531, 409);
			this.buttonShowHideProperty.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonShowHideProperty.Name = "buttonShowHideProperty";
			this.buttonShowHideProperty.Size = new System.Drawing.Size(36, 22);
			this.buttonShowHideProperty.TabIndex = 0;
			this.buttonShowHideProperty.UseVisualStyleBackColor = true;
			this.buttonShowHideProperty.Click += new System.EventHandler(this.buttonShowHideProperty_Click);
			// 
			// tabControlModelList
			// 
			this.tabControlModelList.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControlModelList.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.tabControlModelList.Location = new System.Drawing.Point(0, 0);
			this.tabControlModelList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControlModelList.Name = "tabControlModelList";
			this.tabControlModelList.SelectedIndex = 0;
			this.tabControlModelList.Size = new System.Drawing.Size(570, 22);
			this.tabControlModelList.TabIndex = 2;
			this.tabControlModelList.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlModelList_Selected);
			// 
			// meshView1
			// 
			this.meshView1.ClearStrokesInRealtime = true;
			matrix4d1.Element = new double[] {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0};
			this.meshView1.CurrTransformation = matrix4d1;
			this.meshView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.meshView1.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.meshView1.Location = new System.Drawing.Point(0, 0);
			this.meshView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.meshView1.MultiStroke = false;
			this.meshView1.Name = "meshView1";
			this.meshView1.Size = new System.Drawing.Size(570, 465);
			this.meshView1.TabIndex = 3;
			this.meshView1.Text = "FFsadf";
			// 
			// buttonClearOutputText
			// 
			this.buttonClearOutputText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClearOutputText.Image = ((System.Drawing.Image)(resources.GetObject("buttonClearOutputText.Image")));
			this.buttonClearOutputText.Location = new System.Drawing.Point(1193, 57);
			this.buttonClearOutputText.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonClearOutputText.Name = "buttonClearOutputText";
			this.buttonClearOutputText.Size = new System.Drawing.Size(36, 22);
			this.buttonClearOutputText.TabIndex = 1;
			this.buttonClearOutputText.UseVisualStyleBackColor = true;
			this.buttonClearOutputText.Click += new System.EventHandler(this.buttonClearOutputText_Click);
			// 
			// textBoxOutput
			// 
			this.textBoxOutput.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxOutput.Location = new System.Drawing.Point(0, 0);
			this.textBoxOutput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxOutput.Multiline = true;
			this.textBoxOutput.Name = "textBoxOutput";
			this.textBoxOutput.ReadOnly = true;
			this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxOutput.Size = new System.Drawing.Size(150, 46);
			this.textBoxOutput.TabIndex = 0;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageModel);
			this.tabControl1.Controls.Add(this.tabPageDisplay);
			this.tabControl1.Controls.Add(this.tabPageTools);
			this.tabControl1.Controls.Add(this.tabPageOthers);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(242, 465);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageModel
			// 
			this.tabPageModel.Controls.Add(this.propertyGridModel);
			this.tabPageModel.Location = new System.Drawing.Point(4, 23);
			this.tabPageModel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageModel.Name = "tabPageModel";
			this.tabPageModel.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageModel.Size = new System.Drawing.Size(234, 438);
			this.tabPageModel.TabIndex = 0;
			this.tabPageModel.Text = "Model";
			this.tabPageModel.UseVisualStyleBackColor = true;
			// 
			// propertyGridModel
			// 
			this.propertyGridModel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridModel.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.propertyGridModel.Location = new System.Drawing.Point(3, 2);
			this.propertyGridModel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.propertyGridModel.Name = "propertyGridModel";
			this.propertyGridModel.Size = new System.Drawing.Size(228, 434);
			this.propertyGridModel.TabIndex = 0;
			// 
			// tabPageDisplay
			// 
			this.tabPageDisplay.Controls.Add(this.propertyGridDisplay);
			this.tabPageDisplay.Location = new System.Drawing.Point(4, 23);
			this.tabPageDisplay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageDisplay.Name = "tabPageDisplay";
			this.tabPageDisplay.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageDisplay.Size = new System.Drawing.Size(234, 438);
			this.tabPageDisplay.TabIndex = 1;
			this.tabPageDisplay.Text = "Display";
			this.tabPageDisplay.UseVisualStyleBackColor = true;
			// 
			// propertyGridDisplay
			// 
			this.propertyGridDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridDisplay.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.propertyGridDisplay.Location = new System.Drawing.Point(3, 2);
			this.propertyGridDisplay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.propertyGridDisplay.Name = "propertyGridDisplay";
			this.propertyGridDisplay.Size = new System.Drawing.Size(228, 434);
			this.propertyGridDisplay.TabIndex = 0;
			// 
			// tabPageTools
			// 
			this.tabPageTools.Controls.Add(this.propertyGridTools);
			this.tabPageTools.Location = new System.Drawing.Point(4, 23);
			this.tabPageTools.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageTools.Name = "tabPageTools";
			this.tabPageTools.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageTools.Size = new System.Drawing.Size(234, 438);
			this.tabPageTools.TabIndex = 2;
			this.tabPageTools.Text = "Tools";
			this.tabPageTools.UseVisualStyleBackColor = true;
			// 
			// propertyGridTools
			// 
			this.propertyGridTools.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridTools.Location = new System.Drawing.Point(3, 2);
			this.propertyGridTools.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.propertyGridTools.Name = "propertyGridTools";
			this.propertyGridTools.Size = new System.Drawing.Size(228, 434);
			this.propertyGridTools.TabIndex = 0;
			// 
			// tabPageOthers
			// 
			this.tabPageOthers.Controls.Add(this.propertyGridOthers);
			this.tabPageOthers.Location = new System.Drawing.Point(4, 23);
			this.tabPageOthers.Margin = new System.Windows.Forms.Padding(2);
			this.tabPageOthers.Name = "tabPageOthers";
			this.tabPageOthers.Padding = new System.Windows.Forms.Padding(2);
			this.tabPageOthers.Size = new System.Drawing.Size(234, 438);
			this.tabPageOthers.TabIndex = 3;
			this.tabPageOthers.Text = "Others";
			this.tabPageOthers.UseVisualStyleBackColor = true;
			// 
			// propertyGridOthers
			// 
			this.propertyGridOthers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridOthers.Location = new System.Drawing.Point(2, 2);
			this.propertyGridOthers.Margin = new System.Windows.Forms.Padding(2);
			this.propertyGridOthers.Name = "propertyGridOthers";
			this.propertyGridOthers.Size = new System.Drawing.Size(230, 434);
			this.propertyGridOthers.TabIndex = 0;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.BottomToolStripPanel
			// 
			this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
			this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(816, 465);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.LeftToolStripPanelVisible = false;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.RightToolStripPanelVisible = false;
			this.toolStripContainer1.Size = new System.Drawing.Size(816, 526);
			this.toolStripContainer1.TabIndex = 4;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 0);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(816, 22);
			this.statusStrip1.TabIndex = 5;
			this.statusStrip1.Text = "statusbar";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(42, 17);
			this.toolStripStatusLabel1.Text = "status";
			// 
			// timer1
			// 
			this.timer1.Interval = 20;
			
			// 
			// buttonSkepSkeletonizer
			// 
			this.buttonSkepSkeletonizer.Location = new System.Drawing.Point(0, 0);
			this.buttonSkepSkeletonizer.Name = "buttonSkepSkeletonizer";
			this.buttonSkepSkeletonizer.Size = new System.Drawing.Size(75, 23);
			this.buttonSkepSkeletonizer.TabIndex = 0;
			// 
			// timer2
			// 
			this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(816, 526);
			this.Controls.Add(this.toolStripContainer1);
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "FormMain";
			this.Text = "Cross-Boundary Brushes";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			this.panelOperation.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPageModel.ResumeLayout(false);
			this.tabPageDisplay.ResumeLayout(false);
			this.tabPageTools.ResumeLayout(false);
			this.tabPageOthers.ResumeLayout(false);
			this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageModel;
        private System.Windows.Forms.TabPage tabPageDisplay;
        private System.Windows.Forms.TabPage tabPageTools;
        private System.Windows.Forms.PropertyGrid propertyGridModel;
        private System.Windows.Forms.PropertyGrid propertyGridDisplay;
		private System.Windows.Forms.PropertyGrid propertyGridTools;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonShowHideProperty;
		private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButtonViewingTool;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button buttonClearOutputText;
		private System.Windows.Forms.TabControl tabControlModelList;
		private MeshView meshView1;
		private System.Windows.Forms.ToolStripMenuItem openMeshFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripMenuItem openCameraFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonSaveFile;
		private System.Windows.Forms.ToolStripMenuItem saveMeshFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveCameraFileToolStripMenuItem;
		private System.Windows.Forms.Button buttonCloseTab;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.TabPage tabPageOthers;
		private System.Windows.Forms.PropertyGrid propertyGridOthers;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button buttonSkepSkeletonizer;
		private System.Windows.Forms.ToolStripButton toolStripButtonSketchTool;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Timer timer2;
		private System.Windows.Forms.ToolStripMenuItem segmentationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem segmentationToolStripMenuItem1;
		private System.Windows.Forms.Panel panelOperation;
		private System.Windows.Forms.Button switchModeButton;
		private System.Windows.Forms.Button Undo;
		private System.Windows.Forms.Button clear;
    }
}

