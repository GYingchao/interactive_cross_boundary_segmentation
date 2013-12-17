using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CsGL.OpenGL;
using MyGeometry;

namespace CrossBoundaryBrushes
{
	public unsafe partial class MeshView : OpenGLControl
	{
		[DllImport("opengl32", EntryPoint = "wglUseFontBitmaps", CallingConvention=CallingConvention.Winapi)]
		public static extern bool wglUseFontBitmaps(
		IntPtr hDC,
		[MarshalAs(UnmanagedType.U4)] UInt32 first,
		[MarshalAs(UnmanagedType.U4)] UInt32 count,
		[MarshalAs(UnmanagedType.U4)] UInt32 listBase
		);

		[DllImport("GDI32.DLL", EntryPoint = "SelectObject",
		CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr SelectObject(
		[In] IntPtr hDC,
		[In] IntPtr font
		);

		private class FaceDepth : IComparable
		{
			public int index;
			public double depth;
			public FaceDepth(int index, double depth)
			{
				this.index = index;
				this.depth = depth;
			}

			public int CompareTo(object obj)
			{
				FaceDepth f = obj as FaceDepth;
				if (depth < f.depth) return -1;
				if (depth > f.depth) return 1;
				return 0;
			}
		};

		private MeshRecord currMeshRecord = null;
		private Matrix4d currTransformation = Matrix4d.IdentityMatrix();
		private Vector2d prevMousePosition = new Vector2d();
		private Vector2d currMousePosition = new Vector2d();


		// for viewing
		private Trackball ball;
		private double scaleRatio;
		private int[] faceDepth = null;

		// for selection
		private Vector2d mouseDownPosition = new Vector2d();
		private bool isMouseDown = false;

		// for moving
		private Trackball movingBall = new Trackball(200, 200);
		private Vector3d projectedCenter = new Vector3d();
		private Vector4d handleCenter = new Vector4d();
		private List<int> handleIndex = new List<int>();
		private List<Vector3d> oldHandlePos = new List<Vector3d>();
		private int handleFlag = -1;

		// for sketching
		private List<Vector2d> strokeScreenPos = null;
		private List<List<Vector2d>> strokes_on_Srceen = new List<List<Vector2d>>();
		private List<int> curr_v_stroke = null;
		private List<List<int>> strokes = new List<List<int>>(); // -- multi-stroke possible --
		private List<int> curr_f_stroke = null;
		private List<List<int>> faces_on_strokes = new List<List<int>>();
		public List<List<int>> Strokes
		{
			get { return strokes; }
		}
		public List<List<int>> FacesOnStrokes
		{
			get { return faces_on_strokes; }
		}
		private bool multi_stroke = false;
		public bool MultiStroke	
		{
			get { return multi_stroke; }
			set { multi_stroke = value; }
		}
		private Color main_stroke_color = Color.Black;
		public void SetMainStrokeColor(Color c)
		{
			main_stroke_color = c;
		}


		public void Project2Mesh()
		{
			if (this.strokeScreenPos.Count < 1) return;

			Rectangle viewport = new Rectangle(0, 0, this.Width, this.Height);
			OpenGLProjector projector = new OpenGLProjector();
			Mesh m = currMeshRecord.Mesh;
			bool laser = Program.toolsProperty.Laser;
			double eps = Program.toolsProperty.DepthTolerance;


			foreach (Vector2d pos in this.strokeScreenPos)
			{
				double minDis = double.MaxValue;
				int minIndex = -1;
				for (int i = 0, j = 0; i < m.VertexCount; i++, j += 3)
				{
					Vector3d v = projector.Project(m.VertexPos, j);
					Vector2d u = new Vector2d(v.x, v.y);
					if (!viewport.Contains((int)v.x, (int)v.y)) continue;
					if (projector.GetDepthValue((int)v.x, (int)v.y) - v.z < eps) continue;

					double dis = (u - pos).Length();
					if (dis < minDis)
					{
						minIndex = i;
						minDis = dis;
					}
				}
				if (minIndex != -1)
				{
					this.curr_v_stroke.Add(minIndex);
				}

				minIndex = -1;
				for (int i = 0, j = 0; i < m.FaceCount; i++, j += 3)
				{
					int c0 = m.FaceIndex[j]*3;
					int c1 = m.FaceIndex[j + 1]*3;
					int c2 = m.FaceIndex[j + 2]*3;
					Vector3d v0 = projector.Project(m.VertexPos, c0);
					Vector3d v1 = projector.Project(m.VertexPos, c1);
					Vector3d v2 = projector.Project(m.VertexPos, c2);
					Vector2d u0 = new Vector2d(v0.x, v0.y);
					Vector2d u1 = new Vector2d(v1.x, v1.y);
					Vector2d u2 = new Vector2d(v2.x, v2.y);

					if (!viewport.Contains((int)v0.x, (int)v0.y)) continue;
					if (projector.GetDepthValue((int)v0.x, (int)v0.y) - v0.z < eps) continue;
					if (PointInTriangle(pos, u0, u1, u2))
					{
						minIndex = i;
						this.curr_f_stroke.Add(minIndex);
						break;
					}
				}
			}

		}


		// testing
		private bool initFont = false;
		public UInt32 fontBase = 0;

		public MeshView()
		{
			InitializeComponent();

			ball = new Trackball(this.Width * 1.0, this.Height * 1.0);
		}
		~MeshView()
		{
			ReleaseFont();
		}
		public Matrix4d CurrTransformation
		{
			get { return currTransformation; }
			set { currTransformation = value; }
		}
		
		private void BuildFont(PaintEventArgs pe)
		{
			IntPtr dc = pe.Graphics.GetHdc();
			IntPtr oldFontH = IntPtr.Zero;
			System.Drawing.Font font =
				new Font(
				"Verdana",
				12F,
				System.Drawing.FontStyle.Regular,
				System.Drawing.GraphicsUnit.Point,
				((System.Byte)(0)));

			fontBase = GL.glGenLists(128);

			IntPtr fontH = font.ToHfont();
			oldFontH = SelectObject(dc, fontH);

			bool ret = wglUseFontBitmaps(
				dc,
				0,
				128,
				fontBase);

			SelectObject(dc, oldFontH);						// Selects The Font We Want

			pe.Graphics.ReleaseHdc(dc);

			if (!ret) throw new Exception();
		}
		private void ReleaseFont()
		{
			GL.glDeleteLists(fontBase, 255);
		}

		protected override void InitGLContext()
		{
			base.InitGLContext();
			Color c = SystemColors.Control;
			GL.glClearColor(1f, 1f, 1f, 0.0f);  // BackGround Color                
			GL.glShadeModel(GL.GL_SMOOTH);				 // Set Smooth Shading                 
			GL.glClearDepth(1.0f);						 // Depth buffer setup             
			GL.glEnable(GL.GL_DEPTH_TEST);				 // Enables Depth Testing             
			GL.glDepthFunc(GL.GL_LEQUAL);				 // The Type Of Depth Test To Do     
			GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);     /* Really Nice Perspective Calculations */
			GL.glEnable(GL.GL_CULL_FACE);
			GL.glPolygonOffset(1f, 1f);
			GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

			GL.glEnable(GL.GL_POINT_SMOOTH);
			GL.glEnable(GL.GL_LINE_SMOOTH);
			GL.glEnable(GL.GL_BLEND);


			float diffuse = 1f;
			float specular = 0.1f;
			int shinness = 32;


			float[] LightDiffuse = { diffuse, diffuse, diffuse, 0f };
			float[] LightSpecular = { specular, specular, specular, 1f };
			float[] SpecularRef = { 0.9f, 0.9f, 0.9f, 1f };
			GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, LightDiffuse);
			GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, LightSpecular);
			GL.glEnable(GL.GL_LIGHT0);
			GL.glEnable(GL.GL_COLOR_MATERIAL);
			GL.glColorMaterial(GL.GL_FRONT, GL.GL_DIFFUSE);
			GL.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, SpecularRef);
			GL.glMateriali(GL.GL_FRONT, GL.GL_SHININESS, shinness);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			if (this.Width == 0 || this.Height == 0) return;
			this.scaleRatio = (this.Width > this.Height) ? this.Height : this.Width;
			this.InitMatrix();
			this.ball.SetBounds(this.Width * 1.0, this.Height * 1.0);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.currMousePosition = new Vector2d(e.X, this.Height - e.Y);
			this.mouseDownPosition = currMousePosition;
			this.isMouseDown = true;

