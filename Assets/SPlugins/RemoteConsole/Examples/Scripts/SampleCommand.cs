using UnityEngine;

using System;



public class SampleCommand : MonoBehaviour
{
	private const string COMMAND_HIDE_MESH = "HIDE_MESH";
	private const string COMMAND_SHOW_MESH = "SHOW_MESH";
	void Awake()
	{
		SPlugins.SRemoteConsole.StartLocal();
		SPlugins.SRemoteConsole.RegisterUnityDebugLogCallback();
		SPlugins.SRemoteConsole.delegateOnReceiveCommand = DelegetOnCommand;

		SPlugins.SRemoteConsole.RegistCommand(COMMAND_HIDE_MESH);
		SPlugins.SRemoteConsole.RegistCommand(COMMAND_SHOW_MESH);
	}

	void OnGUI()
	{
		GUILayout.Label("SampleStartCommand");

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

		this._log = GUILayout.TextField(this._log);
		if (true == GUILayout.Button("Send Log"))
		{
			SPlugins.SRemoteConsole.Log(this._log);
		}
	}

	void DelegetOnCommand(string command_)
	{
		if( true == command_.Equals(COMMAND_HIDE_MESH) )
		{
			Renderer[] rendererArray = GameObject.FindObjectsOfType(typeof(Renderer)) as Renderer[];
			foreach (Renderer renderer in rendererArray)
			{
				renderer.enabled = false;
			}
		}
		else if (true == command_.Equals(COMMAND_SHOW_MESH))
		{
			Renderer[] rendererArray = GameObject.FindObjectsOfType(typeof(Renderer)) as Renderer[];
			foreach (Renderer renderer in rendererArray)
			{
				renderer.enabled = true;
			}
		}
	}

	void OnDestroy()
	{
		SPlugins.SRemoteConsole.ShoutDown();
	}

	private string _log = string.Empty;
}

