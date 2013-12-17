using System;
using System.Collections.Generic;
using System.Text;

using CsGL.OpenGL;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MyGeometry;
using System.Drawing;
using NumericalRecipes;

using MyCholmodSolver;
using System.IO;

using System.Windows;

namespace CrossBoundaryBrushes
{
	[TypeConverterAttribute(typeof(DeformerConverter))]
	public unsafe class CrossBoundaryBrushes : IDisposable
	{
		public class Option
		{
			public enum EnumFieldType
			{
				Harmonic = 0,
			};
			private EnumFieldType fieldType = EnumFieldType.Harmonic;
			public enum EnumSegMethods
			{
				Ours = 0,
				CGI07,
			};
			private EnumSegMethods cutMethod = EnumSegMethods.Ours;
			[Category("0.General Options")]
			public EnumFieldType FieldType
			{
				get { return fieldType; }
				set { fieldType = value; }
			}
			private bool cgi_multiple = false;
			[Category("0.General Options")]
			public bool Cgi_Multiple
			{
				get { return cgi_multiple; }
				set { cgi_multiple = value; }
			}
			[Category("0.General Options")]
			public EnumSegMethods CutMethod
			{
				get { return cutMethod; }
				set { cutMethod = value; }
			}
			private bool part_type = true;
			[Category("0.General Options")]
			public bool Part_Type
			{
				get { return part_type; }
				set { part_type = value; }
			}
			private int numberOfIsolines = 15;
			[Category("1.Harmonic Options")]
			public int NumberOfIsolines
			{
				get { return numberOfIsolines; }
				set { numberOfIsolines = value; }
			}
			private bool useAtb = false;
			[Category("1.Harmonic Options")]
			public bool UseAtb
			{
				get { return useAtb; }
				set { useAtb = value; }
			}
			private bool useNormalFilter = false;
			[Category("2.Patch Options")]
			public bool UseNormalFilter
			{
				get { return useNormalFilter; }
				set { useNormalFilter = value; }
			}
			private int iterations = 1;
			[Category("2.Patch Options")]
			public int Iterations
			{
				get { return iterations; }
				set { iterations = value; }
			}
			private double sigma = 0.35;
			[Category("2.Patch Options")]
			public double Sigma
			{
				get { return sigma; }
				set { sigma = value; }
			}
		}
		public class Isoline
		{
			public List<FaceRecord> faces = null;
			public double radious = 0;
			public double value = -1;
			public int id = -1;
			public Isoline(int id_)
			{
				id = id_;
			}
			public Isoline()
			{
			}
		}
		public class PQPairs : ICloneable
		{
			public int findex, lindex; // lindex-->contour line index
			public Vector3d p, q, n;
			public int pId = -1, qId = -1; // stand for new vertex index of p and q after subdivision
			public int e1, e2;
			public double ratio1, ratio2;
			public double isovalue;
			public PQPairs() { e1 = e2 = -1; }
			#region ICloneable Members
			public object Clone()
			{
				PQPairs pair = new PQPairs();
				pair.p = this.p;
				pair.q = this.q;
				pair.n = this.n;
				pair.pId = this.pId;
				pair.e1 = this.e1;
				pair.e2 = this.e2;
				pair.findex = this.findex;
				pair.lindex = this.lindex;
				pair.ratio1 = this.ratio1;
				pair.ratio2 = this.ratio2;
				pair.isovalue = this.isovalue;

				return pair;
			}

			#endregion
		}
		public class IsoFaceRec
		{
			public int index;
			public bool valid = false;
			public bool subdived = false;
			public List<PQPairs> pqPairs = new List<PQPairs>();
			public IsoFaceRec() { }
		}
		public class FaceRecord
		{
			public IsoFaceRec face = null;
			public PQPairs pq = null;
			public FaceRecord(IsoFaceRec r, PQPairs _pq)
			{
				face = r;
				pq = _pq;
			}
		} // each face <-> a unique pqpair
		

		public Option opt = null;
		private Mesh mesh = null;
		private CholmodSolver sparseSolver = null;
		private ColorInterpolater colorInterpolater = null;
		public bool hasharmonic = false; // -- indicate whether the hasharmonic filed is generated
		

		private IsoFaceRec[] isofaces = null;
		private IsoFaceRec[] finalFaceRec = null;
		List<List<Isoline>> isolineLists = new List<List<Isoline>>();
		List<Isoline> fullIsolineList = new List<Isoline>();

		private Set<int> sourceVertices = new Set<int>();	// vertices value = 1
		private Set<int> sinkVertices = new Set<int>();		// vertices value = 0
		private Set<int> seedVertices = new Set<int>();		// seed verteices, for segmentation
		private Set<int> prevSourceVertices = new Set<int>();
		private Set<int> prevSinkVertices = new Set<int>();
		private List<int> verticesOnStroke = new List<int>();
		private List<List<int>> facesOnStrokes = new List<List<int>>();
		private List<Isoline> isolinesOnStroke = new List<Isoline>();
		private Set<int>[] neighbors = null;

		private bool is_a_part_brush = true;
		public bool finalized = false;

		public enum EnumDisplayMode { None = 0, Isolines, HarminicField, Segmentation};
		private double[] triArea = null;
		private double[] isovalues = null;
		private double[] harmonicField = null;
		private double[] vrtDisplayColor = null;
		private double[] triDisplayColor = null;
		private int regionNum = 15;
		private double ConstantWeight = 1000;
		private EnumDisplayMode displayMode = EnumDisplayMode.None;
		public EnumDisplayMode DisplayMode
		{
			get { return displayMode; }
			set { 
				displayMode = value;
				switch (displayMode)
				{
					case EnumDisplayMode.HarminicField:
						this.vrtDisplayColor = AssignColors(this.harmonicField);
						break;
					case EnumDisplayMode.Segmentation:
						this.AssignVColors();
						break;
				}
			}
		}


