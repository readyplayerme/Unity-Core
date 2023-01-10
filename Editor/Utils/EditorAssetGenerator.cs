using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Core.Data;

namespace ReadyPlayerMe.Core.Editor
{
    public static class EditorAssetGenerator
    {
        private const string SETTINGS_SAVE_FOLDER = "Ready Player Me/Resources/Settings";
        private const string PARTNER_SUB_DOMAIN_ASSET_NAME = "PartnerSubdomainSettings.asset";

        public static void CreateSettingsAssets()
        {
            DirectoryUtility.ValidateDirectory($"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}");
            AssetDatabase.Refresh();
            CreatePartnerSubdomainSetting();
        }

        private static void CreatePartnerSubdomainSetting()
        {
            var partnerSubdomainSettings = ScriptableObject.CreateInstance<PartnerSubdomainSettings>();
            partnerSubdomainSettings.Subdomain = "demo";

            AssetDatabase.CreateAsset(partnerSubdomainSettings, $"Assets/{SETTINGS_SAVE_FOLDER}/{PARTNER_SUB_DOMAIN_ASSET_NAME}");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
