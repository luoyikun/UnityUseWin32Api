using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class PInvoke
{
    static IntPtr ptr;
    public static IntPtr UnityHWnd
    {
        get
        {
            if (ptr == null || ptr == IntPtr.Zero)
            {
                ptr = GetUnityWindow();
            }
            return ptr;
        }
    }

    #region 常量
    //https://docs.microsoft.com/zh-cn/windows/win32/winmsg/window-styles
    public const ulong WS_MAXIMIZEBOX = 0x00010000L; //最大化的按钮禁用
    public const ulong WS_DLGFRAME = 0x00400000L; //不现实边框
    public const ulong WS_SIZEBOX = 0x00040000L; //调大小的边框
    public const ulong WS_BORDER = 0x00800000L; //边框
    public const ulong WS_CAPTION = 0x00C00000L; //标题栏

    // Retreives pointer to WindowProc function.
    public const int GWLP_WNDPROC = -4; //Windows 绘制方法的指针
    public const int WM_SIZING = 0x214;
    public const int WS_POPUP = 0x800000;
    public const int GWL_STYLE = -16;
    //边框参数
    public const uint SWP_SHOWWINDOW = 0x0040;
    public const uint SWP_NOMOVE = 0x0002;
    public const int SW_SHOWMINIMIZED = 2;//(最小化窗口)
    // Name of the Unity window class used to find the window handle.
    public const string UNITY_WND_CLASSNAME = "UnityWndClass";
    #endregion

    #region Win32 API
    // Passes message information to the specified window procedure.
    [DllImport("user32.dll")]
    public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    //获得窗口样式
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hwnd, int nIndex);
    // Retrieves the dimensions of the bounding rectangle of the specified window.
    // The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);
    // Retrieves the coordinates of a window's client area. The client coordinates specify the upper-left
    // and lower-right corners of the client area. Because client coordinates are relative to the upper-left
    // corner of a window's client area, the coordinates of the upper-left corner are (0,0).
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

    // 改变指定窗口的属性 ，该函数还在额外窗口内存中的指定偏移处设置一个值。
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    //设置当前窗口的显示状态
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);

    //设置窗口位置，大小
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    // 通过将每个窗口的句柄依次传递给应用程序定义的回调函数，枚举与线程关联的所有非子窗口。
    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    //检索调用线程的线程标识符。
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
    // 检索指定窗口所属的类的名称。
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


    //窗口拖动
    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);


    #endregion
    #region Static Function
    //最小化窗口
    //具体窗口参数看这     https://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
    public static void SetMinWindows()
    {
        if (!Application.isEditor)
        {
            ShowWindow(UnityHWnd, SW_SHOWMINIMIZED);
        }
        else
        {
            Debug.LogWarning($"{nameof(PInvoke)}:为避免编辑器行为异常， 请打包 exe 后测试！");
        }
    }

    // 应用窗口置顶
    public static void SetTopmost(bool isTopmost)
    {
        if (!Application.isEditor)
        {
            int ptr = isTopmost ? -1 : -2;
            SetWindowPos(UnityHWnd, ptr, 0, 0, 0, 0, 1 | 2 | 64);//0x0040
        }
        else
        {
            Debug.LogWarning($"{nameof(PInvoke)}: 为避免编辑器行为异常，请打包 exe 后测试！");
        }
    }

    //拖动窗口
    public static void DragWindow()
    {
        ReleaseCapture();
        SendMessage(UnityHWnd, 0xA1, 0x02, 0);
        SendMessage(UnityHWnd, 0x0202, 0, 0);
    }

    public static IntPtr GetUnityWindow()
    {
        var unityHWnd = IntPtr.Zero;
        EnumThreadWindows(GetCurrentThreadId(), (hWnd, lParam) =>
        {
            var classText = new StringBuilder(UNITY_WND_CLASSNAME.Length + 1);
            GetClassName(hWnd, classText, classText.Capacity);

            if (classText.ToString() == UNITY_WND_CLASSNAME)
            {
                unityHWnd = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);
        return unityHWnd;
    }

    #endregion
    #region Assistant
    /// <summary>
    /// WinAPI RECT definition.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public override string ToString()
        {
            return $"left = {Left}\nright = {Right}\ntop = {Top}\nbottom = {Bottom}";
        }
    }
    #endregion

    /// <summary>
    /// 自动运行无边框窗口，但是有时在一些工程中会失效
    /// </summary>
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    //public static void HideBar()
    //{
        
    //    if (Application.isEditor) return;
    //    var dwStyles = GetWindowLongPtr(UnityHWnd, GWL_STYLE);
    //    var sty = ((ulong)dwStyles);
    //    sty &= ~(WS_CAPTION | WS_DLGFRAME) & WS_POPUP;
    //    SetWindowLongPtr(UnityHWnd, GWL_STYLE, (IntPtr)sty);

    //}
}