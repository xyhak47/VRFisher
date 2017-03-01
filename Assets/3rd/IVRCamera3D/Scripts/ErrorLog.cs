/*
 * @brief: 
 * [Header("错误记录保存在名为：ErrorLog的文件夹中")]
 * 首先初始化：ErrorLog.Instance.init ();
 * @author: Uranus
 * @date: 20160809
 * 
*/
using UnityEngine;
using System.Collections;
using System;

using System.IO;
public class ErrorLog{
    private static ErrorLog _instance;

    private ErrorLog()
    {
 
    }

    //config
    private const string LOG_FOLDER = "D:/ErrorLog/";

    //member
    private string _txtFimeName;
    private bool _isFirstWirte = true;

    public static ErrorLog Instance
    {
        get { return _instance ?? (_instance = new ErrorLog()); }
    }


    public void init()
    {
        if (!Directory.Exists(LOG_FOLDER))
        {
            //若文件夹不存在, 则创建文件夹
            Directory.CreateDirectory(LOG_FOLDER);
        }
        Application.logMessageReceived += HandleLog;
    }

    public void WriteLog(string logContent)
    {
#if UNITY_STANDALONE_WIN

		_txtFimeName = Application.productName.ToString() + "_" + System.DateTime.Now.ToString("yyyyMMdd");
		FileInfo file = new FileInfo(LOG_FOLDER + _txtFimeName);

		StreamWriter sw = null;
		try {
			if (!file.Exists) {
				//不存在该文档则创建
				sw = file.CreateText();
			} else {
				//存在该文档则打开
				sw = file.AppendText();
			}

			if (_isFirstWirte) {
				//写入时间
				sw.WriteLine ("\n***************" + System.DateTime.Now.ToString("HH:mm:ss") + "***************");
				_isFirstWirte = false;
			}
			//写入内容
			sw.WriteLine(logContent);

		} catch (Exception ex) {
			
		} finally {
			if (sw != null) {
				sw.Close();
				sw.Dispose();
			}
		}

#endif
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
       if (type == LogType.Error || type == LogType.Exception)
       {
          WriteLog("[" + type.ToString() + "]" + logString + "stack trace:" + stackTrace);
       }
    }
}
