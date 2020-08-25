using System;
using UnityEngine;
using Sisus.Attributes;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	/// <summary>
	/// Class responsible for determining when Power Inspector is uninstalled and handling removal of related defines in Scripting Define Symbols of Player Settings.
	/// </summary>
	#if DEV_MODE
	[CreateAssetMenu]
	#endif
	public class PluginCompatibilityPackageInstaller : ScriptableObject
	{
		[NotNullOrEmpty, Tooltip("Full name of a type that is found in the plugin.\nWe detect whether or not type by name exists to determine if the plugin is currently installed or not, and install the compatibility package if so.")]
		public string fullTypeName;

		#if UNITY_EDITOR
		private bool typeFetched;
		private Type type;

		public bool autoInstallEnabled = true;

		[ShowInInspector]
		public Type Type
		{
			get
			{
				if(!typeFetched)
				{
					typeFetched = true;
					type = TypeExtensions.GetType(fullTypeName);
				}
				return type;
			}
		}

		public string PackageInstallPath
		{
			get
			{
				return GetDirectoryPath() + "/package";
			}
		}

		public string PackageDisablePath
		{
			get
			{
				return GetDirectoryPath() + "/package~";
			}
		}

		[ShowInInspector]
		public bool PluginIsInstalled
		{
			get
			{
				return Type != null;
			}
		}

		[ShowInInspector]
		public bool CompatibilityPackageIsInstalled
		{
			get
			{
				return AssetDatabase.IsValidFolder(PackageInstallPath);
			}
		}

		[PSpace(3f)]
		[Button("Install"), ShowIf("CompatibilityPackageIsInstalled", false)]
		[ContextMenu("Install")]
		private void InstallManually()
		{
			autoInstallEnabled = false;

			AssetDatabase.StartAssetEditing();			
			Install();
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(PackageInstallPath);			
		}

		public void Install()
		{
			if(CompatibilityPackageIsInstalled)
			{
				Debug.LogWarning("Can't install " + GetDirectoryPath() + " because it is already installed.");
				return;
			}

			#if DEV_MODE
			Debug.Log("PluginCompatibilityPackageInstaller.Install");
			#endif
			
			var disabledFullPath = FileUtility.LocalToFullPath(PackageDisablePath);
			var installedFullPath = FileUtility.LocalToFullPath(PackageInstallPath);
			Directory.Move(disabledFullPath, installedFullPath);
		}

		[Button("Uninstall"), ShowIf("CompatibilityPackageIsInstalled", true)]
		[ContextMenu("Uninstall")]
		private void UninstallManually()
		{
			autoInstallEnabled = false;

			AssetDatabase.StartAssetEditing();
			Uninstall();
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
		}

		public void Uninstall()
		{
			if(!CompatibilityPackageIsInstalled)
			{
				Debug.LogWarning("Can't uninstall " + GetDirectoryPath() + " because it is not installed.");
				return;
			}

			#if DEV_MODE
			Debug.Log("PluginCompatibilityPackageInstaller.Uninstall");
			#endif

			var installedFullPath = FileUtility.LocalToFullPath(PackageInstallPath);
			var disabledFullPath = FileUtility.LocalToFullPath(PackageDisablePath);			
			Directory.Move(installedFullPath, disabledFullPath);
		}

		private string GetDirectoryPath()
		{
			var scriptAssetPath = AssetDatabase.GetAssetPath(this);
			var directory = Path.GetDirectoryName(scriptAssetPath);
			#if DEV_MODE
			Debug.Log("scriptAssetPath="+scriptAssetPath+", directory="+directory);
			#endif
			return directory;
		}
		#endif
	}
}