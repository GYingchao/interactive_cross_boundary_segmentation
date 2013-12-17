using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using CsGL.OpenGL;
using MyGeometry;

namespace CrossBoundaryBrushes
{
    public partial class FormMain : Form
    {
		List<MeshRecord> meshes = new List<MeshRecord>();
		MeshRecord currentMeshRecord = null;

		delegate void SetTextCallback(string text);
		

		// --- cursors ---
		//private Cursor cursor = new Cursor();

        public FormMain()
        {
            InitializeComponent();

			this.propertyGridDisplay.SelectedObject = Program.displayProperty;
			this.propertyGridTools.SelectedObject = Program.toolsProperty;
			toolStripButtonViewingTool.Checked = true;

        }
		public void PrintText(string s)
		{
			if (this.textBoxOutput.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(PrintText);
				this.Invoke(d, new object[] { s });
			}
			else
			{
				this.textBoxOutput.AppendText(s + "\n");
			}
		}
		public void Print3DText(Vector3d pos, string s)
		{
			GL.glRasterPos3d(pos.x, pos.y, pos.z);
			GL.glPushAttrib(GL.GL_LIST_BIT);					// Pushes The Display List Bits
			GL.glListBase(this.meshView1.fontBase);				// Sets The Base Character to 32
			GL.glCallLists(s.Length, GL.GL_UNSIGNED_SHORT, s);	// Draws The Display List Text
			GL.glPopAttrib();									// Pops The Display List Bits
		}
		public void OpenMeshFile()
		{
			openFileDialog1.FileName = "";
			openFileDialog1.Filter = "Mesh files (*.obj)|*.obj";
			openFileDialog1.CheckFileExists = true;

			DialogResult ret = openFileDialog1.ShowDialog(this);

			if (ret == DialogResult.OK)
			{
				StreamReader sr = new StreamReader(openFileDialog1.FileName);
				Mesh m = new Mesh(sr);
				sr.Close();
				MeshRecord rec = new MeshRecord(openFileDialog1.FileName, m);

				meshes.Add(rec);
				currentMeshRecord = rec;
				TabPage page = new TabPage(rec.ToString());
				page.Tag = rec;
				tabControlModelList.TabPages.Add(page);
				tabControlModelList.SelectedTab = page;
				meshView1.SetModel(rec);
				propertyGridModel.SelectedObject = rec;
				PrintText("Loaded mesh " + openFileDialog1.FileName + "\n");
			
				// ----------------------------------------------------------------------
				// initialize the harminic field
				// ----------------------------------------------------------------------
				CrossBoundaryBrushes.Option opt = new CrossBoundaryBrushes.Option();
				opt.Part_Type = true;
				currentMeshRecord.CrossBoundaryBrushes =
					new CrossBoundaryBrushes(currentMeshRecord.Mesh, opt);
				currentMeshRecord.CrossBoundaryBrushes.InitialHarminicField();
				
				this.meshView1.Refresh();
			
			}
		}
		public void SaveMeshFile()
		{
			if (currentMeshRecord == null || currentMeshRecord.Mesh == null)
				return;

			saveFileDialog1.FileName = "";
			saveFileDialog1.Filter = "Mesh files (*.obj)|*.obj";
			saveFileDialog1.OverwritePrompt = true;

			DialogResult ret = saveFileDialog1.ShowDialog(this);

			if (ret == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
				currentMeshRecord.Mesh.Write(sw);
				sw.Close();
				PrintText("Saved mesh " + saveFileDialog1.FileName + "\n");
			}
		}
		public void OpenCameraFile()
		{
			openFileDialog1.FileName = "";
			openFileDialog1.Filter = "Camera files (*.camera)|*.camera";
			openFileDialog1.CheckFileExists = true;

			DialogResult ret = openFileDialog1.ShowDialog(this);
			if (ret == DialogResult.OK)
			{
				StreamReader sr = new StreamReader(openFileDialog1.FileName);
				XmlSerializer xs = new XmlSerializer(typeof(Matrix4d));
				meshView1.CurrTransformation = (Matrix4d)xs.Deserialize(sr);
				sr.Close();
				PrintText("Loaded camera " + openFileDialog1.FileName + "\n");
			}
		}
		public void SaveCameraFile()
		{
			saveFileDialog1.FileName = "";
			saveFileDialog1.Filter = "Camera files (*.camera)|*.camera";
			saveFileDialog1.OverwritePrompt = true;

			DialogResult ret = saveFileDialog1.ShowDialog(this);
			if (ret == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
				XmlSerializer xs = new XmlSerializer(typeof(Matrix4d));
				xs.Serialize(sw, meshView1.CurrTransformation);
				sw.Close();
				PrintText("Saved camera " + saveFileDialog1.FileName + "\n");
			}
		}
		public void OpenSelectionFile()
		{
			if (currentMeshRecord == null || currentMeshRecord.Mesh == null)
				return;

			openFileDialog1.FileName = "";
			openFileDialog1.Filter = "Selection files (*.sel)|*.sel";
			openFileDialog1.CheckFileExists = true;

			DialogResult ret = openFileDialog1.ShowDialog(this);
			if (ret == DialogResult.OK)
			{
				StreamReader sr = new StreamReader(openFileDialog1.FileName);
				XmlSerializer xs = new XmlSerializer(typeof(byte[]));
				currentMeshRecord.Mesh.Flag = (byte[])xs.Deserialize(sr);
				sr.Close();
				PrintText("Loaded selection " + openFileDialog1.FileName + "\n");
			}
		}
		public void SaveSelectionFile()
		{
			if (currentMeshRecord == null || currentMeshRecord.Mesh == null)
				return;

			saveFileDialog1.FileName = "";
			saveFileDialog1.Filter = "Selection files (*.sel)|*.sel";
			saveFileDialog1.OverwritePrompt = true;

			DialogResult ret = saveFileDialog1.ShowDialog(this);
			if (ret == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
				XmlSerializer xs = new XmlSerializer(typeof(byte[]));
				xs.Serialize(sw, currentMeshRecord.Mesh.Flag);
				sw.Close();
				PrintText("Saved selection " + saveFileDialog1.FileName + "\n");
			}
		}
	

