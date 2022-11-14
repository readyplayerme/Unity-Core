using UnityEngine;

namespace ReadyPlayerMe.Core
{
    public class PartnerSO : ScriptableObject
    {
        public string Subdomain;

        public string GetUrl(bool keepSessionAlive = true)
        {
            var cacheParam = keepSessionAlive ? "" : "&clearCache";
            return $"https://{GetSubdomain()}.readyplayer.me/avatar?frameApi{cacheParam}";
        }

        public string GetSubdomain()
        {
            if (string.IsNullOrEmpty(Subdomain)) Subdomain = "demo";
            return Subdomain;
        }
    }
}
