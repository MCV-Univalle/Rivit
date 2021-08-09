using UnityEngine;

using System;



public class SampleStartHost : MonoBehaviour
{
	void Awake()
	{
		SPlugins.SRemoteConsole.StartHost(port);
		SPlugins.SRemoteConsole.RegisterUnityDebugLogCallback();
		this._localIPAddress = SPlugins.Network.Util.GetLocalIPAdress();
	}

	void OnGUI()
	{
		GUILayout.Label("SampleStartHost");
		GUILayout.Label(string.Format("LocalIPAddress {0}", this._localIPAddress));
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
	}

	void OnDestroy()
	{
		SPlugins.SRemoteConsole.ShoutDown();
	}

	private string _localIPAddress = string.Empty;
	public int port = SPlugins.SConsoleNetworkUtil.DEFAULT_PORT;
}