			if (currMeshRecord == null) return;

			switch (Program.currentMode)
			{
				case Program.EnumOperationMode.Viewing:
					switch (e.Button)
					{
						case MouseButtons.Left: ball.Click(currMousePosition, Trackball.MotionType.Rotation); break;
						case MouseButtons.Middle: ball.Click(currMousePosition / this.scaleRatio, Trackball.MotionType.Pan); break;
						case MouseButtons.Right: ball.Click(currMousePosition, Trackball.MotionType.Scale); break;
					}
					break;

				case Program.EnumOperationMode.Selection:
					break;

				case Program.EnumOperationMode.Sketching:
					if (e.Button == MouseButtons.Left && currMeshRecord.CrossBoundaryBrushes != null)
					{
						if (!this.multi_stroke)
						{
							this.strokes.Clear();
							this.faces_on_strokes.Clear();
							this.strokes_on_Srceen.Clear();
						}

						this.curr_f_stroke = new List<int>();
						this.curr_v_stroke = new List<int>();
						this.strokeScreenPos = new List<Vector2d>();


						this.strokeScreenPos.Add(currMousePosition);
						prevMousePosition.x = currMousePosition.x;
						prevMousePosition.y = currMousePosition.y;
					}
					break;

				case Program.EnumOperationMode.Moving:
					
					break;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.currMousePosition = new Vector2d(e.X, this.Height - e.Y);


			if (currMeshRecord == null) return;

			switch (Program.currentMode)
			{
				case Program.EnumOperationMode.Viewing:
					switch (e.Button)
					{
						case MouseButtons.Left: ball.Drag(currMousePosition); break;
						case MouseButtons.Middle: ball.Drag(currMousePosition / this.scaleRatio); break;
						case MouseButtons.Right: ball.Drag(currMousePosition); break;
					}
					this.Refresh();
					break;

				case Program.EnumOperationMode.Selection:
					if (isMouseDown)
						this.Refresh();
					break;
				case Program.EnumOperationMode.Sketching:
					if (e.Button == MouseButtons.Left  && currMeshRecord.CrossBoundaryBrushes != null)
					{
						if ((currMousePosition - prevMousePosition).Length() > 1)
						{
							
							this.strokeScreenPos.Add(currMousePosition);
							prevMousePosition.x = currMousePosition.x;
							prevMousePosition.y = currMousePosition.y;
							
						}
						this.Refresh();
					}
					break;

				case Program.EnumOperationMode.Moving:
					if (isMouseDown)
					{
						Vector2d p = currMousePosition - new Vector2d(projectedCenter.x, projectedCenter.y);
						int d = 0;
						p.x += 100;
						p.y += 100;
						switch (e.Button)
						{
							case MouseButtons.Right: movingBall.Drag(p); break;
							case MouseButtons.Left: movingBall.Drag(p / this.scaleRatio); d = 1; break;
							case MouseButtons.Middle: movingBall.Drag(p); break;
						}
						Matrix4d currInverseTransformation = this.currTransformation.Inverse();
						Matrix4d tran = currInverseTransformation * this.movingBall.GetMatrix() * currTransformation;
						//Matrix4d tran = currTransformation * this.movingBall.GetMatrix() * currInverseTransformation;
						double[] pos = this.currMeshRecord.Mesh.VertexPos;
						for (int i = 0; i < handleIndex.Count; i++)
						{
							int j = handleIndex[i] * 3;
							Vector4d q = new Vector4d((Vector3d)this.oldHandlePos[i], d);
							q = tran * (q - this.handleCenter) + this.handleCenter;
							pos[j] = q.x;
							pos[j + 1] = q.y;
							pos[j + 2] = q.z;
						}
						
						this.Refresh();
					}
					break;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			this.currMousePosition = new Vector2d(e.X, this.Height - e.Y);
			this.isMouseDown = false;

			if (currMeshRecord == null) return;

			switch (Program.currentMode)
			{
				case Program.EnumOperationMode.Viewing:
					if (currMousePosition.Equals(mouseDownPosition)) break;
					Matrix4d m = ball.GetMatrix();
					this.currTransformation = m * currTransformation;
//					this.currInverseTransformation = this.currTransformation.Inverse();
					this.ball.End();
					this.Refresh();
					break;

				case Program.EnumOperationMode.Selection:
					switch (Program.toolsProperty.SelectionMethod)
					{
						case ToolsProperty.EnumSelectingMethod.Rectangle:
							SelectVertexByRect();
							break;
						case ToolsProperty.EnumSelectingMethod.Point:
							//SelectVertexByPoint();
							break;
					}
					currMeshRecord.Mesh.GroupingFlags();
					this.Refresh();
					break;

				case Program.EnumOperationMode.Sketching:
					if (e.Button == MouseButtons.Left  && currMeshRecord.CrossBoundaryBrushes != null)
					{
						this.Project2Mesh();

						this.faces_on_strokes.Add(curr_f_stroke);
						this.strokes.Add(curr_v_stroke);
						this.strokes_on_Srceen.Add(strokeScreenPos);


						if (currMeshRecord.CrossBoundaryBrushes != null && this.curr_v_stroke.Count >= 2)
						{
							if (!this.multi_stroke)
							{
								this.Cut();
								this.Refresh();
							}
							else
							{
								currMeshRecord.CrossBoundaryBrushes.RemovePrevCut();
								this.Cut();
								this.Refresh();
							}
						}
						this.Refresh();
					}
					break;

				case Program.EnumOperationMode.Moving:
					
					this.Refresh();
					break;
			}

		}
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			if (!initFont)
			{
				BuildFont(pe);
				initFont = true;
			}
		}
		private bool clearStrokeRealtime = false;
		public bool ClearStrokesInRealtime
		{
			get { return clearStrokeRealtime; }
			set { clearStrokeRealtime = value; }
		}
		public void Cut() // -- harmonic segmentation --
		{
			if (currMeshRecord.CrossBoundaryBrushes.ObtainConstraints(this.strokes, this.faces_on_strokes))
			{
				currMeshRecord.CrossBoundaryBrushes.Update();

				GC.Collect();
			}
		}
		public void ClearStrokes()
		{
			if (this.strokeScreenPos != null)
				this.strokeScreenPos.Clear();
			this.faces_on_strokes.Clear();
			this.strokes_on_Srceen.Clear();
			this.strokes.Clear();
		}
		public void RemovePrevStroke()
		{
			int n = this.strokes.Count-1;
			if (n < 1) return;
			
			this.faces_on_strokes.RemoveAt(n);
			this.strokes_on_Srceen.RemoveAt(n);
			this.strokes.RemoveAt(n);

			this.strokeScreenPos.Clear();
			
			this.Refresh();
		}
		// -- reproject the stroke on screen to the mesh --
		public void RegainStrokeInfo()
		{
			int n = this.strokes_on_Srceen.Count;
			if (n < 1) return;
			this.strokes.Clear();
			this.faces_on_strokes.Clear();
			foreach (List<Vector2d> ls in this.strokes_on_Srceen)
			{
				List<int> v_list = new List<int>();
				List<int> f_list = new List<int>();
				foreach (Vector2d v in ls)
				{
					int selected = SelectVertexByPoint(v);
					if (selected > -1 && !v_list.Contains(selected))
						v_list.Add(selected);
					// -- select face --
					int f = SelectFaceByPoint(v);
					if (f != -1 && !f_list.Contains(f))
						f_list.Add(f);
				}
				this.strokes.Add(v_list);
				this.faces_on_strokes.Add(f_list);
			}
		}

