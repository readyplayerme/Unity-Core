using ReadyPlayerMe.Core.Data;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core
{
    public static class CoreSettings
    {
        public const string SETTINGS_PATH = "Settings/PartnerSubDomainSettings";

        public static PartnerSubdomainSettings PartnerSubdomainSettings
        {
            get
            {
                if (partnerSubdomainSettings == null)
                {
                    partnerSubdomainSettings = Resources.Load<PartnerSubdomainSettings>(SETTINGS_PATH);
                }
                return partnerSubdomainSettings;
            }
        }

        private static PartnerSubdomainSettings partnerSubdomainSettings;

        public static void SaveSubDomain(string subDomain)
        {
            partnerSubdomainSettings.Subdomain = subDomain;
#if UNITY_EDITOR
            EditorUtility.SetDirty(partnerSubdomainSettings);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
