using System.Drawing;
using System.IO;
using System.ComponentModel;
using System;
using MyGeometry;


namespace CrossBoundaryBrushes
{
	public class MeshRecord : IDisposable
	{
		private string filename;
		private Mesh mesh = null;
		private Matrix4d modelViewMatrix = Matrix4d.IdentityMatrix();


		public double[] originalVtPos = null;

		public string Filename
		{
			get { return filename; }
		}
		public int VertexCount
		{
			get { return mesh.VertexCount; }
		}
		public int FaceCount
		{
			get { return mesh.FaceCount; }
		}

		[Browsable(false)]
		public Mesh Mesh
		{
			get { return mesh; }
		}
		[Browsable(false)]
		public Matrix4d ModelViewMatrix
		{
			get { return modelViewMatrix; }
			set { modelViewMatrix = value; }
		}


		private CrossBoundaryBrushes crossBoundaryBrushes = null;
		public CrossBoundaryBrushes CrossBoundaryBrushes
		{
			get { return crossBoundaryBrushes; }
			set { crossBoundaryBrushes = value; }
		}


		public MeshRecord(string filename, Mesh mesh)
		{
			this.filename = filename;
			this.mesh = mesh;
			this.originalVtPos = (double[])mesh.VertexPos.Clone();
		}


		public override string ToString()
		{
			return Path.GetFileName(filename);
		}

		public void Dispose()
		{

		}
	};

	public class DisplayProperty
	{
		public enum EnumMeshDisplayMode
		{
			None, Points, Wireframe, FlatShaded, SmoothShaded,
			FlatShadedHiddenLine, SmoothShadedHiddenLine, TransparentSmoothShaded,
			TransparentSmoothShaded2, DualSurface,
		};
		
