using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*	THESE 4 ARE FOR THE CURSOR:	*/
using System.Windows.Forms;
// For the NativeMethods helper class:
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;

/*	THIS WHOLE MESS IS FOR THE CUSTOM CURSOR:	*/
namespace isometric_mono
{
	// Thanks Hans Passant!
	// http://stackoverflow.com/questions/4305800/using-custom-colored-cursors-in-a-c-windows-application
	static class NativeMethods
	{
		public static Cursor LoadCustomCursor(string path)
		{
			IntPtr hCurs = LoadCursorFromFile(path);
			if (hCurs == IntPtr.Zero) throw new Win32Exception();
			var curs = new Cursor(hCurs);
			// Note: force the cursor to own the handle so it gets released properly
			var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
			fi.SetValue(curs, true);
			return curs;
		}
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr LoadCursorFromFile(string path);
	}
}
