using UnityEditor;
using UnityEngine;
using ReadyPlayerMe.Core.Analytics;

namespace ReadyPlayerMe.Core.Editor
{
    /// <summary>
    ///     Footer for RPM editor windows. Contains buttons for external links.
    /// </summary>
    public class Footer: IEditorWindowComponent
    {
        private const string DOCS_URL = "https://bit.ly/UnitySDKDocs";
        private const string FAQ_URL = "https://docs.readyplayer.me/overview/frequently-asked-questions/game-engine-faq";
        private const string DISCORD_URL = "https://bit.ly/UnitySDKDiscord";

        private readonly float ButtonHeight = 30f;
        
        private readonly GUIStyle webButtonStyle;
        
        public string EditorWindowName { get; set; }
        
        public Footer(string editorWindowName)
        {
            EditorWindowName = editorWindowName;
            webButtonStyle = new GUIStyle(GUI.skin.button);
            webButtonStyle.fontSize = 12;
            webButtonStyle.fixedWidth = 149;
            webButtonStyle.fixedHeight = ButtonHeight;
            webButtonStyle.padding = new RectOffset(5, 5, 5, 5);
        }

        public void Draw(Rect position)
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Documentation", webButtonStyle))
            {
                AnalyticsEditorLogger.EventLogger.LogOpenDocumentation(EditorWindowName);
                Application.OpenURL(DOCS_URL);
            }

            if (GUILayout.Button("FAQ", webButtonStyle))
            {
                AnalyticsEditorLogger.EventLogger.LogOpenFaq(EditorWindowName);
                Application.OpenURL(FAQ_URL);
            }

            if (GUILayout.Button("Discord", webButtonStyle))
            {
                AnalyticsEditorLogger.EventLogger.LogOpenDiscord(EditorWindowName);
                Application.OpenURL(DISCORD_URL);
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