		public DisplayProperty()
		{
			#region assign default colors
			colorMall[0]  = Color.FromArgb(255, 140, 0);
			colorMall[1]  = Color.FromArgb(0, 206, 209);
			colorMall[2]  = Color.FromArgb(222, 141, 184);
			colorMall[3]  = Color.FromArgb(0, 255, 0);
			colorMall[4]  = Color.FromArgb(100, 149, 237);
			colorMall[5]  = Color.FromArgb(30, 144, 255);
			colorMall[6]  = Color.FromArgb(128, 128, 255);
			colorMall[7]  = Color.FromArgb(98, 215, 159);
			colorMall[8]  = Color.FromArgb(243, 147, 78);
			colorMall[9]  = Color.FromArgb(255, 215, 0);
			colorMall[10] = Color.FromArgb(0, 255, 127);
			colorMall[11] = Color.FromArgb(255, 99, 71);
			colorMall[12] = Color.FromArgb(218, 165, 32);
			colorMall[13] = Color.FromArgb(230, 34, 110);
			colorMall[14] = Color.FromArgb(250, 67, 89);
			colorMall[15] = Color.FromArgb(255, 165, 0);
			colorMall[16] = Color.FromArgb(0, 255, 255);
			colorMall[17] = Color.FromArgb(227, 249, 136);
			colorMall[18] = Color.FromArgb(210, 105, 30);
			colorMall[19] = Color.FromArgb(0, 160, 0);
			colorMall[20] = Color.FromArgb(173, 255, 47);
			colorMall[21] = Color.FromArgb(218, 112, 214);
			colorMall[22] = Color.FromArgb(100, 149, 237);
			colorMall[23] = Color.FromArgb(255, 79, 163);
			colorMall[24] = Color.FromArgb(243, 243, 5);
			colorMall[25] = Color.FromArgb(255, 255, 0);
			colorMall[26] = Color.FromArgb(192, 160, 0);
			colorMall[27] = Color.FromArgb(50, 141, 216);
			colorMall[28] = Color.FromArgb(186, 85, 211);
			colorMall[29] = Color.FromArgb(219, 112, 147);
			colorMall[30] = Color.FromArgb(255, 0, 0);
			colorMall[31] = Color.FromArgb(148, 0, 211);
			colorMall[32] = Color.FromArgb(205, 54, 22);
			colorMall[33] = Color.FromArgb(32, 192, 255);

            // Add more..
            colorMall[34] = Color.FromArgb(0, 192, 255);
            colorMall[35] = Color.FromArgb(185, 54, 22);
            colorMall[36] = Color.FromArgb(118, 0, 211);
            colorMall[37] = Color.FromArgb(200, 10, 0);
            colorMall[38] = Color.FromArgb(189, 102, 147);
            colorMall[39] = Color.FromArgb(146, 185, 211);
            colorMall[40] = Color.FromArgb(50, 141, 186);
            colorMall[41] = Color.FromArgb(152, 160, 0);
            colorMall[42] = Color.FromArgb(200, 255, 0);
            colorMall[43] = Color.FromArgb(255, 140, 30);
            colorMall[44] = Color.FromArgb(50, 206, 209);
            colorMall[45] = Color.FromArgb(202, 141, 164);
            colorMall[46] = Color.FromArgb(40, 255, 10);
            colorMall[47] = Color.FromArgb(70, 149, 237);
            colorMall[48] = Color.FromArgb(30, 144, 225);
            colorMall[49] = Color.FromArgb(128, 128, 205);
            colorMall[50] = Color.FromArgb(98, 215, 129);
            colorMall[51] = Color.FromArgb(243, 147, 28);
            colorMall[52] = Color.FromArgb(255, 215, 20);
            colorMall[53] = Color.FromArgb(0, 255, 167);
            colorMall[54] = Color.FromArgb(255, 99, 21);
            colorMall[55] = Color.FromArgb(218, 165, 132);
            colorMall[56] = Color.FromArgb(230, 34, 80);
            colorMall[57] = Color.FromArgb(250, 67, 129);
            colorMall[58] = Color.FromArgb(255, 165, 30);
            colorMall[59] = Color.FromArgb(0, 255, 215);
            colorMall[60] = Color.FromArgb(227, 249, 176);
            colorMall[61] = Color.FromArgb(210, 105, 80);
            colorMall[62] = Color.FromArgb(0, 160, 40);
            colorMall[63] = Color.FromArgb(173, 255, 17);
            colorMall[64] = Color.FromArgb(218, 112, 144);
            colorMall[65] = Color.FromArgb(100, 149, 207);
            colorMall[66] = Color.FromArgb(255, 79, 123);
            colorMall[67] = Color.FromArgb(243, 243, 35);
            colorMall[68] = Color.FromArgb(255, 120, 0);
            colorMall[69] = Color.FromArgb(0, 186, 209);
            colorMall[70] = Color.FromArgb(222, 111, 184);
            colorMall[71] = Color.FromArgb(0, 205, 0);
            colorMall[72] = Color.FromArgb(100, 189, 237);
            colorMall[73] = Color.FromArgb(30, 104, 255);
            colorMall[74] = Color.FromArgb(128, 168, 255);
            colorMall[75] = Color.FromArgb(98, 175, 159);
            colorMall[76] = Color.FromArgb(243, 187, 78);
            colorMall[77] = Color.FromArgb(255, 165, 0);
            colorMall[78] = Color.FromArgb(0, 195, 127);
            colorMall[79] = Color.FromArgb(255, 199, 71);
            colorMall[80] = Color.FromArgb(218, 125, 32);
            colorMall[81] = Color.FromArgb(230, 74, 110);
            colorMall[82] = Color.FromArgb(250, 167, 89);
            colorMall[83] = Color.FromArgb(255, 125, 0);
            colorMall[84] = Color.FromArgb(0, 200, 255);
            colorMall[85] = Color.FromArgb(227, 209, 136);
            colorMall[86] = Color.FromArgb(210, 145, 30);
            colorMall[87] = Color.FromArgb(0, 110, 10);
            colorMall[88] = Color.FromArgb(173, 215, 47);
            colorMall[89] = Color.FromArgb(218, 172, 214);
            colorMall[90] = Color.FromArgb(100, 189, 237);
            colorMall[91] = Color.FromArgb(255, 139, 163);
            colorMall[92] = Color.FromArgb(243, 203, 5);
            colorMall[93] = Color.FromArgb(255, 195, 0);
            colorMall[94] = Color.FromArgb(192, 240, 0);
            colorMall[95] = Color.FromArgb(50, 101, 216);
            colorMall[96] = Color.FromArgb(186, 125, 211);
            colorMall[97] = Color.FromArgb(219, 152, 147);
            colorMall[98] = Color.FromArgb(255, 40, 0);
            colorMall[99] = Color.FromArgb(148, 60, 211);
			#endregion
		}