		public CrossBoundaryBrushes(Mesh _mesh, Option _opt)
		{
			this.mesh = _mesh; 
			this.opt = _opt;
			this.regionNum = opt.NumberOfIsolines;


			// initialize triareas 
			int fn = mesh.FaceCount, vn = mesh.VertexCount;
			this.triArea = new double[fn];
			for (int i = 0; i < fn; ++i)
			{
				triArea[i] = mesh.ComputeFaceArea(i);
			}
			this.colorInterpolater = new ColorInterpolater();
			this.vrtDisplayColor = new double[vn * 3];
			this.triDisplayColor = new double[fn * 3];
			this.linesOnFace = new List<int>[fn];
			for (int i = 0; i < fn; ++i)
			{
				this.linesOnFace[i] = new List<int>();
			}

			// --- for patch -type --
			this.fnode = new GraphNode[fn];
			this.vnode = new GraphNode[vn];
			for (int i = 0; i < fn; ++i) fnode[i] = new GraphNode(i, -1);
			for (int i = 0; i < vn; ++i) vnode[i] = new GraphNode(i, -1);

			// --- initialize sampling ---
			this.finalFaceRec = new IsoFaceRec[fn];
			for (int i = 0; i < fn; ++i)
			{
				finalFaceRec[i] = new IsoFaceRec();
				finalFaceRec[i].index = i;
			}

			this.ShowCuts = true;
			this.patchid = new byte[fn];
			this.user_operation_stack = new Stack<bool>();

		}
		public void InitialHarminicField()
		{
			int n = mesh.VertexCount;
			this.sourceVertices.Add(0);
			this.sinkVertices.Add(n - 2);
			this.ApplyHarmonicSolver_Cholmod();
		}
		public bool ObtainConstraints(List<List<int>> strokes, List<List<int>> faces_on_strokes)
		{
			if (strokes.Count < 1 || strokes[0].Count < 2) return false;
			foreach (List<int> faces in faces_on_strokes)
				if (faces.Count < 1)
				{
					return false;
				}

			this.verticesOnStroke.Clear(); // obtain vertices on stroke
			this.verticesOnStroke.AddRange(strokes[0]);
			this.facesOnStrokes.Clear();
			this.facesOnStrokes.AddRange(faces_on_strokes);
			
			this.sourceVertices.Clear();
			this.sinkVertices.Clear();
			// -- support multi-strokes --
			if (opt.CutMethod == Option.EnumSegMethods.CGI07 && opt.Cgi_Multiple)
			{
				if (strokes.Count < 2)
					return false;
				foreach (int v in strokes[0])
					this.sourceVertices.Add(v);
				foreach (int v in strokes[1])
					this.sinkVertices.Add(v);
			}
			else
			{
				for (int i = 0; i < strokes.Count; ++i)
				{
					List<int> stroke = strokes[i];

					this.sourceVertices.Add(stroke[0]); // -- add two endpoints --
					this.sinkVertices.Add(stroke[stroke.Count - 1]);
				}
			}
			return true;
		}
		public void Cut()
		{
			switch (opt.CutMethod)
			{
				case Option.EnumSegMethods.Ours:
					if (opt.Part_Type)
					{
						/// solve hasharmonic/stochastic field, obtain isolines.
						is_a_part_brush = true;
						switch (opt.FieldType)
						{
							case Option.EnumFieldType.Harmonic:
								ApplyHarmonicSolver_Cholmod();
								this.ObtainIsolines();
								this.ObtainIsolinesOnStroke();
								this.LocateBestCut();
								this.hasharmonic = true;
								break;
						}
					}
					else /// patch type segmentation
					{
						is_a_part_brush = false;
						patch_type_decompose();
						AssignPatchColors();
						this.ShowPatches = true;
					}
					break;
				case Option.EnumSegMethods.CGI07:
					is_a_part_brush = false;
					CGI_Cut();
					AssignPatchColors();
					this.ShowPatches = true;
					break;
			}
			user_operation_stack.Push(is_a_part_brush);
			RecordPrevStrokes();
		}
		public void Update()
		{
			if (this.finalized) return;
			switch (opt.CutMethod)
			{
				case Option.EnumSegMethods.Ours:
					if (opt.Part_Type)
					{
						is_a_part_brush = true;
						if (hasharmonic == false)
						{
							ApplyHarmonicSolver_Cholmod();
							this.ObtainIsolines();
							this.ObtainIsolinesOnStroke();
							this.LocateBestCut();
							this.hasharmonic = true;
						}
						else
						{
							UpdateHarmonicField();
						}
						this.ShowPatches = false;
					}
					else
					{
						is_a_part_brush = false;
						patch_type_decompose();
						AssignPatchColors();
						this.ShowPatches = true;
					}
					
					break;
				case Option.EnumSegMethods.CGI07:
					CGI_Cut();
					AssignPatchColors();
					this.ShowPatches = true;
					break;
			}

			this.displayMode = EnumDisplayMode.Segmentation;

			user_operation_stack.Push(is_a_part_brush);
			RecordPrevStrokes();
		}
		void RecordPrevStrokes()
		{
			// -- record only for part-brush --
			if (this.is_a_part_brush)
			{
				this.prevSourceVertices.Clear(); // obtain source-sink vertices
				foreach (int v in this.sourceVertices)
					this.prevSourceVertices.Add(v);
				this.prevSinkVertices.Clear();
				foreach (int v in this.sinkVertices)
					this.prevSinkVertices.Add(v);
			}
		}
		