		public void ResetEnviroment()
		{
			InitGLContext();
		}

		public override void glDraw()
		{
			base.glDraw();

			GL.glClear(GL.GL_COLOR_BUFFER_BIT);

			if (this.DesignMode == true) return;
			if (currMeshRecord == null) return;

 			Matrix4d m = ball.GetMatrix() * currTransformation;
			GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
 			GL.glMatrixMode(GL.GL_MODELVIEW);

			
			GL.glLoadMatrixd(m.Transpose().ToArray());

			switch (Program.displayProperty.MeshDisplayMode)
			{
				case DisplayProperty.EnumMeshDisplayMode.Points:
					DrawPoints();
					break;

				case DisplayProperty.EnumMeshDisplayMode.Wireframe:
					DrawWireframe();
					break;

				case DisplayProperty.EnumMeshDisplayMode.FlatShaded:
					DrawFlatShaded();
					break;

				case DisplayProperty.EnumMeshDisplayMode.SmoothShaded:
					DrawSmoothShaded();
					break;

				case DisplayProperty.EnumMeshDisplayMode.FlatShadedHiddenLine:
					DrawFlatHiddenLine();
					break;

				case DisplayProperty.EnumMeshDisplayMode.SmoothShadedHiddenLine:
					DrawSmoothHiddenLine();
					break;

				case DisplayProperty.EnumMeshDisplayMode.TransparentSmoothShaded:
					if (isMouseDown)
						DrawSmoothShaded();
					else
						DrawTransparentSmoothShaded();
					break;
				case DisplayProperty.EnumMeshDisplayMode.DualSurface:
					DrawDualSurface();
					break;
				//case DisplayProperty.EnumMeshDisplayMode.TransparentSmoothShaded2:
				//    if (isMouseDown)
				//        DrawSmoothShaded();
				//    else
				//        DrawTransparentSmoothShaded2();
				//    break;
			}

			if (DisplayProperty.transparent)
			{
				DrawTransparentSmoothShaded2();
			}


			if (currMeshRecord.CrossBoundaryBrushes != null) currMeshRecord.CrossBoundaryBrushes.Display();


			switch (Program.currentMode)
			{
				case Program.EnumOperationMode.Selection:
					if (isMouseDown)
						DrawSelectionRect();
					break;
				case Program.EnumOperationMode.Sketching:
					//if (isMouseDown)
					//	DrawStrokes();
					break;
			}

			DrawStrokes();

            if (Program.displayProperty.DisplaySelectedVertices)
                DrawSelectedVertice_ByPoint();
			if (Program.displayProperty.DisplayVNormals)
				DrawVNormals();
			if (Program.displayProperty.DisplayFNormals)
				DrawFNormals();
			if (Program.displayProperty.DisplayFaceIndex >= 0)
				DrawSpecificFace(Program.displayProperty.DisplayFaceIndex);
			
		}
		private void InitMatrix()
		{
			double w = Size.Width;
			double h = Size.Height;
			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glLoadIdentity();

			if (w > h)
			{
				double ratio = (w / h) / 2.0;
				GL.glOrtho(-ratio, ratio, -0.5, 0.5, -100, 100);
			}
			else
			{
				double ratio = (h / w) / 2.0;
				GL.glOrtho(-0.5, 0.5, -ratio, ratio, -100, 100);
			}

			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glLoadIdentity();
		}
		private void DrawPoints()
		{
			Mesh m = currMeshRecord.Mesh;

			GL.glPointSize(Program.displayProperty.PointSize);
			Color c = Program.displayProperty.PointColor;
			GL.glColor3ub(c.R, c.G, c.B);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			fixed (double* vp = m.VertexPos)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glDrawArrays(GL.GL_POINTS, 0, m.VertexCount);
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
		}
		private void DrawWireframe()
		{
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);
			GL.glDisable(GL.GL_CULL_FACE);
			Color c = Program.displayProperty.LineColor;
			GL.glColor3ub(c.R, c.G, c.B);

