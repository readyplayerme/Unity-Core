using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading;
using ReadyPlayerMe.Core;
using UnityEditor.PackageManager;

#if !DISABLE_AUTO_INSTALLER

namespace ReadyPlayerMe
{
    [InitializeOnLoad]
    public class ModuleInstaller : AssetPostprocessor
    {
        private const string PROGRESS_BAR_TITLE = "Ready Player Me";
        
        static ModuleInstaller()
        {
            if (HasAnyMissingModule())
            {
                EditorApplication.update += InstallModules;
            }
        }

        private static void InstallModules()
        {
            EditorUtility.DisplayProgressBar(PROGRESS_BAR_TITLE, "Installing modules...", 0);

            var count = ModuleList.Modules.Length;
            for(var i = 0; i < count; i++)
            {
                var packages = GetPackageList();
                
                var module = ModuleList.Modules[i];
                
                if (packages.All(info => info.name != module.name))
                {
                    EditorUtility.DisplayProgressBar(PROGRESS_BAR_TITLE, $"Installing module {module.name}", i * count * 0.1f + 0.1f);
                    AddModule(module.Identifier);
                }
                else
                {
                    EditorUtility.DisplayProgressBar(PROGRESS_BAR_TITLE, $"All modules are loaded.", 1);
                }
            }

            Thread.Sleep(200);
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= InstallModules;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(path => path.StartsWith("Packages")))
            {
                if (HasAnyMissingModule())
                {
                    InstallModules();
                }
            }
        }

        private static bool HasAnyMissingModule()
        {
            var packages = GetPackageList();
            var modules = ModuleList.Modules;
            return modules.Select(module => packages.All(info => info.name != module.name)).FirstOrDefault();
        }

        private static PackageCollection GetPackageList()
        {
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
                Thread.Sleep(20);
 
            if (listRequest.Error != null)
            {
                Debug.Log("Error: " + listRequest.Error.message);
                return null;
            }
            
            return listRequest.Result;
        }

        private static void AddModule(string name)
        {
            var addRequest = Client.Add(name);
            while (!addRequest.IsCompleted)
                Thread.Sleep(20);
            EditorAssetLoader.CreateSettingsAssets();
            if (addRequest.Error != null)
            {
                Debug.Log("Error: " + addRequest.Error.message);
            }
        }
    }
}
#endif
