using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReadyPlayerMe.Core
{
    public static class ApplicationData
    {
        private const string SDK_VERSION = "v1.12.0";
        private const string TAG = "ApplicationData";
        private const string DEFAULT_RENDER_PIPELINE = "Built-In Render Pipeline";
        private static readonly AppData Data;

        static ApplicationData()
        {
            Data.SDKVersion = SDK_VERSION;
            Data.PartnerName = GetPartnerSubdomain();
            Data.UnityVersion = Application.unityVersion;
            Data.UnityPlatform = Application.platform.ToString();
            Data.RenderPipeline = GetRenderPipeline();
#if UNITY_EDITOR
            Data.BuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
#endif
        }

        private static string GetPartnerSubdomain()
        {
            var partner = Resources.Load<ScriptableObject>("Partner");
            Type type = partner != null ? partner.GetType() : null;
            MethodInfo method = type?.GetMethod("GetSubdomain");
            var partnerSubdomain = method?.Invoke(partner, null) as string;
            return partnerSubdomain;
        }

        private static string GetRenderPipeline()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline == null
                ? DEFAULT_RENDER_PIPELINE
                : GraphicsSettings.currentRenderPipeline.name;
            return renderPipeline;
        }

        public static void Log()
        {
            SDKLogger.Log(TAG, $"Partner Subdomain: <color=green>{Data.PartnerName}</color>");
            SDKLogger.Log(TAG, $"SDK Version: <color=green>{Data.SDKVersion}</color>");
            SDKLogger.Log(TAG, $"Unity Version: <color=green>{Data.UnityVersion}</color>");
            SDKLogger.Log(TAG, $"Unity Platform: <color=green>{Data.UnityPlatform}</color>");
            SDKLogger.Log(TAG, $"Unity Render Pipeline: <color=green>{Data.RenderPipeline}</color>");
#if UNITY_EDITOR
            SDKLogger.Log(TAG, $"Unity Build Target: <color=green>{Data.BuildTarget}</color>");
#endif
        }

        public static AppData GetData()
        {
            return Data;
        }
    }
}