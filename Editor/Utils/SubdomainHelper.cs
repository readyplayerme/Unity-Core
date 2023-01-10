using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    public static class SubdomainHelper
    {
        public static string PartnerDomain;

        private static readonly bool IsLoaded;
        private static ScriptableObject partner;

        static SubdomainHelper()
        {
            if (IsLoaded) return;

            Load();
            IsLoaded = true;
        }

        public static void SaveToScriptableObject(string newSubdomain)
        {
            if (partner == null)
            {
                return;
            }

            Type type = partner.GetType();
            FieldInfo field = type.GetField("Subdomain");
            field.SetValue(partner, newSubdomain);
            PartnerDomain = newSubdomain;
            EditorUtility.SetDirty(partner);
            AssetDatabase.SaveAssets();
        }

        private static void Load()
        {
            partner = Resources.Load<ScriptableObject>("Partner");
            Type type = partner != null ? partner.GetType() : null;
            MethodInfo method = type?.GetMethod("GetSubdomain");
            PartnerDomain = method?.Invoke(partner, null) as string;
        }
    }
}