			Mesh m = currMeshRecord.Mesh;

			GL.glLineWidth(Program.displayProperty.LineWidth);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			fixed (double* vp = m.VertexPos)
			fixed (int* index = m.FaceIndex)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glDrawElements(GL.GL_TRIANGLES, m.FaceCount * 3, GL.GL_UNSIGNED_INT, index);
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glEnable(GL.GL_CULL_FACE);
		}
		private void DrawFlatShaded()
		{
			GL.glShadeModel(GL.GL_FLAT);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);

			Mesh m = currMeshRecord.Mesh;

			Color c = Program.displayProperty.MeshColor;
			GL.glColor3ub(c.R, c.G, c.B);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			fixed (double* vp = m.VertexPos)
			fixed (double* np = m.FaceNormal)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glBegin(GL.GL_TRIANGLES);
				for (int i = 0, j = 0; i < m.FaceCount; i++, j += 3)
				{
					GL.glNormal3dv(np + j);
					GL.glArrayElement(m.FaceIndex[j]);
					GL.glArrayElement(m.FaceIndex[j + 1]);
					GL.glArrayElement(m.FaceIndex[j + 2]);
				}
				GL.glEnd();
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisable(GL.GL_LIGHTING);
		}
		private void DrawSmoothShaded()
		{
			GL.glShadeModel(GL.GL_SMOOTH);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);

			Mesh m = currMeshRecord.Mesh;

			Color c = Program.displayProperty.MeshColor;
			GL.glColor3ub(c.R, c.G, c.B);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			GL.glEnableClientState(GL.GL_NORMAL_ARRAY);
			fixed (double* vp = m.VertexPos)
			fixed (double* np = m.VertexNormal)
			fixed (int* index = m.FaceIndex)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glNormalPointer(GL.GL_DOUBLE, 0, np);
				GL.glDrawElements(GL.GL_TRIANGLES, m.FaceCount * 3, GL.GL_UNSIGNED_INT, index);
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisableClientState(GL.GL_NORMAL_ARRAY);
			GL.glDisable(GL.GL_LIGHTING);
		}
		private void DrawSmoothHiddenLine()
		{
			GL.glEnable(GL.GL_POLYGON_OFFSET_FILL);
			DrawSmoothShaded();
			//GL.glDisable(GL.GL_POLYGON_OFFSET_FILL);
			DrawWireframe();
		}
		private void DrawFlatHiddenLine()
		{
			GL.glEnable(GL.GL_POLYGON_OFFSET_FILL);
			DrawFlatShaded();
			//GL.glDisable(GL.GL_POLYGON_OFFSET_FILL);
			DrawWireframe();

			//GL.GL_EXT_vertex_shader
		}
		private void DrawSelectionRect()
		{
			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glPushMatrix();
			GL.glLoadIdentity();
			GL.gluOrtho2D(0, this.Width, 0, this.Height);
			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glPushMatrix();
			GL.glLoadIdentity();

			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);
			GL.glDisable(GL.GL_CULL_FACE);
			GL.glDisable(GL.GL_DEPTH_TEST);
			GL.glColor3f(0.0f, 0.0f, 0.0f);
			GL.glRectd(mouseDownPosition.x, mouseDownPosition.y, currMousePosition.x, currMousePosition.y);

			GL.glEnable(GL.GL_CULL_FACE);
			GL.glEnable(GL.GL_DEPTH_TEST);

			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glPopMatrix();
			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glPopMatrix();
		}
		private void DrawSelectedVertice_ByPoint()
		{
			Mesh m = currMeshRecord.Mesh;

			GL.glColor3f(1.0f, 0.4f, 0.4f);
			GL.glPointSize(Program.displayProperty.PointSize);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			fixed (double* vp = m.VertexPos)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glBegin(GL.GL_POINTS);
				for (int i = 0; i < m.VertexCount; i++)
				{
					if (m.Flag[i] == 0) continue;
					switch (m.Flag[i] % 6)
					{
						case 0: GL.glColor3f(1.0f, 1.0f, 0.0f); break;
						case 1: GL.glColor3f(1.0f, 0.0f, 0.0f); break;
						case 2: GL.glColor3f(0.0f, 1.0f, 0.0f); break;
						case 3: GL.glColor3f(0.0f, 0.0f, 1.0f); break;
						case 4: GL.glColor3f(0.0f, 1.0f, 1.0f); break;
						case 5: GL.glColor3f(1.0f, 0.0f, 1.0f); break;
					}
					GL.glArrayElement(i);
				}
				GL.glEnd();
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
		}
		private void DrawTransparentSmoothShaded()
		{
			Mesh m = currMeshRecord.Mesh;
			SortFaces();

			GL.glShadeModel(GL.GL_SMOOTH);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);


			Color c = Program.displayProperty.MeshColor;
			GL.glColor4ub(c.R, c.G, c.B, 128);
			GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			GL.glDisable(GL.GL_DEPTH_TEST);
			GL.glDisable(GL.GL_CULL_FACE);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			GL.glEnableClientState(GL.GL_NORMAL_ARRAY);
			fixed (double* vp = m.VertexPos)
			fixed (double* np = m.VertexNormal)
			fixed (int* index = m.FaceIndex)
			fixed (int* fd = faceDepth)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glNormalPointer(GL.GL_DOUBLE, 0, np);
				GL.glBegin(GL.GL_TRIANGLES);
				for (int i = 0; i < m.FaceCount; i++)
				{
					int j = faceDepth[i] * 3;
					GL.glArrayElement(m.FaceIndex[j]);
					GL.glArrayElement(m.FaceIndex[j + 1]);
					GL.glArrayElement(m.FaceIndex[j + 2]);
				}
				GL.glEnd();
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisableClientState(GL.GL_NORMAL_ARRAY);
			GL.glDisable(GL.GL_LIGHTING);
			GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			GL.glEnable(GL.GL_DEPTH_TEST);
			GL.glEnable(GL.GL_CULL_FACE);
		}
		private void DrawTransparentSmoothShaded2()
		{
            Mesh m = currMeshRecord.Mesh;
            Color c = Program.displayProperty.AlphaColor;
            //	SortFaces();

            GL.glPushClientAttrib(GL.GL_CLIENT_VERTEX_ARRAY_BIT);
            GL.glPushAttrib(GL.GL_ALL_ATTRIB_BITS);

            GL.glShadeModel(GL.GL_SMOOTH);
            GL.glEnable(GL.GL_NORMALIZE);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glEnable(GL.GL_CULL_FACE);
            GL.glDisable(GL.GL_DEPTH_TEST);
            GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);

            GL.glColor4ub(c.R, c.G, c.B, Program.displayProperty.AlphaChanel);

            GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
            GL.glEnableClientState(GL.GL_NORMAL_ARRAY);
            fixed (double* vp = currMeshRecord.originalVtPos)
            fixed (double* np = m.VertexNormal)
            fixed (int* index = m.FaceIndex)
            {
              GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
              GL.glNormalPointer(GL.GL_DOUBLE, 0, np);
              GL.glDrawElements(GL.GL_TRIANGLES, m.FaceCount * 3, GL.GL_UNSIGNED_INT, index);
            }
            GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
            GL.glDisableClientState(GL.GL_NORMAL_ARRAY);

			//////////////////////////////////////////////////////////////////////////             

			GL.glEnable(GL.GL_LIGHTING);
			GL.glDepthFunc(GL.GL_EQUAL);

			float[] LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
			float[] LightSpecular = { 1.0f, 1.0f, 1.0f, 1.0f };
			GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, LightDiffuse);
			GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, LightSpecular);
			GL.glEnable(GL.GL_LIGHT0);

			float[] Ambient = { 0.0f, 0.0f, 0.0f, 1.0f };
			float[] Diffuse = { 0.0f, 0.0f, 0.0f, 1.0f };
			float[] SpecularRef = { 1.0f, 1.0f, 1.0f, 1.0f };
			
			GL.glColorMaterial(GL.GL_FRONT, GL.GL_DIFFUSE);
			GL.glMaterialfv(GL.GL_FRONT, GL.GL_DIFFUSE, Diffuse);
			GL.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT, Ambient);
			GL.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, SpecularRef);
			GL.glMateriali(GL.GL_FRONT, GL.GL_SHININESS, 128);

			GL.glColor4ub(c.R, c.G, c.B, Program.displayProperty.AlphaChanel);

			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			GL.glEnableClientState(GL.GL_NORMAL_ARRAY);
			fixed (double* vp = currMeshRecord.originalVtPos)
			fixed (double* np = m.VertexNormal)
			fixed (int* index = m.FaceIndex)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glNormalPointer(GL.GL_DOUBLE, 0, np);
				GL.glDrawElements(GL.GL_TRIANGLES, m.FaceCount * 3, GL.GL_UNSIGNED_INT, index);
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisableClientState(GL.GL_NORMAL_ARRAY);

			GL.glPopAttrib();
			GL.glPopClientAttrib();
		}

		private void DrawDualSurface()
		{
			GL.glEnable(GL.GL_POLYGON_OFFSET_FILL);
			DrawSmoothShaded();

			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);
			GL.glDisable(GL.GL_CULL_FACE);
			Color c = Program.displayProperty.LineColor;
			GL.glColor3ub(c.R, c.G, c.B);

			Mesh m = currMeshRecord.Mesh;

			GL.glLineWidth(Program.displayProperty.LineWidth);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			fixed (double* vp = m.DualVertexPos)
			fixed (double* np = m.FaceNormal)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glBegin(GL.GL_LINES);
				for (int i = 0, j = 0; i < m.FaceCount; i++, j += 3)
				{
					foreach (int f in m.AdjFF[i])
					{
						GL.glNormal3dv(np + j);
						GL.glArrayElement(i);
						GL.glNormal3dv(np + f*3);
						GL.glArrayElement(f);
					}
				}
				GL.glEnd();
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisable(GL.GL_LIGHTING);
		}
		private void DrawVNormals()
		{
			GL.glColor3f(0.0f, 0.0f, 1.0f);
			GL.glLineWidth(1.0f);
			GL.glBegin(GL.GL_LINES);
			Mesh m = currMeshRecord.Mesh;
			for (int i = 0; i < m.VertexCount; ++i)
			{
				Vector3d u = new Vector3d(m.VertexPos, 3 * i);
				Vector3d n = new Vector3d(m.VertexNormal, 3 * i);
				Vector3d v = u + n * 0.03;
				GL.glVertex3d(u.x, u.y, u.z);
				GL.glVertex3d(v.x, v.y, v.z);
			}
			GL.glEnd();
		}
		private void DrawFNormals()
		{
			GL.glColor3f(0.0f, 0.0f, 1.0f);
			GL.glLineWidth(1.0f);
			GL.glBegin(GL.GL_LINES);
			Mesh m = currMeshRecord.Mesh;
			for (int i = 0; i < m.FaceCount; ++i)
			{
				Vector3d u = new Vector3d(m.DualVertexPos, 3 * i);
				Vector3d n = new Vector3d(m.FaceNormal, 3 * i);
				Vector3d v = u + n * 0.03;
				GL.glVertex3d(u.x, u.y, u.z);
				GL.glVertex3d(v.x, v.y, v.z);
			}
			GL.glEnd();
		}
		private void DrawSpecificFace(int f)
		{
			GL.glShadeModel(GL.GL_FLAT);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);

			Mesh m = currMeshRecord.Mesh;

			Color c = Color.Red;
			GL.glColor3ub(c.R, c.G, c.B);
			
			int b = f*3;
			int c0 = m.FaceIndex[b];
			int c1 = m.FaceIndex[b+1];
			int c2 = m.FaceIndex[b+2];
			Vector3d u = new Vector3d(m.VertexPos, c0*3);
			Vector3d v = new Vector3d(m.VertexPos, c1*3);
			Vector3d w = new Vector3d(m.VertexPos, c2*3);
			Vector3d n = new Vector3d(m.FaceNormal, f*3);
			GL.glBegin(GL.GL_TRIANGLES);
			{
				GL.glNormal3d(n.x,n.y,n.z);
				GL.glVertex3d(u.x,u.y,u.z);
				GL.glVertex3d(v.x,v.y,v.z);
				GL.glVertex3d(w.x,w.y,w.z);
			}
			GL.glEnd();
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisable(GL.GL_LIGHTING);
		}

		// -- draw stroke on the screen --
		private void DrawStrokes()
		{
			DrawStroke(strokeScreenPos, this.main_stroke_color); 
			foreach (List<Vector2d> stroke in this.strokes_on_Srceen)
			{
			    DrawStroke(stroke, this.main_stroke_color);
			}
		}
		private void DrawStroke(List<Vector2d> stroke, Color c)
		{
			if (stroke == null || stroke.Count < 1) return;


			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glPushMatrix();
			GL.glLoadIdentity();
			GL.gluOrtho2D(0, this.Width, 0, this.Height);
			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glPushMatrix();
			GL.glLoadIdentity();

			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);
			GL.glDisable(GL.GL_CULL_FACE);
			GL.glDisable(GL.GL_DEPTH_TEST);
			GL.glDisable(GL.GL_LIGHTING);

			GL.glColor3ub(c.R, c.G, c.B);

			GL.glLineWidth(7.0f);
			GL.glEnable(GL.GL_LINE_SMOOTH);
			GL.glBegin(GL.GL_LINE_STRIP);
			foreach (Vector2d u in stroke)
			{
			    GL.glVertex2d(u.x, u.y);
			}
			GL.glEnd();
			GL.glLineWidth(1.0f);

			//GL.glPointSize(10.0f);
			//GL.glBegin(GL.GL_POINTS);
			//foreach (Vector2d u in stroke)
			//{
			//    GL.glVertex2d(u.x, u.y);
			//}
			//GL.glEnd();
			//GL.glPointSize(1.0f);

			GL.glDisable(GL.GL_LINE_SMOOTH);
			GL.glEnable(GL.GL_CULL_FACE);
			GL.glEnable(GL.GL_DEPTH_TEST);


			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glPopMatrix();
			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glPopMatrix();
		}

		private void SelectVertexByRect()
		{
			Vector2d minV = Vector2d.Min(mouseDownPosition, currMousePosition);
			Vector2d size = Vector2d.Max(mouseDownPosition, currMousePosition) - minV;
			Rectangle rect = new Rectangle((int)minV.x, (int)minV.y, (int)size.x, (int)size.y);
			Rectangle viewport = new Rectangle(0, 0, this.Width, this.Height);
			OpenGLProjector projector = new OpenGLProjector();
			Mesh m = currMeshRecord.Mesh;
			bool laser = Program.toolsProperty.Laser;
			double eps = Program.toolsProperty.DepthTolerance;

			if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
				for (int i = 0, j = 0; i < m.VertexCount; i++, j += 3)
				{
					Vector3d v = projector.Project(m.VertexPos, j);
					if (viewport.Contains((int)v.x, (int)v.y))
					{
						bool flag = rect.Contains((int)v.x, (int)v.y);
						flag &= (laser || projector.GetDepthValue((int)v.x, (int)v.y) - v.z >= eps);
						if (flag) m.Flag[i] = (byte)1;
					}
				}

			else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
				for (int i = 0, j = 0; i < m.VertexCount; i++, j += 3)
				{
					Vector3d v = projector.Project(m.VertexPos, j);
					if (viewport.Contains((int)v.x, (int)v.y))
					{
						bool flag = rect.Contains((int)v.x, (int)v.y);
						flag &= (laser || projector.GetDepthValue((int)v.x, (int)v.y) - v.z >= eps);
						if (flag) m.Flag[i] = (byte)0;
					}
				}

			else
				for (int i = 0, j = 0; i < m.VertexCount; i++, j += 3)
				{
					Vector3d v = projector.Project(m.VertexPos, j);
					if (viewport.Contains((int)v.x, (int)v.y))
					{
						bool flag = rect.Contains((int)v.x, (int)v.y);
						flag &= (laser || projector.GetDepthValue((int)v.x, (int)v.y) - v.z >= eps);
						m.Flag[i] = (byte)((flag) ? 1 : 0);
					}
				}
		}
		private int SelectVertexByPoint(Vector2d currPos)
		{
			Rectangle viewport = new Rectangle(0, 0, this.Width, this.Height);
			OpenGLProjector projector = new OpenGLProjector();
			Mesh m = currMeshRecord.Mesh;
			bool laser = Program.toolsProperty.Laser;
			double eps = Program.toolsProperty.DepthTolerance;

			double minDis = double.MaxValue;
			int minIndex = -1;
			for (int i = 0, j = 0; i < m.VertexCount; i++, j += 3)
			{
				Vector3d v = projector.Project(m.VertexPos, j);
				Vector2d u = new Vector2d(v.x, v.y);
				if (!viewport.Contains((int)v.x, (int)v.y)) continue;
				if (projector.GetDepthValue((int)v.x, (int)v.y) - v.z < eps) continue;

				double dis = (u - currPos).Length();
				if (dis < minDis)
				{
					minIndex = i;
					minDis = dis;
				}
			}
			if (minIndex == -1) return -1;
			//FormMain.CurrForm.OutputText("selected vertex: " + minIndex.ToString());

			if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
				m.Flag[minIndex] = (byte)1;
			else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
				m.Flag[minIndex] = (byte)0;
			else
			{
				for (int i = 0; i < m.VertexCount; i++) m.Flag[i] = (byte)0;
				m.Flag[minIndex] = (byte)1;
			}

			return minIndex;
		}
		private int SelectFaceByPoint(Vector2d screen_pos)
		{
			Rectangle viewport = new Rectangle(0, 0, this.Width, this.Height);
			OpenGLProjector projector = new OpenGLProjector();
			Mesh m = currMeshRecord.Mesh;
			bool laser = Program.toolsProperty.Laser;
			double eps = Program.toolsProperty.DepthTolerance;

			int minIndex = -1;
			for (int i = 0, j = 0; i < m.FaceCount; i++, j += 3)
			{
				int c0 = m.FaceIndex[j]*3;
				int c1 = m.FaceIndex[j + 1]*3;
				int c2 = m.FaceIndex[j + 2]*3;
				Vector3d v0 = projector.Project(m.VertexPos, c0);
				Vector3d v1 = projector.Project(m.VertexPos, c1);
				Vector3d v2 = projector.Project(m.VertexPos, c2);
				Vector2d u0 = new Vector2d(v0.x, v0.y);
				Vector2d u1 = new Vector2d(v1.x, v1.y);
				Vector2d u2 = new Vector2d(v2.x, v2.y);

				if (!viewport.Contains((int)v0.x, (int)v0.y)) continue;
				if (projector.GetDepthValue((int)v0.x, (int)v0.y) - v0.z < eps) continue;
				if (PointInTriangle(screen_pos, u0, u1, u2))
				{
					minIndex = i;
					break;
				}
			}
			return minIndex;
		}
		private bool PointInTriangle(Vector2d p, Vector2d a, Vector2d b, Vector2d c)
		{
			if (SameSide(p,a,b,c) && SameSide(p,b,a,c) && SameSide(p,c, a,b))
				return true;
			return false;
		}
		// -- whether two points p1 and p2 are on the same side of edge ab --
		private bool SameSide(Vector2d p1, Vector2d p2, Vector2d a, Vector2d b)
		{
			Vector3d A = new Vector3d(a);
			Vector3d B = new Vector3d(b);
			Vector3d P = new Vector3d(p1);
			Vector3d Q = new Vector3d(p2);
			Vector3d E = B-A;
			Vector3d cp1 = E.Cross(P-A);
			Vector3d cp2 = E.Cross(Q-A);
			if (cp1.Dot(cp2) >= 0) 
				return true;
			return false;
		}
		private void SortFaces()
		{
			Mesh m = currMeshRecord.Mesh;
			Matrix4d tran = ball.GetMatrix() * currTransformation;

			//sort faces
			FaceDepth[] d = new FaceDepth[m.FaceCount];
			for (int i = 0; i < m.FaceCount; i++)
			{
				Vector4d v = new Vector4d(new Vector3d(m.DualVertexPos, i * 3), 1.0);
				v = tran * v;
				d[i] = new FaceDepth(i, v.z);
			}
			Array.Sort(d);
			faceDepth = new int[m.FaceCount];
			for (int i = 0; i < m.FaceCount; i++)
				faceDepth[i] = d[i].index;
		}

		public void SetModel(MeshRecord rec)
		{
			if (currMeshRecord != null)
				currMeshRecord.ModelViewMatrix = currTransformation;
			if (rec != null) 
				currTransformation = rec.ModelViewMatrix;
			currMeshRecord = rec;
			this.Refresh();
		}

		// -- rotating --
		private Vector2d apoint = new Vector2d();
		internal void RotateModel()
		{
			float ratio = Program.displayProperty.RotateAngle;
			int x = this.Width / 2, y = this.Height / 2;
			apoint.x = x; apoint.y = y;
			this.ball.Click(apoint, Trackball.MotionType.Rotation);
			apoint.x += ratio;
			this.ball.Drag(apoint);
			Matrix4d m = ball.GetMatrix();
			this.currTransformation = m * currTransformation;
			this.ball.End();
			this.Refresh();
		}
	}
}