		#region --- hasharmonic filed + isolines ---
		private double[] GetIsovalues(double min, double max, int steps)
		{
			double[] isovalues = new double[steps - 1];
			double s = (max - min) / steps;
			for (int i = 0; i < steps - 1; i++)
				isovalues[i] = s * (i + 1);
			return isovalues;
		}
		private void ApplyHarmonicSolver_Cholmod()
		{
			// --- build up lefthand side matrix A
			int vn = mesh.VertexCount;
			int fn = mesh.FaceCount;
			int bn = this.sourceVertices.Count + this.sinkVertices.Count;
			int m = vn+bn;

			if (this.sparseSolver != null) this.sparseSolver.Release();
			this.sparseSolver = new CholmodSolver();
			
			this.sparseSolver.InitializeMatrixA(m, vn);
			for (int i = 0, j = 0; i < fn; i++, j += 3)
			{
				int c1 = mesh.FaceIndex[j];
				int c2 = mesh.FaceIndex[j + 1];
				int c3 = mesh.FaceIndex[j + 2];
				Vector3d v1 = new Vector3d(mesh.VertexPos, c1 * 3);
				Vector3d v2 = new Vector3d(mesh.VertexPos, c2 * 3);
				Vector3d v3 = new Vector3d(mesh.VertexPos, c3 * 3);
				double cot1 = (v2 - v1).Dot(v3 - v1) / (v2 - v1).Cross(v3 - v1).Length();
				double cot2 = (v3 - v2).Dot(v1 - v2) / (v3 - v2).Cross(v1 - v2).Length();
				double cot3 = (v1 - v3).Dot(v2 - v3) / (v1 - v3).Cross(v2 - v3).Length();
				sparseSolver.Add_Coef(c2, c2, -cot1); sparseSolver.Add_Coef(c2, c3, cot1);
				sparseSolver.Add_Coef(c3, c3, -cot1); sparseSolver.Add_Coef(c3, c2, cot1);
				sparseSolver.Add_Coef(c3, c3, -cot2); sparseSolver.Add_Coef(c3, c1, cot2);
				sparseSolver.Add_Coef(c1, c1, -cot2); sparseSolver.Add_Coef(c1, c3, cot2);
				sparseSolver.Add_Coef(c1, c1, -cot3); sparseSolver.Add_Coef(c1, c2, cot3);
				sparseSolver.Add_Coef(c2, c2, -cot3); sparseSolver.Add_Coef(c2, c1, cot3);
			}

			// add positional weights
			int index = 0;
			foreach (int v in sourceVertices)
			{
				sparseSolver.Add_Coef(vn + index, v, 1.0 * ConstantWeight);
				++index;
			}
			foreach (int v in sinkVertices)
			{
				sparseSolver.Add_Coef(vn + index, v, 1.0 * ConstantWeight);
				++index;
			}

			if (!opt.UseAtb) m = vn;

			double[] b = new double[m];
			double[] x = new double[vn];

			// -- assign values to the right hand side b,-- Ax=b
			if (opt.UseAtb)
			{
				for (int j = 0; j < b.Length; j++) b[j] = 0;
				int count = 0;
				for (int j = 0; j < sourceVertices.Count; ++j, ++count)
					b[vn + j] = 1.0 * ConstantWeight;
				for (int j = 0; j < sinkVertices.Count; ++j)
					b[vn + count + j] = 0.0;
			}
			else // set by user.. 
			{
				foreach (int v in this.sourceVertices)
					b[v] = 1.0 * ConstantWeight * ConstantWeight;
			}

			fixed (double* _b = b)
			{
				sparseSolver.InitializeMatrixB(_b, m, 1);
			}
			sparseSolver.InitializeSolver();
			sparseSolver.SetFinalPack(0);
			sparseSolver.Factorize();

			fixed (double* _x = x)
			{
				sparseSolver.Linear_Solve(_x, !opt.UseAtb);
			}

			this.harmonicField = x;
		}
		private void ObtainIsolines()
		{
			this.isovalues = GetIsovalues(0.0, 1.0, regionNum);
			LocateFacesIsoPosition(this.harmonicField, this.isovalues);
			LocateIsoContours();
		}
		private void LocateFacesIsoPosition(double[] f, double[] isovals)
		{
			if (f == null) throw new ArgumentException();

			// initialize isofaces
			this.isofaces = new IsoFaceRec[mesh.FaceCount];
			for (int i = 0; i < mesh.FaceCount; ++i)
				this.isofaces[i] = new IsoFaceRec();
			for (int i = 0; i < isovalues.Length; ++i)
			{
				double v = isovals[i];
				for (int k = 0, m = 0; k < this.mesh.FaceCount; ++k, m += 3)
				{
					int c1 = this.mesh.FaceIndex[m];
					int c2 = this.mesh.FaceIndex[m + 1];
					int c3 = this.mesh.FaceIndex[m + 2];
					PQPairs r = null;
					if ((f[c1] <= v && f[c2] >= v) || (f[c2] <= v && f[c1] >= v))
					{
						if (r == null) { r = new PQPairs(); r.findex = k; }
						if (r.e1 == -1)
						{
							r.e1 = 0; r.ratio1 = (v - f[c1]) / (f[c2] - f[c1]);
						}
						else
						{
							r.e2 = 0; r.ratio2 = (v - f[c1]) / (f[c2] - f[c1]);
						}
					}
					if ((f[c2] <= v && f[c3] >= v) || (f[c3] <= v && f[c2] >= v))
					{
						if (r == null) { r = new PQPairs(); r.findex = k; }
						if (r.e1 == -1)
						{
							r.e1 = 1; r.ratio1 = (v - f[c2]) / (f[c3] - f[c2]);
						}
						else
						{
							r.e2 = 1; r.ratio2 = (v - f[c2]) / (f[c3] - f[c2]);
						}
					}
					if ((f[c3] <= v && f[c1] >= v) || (f[c1] <= v && f[c3] >= v))
					{
						if (r == null) { r = new PQPairs(); r.findex = k; }
						if (r.e1 == -1)
						{
							r.e1 = 2; r.ratio1 = (v - f[c3]) / (f[c1] - f[c3]);
						}
						else
						{
							r.e2 = 2; r.ratio2 = (v - f[c3]) / (f[c1] - f[c3]);
						}
					}
					if (r == null) continue;
					if (r.e1 == -1 || r.e2 == -1) continue;

					r.isovalue = v;
					r.lindex = i;

					Vector3d v1 = new Vector3d(mesh.VertexPos, c1 * 3);
					Vector3d v2 = new Vector3d(mesh.VertexPos, c2 * 3);
					Vector3d v3 = new Vector3d(mesh.VertexPos, c3 * 3);
					Vector3d n1 = new Vector3d(mesh.FaceNormal, k * 3);
					Vector3d p = new Vector3d(), q = new Vector3d();
					switch (r.e1)
					{
						case 0: p = v2 * r.ratio1 + v1 * (1.0 - r.ratio1); break;
						case 1: p = v3 * r.ratio1 + v2 * (1.0 - r.ratio1); break;
						case 2: p = v1 * r.ratio1 + v3 * (1.0 - r.ratio1); break;
					}
					switch (r.e2)
					{
						case 0: q = v2 * r.ratio2 + v1 * (1.0 - r.ratio2); break;
						case 1: q = v3 * r.ratio2 + v2 * (1.0 - r.ratio2); break;
						case 2: q = v1 * r.ratio2 + v3 * (1.0 - r.ratio2); break;
					}
					r.n = n1; r.p = p; r.q = q;
					isofaces[k].pqPairs.Add(r);
					isofaces[k].index = k;
					isofaces[k].valid = true;
				}
			}
		}
		private void LocateIsoContours()
		{
			// -- find out all the faces belong to a group (with the same iso-value)
			int n = mesh.FaceCount, m = opt.NumberOfIsolines;
			List<FaceRecord>[] faces = new List<FaceRecord>[m];
			for (int i = 0; i < m; ++i)
			{
				faces[i] = new List<FaceRecord>();
			}
			for (int i = 0; i < isofaces.Length; ++i)
			{
				if (isofaces[i] != null && isofaces[i].valid)
				{
					foreach (PQPairs pq in isofaces[i].pqPairs)
					{
						int idx = pq.lindex;
						faces[idx].Add(new FaceRecord(isofaces[i], pq));
					}
				}
			}
			// -- further divid each set into connected groups --> isocontours
			bool[] tag = new bool[mesh.FaceCount]; int lineIndex = 0;
			this.fullIsolineList.Clear();
			for (int i = 0; i < m; ++i)
			{
				List<FaceRecord> s = faces[i];
				foreach (FaceRecord rec in s)
					tag[rec.face.index] = true;
				FaceRecord prev = null;
				foreach (FaceRecord rec in s)
				{
					int v = rec.face.index;
					if (tag[v])
					{
						List<FaceRecord> flist = new List<FaceRecord>();

						int next = v; flist.Add(rec); tag[next] = false; prev = rec; // visited
						bool nextExist = true;
						while (nextExist)
						{
							nextExist = false;
							foreach (int adj in mesh.AdjFF[next])
							{
								if (tag[adj])
								{
									next = adj;

									FaceRecord fr = null;
									foreach (FaceRecord r in s)
									{
										if (adj == r.face.index)
										{
											fr = r;
											break;
										}
									}
									
									if (fr == null) throw new Exception();

									if (!HasCommonPorQ(prev, fr))
									{
										continue;
									}

									flist.Add(fr); prev = fr;
									tag[next] = false;
									nextExist = true;
									break;
								}
							}
						}
						Isoline line = new Isoline(lineIndex++);
						line.faces = flist;
						line.value = isovalues[i];
						this.fullIsolineList.Add(line);
					}
				}
			}
		}
		private bool HasCommonPorQ(FaceRecord f, FaceRecord g)
		{
			return IsTheSamePosition(f.pq.p, g.pq.p) || IsTheSamePosition(f.pq.p, g.pq.q)
				|| IsTheSamePosition(f.pq.q, g.pq.p) || IsTheSamePosition(f.pq.q, g.pq.q);
		}
		private bool IsTheSamePosition(Vector3d s, Vector3d t)
		{
			if ((s - t).Length() < 1e-08)
				return true;
			return false;
		}
		private Set<int> ObtainStrokeFaces()
		{
			int m = this.verticesOnStroke.Count;
			if (m < 2) return null;
			
			int n = mesh.VertexCount;
			int source = this.verticesOnStroke[0];
			int target = this.verticesOnStroke[m - 1];
			int[] pred = new int[n];
			for (int i = 0; i < n; ++i) pred[i] = -1;
			bool[] mark = new bool[n];
			Queue<int> iQue = new Queue<int>();
			iQue.Enqueue(source); mark[source] = true;
			bool find = false;
			while (iQue.Count > 0 && !find)
			{
				int seed = iQue.Dequeue();
				foreach (int j in mesh.AdjVV[seed])
				{
					if (!mark[j])
					{
						mark[j] = true;
						pred[j] = seed;
						if (j == target) { find = true; break; }
						iQue.Enqueue(j);
					}
				}
			}
			List<int> vset = new List<int>();
			int tmp = target; 
			while (pred[tmp] != -1)
			{
				vset.Add(tmp);
				tmp = pred[tmp];
			}
			vset.Add(tmp); vset.Reverse(0,vset.Count);
			Set<int> fSet = new Set<int>();
			foreach (int v in vset)
			{
				foreach (int f in mesh.AdjVF[v])
				{
					fSet.Add(f);
				}
			}
			return fSet;
		}
		private List<int>[] linesOnFace = null; // record isolines that passing through each face
		private List<Isoline> bestCuts = new List<Isoline>();
		private Stack<bool> user_operation_stack = null;
		private void ObtainIsolinesOnStroke()
		{
			this.isolinesOnStroke.Clear();
			int n = mesh.FaceCount;
			// -- obtain face-isoline mapping --
			for (int i = 0; i < n; ++i) this.linesOnFace[i].Clear();
			int index = 0;
			foreach (Isoline line in this.fullIsolineList)
			{
				foreach (FaceRecord f in line.faces)
				{
					int b = f.face.index;
					if (!linesOnFace[b].Contains(index))
						this.linesOnFace[b].Add(index);
				}
				index++;
			}
			// -- get lines that passing through vertices on stroke --
			List<int> linesOnStroke = new List<int>();
			Set<int> fOnStrokes = ObtainStrokeFaces();
			foreach (int f in fOnStrokes)
			{
				List<int> list = this.linesOnFace[f];
				foreach (int lineid in list)
					if (!linesOnStroke.Contains(lineid))
					{
						linesOnStroke.Add(lineid);
					}
			}
			
			SortLines(linesOnStroke); // --- sort according to the isovalues

			this.isolinesOnStroke.Clear();
			foreach (int id in linesOnStroke)
			{
				Isoline line = this.fullIsolineList[id];
				ObtainIsolineProperties(line);
				this.isolinesOnStroke.Add(line);
			}

			// -- testing  --
			index = 0;
			foreach (Isoline line in this.isolinesOnStroke)
			{
				Program.PrintText(index.ToString() + ": " + line.value.ToString());
				index++;
			}
		}
		private void SortLines(List<int> linesOnV)
		{
			// -- sort isolines according to a desending order of isovalue --

			if (linesOnV.Count <= 1) return;

			List<double> arr = new List<double>();
			Dictionary<double, int> dict = new Dictionary<double, int>();

			foreach (int v in linesOnV)
			{
				double val = this.fullIsolineList[v].value;

				if (dict.ContainsKey(val))
				{
					Program.PrintText("Key exisit: " + val.ToString());
					continue;
				}
				dict.Add(val, v);
				arr.Add(val);
			}
			arr.Sort();
			linesOnV.Clear();
			for (int i = arr.Count - 1; i >= 0; --i)
			{
				double val = arr[i];
				linesOnV.Add(dict[val]);
			}
		}
		private void ObtainIsolineProperties(Isoline line)
		{
			// -- obtain radius --
			double C = 0;
			foreach (FaceRecord f in line.faces)
			{
				if (f.face.valid == false) throw new ArgumentException();
				PQPairs pair = f.pq;
				C += (pair.p - pair.q).Length();
			}
			line.radious = C / (2 * Math.PI);
		}
		private void LocateBestCut()
		{
			if (this.isolinesOnStroke.Count <= 0) return;
			List<Isoline> list = this.isolinesOnStroke;
			
			int n = list.Count;
			int best = -1;
			if (n == 1)
			{
				best = 0;
			}
			else
			{
				double[] d = new double[n]; // -- metric --
				double max_r = double.MinValue;
				for (int i = 0; i < n; ++i)
				{
					if (max_r < list[i].radious)
						max_r = list[i].radious;
				}
				for (int i = 0; i < n; ++i)
				{
					double r = list[i].radious, r1 = r, r2 = r, concavity = 0;
					if (i > 0) r1 = list[i - 1].radious;
					if (i < n - 1) r2 = list[i + 1].radious;

					// -- multi-scale -- compute the concavity
					if (i == 0)
					{
						r2 = list[i + 1].radious;
						concavity = (2 * r - r2 - r1) / max_r;
					}
					else if (i == n - 1)
					{
						r1 = list[i - 1].radious;
						concavity = (2 * r - r2 - r1) / max_r;
					}
					else
					{
						int s = i - 1, t = i + 1, count = 0;
						double w_sum = 0;
						while (s >= 0 && t < n)
						{
							r1 = list[s].radious;
							r2 = list[t].radious;
							double w = Math.Exp(-(double)count * count / (double)(4 * 2));
							concavity += (2 * r - r1 - r2) / max_r * w;
							s--; t++; count++;
							w_sum += w;
						}
						concavity /= w_sum;
					}

					double c = Math.Abs((double)n * 0.5 - i) / ((double)n * 0.5);
					double sigma = 0.5;
					double center_ness = Math.Exp(-c * c / (2 * sigma * sigma));
					// double tight_ness = r / max_r;
					d[i] = center_ness * concavity;
				}
				double min = double.MaxValue;
				for (int i = 0; i < n; ++i)
				{
					if (d[i] < min)
					{
						min = d[i];
						best = i;
					}
				}
			}
			if (best != -1)
			{
				this.bestCuts.Add(list[best]);
				RecordIsofaceOnBestCut();
				
				// -- for video --
				ObtainNewPatch();
				AssignPatchColors();
			//	this.ShowPatches = true;
			}
		}
		private void RecordIsofaceOnBestCut()	// -- this should be called each time new best cut is located!
		{
			int n = this.bestCuts.Count;
			if (n < 1) return;
			Isoline line = this.bestCuts[n - 1];
			foreach (FaceRecord fr in line.faces)
			{
				int id = fr.face.index;
				PQPairs newPr = fr.pq.Clone() as PQPairs;
				fr.pq = newPr;
				this.finalFaceRec[id].pqPairs.Add(newPr);
			}
		}
		private void ObtainNewPatch()
		{
			if (bestCuts.Count < 1) return;

			int n = mesh.FaceCount;
			bool[] visited =  new bool[n];
			Isoline lastLine = this.bestCuts[bestCuts.Count - 1];
			foreach (FaceRecord fr in lastLine.faces)
			{
				visited[fr.face.index] = true;
			}
			
		/*	foreach (Patch pch in this.patches)
			{
				foreach (int f in pch.faces)
					visited[f] = true;
			}
		 * */

			int i = -1;
			foreach (List<int> flist in this.facesOnStrokes)
			{
				int f = flist[0];
				i = f;
				break;
			}
			if (i == -1)
			{
				Program.PrintText("Invalid source vertex!");
				return;
			}
			int index = this.all_patches.Count + 1;
			int pid = patchid[i];
			Patch p = new Patch(index);
			Queue<int> iQue = new Queue<int>();
			iQue.Enqueue(i);
			visited[i] = true;
			p.faces.Add(i);
			while(iQue.Count > 0)
			{
				int f = iQue.Dequeue();
				foreach (int adj in mesh.AdjFF[f])
				{
					if (patchid[adj] == pid && !visited[adj])
					{
						iQue.Enqueue(adj);
						visited[adj] = true;
						p.faces.Add(adj);
					}
				}
			}
			// --  assign faces on the isoline --
			foreach (FaceRecord fr in lastLine.faces)
			{
				int f = fr.face.index;
				p.faces.Add(f);
			}
			p.prev_label = pid;
			foreach (int f in p.faces)
				patchid[f] = (byte)index;
			this.all_patches.Add(p);
		}
		public void RemovePrevCut()
		{
			if (this.user_operation_stack.Count > 0)
				is_a_part_brush = this.user_operation_stack.Peek();
			else
				return;
			if (is_a_part_brush)
			{
				// -- remove prev isoface records --
				int n = this.bestCuts.Count;
				if (n > 0) 
				{
					Isoline line = this.bestCuts[n - 1];
					foreach (FaceRecord fr in line.faces)
					{
						int id = fr.face.index;
						List<PQPairs> pqs = this.finalFaceRec[id].pqPairs;
						pqs.RemoveAt(pqs.Count-1);
						patchid[id] = 0;
					}
					// -- remove the isoline --
					if (this.bestCuts.Count > 0)
					{
						this.bestCuts.RemoveAt(n - 1);
					}
				}
			}
			else
			{
				// -- remove previous patches obtain by a patch-brush --
				int n = patches.Count;
				if (n > 0) 
				{
					this.patches.RemoveAt(n-1);
				}
			}
			// -- remove previous assigned patch --
			int nn = all_patches.Count;
			if (nn < 1) return;
			Patch p = this.all_patches[nn-1];
			byte prev_id = p.prev_label < 0 ? (byte)0 : (byte)p.prev_label;
			foreach (int f in p.faces)
			{
				patchid[f] = prev_id;
			}
			this.all_patches.Remove(p);
			this.AssignPatchColors();
			this.user_operation_stack.Pop();
		}
		private void UpdateHarmonicField()
		{
			if (this.sparseSolver == null) return;

			//this.ObtainConstrainVertices(); // -- update constraints.

			if (this.sourceVertices.Count == 0 || this.sinkVertices.Count == 0) return;

			int n = mesh.VertexCount;
			// --- change positional weights -- update factorization
			this.sparseSolver.InitializeMatrixC(n,n);
			foreach (int v in sourceVertices)
			{
				sparseSolver.AddCoef_C(v, v, 1.0 * ConstantWeight);
			}
			foreach (int v in sinkVertices)
			{
				sparseSolver.AddCoef_C(v, v, 1.0 * ConstantWeight);
			}

			if (!sparseSolver.UpdateFactorization(true))
			{
				Program.PrintText("fail to update factorization!\n");
			}

			// --- change positional weights -- downdate factorization
			foreach (int v in sourceVertices)
			{
				sparseSolver.AddCoef_C(v, v, -1.0 * ConstantWeight);
			}
			foreach (int v in sinkVertices)
			{
				sparseSolver.AddCoef_C(v, v, -1.0 * ConstantWeight);
			}
			foreach (int v in prevSourceVertices)
			{
				sparseSolver.AddCoef_C(v, v, 1.0 * ConstantWeight);
			}
			foreach (int v in prevSinkVertices)
			{
				sparseSolver.AddCoef_C(v, v, 1.0 * ConstantWeight);
			}
			
			if (!sparseSolver.UpdateFactorization(false))
			{
				Program.PrintText("fail to downdate factorization!\n");
			}

			if (opt.UseAtb)
			{
				// --- update the sparse matrix A
				int m = n, index = 0;
				foreach (int v in sourceVertices)
				{
					sparseSolver.Add_Coef(m+index, v, 1.0 * ConstantWeight);
					index++;
				}
				foreach (int v in sinkVertices)
				{
					sparseSolver.Add_Coef(m+index, v, 1.0 * ConstantWeight);
					index++;
				}
				index = 0;
				foreach (int v in prevSourceVertices)
				{
					//sparseSolver.Set_Coef(m + index, v, 0);
					sparseSolver.Add_Coef(m + index, v, -1.0 * ConstantWeight);
					index++;
				}
				foreach (int v in prevSinkVertices)
				{
					//sparseSolver.Set_Coef(m + index, v, 0);
					sparseSolver.Add_Coef(m + index, v, -1.0 * ConstantWeight);
					index++;
				}

				sparseSolver.UpdateAT();
			}
			else
			{
				// -- directly modify b --
				foreach (int v in prevSourceVertices)
				{
					sparseSolver.SetCoef_B(v, 0, 0.0);
				}
				foreach (int v in sourceVertices)
				{
					sparseSolver.SetCoef_B(v, 0, 1.0 * ConstantWeight * ConstantWeight);
				}
			}

			double[] x = new double[n];
			fixed (double* _x = x)
			{
				sparseSolver.Linear_Solve(_x, !opt.UseAtb);
			}

			//for (int j = 0; j < n; j++)
			//    if (x[j] < 0)
			//        x[j] = 0;
			this.harmonicField = x;

			// assign hasharmonic colros to each vertex..
			this.ObtainIsolines();
			this.ObtainIsolinesOnStroke();
			this.LocateBestCut();
		}
		#endregion



