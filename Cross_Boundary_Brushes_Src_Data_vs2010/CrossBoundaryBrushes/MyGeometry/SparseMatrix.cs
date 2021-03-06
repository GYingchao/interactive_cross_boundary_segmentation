using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MyGeometry
{
	public class SparseMatrix
	{
		#region Helper Classes
		public class Element
		{
			public int i, j;
			public double value;

			public Element(int i, int j, double value)
			{
				this.i = i;
				this.j = j;
				this.value = value;
			}
		}

		private class RowComparer : IComparer<Element>
		{
			#region IComparer Members
			public int Compare(Element e1, Element e2)
			{
				return e1.j - e2.j;
			}
			#endregion
		}

		private class ColumnComparer : IComparer<Element>
		{
			#region IComparer Members
			public int Compare(Element e1, Element e2)
			{
				return e1.i - e2.i;
			}
			#endregion
		}

		#endregion

		private int m, n;
		private List<List<Element>> rows, columns;

		public int RowSize { get { return m; } }
		public int ColumnSize { get { return n; } }
		public int NumOfElements() 
		{
			int count = 0;
			if (m < n) 
				foreach (List<Element> r in rows) count += r.Count;
			else
				foreach (List<Element> c in columns) count += c.Count;
			return count;
		}
		public List<List<Element>> Rows { get { return rows; } }
		public List<List<Element>> Columns { get { return columns; } }
		public List<Element> GetRow(int index) { return (List<Element>) rows[index]; }
		public List<Element> GetColumn(int index) { return (List<Element>) columns[index]; }


		public SparseMatrix(int m, int n)
		{
			this.m = m;
			this.n = n;
			rows = new List<List<Element>>(m);
			columns = new List<List<Element>>(n);

			for (int i=0; i<m; i++)
				rows.Add(new List<Element>());
			for (int i=0; i<n; i++)
				columns.Add(new List<Element>());
		}
		public SparseMatrix(int m, int n, int nElements)
		{
			this.m = m;
			this.n = n;
			rows = new List<List<Element>>(m);
			columns = new List<List<Element>>(n);

			for (int i=0; i<m; i++)
				rows.Add(new List<Element>(nElements));
			for (int i=0; i<n; i++)
				columns.Add(new List<Element>(nElements));
		}
		public SparseMatrix(SparseMatrix right) 
		{
			m = right.m;
			n = right.n;
			rows = new List<List<Element>>(m);
			columns = new List<List<Element>>(n);
			for (int i=0; i<m; i++)
				rows.Add(new List<Element>());
			for (int i=0; i<n; i++)
				columns.Add(new List<Element>());
			foreach (List<Element> list in right.Rows)
				foreach (Element e in list)
					AddElement(e.i, e.j, e.value);
		}
		public override bool Equals(object obj)
		{
			SparseMatrix right = obj as SparseMatrix;
			if (obj == null) return false;
			if (right.m != m) return false;
			if (right.n != n) return false;

			for (int i=0; i<n; i++) 
			{
				List<Element> c1 = columns[i] as List<Element>;
				List<Element> c2 = right.columns[i] as List<Element>;
				if (c1.Count != c2.Count) return false;
				for (int j=0; j<c1.Count; j++) 
				{
					Element e1 = c1[j] as Element;
					Element e2 = c2[j] as Element;
					if (e1.j != e2.j) return false;
					if (e1.value != e2.value) return false;
				}
			}
			return true;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() + this.NumOfElements();
		}
		public bool CheckElements() 
		{
			foreach (List<Element> r in rows)
				foreach (Element e in r)
					if (Double.IsInfinity(e.value) ||
						Double.IsNaN(e.value) ||
						e.value == 0.0)
						return false;
			return true;
		}
		public bool IsSymmetric()
		{
			if (m != n) return false;

			for (int i = 0; i < m; i++)
			{
				List<Element> row = GetRow(i);
				List<Element> col = GetColumn(i);

				if (row.Count != col.Count) 
					return false;

				for (int j = 0; j < row.Count; j++)
				{
					SparseMatrix.Element e1 = row[j] as SparseMatrix.Element;
					SparseMatrix.Element e2 = col[j] as SparseMatrix.Element;
					if (e1.i != e2.j) 
						return false;
					if (e1.j != e2.i) 
						return false;
					//if (e1.value != e2.value) return false;
				}
			}

			return true;

		}
		public Element AddElement(int i, int j, double value) 
		{
			List<Element> r = rows[i] as List<Element>;
			List<Element> c = columns[j] as List<Element>;
			Element e = new Element(i, j, value);
			r.Add(e);
			c.Add(e);
			return e;
		}
		public Element AddElement(Element e) 
		{
			List<Element> r = rows[e.i] as List<Element>;
			List<Element> c = columns[e.j] as List<Element>;
			r.Add(e);
			c.Add(e);
			return e;
		}
		public Element FindElement(int i, int j) 
		{
			List<Element> rr = rows[i] as List<Element>;
			foreach (Element e in rr)
				if (e.j == j) return e;
			return null;
		}
		public Element AddElementIfNotExist(int i, int j, double value)
		{
			Element e = FindElement(i, j);
			if (e == null) 
				return AddElement(i, j, value);
			else
				return null;
		}
		public Element AddValueTo(int i, int j, double value)
		{
			Element e = FindElement(i, j);
			if (e == null) 
			{
				e = new Element(i, j, 0);
				AddElement(e);
			}

			e.value += value;
			return e;
		}
		public Element SetValueTo(int i, int j, double value)
		{
			Element e = FindElement(i, j);
			if (e == null)
			{
				e = new Element(i, j, 0);
				AddElement(e);
			}

			e.value = value;
			return e;
		}
		public Element MulValueTo(int i, int j, double value)
		{
			Element e = FindElement(i, j);
			if (e == null)
			{
				e = new Element(i, j, 0);
				AddElement(e);
			}

			e.value *= value;
			return e;
		}
		public void SortElement() 
		{
			RowComparer rComparer = new RowComparer();
			ColumnComparer cComparer = new ColumnComparer();
			foreach (List<Element> r in rows) r.Sort(rComparer);
			foreach (List<Element> c in columns) c.Sort(cComparer);
		}

		public void AddRow() 
		{
			rows.Add(new List<Element>());
			++m;
		}
		public void AddCol() 
		{
			columns.Add(new List<Element>());
			++n;
		}
		public void RemoveRow()
		{
			rows.RemoveAt(rows.Count - 1);
			--m;
		}
		public void RemoveCol()
		{
			columns.RemoveAt(columns.Count - 1);
			--n;
		}
		public void Multiply(double[] xIn, double[] xOut) 
		{
			if (xIn.Length<n || xOut.Length<m) throw new ArgumentException();

			for (int i=0; i<m; i++) 
			{
				List<Element> r = rows[i] as List<Element>;
				double sum = 0.0;
				foreach (Element e in r) sum += e.value * xIn[e.j];
				xOut[i] = sum;
			}
		}
		public void Multiply(double[] xIn, int indexIn, double[] xOut, int indexOut) 
		{
			if (xIn.Length-indexIn<n || xOut.Length-indexOut<m) throw new ArgumentException();

			for (int i=0; i<m; i++) 
			{
				List<Element> r = rows[i] as List<Element>;
				double sum = 0.0;
				foreach (Element e in r) sum += e.value * xIn[e.j+indexIn];
				xOut[i+indexOut] = sum;
			}
		}
		public void PreMultiply(double[] xIn, double[] xOut) 
		{
			if (xIn.Length<m || xOut.Length<n) throw new ArgumentException();

			for (int j=0; j<n; j++) 
			{
				List<Element> c = columns[j] as List<Element>;
				double sum = 0.0;
				foreach (Element e in c) sum += e.value * xIn[e.i];
				xOut[j] = sum;
			}
		}
		public void Scale(double s) 
		{
			foreach (List<Element> list in rows)
				foreach (Element e in list)
					e.value *= s;
		}
		// to set all the diagonals to 1, youyi, it does not work.
		public void UnifyMatrix()
		{
			if (rows.Count != columns.Count) throw new ArgumentException();
			for (int i = 0; i < rows.Count; ++i)
			{
				Element diag = FindElement(i, i);
				if (diag == null) throw new ArgumentException();
				double val = Math.Abs(diag.value);
				foreach (Element e in rows[i])
					e.value /= val;
			}
		}
		public SparseMatrix Multiply(SparseMatrix right) 
		{
			if (n != right.m) throw new ArgumentException();

			SparseMatrix ret = new SparseMatrix(m, right.n);

			for (int i=0; i< Rows.Count; i++)
			{
				List<Element> rr = Rows[i] as List<Element>;

				for (int j=0; j<right.Columns.Count; j++)
				{
					List<Element> cc = right.Columns[j] as List<Element>;
					int c1 = 0;
					int c2 = 0;
					double sum = 0;
					bool used = false;

					while (c1 < rr.Count && c2 < cc.Count) 
					{
						Element e1 = rr[c1] as Element;
						Element e2 = cc[c2] as Element;
						if (e1.j < e2.i) { c1++; continue; }
						if (e1.j > e2.i) { c2++; continue; }
						sum += e1.value * e2.value;
						c1++;
						c2++;
						used = true;
					}
					if (used) ret.AddElement(i, j, sum);
				}
			}

			return ret;
		}
		public SparseMatrix Add(SparseMatrix right) 
		{
			if (m != right.m || n != right.n)
				throw new ArgumentException();

			SparseMatrix ret = new SparseMatrix(m, m);

			for (int i=0; i<m; i++) 
			{
				List<Element> r1 = Rows[i] as List<Element>;
				List<Element> r2 = right.Rows[i] as List<Element>;
				int c1 = 0;
				int c2 = 0;
				while (c1 < r1.Count && c2 < r2.Count) 
				{
					Element e1 = r1[c1] as Element;
					Element e2 = r2[c2] as Element;
					if (e1.j < e2.j) {
						c1++; 
						ret.AddElement(i, e1.j, e1.value); 
						continue; 
					}
					if (e1.j > e2.j) 
					{
						c2++; 
						ret.AddElement(i, e2.j, e2.value); 
						continue; 
					}
					ret.AddElement(i, e1.j, e1.value + e2.value);
					c1++;
					c2++;
				}
				while (c1 < r1.Count) 
				{
					Element e = r1[c1] as Element;
					ret.AddElement(e.i, e.j, e.value);
					c1++;
				}
				while (c2 < r2.Count) 
				{
					Element e = r2[c2] as Element;
					ret.AddElement(e.i, e.j, e.value);
					c2++;
				}
			}
			return ret;
		}
		public SparseMatrix Transpose() 
		{
			SparseMatrix ret = new SparseMatrix(this);
			int t = ret.m;
			ret.m = ret.n;
			ret.n = t;
			List<List<Element>> tmp = ret.rows;
			ret.rows = ret.columns;
			ret.columns = tmp;
			foreach (List<Element> r in ret.rows)
				foreach (Element e in r) 
				{
					t = e.i;
					e.i = e.j;
					e.j = t;
				}
			return ret;
		}
		public double[] GetDiagonalPreconditionor() 
		{
			if (m != n) return null;

			double[] ret = new double[n];
			for (int i=0; i<n; i++) 
			{
				Element d = FindElement(i,i);
				if (d==null) ret[i] = 1.0;
				else if (d.value==0.0) ret[i] = 1.0;
				else ret[i] = 1.0/d.value;
			}
			return ret;
		}
		public SparseMatrix ConcatRows(SparseMatrix right) 
		{
			if (this.ColumnSize != right.ColumnSize) throw new ArgumentException();

			SparseMatrix m = new SparseMatrix(this.RowSize + right.RowSize, this.ColumnSize);

			foreach (List<Element> r in this.rows)
				foreach (Element e in r)
					m.AddElement(e.i, e.j, e.value);

			int r_base = this.RowSize;
			foreach (List<Element> r in right.rows)
				foreach (Element e in r)
					m.AddElement(r_base + e.i, e.j, e.value);

			return m;
		}
		public int[][] GetRowIndex()
		{
			int [][] arr = new int[m][];

			for (int i=0; i<m; i++)
			{
				arr[i] = new int[rows[i].Count];
				int j=0;
				foreach (Element e in rows[i])
					arr[i][j++] = e.j;
			}
			return arr;
		}
		public int[][] GetColumnIndex()
		{
			int[][] arr = new int[n][];

			for (int i = 0; i < n; i++)
			{
				arr[i] = new int[columns[i].Count];
				int j = 0;
				foreach (Element e in columns[i])
					arr[i][j++] = e.i;
			}
			return arr;
		}


		public static void ConjugateGradientsMethod
			(SparseMatrix A, double[] b, double[] x, int iter, double tolerance)
		{
			int n = A.ColumnSize;
			int rn = (int)Math.Sqrt(n);
			if (A.RowSize != A.ColumnSize) throw new ArgumentException();
			if (b.Length != n || x.Length != n) throw new ArgumentException();
			double[] r = new double[n];
			double[] d = new double[n];
			double[] q = new double[n];
			double[] t = new double[n];

			int i = 0;						// i<= 0
			A.Multiply(x, r);				// r <= b - Ax
			Subtract(r, b, r);
			Assign(d, r);					// d <= r
			double newError = Dot(r, r);	// newError <= rTr
			double oError = newError;		// oError <= newError
			double oldError;
			//tolerance = tolerance * tolerance * oError;

			// While i<iMax and newError > tolerance^2*oldError do
			while ((i < iter) && (newError > tolerance))
			{
				A.Multiply(d, q);			// q <= Ad
				double alpha =				// alpha <= newError/(dTq)
					newError/Dot(d, q);	
				Scale(t, d, alpha);			// x <= x + aplha*d
				Add(x, x, t);
				if (i%rn == 0)				// If i is divisible by 50
				{
					A.Multiply(x, r);		// r <= b - Ax
					Subtract(r, b, r);
				}
				else 
				{
					Scale(t, q, alpha);		// r <= r - aplha * q
					Subtract(r, r, t);
				}
				oldError = newError;		// oldError <= newError
				newError = Dot(r, r);		// newError = rTr
				if (newError < tolerance)	// prevents roundoff error
				{
					A.Multiply(x, r);
					Subtract(r, b, r);
					newError = Dot(r, r);
				}
				double beta =				// beta = newError/oldError
					newError / oldError;
				Scale(d, d, beta);			// d <= r + beta * d
				Add(d, d, r);
				i++;						// i <= i + 1

				//if (i%100 == 0)
				//	MyDebug.WriteLine(i.ToString() + ": " + newError.ToString());
				//for(int kk=0; kk<n; kk++) MyDebug.Write(" " + x[kk].ToString());
				//MyDebug.WriteLine("");
			}
			//MyDebug.WriteLine(i.ToString() + ": " + newError.ToString());
		}
		public static void ConjugateGradientsMethod2
			(SparseMatrix A, double[] b, double[] x, int iter, double tolerance)
		{
			int n = A.ColumnSize;
			int rn = (int)Math.Sqrt(n);
			if (A.RowSize != A.ColumnSize) throw new ArgumentException();
			if (b.Length != n || x.Length != n) throw new ArgumentException();
			double[] r1 = new double[n];
			double[] r2 = new double[n];
			double[] d1 = new double[n];
			double[] d2 = new double[n];
			double[] q1 = new double[n];
			double[] q2 = new double[n];
			double[] t = new double[n];

			int i = 0;						// i<= 0
			A.Multiply(x, r1);				// r1 <= b - Ax
			Subtract(r1, b, r1);
			Assign(r2, r1);					// r2 <= r1
			Assign(d1, r1);					// d1 <= r1
			Assign(d2, r2);					// d2 <= r2
			double newError = Dot(r2, r1);	// newError <= rTr
			double oError = newError;		// oError <= newError
			double oldError;
			//tolerance = tolerance * tolerance * oError;

			// While i<iMax and newError > tolerance^2*oldError do
			while ((i < iter) && (newError > tolerance))
			{
				A.Multiply(d1, q1);			// q1 <= Ad1
				A.PreMultiply(d2, q2);		// q2 <= d2A
				double alpha =				// alpha <= newError/(d2Tq1)
					newError/Dot(d2, q1);	
				Scale(t, d1, alpha);			// x <= x + aplha*d1
				Add(x, x, t);
				if (i%rn == 0)				// If i is divisible by 50
				{
					A.Multiply(x, r1);		// r <= b - Ax
					Subtract(r1, b, r1);
					Assign(r2, r1);
				}
				else 
				{
					Scale(t, q1, alpha);		// r <= r - aplha * q1
					Subtract(r1, r1, t);
					Scale(t, q2, alpha);
					Subtract(r2, r2, t);
				}
				oldError = newError;		// oldError <= newError
				newError = Dot(r2, r1);		// newError = rTr
				if (newError < tolerance)	// prevents roundoff error
				{
					A.Multiply(x, r1);
					Subtract(r1, b, r1);
					Assign(r2, r1);
					newError = Dot(r2, r1);
				}
				double beta =				// beta = newError/oldError
					newError / oldError;
				Scale(d1, d1, beta);		// d1 <= r1 + beta * d1
				Add(d1, d1, r1);
				Scale(d2, d2, beta);		// d2 <= r2 + beta * d2
				Add(d2, d2, r2);
				i++;						// i <= i + 1

				//if (i%100 == 0)
				//	MyDebug.WriteLine(i.ToString() + ": " + newError.ToString());
				//for(int kk=0; kk<n; kk++) MyDebug.Write(" " + x[kk].ToString());
				//MyDebug.WriteLine("");
			}
			//MyDebug.WriteLine(i.ToString() + ": " + newError.ToString());
		}
		public static void ConjugateGradientsMethod3
			(SparseMatrix A, double[] b, double[] x, bool[] boundary, int iter, double tolerance)
		{
			int n = A.ColumnSize;
			int rn = (int)Math.Sqrt(n);
			if (A.RowSize != A.ColumnSize) throw new ArgumentException();
			if (b.Length != n || x.Length != n) throw new ArgumentException();
			double[] r = new double[n];
			double[] d = new double[n];
			double[] q = new double[n];
			double[] t = new double[n];

			int i = 0;						// i<= 0
			A.Multiply(x, r);				// r <= b - Ax
			Subtract(r, b, r);
			Assign(d, r);					// d <= r
			double newError = Dot(r, r);	// newError <= rTr
			double oError = newError;		// oError <= newError
			double oldError;
			//tolerance = tolerance * tolerance * oError;

			// While i<iMax and newError > tolerance^2*oldError do
			while ((i < iter) && (newError > tolerance))
			{
				A.Multiply(d, q);			// q <= Ad
				double alpha =				// alpha <= newError/(dTq)
					newError/Dot(d, q);	
				Scale(t, d, alpha);			// x <= x + aplha*d (if x is not boundary)
				Add(x, x, t);

				if (i%rn == 0)				// If i is divisible by 50
				{
					A.Multiply(x, t);
					//for (int kk=0; kk<n; kk++)
					//	if (boundary[kk] == false)
					//		b[kk] = t[kk];
					Subtract(r, b, t);		// r <= b - Ax
				}
				else 
				{
					Scale(t, q, alpha);		// r <= r - aplha * q
					Subtract(r, r, t);
				}
				oldError = newError;		// oldError <= newError
				newError = Dot(r, r);		// newError = rTr
				if (newError < tolerance)	// prevents roundoff error
				{
					A.Multiply(x, r);
					Subtract(r, b, r);
					newError = Dot(r, r);
				}
				double beta =				// beta = newError/oldError
					newError / oldError;
				Scale(d, d, beta);			// d <= r + beta * d
				Add(d, d, r);
				i++;						// i <= i + 1

				//if (i%100 == 0)
				//	MyDebug.WriteLine(i.ToString() + ": " + newError.ToString());
				//for(int kk=0; kk<n; kk++) MyDebug.Write(" " + x[kk].ToString());
				//MyDebug.WriteLine("");
			}
			//MyDebug.WriteLine(i.ToString() + ": " + newError.ToString());
		}
		public static void JacobiMethod
			(SparseMatrix A, double[] b, double[] x, int iter, double tolerance) 
		{
			int n = A.ColumnSize;
			if (A.ColumnSize != A.RowSize) throw new ArgumentException();
			if (b.Length != n || x.Length != n) throw new ArgumentException();
			double [] r = new double[n];
			double [] d = new double[n];
			double [] t = new double[n];
			double error;

			for (int i=0; i<n; i++) 
			{
				Element e = A.FindElement(i, i);
				if (e != null) d[i] = (3.0/4.0) / e.value;
				else d[i] = 0;
			}

			A.Multiply(x, t);
			Subtract(r, b, t);
			error = Dot(r, r);
			int count = 0;
			while (count<iter && (error>tolerance)) 
			{
				for (int i=0; i<n; i++)
					t[i] = r[i] * d[i];
				Add(x, x, t);
				A.Multiply(x, t);
				Subtract(r, b, t);
				error = Dot(r, r);
				count++;
				//if (count % 100 == 0)
				//	MyDebug.WriteLine(count.ToString() + ": " + error.ToString());
			}
			//MyDebug.WriteLine(count.ToString() + ": " + error.ToString());
		}
		// u <= v;
		private static void Assign(double[] u, double[] v) 
		{
			if (u.Length != v.Length) throw new ArgumentException();
			for (int i=0; i<u.Length; i++)
				u[i] = v[i];
		}
		// w = u-v
		private static void Subtract(double[] w, double[] u, double[] v) 
		{
			if (u.Length != v.Length || v.Length != w.Length)
				throw new ArgumentException();

			for (int i=0; i<u.Length; i++)
				w[i] = u[i] - v[i];
		}
		// w = u+v
		private static void Add(double[] w, double[] u, double[] v) 
		{
			if (u.Length != v.Length || v.Length != w.Length)
				throw new ArgumentException();

			for (int i=0; i<u.Length; i++)
				w[i] = u[i] + v[i];
		}
		private static void Scale(double[] w, double[] u, double s) 
		{
			if (u.Length != w.Length) throw new ArgumentException();
			for (int i=0; i< u.Length; i++)
				w[i] = u[i] * s;
		}
		private static double Dot(double[] u, double[] v) 
		{
			if (u.Length != v.Length) throw new ArgumentException();
			double sum = 0.0;
			for(int i=0; i<u.Length; i++)
				sum += u[i] * v[i];
			return sum;
		}
	}

	public class CCSMatrix 
	{
		private int m;
		private int n;
		private int [] rowIndex;
		private int [] colIndex;
		private double [] values;

		public int RowSize 
		{
			get { return m; }
		}
		public int ColumnSize 
		{
			get { return n; }
		}
		public int [] RowIndex 
		{
			get { return rowIndex; }
			set { rowIndex = value; }
		}
		public int [] ColIndex
		{
			get { return colIndex; }
		}
		public double [] Values 
		{
			get { return values; }
			set { values = value; }
		}
		public int NumNonZero
		{
			get { return values.Length;  }
		}

		public CCSMatrix (SparseMatrix matrix) 
		{
			// get number of non-zero elements
			m = matrix.RowSize;
			n = matrix.ColumnSize;
			int nnz = 0;
			foreach (List<SparseMatrix.Element> col in matrix.Columns) nnz += col.Count;

			// create temp arrays
			rowIndex = new int[nnz];
			colIndex = new int[n+1];
			values = new double[nnz];

			// copy values to arrays
			int index = 0;
			int index2 = 0;
			colIndex[0] = 0;
			foreach (List<SparseMatrix.Element> col in matrix.Columns) 
			{
				foreach (SparseMatrix.Element e in col) 
				{
					rowIndex[index] = e.i;
					values[index] = e.value;
					index++;
				}
				colIndex[++index2] = index;
			}
		}
		public CCSMatrix (SparseMatrix matrix, bool transponse) 
		{
			// get number of non-zero elements
			m = matrix.ColumnSize;
			n = matrix.RowSize;
			int nnz = 0;
			foreach (List<SparseMatrix.Element> col in matrix.Columns) nnz += col.Count;

			// create temp arrays
			rowIndex = new int[nnz];
			colIndex = new int[n+1];
			values = new double[nnz];

			// copy values to arrays
			int index = 0;
			int index2 = 0;
			colIndex[0] = 0;
			foreach (List<SparseMatrix.Element> row in matrix.Rows) 
			{
				foreach (SparseMatrix.Element e in row) 
				{
					rowIndex[index] = e.j;
					values[index] = e.value;
					index++;
				}
				colIndex[++index2] = index;
			}
		}
		public CCSMatrix (double[,] matrix)
		{
			// get number of non-zero elements
			m = matrix.GetLength(0);
			n = matrix.GetLength(1);
			int nnz = 0;
			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					if (matrix[i, j] != 0.0) nnz++;

			// create temp arrays
			rowIndex = new int[nnz];
			colIndex = new int[n + 1];
			values = new double[nnz];

			// copy values to arrays
			int index = 0;
			int index2 = 0;
			colIndex[0] = 0;
			for (int j = 0; j < n; j++)
			{
				for (int i = 0; i < n; i++)
					if (matrix[i, j] != 0)
					{
						rowIndex[index] = i;
						values[index] = matrix[i, j];
						index++;
					}
				colIndex[++index2] = index;
			}
		}
		public CCSMatrix (int m, int n)
		{
			this.m = m;
			this.n = n;
			this.colIndex = new int[n + 1];
			this.colIndex[0] = 0;
		}

		public CCSMatrix FastMultiply(CCSMatrix B)
		{
			CCSMatrix A = this;
			//if (A.n != B.m) throw new ArgumentException();

			CCSMatrix C = new CCSMatrix(A.m, B.n);
			Set<int> tmpIndex = new Set<int>();
			double[] tmp = new double[A.m];
			List<int> C_RowIndex = new List<int>();
			List<double> C_Value = new List<double>();

			for (int i = 0; i < A.m; i++) tmp[i] = 0;

			for (int j = 0; j < B.n; j++) // for each col in B
			{
				for (int col = B.colIndex[j]; col < B.colIndex[j + 1]; col++)
				{
					int k = B.rowIndex[col];
					double valB = B.values[col];

					if (k < A.ColumnSize)
						for (int col2 = A.colIndex[k]; col2 < A.colIndex[k + 1]; col2++)
						{
							int k2 = A.rowIndex[col2];
							double valA = A.values[col2];
							tmpIndex.Add(k2);
							tmp[k2] += valA * valB;
						}
				}

				int[] t = tmpIndex.ToArray();
				int count = 0;
				Array.Sort(t);
				foreach (int k in t)
				{
					if (tmp[k] == 0) continue;
					C_RowIndex.Add(k);
					C_Value.Add(tmp[k]);
					tmp[k] = 0;
					count++;
				}
				C.colIndex[j + 1] = C.colIndex[j] + count;
				tmpIndex.Clear();
			}

			C.rowIndex = C_RowIndex.ToArray(); C_RowIndex = null;
			C.values = C_Value.ToArray(); C_Value = null;
			return C;
		}
		public CCSMatrix Transpose()
		{
			CCSMatrix C = new CCSMatrix(n, m);

			int[] rowCount = new int[m];
			for (int i = 0; i < rowCount.Length; i++) 
				rowCount[i] = 0;
			for (int i = 0; i < this.rowIndex.Length; i++) 
				rowCount[this.rowIndex[i]]++;

			C.ColIndex[0] = 0;
			for (int i = 0; i < m; i++)
			{
				C.ColIndex[i + 1] = C.ColIndex[i] + rowCount[i];
				rowCount[i] = C.ColIndex[i];
			}

			C.values = new double[this.NumNonZero];
			C.rowIndex = new int[this.NumNonZero];
			for (int i = 0; i < n; i++)
			{
				for (int j=colIndex[i]; j<colIndex[i+1]; j++)
				{
					int k = rowIndex[j];
					C.values[rowCount[k]] = values[j];
					C.rowIndex[rowCount[k]] = i;
					rowCount[k]++;
				}
			}

			return C;
		}

		public void Multiply(double[] xIn, double[] xOut) 
		{
			if (xIn.Length<n || xOut.Length<m) throw new ArgumentException();

			for (int i=0; i<m; i++) xOut[i] = 0;

			for (int i=0; i<n; i++) 
			{
				for (int j=colIndex[i]; j<colIndex[i+1]; j++) 
				{
					int r = rowIndex[j];
					xOut[r] += values[j] * xIn[i];
				}
			}
		}
		public void PreMultiply(double[] xIn, double[] xOut)
		{
			if (xIn.Length < m || xOut.Length < n) throw new ArgumentException();

			for (int i = 0; i < n; i++) xOut[i] = 0;

			for (int i = 0; i < n; i++)
			{
				double sum = 0.0;
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
				{
					int r = rowIndex[j];
					sum += values[j] * xIn[r];
				}
				xOut[i] = sum;
			}
		}
		public void PreMultiply(double[] xIn, double[] xOut, int startIndex, bool init)
		{
			if (xIn.Length < m || xOut.Length < n) throw new ArgumentException();

			if (init)
				for (int i = 0; i < n; i++) xOut[i] = 0;

			for (int i = 0; i < n; i++)
			{
				double sum = 0.0;
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
				{
					int r = rowIndex[j];
					sum += values[j] * xIn[r + startIndex];
				}
				xOut[i] += sum;
			}
		}
		public void PreMultiply(double[] xIn, double[] xOut, int inStart, int outStart, bool init)
		{
			if (xIn.Length < m || xOut.Length < n) throw new ArgumentException();

			if (init)
				for (int i = 0; i < n; i++) xOut[i + outStart] = 0;

			for (int i = 0; i < n; i++)
			{
				double sum = 0.0;
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
				{
					int r = rowIndex[j];
					sum += values[j] * xIn[r + inStart];
				}
				xOut[i + outStart] += sum;
			}
		}
		public void PreMultiply(double[] xIn, double[] xOut, int[] index)
		{
			if (xIn.Length < m || xOut.Length < n) throw new ArgumentException();

			foreach (int i in index) xOut[i] = 0;

			foreach (int i in index)
			{
				double sum = 0.0;
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
				{
					int r = rowIndex[j];
					sum += values[j] * xIn[r];
				}
				xOut[i] = sum;
			}
		}
		public void PreMultiplyOffset(double[] xIn, double[] xOut, int startOut, int offsetOut)
		{
			for (int i = startOut; i < n + offsetOut; i += offsetOut) 
				xOut[i] = 0;

			for (int i = 0, k = startOut; i < n; i++, k += offsetOut)
			{
				double sum = 0.0;
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
				{
					int r = rowIndex[j];
					sum += values[j] * xIn[r];
				}
				xOut[k] = sum;
			}
		}
		public CCSMatrix MultiplyATA()
		{
			int[] last = new int[this.RowSize];
			int[] next = new int[this.NumNonZero];
			int[] colIndex = new int[this.NumNonZero];
			for (int i = 0; i < last.Length; i++) last[i] = -1;
			for (int i = 0; i < next.Length; i++) next[i] = -1;
			for (int i = 0; i < this.ColumnSize; i++)
			{
				for (int j = this.ColIndex[i]; j < this.ColIndex[i + 1]; j++)
				{
					int k = this.RowIndex[j];
					if (last[k] != -1) next[last[k]] = j;
					last[k] = j;
					colIndex[j] = i;
				}
			}
			last = null;

			CCSMatrix ATA = new CCSMatrix(this.ColumnSize, this.ColumnSize);
			Set<int> set = new Set<int>();
			double[] tmp = new double[this.ColumnSize];
			List<int> ATA_RowIndex = new List<int>();
			List<double> ATA_Value = new List<double>();

			for (int i = 0; i < this.ColumnSize; i++) tmp[i] = 0;

			for (int j = 0; j < this.ColumnSize; j++)
			{
				for (int col = this.ColIndex[j]; col < this.ColIndex[j + 1]; col++)
				{
					int k = this.RowIndex[col];
					double val = this.Values[col];

					int curr = col;
					while (true)
					{
						int i = colIndex[curr];
						set.Add(i);
						tmp[i] += val * this.Values[curr];
						if (next[curr] != -1)
							curr = next[curr];
						else
							break;
					}
				}

				int[] s = set.ToArray(); Array.Sort(s);
				int count = 0;
				foreach (int k in s)
				{
					if (tmp[k] == 0) continue;
					ATA_RowIndex.Add(k);
					ATA_Value.Add(tmp[k]);
					tmp[k] = 0;
					count++;
				}
				ATA.ColIndex[j + 1] = ATA.ColIndex[j] + count;
				set.Clear();
			}

			ATA.RowIndex = ATA_RowIndex.ToArray(); ATA_RowIndex = null;
			ATA.Values = ATA_Value.ToArray(); ATA_Value = null;
			return ATA;
		}
		public bool Check(CCSMatrix B)
		{
			CCSMatrix A = this;
			if (A.rowIndex.Length != B.rowIndex.Length) throw new Exception();
			if (A.colIndex.Length != B.colIndex.Length) throw new Exception();
			if (A.values.Length != B.values.Length) throw new Exception();

			for (int i = 0; i < rowIndex.Length; i++)
				if (A.rowIndex[i] != B.rowIndex[i]) throw new Exception();
			for (int i = 0; i < colIndex.Length; i++)
				if (A.colIndex[i] != B.colIndex[i]) throw new Exception();
			for (int i = 0; i < values.Length; i++)
				if (A.values[i] != B.values[i]) throw new Exception();

			return true;
		}

		public void CG(double[] x, double[] b, double eps, int maxIter)
		{
			double[] inv = new double[m];
			double[] r = new double[m];
			double[] d = new double[m];
			double[] q = new double[m];
			double[] s = new double[m];
			double errNew = 0;
			double err = 0;
			double errOld = 0;

			for (int i=0; i<n; i++)
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
				{
					int row = rowIndex[j];
					if (i == row)
						inv[i] = 1.0 / values[j];
				}

			int iter = 0;
			PreMultiply(x, r);
			for (int i = 0; i < m; i++) r[i] = b[i] - r[i];
			for (int i = 0; i < m; i++) d[i] = inv[i] * r[i];
			for (int i = 0; i < m; i++) errNew += r[i] * d[i];
			err = errNew;

			while (iter < maxIter && errNew > eps * err)
			{
				PreMultiply(d, q);
				double alpha = 0;
				for (int i = 0; i < m; i++) alpha += d[i] * q[i];
				alpha = errNew / alpha;

				for (int i = 0; i < m; i++) x[i] += alpha * d[i];

				if (iter % 500 == 0)
				{
					PreMultiply(x, r);
					for (int i = 0; i < m; i++) r[i] = b[i] - r[i];
				}
				else
				{
					for (int i = 0; i < m; i++) r[i] -= alpha * q[i];
				}

				for (int i = 0; i < m; i++) s[i] = inv[i] * r[i];

				errOld = errNew;
				errNew = 0;
				for (int i = 0; i < m; i++) errNew += r[i] * s[i];
				double beta = errNew / errOld;
				for (int i = 0; i < m; i++) d[i] = s[i] + beta * d[i];
				iter++;
			}
			//FormMain.CurrForm.OutputText("iter: " + iter.ToString());
		}
		public void PCG(double[] x, double[] b, double[] inv, double eps, int maxIter)
		{
			double[] r = new double[m];
			double[] d = new double[m];
			double[] q = new double[m];
			double[] s = new double[m];
			double err, new_err, old_err, tmp;

			for (int i = 0; i < m; i++) r[i] = b[i];
			for (int i = 0; i < n; i++)
				for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
					r[rowIndex[j]] -= values[j] * x[i];

			new_err = 0;
			for (int i = 0; i < m; i++)
			{
				d[i] = inv[i] * r[i];
				new_err += d[i] * r[i];
			}
			err = new_err;

			//FormMain.CurrForm.OutputText("start: " + new_err.ToString());
			//FormMain.CurrForm.OutputText("err: " + (eps * eps * err).ToString());

			int iter = 0;
			while (iter < maxIter && new_err > eps * eps * err)
			{
				Multiply(d, q);

				tmp = 0;
				for (int i = 0; i < m; i++) tmp += d[i] * q[i];
				double alpha = new_err / tmp;
				for (int i = 0; i < m; i++) x[i] += alpha * d[i];
				for (int i = 0; i < m; i++) if (double.IsNaN(x[i])) throw new Exception();

				if (iter % 50 == 0)
				{
					for (int i = 0; i < m; i++) r[i] = b[i];
					for (int i = 0; i < n; i++)
						for (int j = colIndex[i]; j < colIndex[i + 1]; j++)
							r[rowIndex[j]] -= values[j] * x[i];
					//FormMain.CurrForm.OutputText(iter.ToString() + ": " + new_err.ToString());
				}
				else
				{
					for (int i = 0; i < m; i++) r[i] -= alpha * q[i];
				}

				for (int i = 0; i < m; i++) s[i] = inv[i] * r[i];

				old_err = new_err;
				new_err = 0;
				for (int i = 0; i < m; i++) new_err += r[i] * s[i];

				double beta = new_err / old_err;

				for (int i = 0; i < m; i++) d[i] = s[i] + beta * d[i];

				iter++;

			}
		}

		public void Write(StreamWriter sw)
		{
			//sw.WriteLine(m + " " + n);
			for (int i=0; i<n; i++)
				for (int j = colIndex[i]; j<colIndex[i+1]; j++)
				{
					
					sw.WriteLine((rowIndex[j]+1) + " " + (i+1) + " " + values[j].ToString());
					if (i != rowIndex[j])
						sw.WriteLine((i + 1) + " " + (rowIndex[j] + 1) + " " + values[j].ToString());
				}
		}
	}
}
