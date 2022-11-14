using System;
using UnityEngine;

namespace ReadyPlayerMe.Core
{
    [Serializable]
    public enum Target
    {
        Development,
        Production
    }

    [CreateAssetMenu(fileName = "Analytics Target", menuName = "Scriptable Objects/Ready Player Me/Analytics Target", order = 1)]
    public class AnalyticsTarget : ScriptableObject
    {
        public Target Target;
    }
}