		public void CloseTab()
		{
			if (tabControlModelList.SelectedTab != null)
			{
				tabControlModelList.TabPages.Remove(tabControlModelList.SelectedTab);
			}
		}

		public void SaveColorList(StreamWriter sw)
		{
			sw.WriteLine(Program.displayProperty.ColorMall.Length.ToString());
			foreach (Color c in Program.displayProperty.ColorMall)
			{
				sw.WriteLine(c.R); sw.WriteLine(c.G); sw.WriteLine(c.B);
			}
		}
		public void LoadColorList(StreamReader sr)
		{
			string s;
			string[] separator = { " ", "\t" };
			s = sr.ReadLine();
			int count = int.Parse(s);
			Color[] c = new Color[count];
			for (int i = 0; i < count; i++)
			{
				s = sr.ReadLine(); byte r = byte.Parse(s);
				s = sr.ReadLine(); byte g = byte.Parse(s);
				s = sr.ReadLine(); byte b = byte.Parse(s);

				c[i] = Color.FromArgb(r, g, b);
			}
			Program.displayProperty.ColorMall = c;
		}

		private void toolStripSplitButtonOpen_ButtonClick(object sender, EventArgs e)
		{
			OpenMeshFile();
		}
		private void toolStripButtonViewingTool_Click(object sender, EventArgs e)
		{
			toolStripButtonViewingTool.Checked = true;
			toolStripButtonSketchTool.Checked = false;
			Program.currentMode = Program.EnumOperationMode.Viewing;
			Cursor = Cursors.Arrow;
		}
		private void toolStripButtonSelectionTool_Click(object sender, EventArgs e)
		{
			toolStripButtonViewingTool.Checked = false;
			toolStripButtonSketchTool.Checked = false;
			Program.currentMode = Program.EnumOperationMode.Selection;
			Cursor = Cursors.Arrow;
		}
		private void toolStripButtonSketchTool_Click(object sender, EventArgs e)
		{
			EnableSketching();
		}
		private void EnableSketching()
		{
			toolStripButtonSketchTool.Checked = true;
			toolStripButtonViewingTool.Checked = false;
			Program.currentMode = Program.EnumOperationMode.Sketching;
			Cursor = Cursors.Hand;
		}
		private void DisableSketching()
		{
			toolStripButtonSketchTool.Checked = false;
			toolStripButtonViewingTool.Checked = true;
			Program.currentMode = Program.EnumOperationMode.Viewing;
			Cursor = Cursors.Default;
		}

		private void toolStripSplitButtonCreateDeformer_ButtonClick(object sender, EventArgs e)
		{
			
		}

		private void toolStripSplitButtonSkeletonization_ButtonClick(object sender, EventArgs e)
		{
			
		}


		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			if (currentMeshRecord.CrossBoundaryBrushes != null)
			{
				currentMeshRecord.CrossBoundaryBrushes.Update();
				meshView1.Refresh();
			}
		}

