using UnityEngine;

using System;
using System.Collections.Generic;



public class SampleStartClient : MonoBehaviour
{
	void Awake()
	{
		SPlugins.SRemoteConsole.StartClient(hostIPAddress, port);
		SPlugins.SRemoteConsole.RegisterUnityDebugLogCallback();
		SPlugins.SRemoteConsole.delegateOnNetworkMessage = HandleOnSystemMessage;
	}

	void HandleOnSystemMessage(LogType logType_, string message_)
	{
		this._systemMessageList.Add(message_);
	}


	void OnGUI()
	{
		GUILayout.Label("SampleStartClient");

		if (true == GUILayout.Button("Write Debug.Log"))
		{
			Debug.Log("Write Debug.Log");
		}
		if (true == GUILayout.Button("Write Debug.LogWarning"))
		{
			Debug.LogWarning("Write Debug.LogWarning");
		}
		if (true == GUILayout.Button("Write Debug.LogError"))
		{
			Debug.LogError("Write Debug.LogError");
		}

		foreach (string message in this._systemMessageList)
		{
			GUILayout.Label(message);
		}
	}
	void OnDestroy()
	{
		SPlugins.SRemoteConsole.ShoutDown();
	}

	private List<string> _systemMessageList = new List<string>();
	[SerializeField]
	public string hostIPAddress = "127.0.0.1";
	[SerializeField]
	public int port = SPlugins.SConsoleNetworkUtil.DEFAULT_PORT;
}