		#region --- patch type segmentation ---
		private double[] old_normals = null;
		private double[] new_normals = null;
		private void FilterFaceNormals(double s, int iterations) /*s is the \sigma*/
		{
			// -- precompute normals based on NII, as a smoothing step --
			int n = mesh.FaceCount; double denominator  = s*s*2;
			old_normals = mesh.FaceNormal.Clone() as double[];
			new_normals = new double[n*3];
			for (int k = 0; k < iterations; ++k)
			{
				for (int i = 0; i < n; ++i)
				{
					// -- get two-ring neighbors
					Set<int> nbrs = this.neighbors[i]; nbrs.Remove(i);
					int count = nbrs.Count, b = i * 3;
					Vector3d ni = new Vector3d(mesh.FaceNormal, b);
					Vector3d nn = new Vector3d();
					foreach (int f in nbrs)
					{
						Vector3d nj = new Vector3d(mesh.FaceNormal, f * 3);
						double d = (ni - nj).Length();
						double w = Math.Exp(-d * d / denominator);
						nn = nn + w * nj;
					}
					nn = nn.Normalize();
					new_normals[b] = nn.x;
					new_normals[b + 1] = nn.y;
					new_normals[b + 2] = nn.z;
				}
				//mesh.FaceNormal = new_normals.Clone() as double[];
			}
		}
		private double Face_dij(int i, int j)
		{
			Vector3d ci = new Vector3d(mesh.DualVertexPos, i * 3);
			Vector3d cj = new Vector3d(mesh.DualVertexPos, j * 3);
			double d = (cj - ci).Length();
			return d;
		}
		private double Face_wij(int i, int j)
		{
			double[] normal = this.new_normals == null ? mesh.FaceNormal : this.new_normals;
			int b = i * 3, c = j * 3;
			Vector3d ni = new Vector3d(normal, b);
			Vector3d nj = new Vector3d(normal, c);
			Vector3d vi = new Vector3d(mesh.DualVertexPos, b);
			Vector3d vj = new Vector3d(mesh.DualVertexPos, c);
			Vector3d eij = (vj - vi).Normalize();
			double d = (nj - ni).Length();
			double e = Math.Abs((ni).Dot(eij));
			return (1+e)*d;
		}
		private class GraphNode : PriorityQueueElement
		{
			public int index = -1;
			public int label = -1;
			public double dis = -1;
			public GraphNode(int id, double d)
			{
				index = id;
				dis = d;
			}
			#region PriorityQueueElement Members
			private int pqIndex = -1;
			public int PQIndex
			{
				get { return pqIndex; }
				set { pqIndex = value; }
			}
			public int CompareTo(object obj)
			{
				GraphNode right = obj as GraphNode;
				if (dis < right.dis) return -1;
				else if (dis > right.dis) return 1;
				else return 0;
			}
			#endregion
		}
		private GraphNode[] fnode = null;


