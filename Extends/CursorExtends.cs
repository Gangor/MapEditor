using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MapEditor.Extends
{
	public sealed class CursorExtends
	{
		[DllImport("User32.dll")]
		private static extern IntPtr LoadCursorFromFile(string str);

		public static Cursor Cross
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "cross.cur"));
			}
		}

		public static Cursor Forbidden
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "forbidden.cur"));
			}
		}

		public static Cursor Help
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "cross.cur"));
			}
		}

		public static Cursor LeftPtr
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "left_ptr.cur"));
			}
		}

		public static Cursor LeftPtrWatch
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "left_ptr_watch.ani"));
			}
		}

		public static Cursor Pencil
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "pencil.cur"));
			}
		}

		public static Cursor PointingHand
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "pointing_hand.cur"));
			}
		}

		public static Cursor SizeAll
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "size_all.cur"));
			}
		}

		public static Cursor SizeBdiag
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "size_bdiag.cur"));
			}
		}

		public static Cursor SizeFdiag
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "size_fdiag.cur"));
			}
		}

		public static Cursor SizeHor
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "size_hor.cur"));
			}
		}

		public static Cursor SizeVer
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "size_ver.cur"));
			}
		}

		public static Cursor Text
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "Text.cur"));
			}
		}

		public static Cursor UpArrow
		{
			get
			{
				return Create(Path.Combine(Application.StartupPath, "cursor", "up_arrow.cur"));
			}
		}

		public static Cursor Create(string filename)
		{
			IntPtr hCursor = LoadCursorFromFile(filename);

			if (!IntPtr.Zero.Equals(hCursor))
			{
				return new Cursor(hCursor);
			}
			else
			{
				throw new ApplicationException("Could not create cursor from file " + filename);
			}
		}
	}
}
