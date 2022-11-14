using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core
{
    public class ReadyPlayerMeSettings : ScriptableObject
    {
        public string partnerSubdomain = "demo";
        public AvatarLoaderSettings AvatarLoaderSettings;

        public void SaveSubdomain(string newSubdomain)
        {
            partnerSubdomain = newSubdomain;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
