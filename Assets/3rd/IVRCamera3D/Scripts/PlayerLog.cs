/*
 * @brief: 
 * [Header("日志保存在名为：PlayerLog的文件夹中")]
 * 首先初始化：PlayerLog.Instance.init ();
 * [Multiline] public string m_Text="Unity将在D:/PlayerLog目录下\n以该游戏的产品名创建文件夹\n"+"本游戏的日志就保存在该目录下\n";
 * [Multiline] public string m_Text2="方法调用示例：PlayerLog.Instance.WriteLog(需要记录的文字);";
 * @author: stan
 * @date: 20160809
 * 
*/

using UnityEngine;
using System.Collections;
using System;

using System.IO;
public class PlayerLog{
	private static PlayerLog _instance;

	private PlayerLog()
	{
		init();
	}

	//config
	private const string LOG_FOLDER = "D:/PlayerLog/";

	//member
	private string _txtFimeName;
	private bool _isFirstWirte = true;

	public static PlayerLog Instance
	{
		get{ return _instance ?? (_instance = new PlayerLog()); }
	}
		

	public void init()
	{
		if (!Directory.Exists(LOG_FOLDER)) {
			//若文件夹不存在, 则创建文件夹
			Directory.CreateDirectory(LOG_FOLDER);
		}
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
}
