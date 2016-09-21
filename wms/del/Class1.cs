using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;


namespace ScreenCap
{
	public sealed class API 
	{
		[DllImport("USER32.DLL", EntryPoint= "PostMessage")]
		public static extern bool PostMessage(IntPtr hwnd, uint msg,
			IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


		[DllImport("gdi32.dll")]
		public static extern int SetBkColor(
			IntPtr hdc, // handle to DC
			int crColor // background color value
			);
		[DllImport("gdi32.dll")]
		public static extern int GetBkColor(IntPtr hdc);

		[DllImport("user32.dll",EntryPoint="GetDC")]
		public static extern IntPtr GetDC(IntPtr ptr);
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
			int x, int y, int cx, int cy, int flags);
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref Point pt, int cPoints);

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern IntPtr GetWindowDC(IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern int ReleaseDC(IntPtr hWnd,IntPtr hDC);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int RegisterWindowMessage(string msg);

		[DllImport("gdi32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern bool BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


		[DllImport("ole32.dll", PreserveSig=false)]
		public static extern IntPtr CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease);
		// public static extern ILockBytes CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease);


		// [DllImport("ole32.dll", PreserveSig=false)]
		// public static extern IntPtr StgCreateDocfileOnILockBytes(IntPtr iLockBytes, STGM grfMode, int reserved);
		// public static extern IStorage StgCreateDocfileOnILockBytes(ILockBytes iLockBytes, STGM grfMode, int reserved);



		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool GetWindowRect( IntPtr hWnd,ref Rectangle lpRect);

		[DllImport("user32.dll")] 
		public static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);


		[DllImport("USER32.DLL")]
		public static extern int SendMessage( IntPtr hwnd, int wMsg,int wParam,IntPtr lParam	); 
		
		[DllImport("USER32.DLL")]
		public static extern int SendMessage( 
			IntPtr hwnd, 
			int wMsg,
			int wParam,
			StringBuilder lParam
			); 
		[DllImport("USER32.DLL")]
		public static extern int SendMessage( 
			IntPtr hwnd, 
			int wMsg,
			int wParam,
			string lParam
			); 
		[DllImport("USER32.DLL")]
		public static extern int SendMessage( IntPtr hwnd, int wMsg,bool wParam,string lParam); 

		[DllImport("USER32.DLL")]
		public static extern int SendMessage( IntPtr hwnd, int wMsg,bool wParam,	int lParam	);
			
			
		
		


		#region

		public const int SRCCOPY =0x00CC0020; /* dest = source */
		public const int SRCPAINT =0x00EE0086; /* dest = source OR dest */
		public const int SRCAND =0x008800C6; /* dest = source AND dest */
		public const int SRCINVERT =0x00660046; /* dest = source XOR dest */
		public const int SRCERASE =0x00440328; /* dest = source AND (NOT dest ) */
		public const int NOTSRCCOPY =0x00330008; /* dest = (NOT source) */
		public const int NOTSRCERASE =0x001100A6; /* dest = (NOT src) AND (NOT dest) */
		public const int MERGECOPY =0x00C000CA; /* dest = (source AND pattern) */
		public const int MERGEPAINT =0x00BB0226; /* dest = (NOT source) OR dest */
		public const int PATCOPY =0x00F00021; /* dest = pattern */
		public const int PATPAINT =0x00FB0A09; /* dest = DPSnoo */
		public const int PATINVERT =0x005A0049; /* dest = pattern XOR dest */
		public const int DSTINVERT =0x00550009; /* dest = (NOT dest) */
		public const int BLACKNESS =0x00000042; /* dest = BLACK */
		public const int WHITENESS =0x00FF0062; /* dest = WHITE */
		static int wm_mouseenter=-1;
		public static int WM_MOUSEENTER
		{
			get
			{
				if (wm_mouseenter == -1)
				{
					wm_mouseenter =RegisterWindowMessage("WinFormsMouseEnter");
				}
				return wm_mouseenter;
			}
		}

	//	public const int EM_GETOLEINTERFACE = WM_USER + 60;
		public const int SC_MOVE =0xF010;
		/*
		* WM_NCHITTEST and MOUSEHOOKSTRUCT Mouse Position Codes
		*/
		public const int HTERROR =(-2);
		public const int HTTRANSPARENT =(-1);
		// public const int HTNOWHERE = 0;
		// public const int HTCLIENT =1;
		public const int HTCAPTION = 2;
		public const int HTSYSMENU = 3;
		public const int HTGROWBOX = 4;
		public const int HTSIZE = HTGROWBOX;
		public const int HTMENU = 5;
		public const int HTHSCROLL = 6;
		public const int HTVSCROLL = 7;
		public const int HTMINBUTTON = 8;
		public const int HTMAXBUTTON =9;
		public const int HTLEFT = 10;
		public const int HTRIGHT =11;
		public const int HTTOP =12;
		public const int HTTOPLEFT = 13;
		public const int HTTOPRIGHT = 14;
		public const int HTBOTTOM = 15;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOMRIGHT = 17;
		public const int HTBORDER = 18;
		public const int HTCLOSE = 20;
		public const int HTHELP = 21;
		public const int ACM_OPENA = 0x464;
		public const int ACM_OPENW = 0x467;
		public const int ADVF_ONLYONCE = 2;
		public const int ADVF_PRIMEFIRST = 4;
		public const int ARW_BOTTOMLEFT = 0;
		public const int ARW_BOTTOMRIGHT = 1;
		public const int ARW_DOWN = 4;
		public const int ARW_HIDE = 8;
		public const int ARW_LEFT = 0;
		public const int ARW_RIGHT = 0;
		public const int ARW_TOPLEFT = 2;
		public const int ARW_TOPRIGHT = 3;
		public const int ARW_UP = 4;
		public const int BDR_RAISED = 5;
		public const int BDR_RAISEDINNER = 4;
		public const int BDR_RAISEDOUTER = 1;
		public const int BDR_SUNKEN = 10;
		public const int BDR_SUNKENINNER = 8;
		public const int BDR_SUNKENOUTER = 2;
		public const int BF_ADJUST = 0x2000;
		public const int BF_BOTTOM = 8;
		public const int BF_FLAT = 0x4000;
		public const int BF_LEFT = 1;
		public const int BF_MIDDLE = 0x800;
		public const int BF_RIGHT = 4;
		public const int BF_TOP = 2;
		public const int BFFM_ENABLEOK = 0x465;
		public const int BFFM_INITIALIZED = 1;
		public const int BFFM_SELCHANGED = 2;
		public const int BFFM_SETSELECTION = 0x467;
		public const int BI_BITFIELDS = 3;
		public const int BI_RGB = 0;
		public const int BITMAPINFO_MAX_COLORSIZE = 0x100;
		public const int BITSPIXEL = 12;
		public const int BM_SETCHECK = 0xf1;
		public const int BM_SETSTATE = 0xf3;
		public const int BN_CLICKED = 0;
		public const int BS_3STATE = 5;
		public const int BS_BOTTOM = 0x800;
		public const int BS_CENTER = 0x300;
		public const int BS_DEFPUSHBUTTON = 1;
		public const int BS_GROUPBOX = 7;
		public const int BS_LEFT = 0x100;
		public const int BS_MULTILINE = 0x2000;
		public const int BS_OWNERDRAW = 11;
		public const int BS_PATTERN = 3;
		public const int BS_PUSHBUTTON = 0;
		public const int BS_PUSHLIKE = 0x1000;
		public const int BS_RADIOBUTTON = 4;
		public const int BS_RIGHT = 0x200;
		public const int BS_RIGHTBUTTON = 0x20;
		public const int BS_TOP = 0x400;
		public const int BS_VCENTER = 0xc00;
		public const int CB_ADDSTRING = 0x143;
		public const int CB_DELETESTRING = 0x144;
		public const int CB_ERR = -1;
		public const int CB_FINDSTRING = 0x14c;
		public const int CB_FINDSTRINGEXACT = 0x158;
		public const int CB_GETCURSEL = 0x147;
		public const int CB_GETDROPPEDSTATE = 0x157;
		public const int CB_GETEDITSEL = 320;
		public const int CB_GETITEMDATA = 0x150;
		public const int CB_GETITEMHEIGHT = 340;
		public const int CB_INSERTSTRING = 330;
		public const int CB_LIMITTEXT = 0x141;
		public const int CB_RESETCONTENT = 0x14b;
		public const int CB_SETCURSEL = 0x14e;
		public const int CB_SETDROPPEDWIDTH = 0x160;
		public const int CB_SETEDITSEL = 0x142;
		public const int CB_SETITEMHEIGHT = 0x153;
		public const int CB_SHOWDROPDOWN = 0x14f;
		public const int CBEM_GETITEMA = 0x404;
		public const int CBEM_GETITEMW = 0x40d;
		public const int CBEM_INSERTITEMA = 0x401;
		public const int CBEM_INSERTITEMW = 0x40b;
		public const int CBEM_SETITEMA = 0x405;
		public const int CBEM_SETITEMW = 0x40c;
		public const int CBEN_ENDEDITA = -805;
		public const int CBEN_ENDEDITW = -806;
		public const int CBN_DBLCLK = 2;
		public const int CBN_DROPDOWN = 7;
		public const int CBN_EDITCHANGE = 5;
		public const int CBN_SELCHANGE = 1;
		public const int CBN_SELENDOK = 9;
		public const int CBS_AUTOHSCROLL = 0x40;
		public const int CBS_DROPDOWN = 2;
		public const int CBS_DROPDOWNLIST = 3;
		public const int CBS_HASSTRINGS = 0x200;
		public const int CBS_NOINTEGRALHEIGHT = 0x400;
		public const int CBS_OWNERDRAWFIXED = 0x10;
		public const int CBS_OWNERDRAWVARIABLE = 0x20;
		public const int CBS_SIMPLE = 1;
		public const int CC_ANYCOLOR = 0x100;
		public const int CC_ENABLEHOOK = 0x10;
		public const int CC_FULLOPEN = 2;
		public const int CC_PREVENTFULLOPEN = 4;
		public const int CC_RGBINIT = 1;
		public const int CC_SHOWHELP = 8;
		public const int CC_SOLIDCOLOR = 0x80;
		public const int CCS_NODIVIDER = 0x40;
		public const int CCS_NOPARENTALIGN = 8;
		public const int CCS_NORESIZE = 4;
		public const int CDDS_ITEM = 0x10000;
		public const int CDDS_ITEMPREPAINT = 0x10001;
		public const int CDDS_POSTPAINT = 2;
		public const int CDDS_PREPAINT = 1;
		public const int CDDS_SUBITEM = 0x20000;
		public const int CDERR_DIALOGFAILURE = 0xffff;
		public const int CDERR_FINDRESFAILURE = 6;
		public const int CDERR_INITIALIZATION = 2;
		public const int CDERR_LOADRESFAILURE = 7;
		public const int CDERR_LOADSTRFAILURE = 5;
		public const int CDERR_LOCKRESFAILURE = 8;
		public const int CDERR_MEMALLOCFAILURE = 9;
		public const int CDERR_MEMLOCKFAILURE = 10;
		public const int CDERR_NOHINSTANCE = 4;
		public const int CDERR_NOHOOK = 11;
		public const int CDERR_NOTEMPLATE = 3;
		public const int CDERR_REGISTERMSGFAIL = 12;
		public const int CDERR_STRUCTSIZE = 1;
		public const int CDIS_CHECKED = 8;
		public const int CDIS_DEFAULT = 0x20;
		public const int CDIS_DISABLED = 4;
		public const int CDIS_FOCUS = 0x10;
		public const int CDIS_GRAYED = 2;
		public const int CDIS_HOT = 0x40;
		public const int CDIS_INDETERMINATE = 0x100;
		public const int CDIS_MARKED = 0x80;
		public const int CDIS_SELECTED = 1;
		public const int CDRF_DODEFAULT = 0;
		public const int CDRF_NEWFONT = 2;
		public const int CDRF_NOTIFYITEMDRAW = 0x20;
		public const int CDRF_NOTIFYPOSTPAINT = 0x10;
		public const int CDRF_NOTIFYSUBITEMDRAW = 0x20;
		public const int CDRF_SKIPDEFAULT = 4;
		public const int CF_APPLY = 0x200;
		public const int CF_BITMAP = 2;
		public const int CF_DIB = 8;
		public const int CF_DIF = 5;
		public const int CF_EFFECTS = 0x100;
		public const int CF_ENABLEHOOK = 8;
		public const int CF_ENHMETAFILE = 14;
		public const int CF_FIXEDPITCHONLY = 0x4000;
		public const int CF_FORCEFONTEXIST = 0x10000;
		public const int CF_HDROP = 15;
		public const int CF_INITTOLOGFONTSTRUCT = 0x40;
		public const int CF_LIMITSIZE = 0x2000;
		public const int CF_LOCALE = 0x10;
		public const int CF_METAFILEPICT = 3;
		public const int CF_NOSIMULATIONS = 0x1000;
		public const int CF_NOVECTORFONTS = 0x800;
		public const int CF_NOVERTFONTS = 0x1000000;
		public const int CF_OEMTEXT = 7;
		public const int CF_PALETTE = 9;
		public const int CF_PENDATA = 10;
		public const int CF_RIFF = 11;
		public const int CF_SCREENFONTS = 1;
		public const int CF_SCRIPTSONLY = 0x400;
		public const int CF_SELECTSCRIPT = 0x400000;
		public const int CF_SHOWHELP = 4;
		public const int CF_SYLK = 4;
		public const int CF_TEXT = 1;
		public const int CF_TIFF = 6;
		public const int CF_TTONLY = 0x40000;
		public const int CF_UNICODETEXT = 13;
		public const int CF_WAVE = 12;
		public const int CFERR_MAXLESSTHANMIN = 0x2002;
		public const int CFERR_NOFONTS = 0x2001;
		public const int CHILDID_SELF = 0;
		public const int CLR_DEFAULT = -16777216;
		public const int CLR_NONE = -1;
		public const int cmb4 = 0x473;
		public const int COLOR_WINDOW = 5;
		public const int CONNECT_E_CANNOTCONNECT = -2147220990;
		public const int CONNECT_E_NOCONNECTION = -2147220992;
		public const int CP_WINANSI = 0x3ec;
		public const int CS_DBLCLKS = 8;
		public const int CSIDL_APPDATA = 0x1a;
		public const int CSIDL_COMMON_APPDATA = 0x23;
		public const int CSIDL_COOKIES = 0x21;
		public const int CSIDL_DESKTOP = 0;
		public const int CSIDL_DESKTOPDIRECTORY = 0x10;
		public const int CSIDL_FAVORITES = 6;
		public const int CSIDL_HISTORY = 0x22;
		public const int CSIDL_INTERNET = 1;
		public const int CSIDL_INTERNET_CACHE = 0x20;
		public const int CSIDL_LOCAL_APPDATA = 0x1c;
		public const int CSIDL_PERSONAL = 5;
		public const int CSIDL_PROGRAM_FILES = 0x26;
		public const int CSIDL_PROGRAM_FILES_COMMON = 0x2b;
		public const int CSIDL_PROGRAMS = 2;
		public const int CSIDL_RECENT = 8;
		public const int CSIDL_SENDTO = 9;
		public const int CSIDL_STARTMENU = 11;
		public const int CSIDL_STARTUP = 7;
		public const int CSIDL_SYSTEM = 0x25;
		public const int CSIDL_TEMPLATES = 0x15;
		public const int CTRLINFO_EATS_ESCAPE = 2;
		public const int CTRLINFO_EATS_RETURN = 1;
		public const int CW_USEDEFAULT = -2147483648;
		public const int CWP_SKIPINVISIBLE = 1;
		public const int DCX_CACHE = 2;
		public const int DCX_LOCKWINDOWUPDATE = 0x400;
		public const int DCX_WINDOW = 1;
		public const int DEFAULT_GUI_FONT = 0x11;
		public const int DFC_BUTTON = 4;
		public const int DFC_CAPTION = 1;
		public const int DFC_MENU = 2;
		public const int DFC_SCROLL = 3;
		public const int DFCS_BUTTON3STATE = 8;
		public const int DFCS_BUTTONCHECK = 0;
		public const int DFCS_BUTTONPUSH = 0x10;
		public const int DFCS_BUTTONRADIO = 4;
		public const int DFCS_CAPTIONCLOSE = 0;
		public const int DFCS_CAPTIONHELP = 4;
		public const int DFCS_CAPTIONMAX = 2;
		public const int DFCS_CAPTIONMIN = 1;
		public const int DFCS_CAPTIONRESTORE = 3;
		public const int DFCS_CHECKED = 0x400;
		public const int DFCS_FLAT = 0x4000;
		public const int DFCS_INACTIVE = 0x100;
		public const int DFCS_MENUARROW = 0;
		public const int DFCS_MENUBULLET = 2;
		public const int DFCS_MENUCHECK = 1;
		public const int DFCS_PUSHED = 0x200;
		public const int DFCS_SCROLLCOMBOBOX = 5;
		public const int DFCS_SCROLLDOWN = 1;
		public const int DFCS_SCROLLLEFT = 2;
		public const int DFCS_SCROLLRIGHT = 3;
		public const int DFCS_SCROLLUP = 0;
		#endregion

	}
}