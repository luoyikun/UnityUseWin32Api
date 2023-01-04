using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MsgBoxBase = System.Windows.Forms.MessageBox;
using WinForms = System.Windows.Forms;
using System.Diagnostics;

public class WinTray
{


    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    private const int SW_HIDE = 0;  //hied task bar
    private const int SW_RESTORE = 9;//show task bar

    private static System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();

    private const int _width = 100, _height = 100;

    private static IntPtr window;


    public static void Hide()//最小化到托盘
    {
        try
        {
            if (File.Exists(Application.streamingAssetsPath + "/icon.png"))
            {
   
                window = GetForegroundWindow();
                ShowWindow(window, SW_HIDE);
                //_notifyIcon.BalloonTipText = "Heroage";//托盘气泡显示内容

                _notifyIcon.Text = "托盘悬浮提示";//鼠标悬浮时显示的内容

                _notifyIcon.Visible = true;//托盘按钮是否可见
                _notifyIcon.Icon = CustomTrayIcon(Application.streamingAssetsPath + "/icon.png", _width, _height);//托盘图标
                //_notifyIcon.ShowBalloonTip(2000);//托盘气泡显示时间

                System.Windows.Forms.MenuItem closeMenu = new System.Windows.Forms.MenuItem("关闭");
                System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { closeMenu };
                _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

                closeMenu.Click += OnMenuClose;
                
                _notifyIcon.MouseClick += notifyIcon_MouseClick;//双击托盘图标响应事件
     
            }
        }
        catch (Exception e)
        {
            //Debug.Log(e.ToString());
            MsgBoxBase.Show(e.ToString(), "异常", WinForms.MessageBoxButtons.OKCancel);
        }
    }

    private static System.Drawing.Icon CustomTrayIcon(string iconPath, int width, int height)
    {
        System.Drawing.Bitmap bt = new System.Drawing.Bitmap(iconPath);

        System.Drawing.Bitmap fitSizeBt = new System.Drawing.Bitmap(bt, width, height);

        return System.Drawing.Icon.FromHandle(fitSizeBt.GetHicon());
    }

    /// <summary>
    /// 后台要关闭进程，按照exe名字来
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnMenuClose(object sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        Application.Quit();

    }


    private static void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)//点击托盘图标
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            _notifyIcon.MouseClick -= notifyIcon_MouseClick;

            _notifyIcon.Visible = false;

            ShowWindow(window, SW_RESTORE);

        }
    }

}