		public class Patch
		{
			public int label = -1;
			public int prev_label = -1;
			public Set<int> faces = new Set<int>();
			public Set<int> adjPatches = new Set<int>();
			public Patch(int _lb)
			{
				label = _lb;
			}
			public bool valid = true; // -- indicate whether is a valid patch --
		}
		public class PatchPair : PriorityQueueElement
		{
			public int p = -1;
			public int q = -1;
			public double merge_cost = 0;
			public PatchPair(int _p, int _q, double _cost)
			{
				merge_cost = _cost;
				p = _p;
				q = _q;
			}
			#region PriorityQueueElement Members
			private int pqIndex = -1;
			public int PQIndex
			{
				get { return pqIndex; }
				set { pqIndex = value; }
			}
			public int CompareTo(object obj)
			{
				PatchPair right = obj as PatchPair;
				if (merge_cost < right.merge_cost) return -1;
				else if (merge_cost > right.merge_cost) return 1;
				else return 0;
			}
			#endregion
		}

		// -- all patches, including the patch generated by part-brush
		private List<Patch> all_patches = new List<Patch>();
		private List<Patch> patches = new List<Patch>();
		
		
		public void AssignPatchColors()
		{
			int n = mesh.FaceCount;
			if (this.triDisplayColor == null || this.triDisplayColor.Length != n*3)
				this.triDisplayColor = new double[n * 3];
			
			for (int i = 0; i < n; ++i)
			{
				Color c = Program.displayProperty.ColorMall[0];
				//if (i < fnode.Length && fnode[i].label < 0) c = Color.Red;
				int b = i * 3;
				triDisplayColor[b] = c.R / 255.0;
				triDisplayColor[b + 1] = c.G / 255.0;
				triDisplayColor[b + 2] = c.B / 255.0;
			}
			
			int label = 1;
			foreach (Patch p in this.all_patches)
			{
				if (!p.valid) continue;
				Color c = Program.displayProperty.ColorMall[(label++)%30];
				foreach (int f in p.faces)
				{
					int b = f*3;
					triDisplayColor[b] = c.R / 255.0;
					triDisplayColor[b+1] = c.G / 255.0;
					triDisplayColor[b+2] = c.B / 255.0;
				}
			}

			AssignVColors();
		}		
		public void AssignVColors()
		{
			int faceCount = mesh.FaceCount;
			int vertexCount = mesh.VertexCount;

			Color[] colors = Program.displayProperty.ColorMall;

			bool[] visited = new bool[vertexCount];
			for (int i = 0,j=0; i < faceCount; ++i,j+=3)
			{
				int index = this.patchid[i];
				Color c = colors[index];
				for (int k = 0; k < 3; ++k)
				{
					int v = mesh.FaceIndex[j+k];
					if (!visited[v])
					{
						visited[v] = true;

						int cc = v * 3;
						this.vrtDisplayColor[cc] = c.R / 255.0;
						this.vrtDisplayColor[cc+1] = c.G / 255.0;
						this.vrtDisplayColor[cc+2] = c.B / 255.0;
					}
				}
			}
		}


