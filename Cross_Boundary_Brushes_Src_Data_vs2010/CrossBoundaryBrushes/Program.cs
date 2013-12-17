using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MyGeometry;

namespace CrossBoundaryBrushes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

		public enum EnumOperationMode { Viewing, Selection, Moving, Sketching }

		static public EnumOperationMode currentMode = EnumOperationMode.Viewing;
		static public DisplayProperty displayProperty = new DisplayProperty();
		static public ToolsProperty toolsProperty = new ToolsProperty();
		
		[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

		static public void PrintText(string s)
		{
			FormMain f = FormMain.ActiveForm as FormMain;
			if (f != null)
				f.PrintText(s);
		}

		static public void Print3DText(Vector3d pos, string s)
		{
			FormMain f = FormMain.ActiveForm as FormMain;
			if (f != null)
				f.Print3DText(pos, s);
		}
    }
}