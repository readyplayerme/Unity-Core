using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    /// <summary>
    ///     This <c>EditorWindow</c> class is used as a base class for Ready Player Me Editor windows and contains
    ///     functionality that is shared between each child class.
    /// </summary>
    public class EditorWindowBase : EditorWindow
    {
        private const string SUPPORT_HEADING = "Support";
        private const string ERROR_ICON_SEARCH_FILTER = "t:Texture rpm_error_icon";

        protected GUIStyle HeadingStyle;
        protected GUIStyle DescriptionStyle;
        
        protected Texture errorIcon;

        private GUIStyle webButtonStyle;

        private readonly GUILayoutOption windowWidth = GUILayout.Width(460);
        protected readonly float ButtonHeight = 30f;
        private Banner banner;
        private Footer footer;

        private string editorWindowName;
        private bool windowResized;

        private void LoadAssets()
        {
            banner ??= new Banner(editorWindowName);
            
            footer ??= new Footer(editorWindowName);
            
            if (errorIcon == null)
            {
                var assetGuid = AssetDatabase.FindAssets(ERROR_ICON_SEARCH_FILTER).FirstOrDefault();
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                if (assetPath != null)
                {
                    errorIcon = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;
                }
            }

            HeadingStyle ??= new GUIStyle
            {
                fontSize = 14,
                richText = true,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(5, 0, 0, 8),
                normal =
                {
                    textColor = Color.white
                }
            };

            DescriptionStyle ??= new GUIStyle
            {
                fontSize = 12,
                richText = true,
                wordWrap = true,
                margin = new RectOffset(5, 0, 0, 0),
                normal =
                {
                    textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f)
                }
            };

            webButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fixedWidth = 149,
                fixedHeight = ButtonHeight,
                padding = new RectOffset(5, 5, 5, 5)
            };
        }

        protected void SetEditorWindowName(string editorName)
        {
            editorWindowName = editorName;
        }

        protected void DrawContent(Action content, bool useFooter = true)
        {
            LoadAssets();

            Horizontal(() =>
            {
                GUILayout.FlexibleSpace();
                Vertical(() =>
                {
                    banner.Draw(position);
                    content?.Invoke();
                    if (useFooter)
                    {
                        Vertical(() =>
                        {
                            GUILayout.Label(SUPPORT_HEADING, HeadingStyle);
                            footer.Draw(position);
                        }, true);
                    }
                }, windowWidth);
                GUILayout.FlexibleSpace();
            });

            SetWindowSize();
        }

        private void SetWindowSize()
        {
            var height = GUILayoutUtility.GetLastRect().height;
            if (!windowResized && height > 1)
            {
                minSize = maxSize = new Vector2(460, height);
                windowResized = true;
            }
        }

        #region Horizontal and Vertical Layouts

        protected void Vertical(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginVertical(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void Vertical(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void Horizontal(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginHorizontal(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        protected void Horizontal(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