		private byte[] patchid = null;
		private void patch_type_decompose()
		{
			int n = mesh.FaceCount;
			for (int i = 0; i < n; ++i) // -- reset the labels --
				fnode[i].label = -1;
			// -- label seed faces --
			bool[] visited = new bool[n];
			bool[] labeled = new bool[n];
			if (patchid == null) patchid = new byte[n];
			List<int> sources = new List<int>();
			List<int> targets = new List<int>();
			foreach (List<int> flist in this.facesOnStrokes)
			{
				if (flist.Count < 1) continue;
				sources.Add(flist[0]);
				targets.Add(flist[flist.Count - 1]);
			}
			byte pid = patchid[sources[0]]; // -- the patch strokes lies on --
			foreach (int f in sources)
			{
				fnode[f].label = 0;
				visited[f] = true;
				labeled[f] = true;
			}
			int label = 1;
			foreach (int f in targets)
			{
				fnode[f].label = label;
				visited[f] = true;
				labeled[f] = true;
			}
			// -- region growing --
			PriorityQueue iQue = new PriorityQueue();
			foreach (int f in sources)
			{
				foreach (int adj in mesh.AdjFF[f])
				{
					if (patchid[adj] != pid) continue;
					fnode[adj].label = fnode[f].label;
					fnode[adj].dis = Face_wij(f, adj);
					iQue.Insert(fnode[adj]);
					visited[adj] = true;
				}
			}
			foreach (int f in targets)
			{
				foreach (int adj in mesh.AdjFF[f])
				{
					if (patchid[adj] != pid) continue;
					fnode[adj].label = fnode[f].label;
					fnode[adj].dis = Face_wij(f, adj);
					iQue.Insert(fnode[adj]);
					visited[adj] = true;
				}
			}
			while (!iQue.IsEmpty())
			{
				GraphNode node = iQue.DeleteMin() as GraphNode;

				int f = node.index;
				labeled[f] = true;
				foreach (int adj in mesh.AdjFF[f])
				{
					if (patchid[adj] != pid) continue;
					if (labeled[adj])
					{
						continue;
					}
					else if (visited[adj])
					{
						double wij = Face_wij(f, adj);
						if (fnode[adj].dis > wij)
						{
							fnode[adj].label = fnode[f].label;
							fnode[adj].dis = wij;
							iQue.Update(fnode[adj]);
						}
					}
					else
					{
						fnode[adj].label = fnode[f].label;
						fnode[adj].dis = Face_wij(f, adj);
						iQue.Insert(fnode[adj]);
						visited[adj] = true;
					}
				}
			}
			// -- patch the results --
			int index = this.all_patches.Count+1;
			Patch p = new Patch(index);
			p.prev_label = (int)pid;
			for (int i = 0; i < n; ++i)
			{
				if (fnode[i].label == 0)
				{
					patchid[i] = (byte)index;
					p.faces.Add(i);
				}
			}
			this.patches.Add(p);
			this.all_patches.Add(p);	// -- global patches --
		}
		public bool ShowPatches { get; set; }
		#endregion



