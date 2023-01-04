using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class WinAPI 
{
    //引用windows接口
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwd, int cmdShow);

    [DllImport("user32.dll")]
    public static extern long GetWindowLong(IntPtr hwd, int nIndex);

    [DllImport("user32.dll")]
    public static extern void SetWindowLong(IntPtr hwd, int nIndex, long dwNewLong);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, StringBuilder lpIconPath,
    out ushort lpiIcon);


    /// <summary>
    /// 最小化
    /// </summary>
    const int SW_SHOWMINIMIZED = 2;

    /// <summary>
    /// 最大化
    /// </summary>
    const int SW_SHOWMAXIMIZED = 3;

    /// <summary>
    /// 还原
    /// </summary>
    const int SW_SHOWRESTORE = 1;

    /// <summary>
    /// 窗口风格
    /// </summary>
    const int GWL_STYLE = -16;
    /// <summary>
    /// 标题栏
    /// </summary>
    const int WS_CAPTION = 0x00c00000;
    /// <summary>
    /// 标题栏按钮
    /// </summary>
    const int WS_SYSMENU = 0x00080000;

    public static void HideBar()
    {
        var hwd = GetForegroundWindow();
        var wl = GetWindowLong(hwd, GWL_STYLE);
        wl &= ~WS_CAPTION;
        SetWindowLong(hwd, GWL_STYLE, wl);
    }

    public static void ShowBar()
    {
        var hwd = GetForegroundWindow();
        var wl = GetWindowLong(hwd, GWL_STYLE);
        wl |= WS_CAPTION;
        SetWindowLong(hwd, GWL_STYLE, wl);
    }

    public static void Minimize()
    {
        ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
    }


    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] FileName ofn);
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] FileName ofd);
    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SHBrowseForFolder([In, Out] DirName ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

    //打开某个文件
    public static string Load(params string[] ext)
    {
        FileName i = new FileName(ext);
        i.title = "打开";
        GetOpenFileName(i);
        return i.file;
    }

    //文件另存为
    public static string Save(string ext)
    {
        FileName i = new FileName(ext);
        i.title = "保存";
        GetSaveFileName(i);
        return i.file;
    }

    //文件夹浏览框
    public static string GetDir()
    {
        DirName d = new DirName();
        IntPtr i = SHBrowseForFolder(d);
        char[] c = new char[256];
        SHGetPathFromIDList(i, c);
        return new string(c);
    }

    public static string OpenFile()
    {
        FileOpenDialog dialog = new FileOpenDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = "exe files\0*.exe\0All Files\0*.*\0\0";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径

        dialog.title = "Open File Dialog";

        dialog.defExt = "exe";//显示文件的类型
        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog))
        {
            UnityEngine.Debug.Log(dialog.file);
            return dialog.file;
        }
        return "";
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class FileOpenDialog
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    public class DialogShow
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out]FileOpenDialog dialog);  //这个方法名称必须为GetOpenFileName
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class DirName
    {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = null;
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
        public DirName()
        {
            pszDisplayName = new string(new char[256]);
            ulFlags = 0x00000040 | 0x00000010; //BIF_NEWDIALOGSTYLE | BIF_EDITBOX;
            lpszTitle = "";
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class FileName
    {
        public int structSize = 0;
        private IntPtr dlgOwner = IntPtr.Zero;
        private IntPtr instance = IntPtr.Zero;
        private string filter = null;
        private string customFilter = null;
        private int maxCustFilter = 0;
        private int filterIndex = 0;
        public string file { get; set; }
        private int maxFile = 0;
        public string fileTitle { get; set; }
        private int maxFileTitle = 0;
        public string initialDir { get; set; }
        public string title { get; set; }
        private int flags = 0;
        private short fileOffset = 0;
        private short fileExtension = 0;
        private string defExt = null;
        private IntPtr custData = IntPtr.Zero;
        private IntPtr hook = IntPtr.Zero;
        private string templateName = null;
        private IntPtr reservedPtr = IntPtr.Zero;
        private int reservedInt = 0;
        private int flagsEx = 0;
        public FileName(params string[] ext)
        {
            structSize = Marshal.SizeOf(this);
            defExt = ext[0];
            string n = null;
            string e = null;
            foreach (string _e in ext)
            {
                if (_e == "*")
                {
                    n += "All Files";
                    e += "*.*;";
                }
                else
                {
                    string _n = "." + _e + ";";
                    n += _n;
                    e += "*" + _n;
                }
            }
            n = n.Substring(0, n.Length - 1);
            filter = n + "\0" + e + "\0";
            file = new string(new char[256]);
            maxFile = file.Length;
            fileTitle = new string(new char[64]);
            maxFileTitle = fileTitle.Length;
            //flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
            flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
            initialDir = Application.dataPath;
        }


        

    }

    // 浏览对话框中包含一个编辑框，在该编辑框中用户可以输入选中项的名字。
    const int BIF_EDITBOX = 0x00000010;
    // 新用户界面
    const int BIF_NEWDIALOGSTYLE = 0x00000040;
    const int BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
    const int MAX_PATH_LENGTH = 2048;

    public static string FolderBrowserDlg(string defaultPath = "")
    {
        OpenDlgDir dlg = new OpenDlgDir();
        dlg.pszDisplayName = defaultPath;
        dlg.ulFlags = BIF_USENEWUI;
        //设置hwndOwner==0时，是非模态对话框，设置hwndOwner!=0时为模态对话框
        dlg.hwndOwner = DllOpenFileDialog.GetForegroundWindow();

        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder(dlg);
        char[] charArray = new char[MAX_PATH_LENGTH];
        DllOpenFileDialog.SHGetPathFromIDList(pidlPtr, charArray);
        string foldPath = new String(charArray);
        foldPath = foldPath.Substring(0, foldPath.IndexOf('\0'));
        return foldPath;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenDlgDir
    {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = null;
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }

    public class DllOpenFileDialog
    {
        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder([In, Out] OpenDlgDir odd);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

        /// <summary>
        /// 获取当前窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
    }

    //获取硬盘剩余大小
    public static long GetFreeSpace(string pan)
    {
        DriveInfo[] driveInfos = DriveInfo.GetDrives();
        foreach (DriveInfo d in driveInfos)
        {
            if (d.Name.StartsWith(pan))
            {
                return d.TotalFreeSpace;
            }
           
        }
        return 0;
    }


    public static void StartProcessPop(string exeFilePath)
    {
        Process myNewProcess = new Process();
        myNewProcess.StartInfo.FileName = exeFilePath; //设置要启动的程序

        //传参数，启动器进程一开始就获取这个参数，如果参数不匹配，认为是非法打开
        myNewProcess.StartInfo.Arguments = "-popupwindow";


        myNewProcess.StartInfo.UseShellExecute = true;
        myNewProcess.StartInfo.Verb = "runas";
        myNewProcess.Start(); //准备重启程序
    }
    public static Process StartProcess(string fileName, string args = "")
    {
        try
        {
            fileName = "\"" + fileName + "\"";
            args = "\"" + args + "\"";
            Process myProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args);
            //startInfo.CreateNoWindow = true;
            //startInfo.RedirectStandardInput = true;
            //startInfo.UseShellExecute = false;
            //startInfo.RedirectStandardOutput = true;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo = startInfo;
            myProcess.Start();
            UnityEngine.Debug.Log("启动外部程序：" + fileName + "args:" + args);
            return myProcess;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("出错原因：" + ex.Message);
        }
        return null;
    }


    const string RegistXTLBBLauncher = @"Software\\CYOU\\XTLBBLauncher";
    public static string GetRegistData(string name)
    {
        string registData = "";

        RegistryKey reg = Registry.CurrentUser.OpenSubKey(RegistXTLBBLauncher, true);

        if (reg == null)
        {
            return "";
        }

        if (IsRegistExistKey(reg, name))
        {
            registData = reg.GetValue(name).ToString();
        }

        reg.Close();
        return registData;
        
    }
    public static void WriteRegistData(string name, string tovalue)
    {
        RegistryKey reg = Registry.CurrentUser.OpenSubKey(RegistXTLBBLauncher, true);
        if (reg == null)
        {
            reg = Registry.CurrentUser.CreateSubKey(RegistXTLBBLauncher);
        }

        reg.SetValue(name, tovalue);

        reg.Close();
    }

    public static bool IsRegistExistKey(RegistryKey key,string name)
    {
        if (key == null)
        {
            return false;
        }
        string[]  subkeyNames = key.GetValueNames();
        foreach (string keyName in subkeyNames)
        {
            if (keyName == name)
            {
                return true;
            }
        }
        return false;

    }

    const string RegistRun = @"Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    const string RegistWin32ApiExe = "Win32ApiExe";
    
    public static string GetExePath
    {
        get {
            string saPath = Application.streamingAssetsPath;
            string[] bufSA = saPath.Split('/');
            string end = bufSA[bufSA.Length - 2] + "/" + bufSA[bufSA.Length - 1];
            string updaterPath = saPath.Replace(end, "Win32Api.exe");
            return updaterPath;
        }
    }

    public static string GetExeOnlyName
    {
        get {
            return "Win32Api";
        }
    }
    public static void SetStartWithWindows()
    {
        RegistryKey reg = Registry.CurrentUser.OpenSubKey(RegistRun, true);
        if (reg == null)
        {
            reg = Registry.CurrentUser.CreateSubKey(RegistRun);
        }

        reg.SetValue(RegistWin32ApiExe, GetExePath);

    }

    public static bool IsRunWithWindows()
    {
        RegistryKey reg = Registry.CurrentUser.OpenSubKey(RegistRun, true);
        if (reg == null)
        {
            reg = Registry.CurrentUser.CreateSubKey(RegistRun);
        }

        if (reg.GetValue(RegistWin32ApiExe) != null)
        {
            return true;
        }
        return false;
    }


    public static void ClearStartWithWindows()
    {
        RegistryKey reg = Registry.CurrentUser.OpenSubKey(RegistRun, true);
        if (reg == null)
        {
            reg = Registry.CurrentUser.CreateSubKey(RegistRun);
        }

        if (reg.GetValue(RegistWin32ApiExe) != null)
        {
            reg.DeleteValue(RegistWin32ApiExe);
        }
    }
}