		private EnumMeshDisplayMode meshDisplayMode = EnumMeshDisplayMode.SmoothShaded;
		private Color meshColor = Color.Tan;// Color.FromArgb(128, 150, 184, 255);//Color.DeepSkyBlue;
		private Color pointColor = Color.Blue;
		private Color lineColor = Color.Black;

        // Modify the colormall to support more colors
        private Color[] colorMall = new Color[100];
		//private Color[] colorMall = new Color[42];
		private float pointSize = 7.0f;
		private float lineWidth = 1.0f;
        private bool displaySelectedVertices = false;
		private bool displayVNormals = false;
		private bool displayFNormals = false;

		private float rotateAngle = 10.0f;
		public float RotateAngle
		{
			get { return rotateAngle; }
			set { rotateAngle = value; }
		}



		public EnumMeshDisplayMode MeshDisplayMode
		{
			get { return meshDisplayMode; }
			set { meshDisplayMode = value; }
		}

		static public bool transparent = false;
		public bool TransparentMode
		{
			get { return transparent; }
			set { transparent = value; }
		}

		public bool DisplayVNormals
		{
			get { return displayVNormals; }
			set { displayVNormals = value; }
		}
		public bool DisplayFNormals
		{
			get { return displayFNormals; }
			set { displayFNormals = value; }
		}
		private int displayFaceIndex = -1;
		public int DisplayFaceIndex
		{
			get { return displayFaceIndex; }
			set { displayFaceIndex = value; }
		}

		[Category("Color")] private Color alphaColor = Color.YellowGreen;
		public Color AlphaColor
		{
			get { return alphaColor; }
			set { alphaColor = value; }
		}
		
		[Category("Color")] private byte alphachanel = 30;
		public byte AlphaChanel
		{
			get { return alphachanel; }
			set { alphachanel = value; }
		}
		
		[Category("Color")] public Color MeshColor
		{
			get { return meshColor; }
			set { meshColor = value; }
		}
		[Category("Color")] public Color PointColor
		{
			get { return pointColor; }
			set { pointColor = value; }
		}
		[Category("Color")] public Color LineColor
		{
			get { return lineColor; }
			set { lineColor = value; }
		}
		[Category("Color")] public Color[] ColorMall
		{
			get { return colorMall; }
			set { colorMall = value; }
		}
		[Category("Element size")] public float PointSize
		{
			get { return pointSize; }
			set { pointSize = value; if (pointSize <= 0) pointSize = 0.1f; }
		}
		[Category("Element size")]
		public float LineWidth
		{
			get { return lineWidth; }
			set { lineWidth = value; }
		}
        public bool DisplaySelectedVertices
        {
            get { return displaySelectedVertices; }
            set { displaySelectedVertices = value; }
        }

	};

	public class ToolsProperty
	{
		public enum EnumSelectingMethod { Rectangle, Point };

		private bool selectionLaser = true;
		private double depthTolerance = -0.0001;
		private EnumSelectingMethod selectionMethod = EnumSelectingMethod.Point;

		[Category("Selection Tool")]
		public bool Laser
		{
			get { return selectionLaser; }
			set { selectionLaser = value; }
		}
		[Category("Selection Tool")]
		public double DepthTolerance
		{
			get { return depthTolerance; }
			set { depthTolerance = value; }
		}
		[Category("Selection Tool")]
		public EnumSelectingMethod SelectionMethod
		{
			get { return selectionMethod; }
			set { selectionMethod = value; }
		}

	};
}