		#region --- implementation of CGI'07 ---
		// -- a method based on vertex region growing --
		private GraphNode[] vnode = null;
		private double Vrt_wij(int i, int j)
		{
			int b = i * 3, c = j * 3;
			Vector3d s = new Vector3d(mesh.VertexPos, b);
			Vector3d t = new Vector3d(mesh.VertexPos, c);
			Vector3d e = t-s;
			Vector3d ns = new Vector3d(mesh.VertexNormal, b);
			Vector3d nt = new Vector3d(mesh.VertexNormal, c);
			double w = (1 + ns.Dot(e) / e.Length()) * (1 + ns.Cross(nt).Length());
			return w;
		}
		private double Face_wij1(int i, int j)
		{
			Vector3d ns = new Vector3d(mesh.FaceNormal, i * 3);
			Vector3d nt = new Vector3d(mesh.FaceNormal, j * 3);
			return -ns.Dot(nt);
		}
		private void CGI_Cut()
		{
			// -- mark verteices on strokes --
			int n = mesh.VertexCount, label = 0;
			bool[] labeled = new bool[n];
			bool[] tag = new bool[n];
			foreach (int v in this.sourceVertices)
			{
				vnode[v].label = label;
				labeled[v] = true;
				tag[v] = true;
			}
			label = 1;
			foreach (int v in this.sinkVertices)
			{
				vnode[v].label = label;
				labeled[v] = true;
				tag[v] = true;
			}
			// -- push to priority queue --
			PriorityQueue iQue = new PriorityQueue();
			foreach (int v in this.sourceVertices)
			{
				foreach (int adj in mesh.AdjVV[v])
				{
					GraphNode node = vnode[adj];
					node.label = vnode[v].label;
					node.dis = Vrt_wij(v, adj);
					tag[adj] = true;
					iQue.Insert(node);
				}
			}
			foreach (int v in this.sinkVertices)
			{
				foreach (int adj in mesh.AdjVV[v])
				{
					GraphNode node = vnode[adj];
					node.label = vnode[v].label;
					node.dis = Vrt_wij(v, adj);
					tag[adj] = true;
					iQue.Insert(node);
				}
			}
			// -- region growing --
			while (!iQue.IsEmpty())
			{
				GraphNode node = iQue.DeleteMin() as GraphNode;
				int v = node.index;
				labeled[v] = true;
				foreach (int adj in mesh.AdjVV[v])
				{
					if (labeled[adj])  // -- already labeled --
					{
						continue;
					}
					else
					{
						double cost = Vrt_wij(v, adj);
						GraphNode adjNode = vnode[adj];
						adjNode.label = node.label;
						if (tag[adj]) // -- already in the queue --
						{
							if (adjNode.dis > cost)
							{
								adjNode.dis = cost;
								iQue.Update(adjNode);
							}
						}
						else // -- a fresh vertex --
						{
							adjNode.dis = cost;
							tag[adj] = true;
							iQue.Insert(adjNode);
						}
					}
				}
			}
			// -- convert to facets --
			List<int> risidual = new List<int>();
			for (int i = 0, j = 0; i < mesh.FaceCount; ++i, j+=3)
			{
				int c0 = mesh.FaceIndex[j];
				int c1 = mesh.FaceIndex[j+1];
				int c2 = mesh.FaceIndex[j+2];
				if (vnode[c0].label == vnode[c1].label &&
					vnode[c0].label == vnode[c2].label)
				{
					fnode[i].label = vnode[c0].label;
				}
				else
				{
					fnode[i].label = -1;
					risidual.Add(i);
				}
			}
			// -- deal with boundary faces --
			while (risidual.Count > 0)
			{
			    List<int> vlist = new List<int>();
			    vlist.AddRange(risidual);
			    risidual.Clear();
			    foreach (int f in vlist)
			    {
			        double min = double.MaxValue; int minid = -1;
			        foreach (int adj in mesh.AdjFF[f])
			        {
			            if (fnode[adj].label < 0) continue;
			            double c = Face_wij1(f, adj);
			            if (c < min)
			            {
			                min = c;
			                minid = adj;
			            }
			        }
			        if (minid != -1)
			            fnode[f].label = fnode[minid].label;
			        else
			            risidual.Add(f);
			    }
			}
			// -- patch the results --
			int index = this.patches.Count + 1;
			Patch p = new Patch(index);
			if (patchid == null) patchid = new byte[mesh.FaceCount];
			for (int i = 0; i < mesh.FaceCount; ++i)
			{
				if (fnode[i].label == 0)
				{
					patchid[i] = (byte)index;
					p.faces.Add(i);
				}
			}
			this.all_patches.Add(p);
		}

		#endregion


