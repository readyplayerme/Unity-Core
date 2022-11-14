using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public static class PackageUpdater
{
    private const string PACKAGE_DOMAIN = "com.readyplayerme";

    private const string GITHUB_WEBSITE = "https://github.com";
    private const string GITHUB_API_URL = "https://api.github.com/repos";

    public static void GetCurrentRelease()
    {
        List<PackageInfo> packages = AssetDatabase.FindAssets("package") // Get all packages files
            .Select(AssetDatabase.GUIDToAssetPath) // Get path
            .Where(x => x.Contains("package.json") && x.Contains(PACKAGE_DOMAIN)) // Get package.json and com.ryuuk packages
            .Select(PackageInfo.FindForAssetPath).ToList();

        if (packages.Count == 0)
        {
            Debug.Log($"No {PACKAGE_DOMAIN} packages found.");
            return;
        }

        PackageInfo package = packages[0];

        // Get url of repository
        var repoUrl = package.packageId.Substring(package.name.Length + 1);
        // Remove .git from the url and /releases
        var pFrom = repoUrl.IndexOf(GITHUB_WEBSITE, StringComparison.Ordinal) + GITHUB_WEBSITE.Length;
        var pTo = repoUrl.LastIndexOf(".git", StringComparison.Ordinal);
        var repoName = repoUrl.Substring(pFrom, pTo - pFrom);

        // Create releases url by adding repoName to api.github url
        var releasesUrl = GITHUB_API_URL + repoName + "/releases";

        // remove #version from url
        var packageUrl = repoUrl.Substring(0, repoUrl.Length - 7);
        FetchReleases(package.name, packageUrl, releasesUrl, new Version(package.version));
    }

    private static async void FetchReleases(string packageName, string packageUrl, string releasesUrl, Version currentVersion)
    {
        UnityWebRequest request = UnityWebRequest.Get(releasesUrl);
        UnityWebRequestAsyncOperation async = request.SendWebRequest();
        while (!async.isDone)
        {
            await Task.Yield();
        }
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log($"Release Fetch Failed: Error {request.error} ");
            Debug.Log($"Release Fetch Failed: Response {request.downloadHandler.text} ");
            return;
        }

        var response = request.downloadHandler.text;

        Release[] releases = JsonConvert.DeserializeObject<Release[]>(response);

        var versions = new Version[releases?.Length ?? 0];

        for (var i = 0; i < releases.Length; i++)
        {
            versions[i] = new Version(releases[i].Tag);
        }

        Version latestVersion = versions.Max();

        if (latestVersion > currentVersion)
        {
            PromptForUpdate(packageName, currentVersion, latestVersion, packageUrl);
        }
    }

    private static void PromptForUpdate(string packageName, Version currentVersion, Version latestVersion, string packageUrl)
    {
        packageUrl += "#v" + latestVersion;
        var option = EditorUtility.DisplayDialogComplex("Update Packages",
            $"New update available for {packageName}\nCurrent version: {currentVersion}\nLatest version: {latestVersion}",
            "Update",
            "Cancel",
            "Don't update");

        switch (option)
        {
            // Update.
            case 0:
                Update(packageName, packageUrl, currentVersion, latestVersion);
                break;
            // Cancel.
            case 1:
            // Don't Update
            case 2:
                break;
            default:
                Debug.LogError("Unrecognized option.");
                break;
        }
    }

    private static async void Update(string packageName, string packageUrl, Version currentVersion, Version latestVersion)
    {
        RemoveRequest removeRequest = Client.Remove(packageName);
        while (!removeRequest.IsCompleted)
        {
            await Task.Yield();
        }

        await Task.Yield();

        Debug.Log("[Updater] " + packageUrl);

        AddRequest addRequest = Client.Add(packageUrl);
        while (!addRequest.IsCompleted)
        {
            await Task.Yield();
        }

        Debug.Log($"Updated {packageName} from {currentVersion} to {latestVersion}");
        EditorPrefs.SetBool("inProgress", false);
    }

    public class Release
    {
        [JsonProperty("tag_name")]
        public string Tag;
    }
}
