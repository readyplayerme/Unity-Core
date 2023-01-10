using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace ReadyPlayerMe.Core.Editor
{
    /// <summary>
    ///     Class <c>ModuleUpdater</c> is responsible for checking and updating the Ready Player Me SDK modules.
    /// </summary>
    public static class ModuleUpdater
    {
        private const string PACKAGE_JSON = "package.json";
        private const string PACKAGE_DOMAIN = "com.readyplayerme";

        private const string GITHUB_WEBSITE = "https://github.com";
        private const string GITHUB_API_URL = "https://api.github.com/repos";

        private const string WARNING_PACKAGES_NOT_FOUND = "No Ready Player Me packages found.";
        private const string WARNING_UPDATE_SKIPPED = "No Ready Player Me packages found.";
        private const int MILLISECONDS_TIMEOUT = 20;
        private const string ASSET_FILTER = "package";

        /// <summary>
        ///     Check for Ready Player Me package updates.
        /// </summary>
        [MenuItem("Ready Player Me/Check For Updates")]
        public static void CheckForUpdates()
        {
            // Get PackageInfo array from RPM Module package.json files
            PackageInfo[] packages = AssetDatabase.FindAssets(ASSET_FILTER)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => x.Contains(PACKAGE_JSON) && x.Contains(PACKAGE_DOMAIN))
                .Select(PackageInfo.FindForAssetPath)
                .ToArray();

            // Turn package_name@repo_url#branch_name into https://api.github.com/repos/readyplayerme/repo_name/releases 
            foreach (PackageInfo package in packages)
            {
                var repoUrl = package.packageId.Split('@')[1];
                var releasesUrl = repoUrl.Replace(GITHUB_WEBSITE, GITHUB_API_URL)
                    .Split(new[] { ".git#" }, StringSplitOptions.None)[0] + "/releases";
                var packageUrl = repoUrl.Split('#')[0];

                // Experimental or prerelease packages might look like 0.1.0-exp.1, remove after dash to parse with Version
                var version = package.version.Split('-')[0];
                FetchReleases(package.name, packageUrl, releasesUrl, new Version(version));
            }
        }

        /// <summary>
        ///     Fetch latest release for each module and prompt for update if available.
        /// </summary>
        /// <param name="packageName">The name of the Unity package.</param>
        /// <param name="packageUrl">The Git URL of the Unity package.</param>
        /// <param name="releasesUrl">The Git URL of the Unity package.</param>
        /// <param name="currentVersion">The current version of the package.</param>
        private static async void FetchReleases(string packageName, string packageUrl, string releasesUrl,
            Version currentVersion)
        {
            UnityWebRequest request = UnityWebRequest.Get(releasesUrl);
            UnityWebRequestAsyncOperation op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = request.downloadHandler.text;
                Release[] releases = JsonConvert.DeserializeObject<Release[]>(response);

                Version[] versions = releases.Select(r => new Version(r.Tag.Substring(1).Split('-')[0])).ToArray();

                Version latestVersion = versions.Max();

                if (latestVersion > currentVersion)
                {
                    DisplayUpdateDialog(packageName, currentVersion, latestVersion, packageUrl);
                }
            }
            else
            {
                Debug.Log($"Failed to fetch {packageName} releases. Error: {request.error} ");
            }
        }

        /// <summary>
        ///     Display a Unity popup with notification about available package updates with buttons to update or skip.
        /// </summary>
        /// <param name="packageName">The name of the Unity package.</param>
        /// <param name="currentVersion">The current version of the package.</param>
        /// <param name="latestVersion">The new version of the package.</param>
        /// <param name="packageUrl">The Git URL of the Unity package.</param>
        private static void DisplayUpdateDialog(string packageName, Version currentVersion, Version latestVersion,
            string packageUrl)
        {
            var shouldUpdate = EditorUtility.DisplayDialog("Update Packages",
                $"New update available for {packageName}\nCurrent version: {currentVersion}\nLatest version: {latestVersion}",
                "Update",
                "Skip");

            if (shouldUpdate)
            {
                packageUrl += "#v" + latestVersion;
                UpdateModule(packageName, packageUrl, currentVersion, latestVersion);
            }
            else
            {
                // TODO: Bring analytics here
                Debug.LogWarning(WARNING_UPDATE_SKIPPED);
            }
        }

        /// <summary>
        ///     Update the specified module by removing the current version and then adding the specified version.
        /// </summary>
        /// <param name="name">The name of the Unity package.</param>
        /// <param name="url">The Git URL of the Unity package.</param>
        /// <param name="current">The current version of the package.</param>
        /// <param name="latest">The new version of the package.</param>
        private static void UpdateModule(string name, string url, Version current, Version latest)
        {
            RemoveRequest removeRequest = Client.Remove(name);
            while (!removeRequest.IsCompleted) Thread.Sleep(MILLISECONDS_TIMEOUT);

            AddRequest addRequest = Client.Add(url);
            while (!addRequest.IsCompleted) Thread.Sleep(MILLISECONDS_TIMEOUT);

            Debug.Log($"Updated {name} from v{current} to v{latest}");
        }
    }

    internal class Release
    {
        [JsonProperty("tag_name")] 
        public string Tag;
    }
}