		private void openMeshFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenMeshFile();
		}
		private void openCameraFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenCameraFile();
		}
		private void openSelectionFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSelectionFile();
		}
		private void saveMeshFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveMeshFile();
		}
		private void saveCameraFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveCameraFile();
		}
		private void saveSelectionFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveSelectionFile();
		}
		private void saveSegmentationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}
		private void createDualLaplacianDeformerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}
		private void createGeneralReducedDeformerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}
		private void createIsolineDeformerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}

        private void buttonShowHideProperty_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = ! splitContainer1.Panel2Collapsed;
        }
		private void buttonClearOutputText_Click(object sender, EventArgs e)
		{
			textBoxOutput.Clear();
		}
		private void buttonCloseTab_Click(object sender, EventArgs e)
		{
			if (currentMeshRecord != null)
			{
				meshes.Remove(currentMeshRecord);
				currentMeshRecord = (MeshRecord)tabControlModelList.SelectedTab.Tag;
			}
			CloseTab();
		}

		private void tabControlModelList_Selected(object sender, TabControlEventArgs e)
		{
			if (tabControlModelList.SelectedTab != null)
			{
				MeshRecord rec = (MeshRecord)tabControlModelList.SelectedTab.Tag;
				meshView1.SetModel(rec);
				propertyGridModel.SelectedObject = rec;
				currentMeshRecord = rec;
			}
			else
			{
				meshView1.SetModel(null);
				propertyGridModel.SelectedObject = null;
				currentMeshRecord = null;
			}
		}

		private void FormMain_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.O: OpenMeshFile(); break;
				}
			}
			else
			{
				switch (e.KeyCode)
				{
					case Keys.F1: OpenMeshFile(); break;
					case Keys.F2: Program.currentMode = Program.EnumOperationMode.Viewing; break;
					case Keys.F3: Program.currentMode = Program.EnumOperationMode.Selection; break;
					case Keys.F4: Program.currentMode = Program.EnumOperationMode.Moving; break;
					case Keys.F5: break;
					case Keys.D:
						if (!this.meshView1.MultiStroke)
							this.meshView1.ClearStrokes();
						this.meshView1.RemovePrevStroke();
						if (currentMeshRecord.CrossBoundaryBrushes != null)
						{
							currentMeshRecord.CrossBoundaryBrushes.RemovePrevCut();
						}
						this.meshView1.Refresh();
						break;
					case Keys.S: //-- enable sketching --
						if (toolStripButtonSketchTool.Checked)
							DisableSketching();
						else
							EnableSketching();
						break;
					case Keys.H: // -- create harmonic solver --
						CreateHarmonicSolver(true);
						this.meshView1.Refresh();
						break;
					case Keys.P:
						CreateHarmonicSolver(false);
						this.meshView1.SetMainStrokeColor(Color.Blue);
						switchModeButton.ForeColor = Color.Blue;
						this.meshView1.Refresh();
						break;
					case Keys.M:
						this.meshView1.MultiStroke = !this.meshView1.MultiStroke;
						toolStripButton1.Checked = this.meshView1.MultiStroke;
						if (!this.meshView1.MultiStroke)
						{
							this.meshView1.ClearStrokes();
							this.meshView1.Refresh();
						}
						break;
					case Keys.C:
						if (currentMeshRecord != null &&
							currentMeshRecord.CrossBoundaryBrushes != null)
						{
							meshView1.Cut();
							meshView1.Refresh();
						}
						break;
					case Keys.R:
						meshView1.ClearStrokesInRealtime = !meshView1.ClearStrokesInRealtime;
						break;
					case Keys.E:
						meshView1.ClearStrokes();
						meshView1.Refresh(); 
						break;
					case Keys.K:
						this.meshView1.RemovePrevStroke();
						break;
					case Keys.G:
						this.meshView1.RegainStrokeInfo();
						break;
				}
			}
		}
		private void CreateHarmonicSolver(bool part_type)
		{
			if (currentMeshRecord.CrossBoundaryBrushes != null || // -- already created --
				this.meshView1.Strokes.Count < 1)
			{	
				currentMeshRecord.CrossBoundaryBrushes.opt.Part_Type = part_type;
				return;
			}
			if (currentMeshRecord == null || currentMeshRecord.Mesh == null)
				return;
			CrossBoundaryBrushes.Option opt = new CrossBoundaryBrushes.Option();
			opt.Part_Type = part_type;
			currentMeshRecord.CrossBoundaryBrushes =
				new CrossBoundaryBrushes(currentMeshRecord.Mesh, opt);
			currentMeshRecord.CrossBoundaryBrushes.ObtainConstraints(
				this.meshView1.Strokes, 
				this.meshView1.FacesOnStrokes
			);

			currentMeshRecord.CrossBoundaryBrushes.Cut();
		}


		private void colorFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveFileDialog1.FileName = "";
			saveFileDialog1.Filter = "Color List (*.color)|*.color";
			saveFileDialog1.OverwritePrompt = true;

			DialogResult ret = saveFileDialog1.ShowDialog(this);

			if (ret == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
				SaveColorList(sw);
				sw.Close();
				PrintText("Saved color list" + saveFileDialog1.FileName + "\n");
			}
		}

		private void colorFileToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			openFileDialog1.FileName = "";
			openFileDialog1.Filter = "Color List(*.color)|*.color";
			openFileDialog1.CheckFileExists = true;

			DialogResult ret = openFileDialog1.ShowDialog(this);
			if (ret == DialogResult.OK)
			{
				StreamReader sr = new StreamReader(openFileDialog1.FileName);
				LoadColorList(sr);
				sr.Close();
				PrintText("Loaded color list" + openFileDialog1.FileName + "\n");
			}
		}

		private void SwitchCuttingMode()
		{
			if (currentMeshRecord != null && currentMeshRecord.CrossBoundaryBrushes != null)
			{
				bool mode = !currentMeshRecord.CrossBoundaryBrushes.opt.Part_Type;
				currentMeshRecord.CrossBoundaryBrushes.opt.Part_Type = mode;

				if (!mode)
				{
					switchModeButton.Text = "Patch";
					switchModeButton.ForeColor = Color.Blue;
					this.meshView1.SetMainStrokeColor(Color.Blue);
				}
				else
				{
					switchModeButton.Text = "Part";
					switchModeButton.ForeColor = Color.Red;
					this.meshView1.SetMainStrokeColor(Color.Black);
				}
				this.meshView1.Refresh();
			}
		}


		private void toolStripButton2_Click_1(object sender, EventArgs e)
		{
			
		}



		private void toolStripButton1_Click_2(object sender, EventArgs e)
		{
		
		}


		private void button3_Click(object sender, EventArgs e)
		{
			splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
		}


		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			
		}


		private void trackBar2_Scroll(object sender, EventArgs e)
		{
			
		}

		private void trackBar3_Scroll(object sender, EventArgs e)
		{
			
		}

		private void button7_Click(object sender, EventArgs e)
		{
			
		}

		private void button8_Click(object sender, EventArgs e)
		{
			
		}

		private void button9_Click(object sender, EventArgs e)
		{
			
		}

		private void toolStripButton1_Click_1(object sender, EventArgs e)
		{
			this.meshView1.MultiStroke = !this.meshView1.MultiStroke;
			Cursor = Cursors.Arrow;
			toolStripButton1.Checked = this.meshView1.MultiStroke;
		}

		private void strokesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentMeshRecord != null && currentMeshRecord.CrossBoundaryBrushes != null)
			{
				this.saveFileDialog1.FileName = "";
				saveFileDialog1.Filter = "txt File(*.txt)|*.txt";

				DialogResult ret = saveFileDialog1.ShowDialog(this);
				if (ret == DialogResult.OK)
				{
					StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
					currentMeshRecord.CrossBoundaryBrushes.SaveStrokes(sw);
					sw.Close();
				}
				
			}
		}


		private void timer2_Tick(object sender, EventArgs e)
		{
			this.meshView1.RotateModel();
		}

		private void segmentationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentMeshRecord != null && currentMeshRecord.CrossBoundaryBrushes != null)
			{
				this.openFileDialog1.FileName = "";
				openFileDialog1.Filter = "txt File(*.txt)|*.txt";

				DialogResult ret = openFileDialog1.ShowDialog(this);
				if (ret == DialogResult.OK)
				{
					StreamReader sr = new StreamReader(openFileDialog1.FileName);
					currentMeshRecord.CrossBoundaryBrushes.LoadSegmentation(sr);
					sr.Close();
				}

			}
		}

		private void segmentationToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (currentMeshRecord != null && currentMeshRecord.CrossBoundaryBrushes != null)
			{
				this.saveFileDialog1.FileName = "";
				saveFileDialog1.Filter = "seg File(*.seg)|*.seg";

				DialogResult ret = saveFileDialog1.ShowDialog(this);
				if (ret == DialogResult.OK)
				{
					StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
					currentMeshRecord.CrossBoundaryBrushes.SaveSegmentation(sw);
					sw.Close();
				}
			}
		}

		private void switchModeButton_Click_1(object sender, EventArgs e)
		{
			SwitchCuttingMode();
		}

		private void Undo_Click(object sender, EventArgs e)
		{
			if (currentMeshRecord == null || currentMeshRecord.CrossBoundaryBrushes == null) return;

			if (!this.meshView1.MultiStroke) this.meshView1.ClearStrokes();
			this.meshView1.RemovePrevStroke();

			currentMeshRecord.CrossBoundaryBrushes.RemovePrevCut();

			this.meshView1.Refresh();
		}


		private void clear_Click(object sender, EventArgs e)
		{
			this.meshView1.ClearStrokes();
			this.meshView1.Refresh();
		}
    }
}