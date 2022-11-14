using UnityEngine;

namespace ReadyPlayerMe.Core
{
    [CreateAssetMenu(fileName = "Avatar Loader Settings", menuName = "Scriptable Objects/Ready Player Me/Avatar Loader Settings", order = 1)]
    public class AvatarLoaderSettings : ScriptableObject
    {
        public bool AvatarCachingEnabled;
        public AvatarConfig AvatarConfig;
        public static readonly string LocalAssetPath = "Settings/AvatarLoaderSettings";

        public static AvatarLoaderSettings GetAsset()
        {
            return Resources.Load<AvatarLoaderSettings>(LocalAssetPath);
        }
    }
}
