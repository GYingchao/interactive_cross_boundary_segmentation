namespace CrossBoundaryBrushes
{
	partial class FormView
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
			MyGeometry.Matrix4d matrix4d1 = new MyGeometry.Matrix4d();
			MyGeometry.Matrix4d matrix4d2 = new MyGeometry.Matrix4d();
			MyGeometry.Matrix4d matrix4d3 = new MyGeometry.Matrix4d();
			MyGeometry.Matrix4d matrix4d4 = new MyGeometry.Matrix4d();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.O = new System.Windows.Forms.Button();
			this.A = new System.Windows.Forms.Button();
			this.meshView1 = new MeshView();
			this.meshView3 = new MeshView();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.B = new System.Windows.Forms.Button();
			this.meshView2 = new MeshView();
			this.meshView4 = new MeshView();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
			this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1076, 758);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(1076, 783);
			this.toolStripContainer1.TabIndex = 0;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer1.Size = new System.Drawing.Size(1076, 758);
			this.splitContainer1.SplitterDistance = 521;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.O);
			this.splitContainer2.Panel1.Controls.Add(this.A);
			this.splitContainer2.Panel1.Controls.Add(this.meshView1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.meshView3);
			this.splitContainer2.Size = new System.Drawing.Size(521, 758);
			this.splitContainer2.SplitterDistance = 364;
			this.splitContainer2.TabIndex = 0;
			// 
			// O
			// 
			this.O.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.O.BackColor = System.Drawing.Color.DodgerBlue;
			this.O.Font = new System.Drawing.Font("STCaiyun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.O.ForeColor = System.Drawing.Color.Blue;
			this.O.Location = new System.Drawing.Point(485, -1);
			this.O.Name = "O";
			this.O.Size = new System.Drawing.Size(33, 25);
			this.O.TabIndex = 2;
			this.O.Text = "O";
			this.O.UseVisualStyleBackColor = false;
			this.O.Click += new System.EventHandler(this.O_Click);
			// 
			// A
			// 
			this.A.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.A.BackColor = System.Drawing.Color.DodgerBlue;
			this.A.Font = new System.Drawing.Font("STCaiyun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.A.ForeColor = System.Drawing.Color.Blue;
			this.A.Location = new System.Drawing.Point(483, 334);
			this.A.Name = "A";
			this.A.Size = new System.Drawing.Size(33, 25);
			this.A.TabIndex = 1;
			this.A.Text = "A";
			this.A.UseVisualStyleBackColor = false;
			this.A.Click += new System.EventHandler(this.A_Click);
			// 
			// meshView1
			// 
			matrix4d1.Element = new double[] {
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1};
			this.meshView1.CurrTransformation = matrix4d1;
			this.meshView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.meshView1.Location = new System.Drawing.Point(0, 0);
			this.meshView1.Name = "meshView1";
			this.meshView1.Size = new System.Drawing.Size(519, 362);
			this.meshView1.TabIndex = 0;
			this.meshView1.Text = "meshView1";
			// 
			// meshView3
			// 
			matrix4d2.Element = new double[] {
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1};
			this.meshView3.CurrTransformation = matrix4d2;
			this.meshView3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.meshView3.Location = new System.Drawing.Point(0, 0);
			this.meshView3.Name = "meshView3";
			this.meshView3.Size = new System.Drawing.Size(519, 388);
			this.meshView3.TabIndex = 0;
			this.meshView3.Text = "meshView3";
			// 
			// splitContainer3
			// 
			this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.B);
			this.splitContainer3.Panel1.Controls.Add(this.meshView2);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.meshView4);
			this.splitContainer3.Size = new System.Drawing.Size(551, 758);
			this.splitContainer3.SplitterDistance = 364;
			this.splitContainer3.TabIndex = 0;
			// 
			// B
			// 
			this.B.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.B.BackColor = System.Drawing.Color.DodgerBlue;
			this.B.Font = new System.Drawing.Font("STCaiyun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.B.ForeColor = System.Drawing.Color.Blue;
			this.B.Location = new System.Drawing.Point(0, 334);
			this.B.Name = "B";
			this.B.Size = new System.Drawing.Size(33, 25);
			this.B.TabIndex = 2;
			this.B.Text = "B";
			this.B.UseVisualStyleBackColor = false;
			this.B.Click += new System.EventHandler(this.B_Click);
			// 
			// meshView2
			// 
			matrix4d3.Element = new double[] {
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1};
			this.meshView2.CurrTransformation = matrix4d3;
			this.meshView2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.meshView2.Location = new System.Drawing.Point(0, 0);
			this.meshView2.Name = "meshView2";
			this.meshView2.Size = new System.Drawing.Size(549, 362);
			this.meshView2.TabIndex = 0;
			this.meshView2.Text = "meshView2";
			// 
			// meshView4
			// 
			matrix4d4.Element = new double[] {
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1};
			this.meshView4.CurrTransformation = matrix4d4;
			this.meshView4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.meshView4.Location = new System.Drawing.Point(0, 0);
			this.meshView4.Name = "meshView4";
			this.meshView4.Size = new System.Drawing.Size(549, 388);
			this.meshView4.TabIndex = 0;
			this.meshView4.Text = "meshView4";
			// 
			// FormView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1076, 783);
			this.Controls.Add(this.toolStripContainer1);
			this.Name = "FormView";
			this.Text = "FormView";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormView_FormClosed);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private MeshView meshView1;
		private MeshView meshView3;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private MeshView meshView2;
		private MeshView meshView4;
		private System.Windows.Forms.Button A;
		private System.Windows.Forms.Button B;
		private System.Windows.Forms.Button O;

	}
}