		#region --- load/save interfaces ---
		public void SaveStrokes(StreamWriter sw)
		{
			foreach (int v in this.sourceVertices)
			{
				sw.WriteLine("f " + (v+1).ToString());
			}
			foreach (int v in this.sinkVertices)
			{
				sw.WriteLine("b " + (v+1).ToString());
			}
		}
		public void SaveSegmentation(StreamWriter sw)
		{
			int n = this.all_patches.Count;
			if (n == 0) return;

			sw.WriteLine(all_patches.Count.ToString());
			int indexer = 0;
			foreach (Patch patch in all_patches)
			{
				foreach (int v in patch.faces)
				{
					sw.WriteLine(v.ToString() + " " + indexer.ToString());
				}
				indexer++;
			}

			//sw.WriteLine((n+1).ToString());
			//for (int i = 0; i < this.patchid.Length; ++i)
			//{
			//    sw.WriteLine(i.ToString() + " " + (patchid[i]).ToString());
			//}

			//if (this.all_patches.Count < 1) return;
			//int index = 0;
			//sw.WriteLine(all_patches.Count.ToString());
			//foreach (Patch p in this.all_patches)
			//{
			//    foreach (int f in p.faces)
			//    {
			//        sw.WriteLine(f.ToString() + " " + index.ToString());
			//    }
			//    index++;
			//}
		}
		public void LoadSegmentation(StreamReader sr)
		{
			this.all_patches.Clear();

			this.patchid = new byte[mesh.FaceCount];

			char[] delimiters = { ' ', '\t' };
			string s = "";

			s = sr.ReadLine();
			string[] tokens = s.Split(delimiters);
			int count = int.Parse(tokens[0]);

			for (int i = 1; i <= count; ++i)
			{
				Patch p = new Patch(i);
				all_patches.Add(p);
			}

			while (sr.Peek() > -1)
			{
				s = sr.ReadLine();
				tokens = s.Split(delimiters);
				int index = int.Parse(tokens[1]);
				int f = int.Parse(tokens[0]);
				all_patches[index].faces.Add(f);

				this.patchid[f] = (byte)index;
			}
			this.AssignPatchColors();

			this.displayMode = EnumDisplayMode.Segmentation;

		}
		#endregion


		#region --- rendering isolines
		public bool ShowCuts
		{
			get;
			set;
		}
		public void Display()
		{
			switch(this.displayMode)
			{
				case EnumDisplayMode.Isolines:
					DrawIsolines(this.fullIsolineList, Color.Red, 2.0f);
					//DrawIsolines(this.isolinesOnStroke, Color.Red);
					DrawHarmonicField();
					break;
				case EnumDisplayMode.HarminicField:
				case EnumDisplayMode.Segmentation:
					DrawHarmonicField();
					break;
				default:
					break;
			}
			if (ShowCuts)
			{
				DrawIsolines(this.bestCuts, Color.Red, 2.0f);
			}
			if (ShowPatches)
			{
				DrawPatches();
			}
		}
		private void DrawIsolines(List<Isoline> list, Color linecolor, float linewidth)
		{
			if (list == null || list.Count == 0) return;
			GL.glPushAttrib(GL.GL_LINE_BIT | GL.GL_ENABLE_BIT);
			GL.glLineWidth(linewidth);
			GL.glEnable(GL.GL_LINE_SMOOTH);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);
			GL.glDisable(GL.GL_POLYGON_OFFSET_FILL);
			GL.glEnable(GL.GL_CULL_FACE);
			GL.glColor3ub(linecolor.R, linecolor.G, linecolor.B);
			GL.glBegin(GL.GL_LINES);
			foreach (Isoline line in list)
				foreach (FaceRecord rec in line.faces)
				{
					Vector3d p = rec.pq.p, q = rec.pq.q, normal = rec.pq.n;
					GL.glNormal3d(normal.x, normal.y, normal.z);
					GL.glVertex3d(p.x, p.y, p.z);
					GL.glVertex3d(q.x, q.y, q.z);
				}
			GL.glEnd();
			GL.glPopAttrib();
			GL.glEnable(GL.GL_POLYGON_OFFSET_FILL);
			GL.glEnable(GL.GL_CULL_FACE);
			GL.glDisable(GL.GL_LIGHTING);
			GL.glDisable(GL.GL_NORMALIZE);

		}
		private void DrawIsoline(Isoline line, Color linecolor)
		{
			GL.glPushAttrib(GL.GL_LINE_BIT | GL.GL_ENABLE_BIT);
			GL.glLineWidth(2.0f);
			GL.glEnable(GL.GL_LINE_SMOOTH);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);
			GL.glDisable(GL.GL_POLYGON_OFFSET_FILL);
			GL.glEnable(GL.GL_CULL_FACE);
			GL.glColor3ub(linecolor.R, linecolor.G, linecolor.B);
			GL.glBegin(GL.GL_LINES);
				foreach (FaceRecord rec in line.faces)
				{
					Vector3d p = rec.pq.p, q = rec.pq.q, normal = rec.pq.n;
					GL.glNormal3d(normal.x, normal.y, normal.z);
					GL.glVertex3d(p.x, p.y, p.z);
					GL.glVertex3d(q.x, q.y, q.z);
				}
			GL.glEnd();
			GL.glPopAttrib();
			GL.glEnable(GL.GL_POLYGON_OFFSET_FILL);
			GL.glEnable(GL.GL_CULL_FACE);
			GL.glDisable(GL.GL_LIGHTING);
			GL.glDisable(GL.GL_NORMALIZE);
		}
		private void DrawHarmonicField()
		{
			if (this.vrtDisplayColor == null) return;
			GL.glShadeModel(GL.GL_SMOOTH);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);
			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			GL.glEnableClientState(GL.GL_COLOR_ARRAY);
			GL.glEnableClientState(GL.GL_NORMAL_ARRAY);
			fixed (double* vp = mesh.VertexPos, cp = this.vrtDisplayColor, np = mesh.VertexNormal)
			fixed (int* index = mesh.FaceIndex)
			{
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glColorPointer(3, GL.GL_DOUBLE, 0, cp);
				GL.glNormalPointer(GL.GL_DOUBLE, 0, np);
				GL.glDrawElements(GL.GL_TRIANGLES, mesh.FaceCount * 3, GL.GL_UNSIGNED_INT, index);
			}
			GL.glDisableClientState(GL.GL_VERTEX_ARRAY);
			GL.glDisableClientState(GL.GL_COLOR_ARRAY);
			GL.glDisableClientState(GL.GL_NORMAL_ARRAY);
			GL.glDisable(GL.GL_LIGHTING);
		}
		private void DrawPatches()
		{
//			if (this.patches.Count <= 0) return;

			GL.glShadeModel(GL.GL_FLAT);
			GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);
			GL.glEnable(GL.GL_LIGHTING);
			GL.glEnable(GL.GL_NORMALIZE);

			Mesh m = this.mesh;

			GL.glEnableClientState(GL.GL_VERTEX_ARRAY);
			fixed (double* vp = m.VertexPos)
			fixed (double* np = m.FaceNormal)
			{			
				GL.glVertexPointer(3, GL.GL_DOUBLE, 0, vp);
				GL.glBegin(GL.GL_TRIANGLES);
				for (int i = 0, j = 0; i < m.FaceCount; i++, j += 3)
				{
					GL.glColor3d(this.triDisplayColor[j], 
						this.triDisplayColor[j+1], this.triDisplayColor[j+2]);
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
		
		private double[] AssignColors(double[] gragient)
		{
			Color[] clrs = this.colorInterpolater.InterpolateColors(gragient);
			double[] color = new double[clrs.Length * 3];
			for (int i = 0, j = 0; i < clrs.Length; ++i, j+=3)
			{
				color[j] = (double)clrs[i].R / 255.0;
				color[j+1] = (double)clrs[i].G / 255.0;
				color[j+2] = (double)clrs[i].B / 255.0;
			}
			return color;
		}
		#endregion


		#region IDisposable Members

		public void Dispose()
		{
			if (this.sparseSolver != null)
				this.sparseSolver.Release();
		}

		#endregion

	}
}
