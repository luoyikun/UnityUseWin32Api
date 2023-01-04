using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestWinApi : MonoBehaviour
{
    public Button m_btnMin;
    public Button m_btnTray;
    public Button m_btnFold;
    public Button m_btnRunWithWindow;
    public Button m_btnCloseRunWithWindow;
    // Start is called before the first frame update
    void Start()
    {
        m_btnMin.onClick.AddListener(()=> {
            WinAPI.Minimize();
        });

        m_btnTray.onClick.AddListener(() =>
        {
            WinTray.Hide();
        });

        m_btnFold.onClick.AddListener(() =>
        {
            string psPath = WinAPI.FolderBrowserDlg();
            Debug.Log(psPath);
        });

        m_btnRunWithWindow.onClick.AddListener(() =>
        {
            WinAPI.SetStartWithWindows();
        });

        m_btnCloseRunWithWindow.onClick.AddListener(() =>
        {
            WinAPI.ClearStartWithWindows();
        });

    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
