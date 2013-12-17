using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CrossBoundaryBrushes
{
	public partial class FormView : Form
	{
		public FormView()
		{
			InitializeComponent();
		}
		public void ShowModels(List<MeshRecord> meshes)
		{
			int sz = meshes.Count > 4 ? 4 : meshes.Count;
			MeshRecord[] r = new MeshRecord[4];
			for (int i = 0; i < sz; ++i)
			{
				r[i] = meshes[i];
			}
			this.meshView1.SetModel(r[0]);
			this.meshView2.SetModel(r[1]);
			this.meshView3.SetModel(r[2]);
			this.meshView4.SetModel(r[3]);
		}

		private void A_Click(object sender, EventArgs e)
		{
			this.splitContainer2.Panel2Collapsed = !this.splitContainer2.Panel2Collapsed;
		}

		private void B_Click(object sender, EventArgs e)
		{
			this.splitContainer3.Panel2Collapsed = !this.splitContainer3.Panel2Collapsed;
		}

		private void O_Click(object sender, EventArgs e)
		{
			this.splitContainer1.Panel2Collapsed = !this.splitContainer1.Panel2Collapsed;
		}

		private void FormView_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.meshView1.Dispose();
			this.meshView2.Dispose();
			this.meshView3.Dispose();
			this.meshView4.Dispose();
			GC.Collect();
		}
	}
}
