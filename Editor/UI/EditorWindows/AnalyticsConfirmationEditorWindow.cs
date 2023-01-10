using ReadyPlayerMe.Core.Analytics;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Core.Editor
{
    /// <summary>
    ///     This class is used to display an Editor window with a prompt to optionally enable Editor analytics and event
    ///     logging. It is also responsible for loading Editor UI styles and drawing the UI content.
    /// </summary>
    [InitializeOnLoad]
    public class AnalyticsConfirmationEditorWindow : EditorWindowBase
    {
        private const string HEADING = "Help us improve Ready Player Me SDK";
        private const string DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TEXT = "Read our Privacy Policy and learn how we use the data <b>here</b>.";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
        private const string METRICS_NEVER_ASK_AGAIN = "rpm-sdk-metrics-never-ask-again";

        private const string EDITOR_WINDOW_NAME = "allow analytics popup";
        private const string ANALYTICS_CONFIRMATION = "Analytics Confirmation";
        private const string NEVER_ASK_AGAIN = "Never Ask Again";
        private const string DONT_ENABLE_ANALYTICS = "Don't Enable Analytics";
        private const string ENABLE_ANALYTICS = "Enable Analytics";
        private const int BUTTON_FONT_SIZE = 12;

        private static bool neverAskAgain;

        private readonly GUILayoutOption toggleWidth = GUILayout.Width(20);
        private GUIStyle descriptionStyle;
        private GUIStyle buttonStyle;

        private bool variablesLoaded;

        /// <summary>
        ///     Constructor method that subscribes to the StartUp event.
        /// </summary>
        static AnalyticsConfirmationEditorWindow()
        {
            EntryPoint.Startup += OnStartup;
        }

        /// <summary>
        ///     This method is called when Unity Editor is closed or this package is removed.
        /// </summary>
        private void OnDestroy()
        {
            EntryPoint.Startup -= OnStartup;
        }

        /// <summary>
        ///     This method is called for rendering and handling GUI event. It triggers the UI updates of the <c>EditorWindow</c>.
        /// </summary>
        private void OnGUI()
        {
            if (!variablesLoaded) LoadCachedVariables();
            LoadStyles();
            DrawContent(DrawContent, false);
        }

        /// <summary>
        ///     This method is called when a Unity project is opened or after this Unity package has finished importing and is
        ///     responsible for displaying the window. It also calls anayltics events if enabled.
        /// </summary>
        private static void OnStartup()
        {
            if (!EditorPrefs.GetBool(METRICS_NEVER_ASK_AGAIN) && !AnalyticsEditorLogger.IsEnabled)
            {
                ShowWindowMenu();
            }

            if (AnalyticsEditorLogger.IsEnabled)
            {
                AnalyticsEditorLogger.EventLogger.LogOpenProject();
                AnalyticsEditorLogger.EventLogger.IdentifyUser();
                EditorApplication.quitting += OnQuit;
            }
        }

        /// <summary>
        ///     This method is called when the Unity Editor is closed and logs the close event.
        /// </summary>
        private static void OnQuit()
        {
            AnalyticsEditorLogger.EventLogger.LogCloseProject();
        }

        /// <summary>
        ///     Show the <c>EditorWindow</c> and log open event.
        /// </summary>
        private static void ShowWindowMenu()
        {
            var window = (AnalyticsConfirmationEditorWindow) GetWindow(typeof(AnalyticsConfirmationEditorWindow));
            window.titleContent = new GUIContent(ANALYTICS_CONFIRMATION);
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadCachedVariables()
        {
            neverAskAgain = EditorPrefs.GetBool(METRICS_NEVER_ASK_AGAIN);
            variablesLoaded = true;
        }

        /// <summary>
        ///     Sets the <c>EditorWindow</c> styles.
        /// </summary>
        private void LoadStyles()
        {
            if (descriptionStyle == null)
            {
                descriptionStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = BUTTON_FONT_SIZE,
                    richText = true,
                    wordWrap = true,
                    fixedWidth = 450
                };
                descriptionStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            }

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fontSize = BUTTON_FONT_SIZE;
                buttonStyle.padding = new RectOffset(5, 5, 5, 5);
                buttonStyle.fixedHeight = ButtonHeight;
                buttonStyle.fixedWidth = 225;
            }
        }

        /// <summary>
        ///     Creates all the GUI elements to be displayed in the window. It also logs analytics events.
        /// </summary>
        private void DrawContent()
        {
            Vertical(() =>
            {
                GUILayout.Label(HEADING, HeadingStyle);

                GUILayout.Space(10);
                GUILayout.Label(DESCRIPTION, descriptionStyle);

                GUILayout.Space(10);
                if (GUILayout.Button(ANALYTICS_PRIVACY_TEXT, descriptionStyle))
                {
                    Application.OpenURL(ANALYTICS_PRIVACY_URL);
                }

                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

                GUILayout.Space(10);
                Horizontal(() =>
                {
                    GUILayout.Space(4);
                    neverAskAgain = EditorGUILayout.Toggle(neverAskAgain, toggleWidth);
                    GUILayout.Label(NEVER_ASK_AGAIN);
                    GUILayout.FlexibleSpace();

                    EditorPrefs.SetBool(METRICS_NEVER_ASK_AGAIN, neverAskAgain);
                });

                GUILayout.Space(10);
                Horizontal(() =>
                {
                    if (GUILayout.Button(DONT_ENABLE_ANALYTICS, buttonStyle))
                    {
                        AnalyticsEditorLogger.Disable();
                        Close();
                    }
                    if (GUILayout.Button(ENABLE_ANALYTICS, buttonStyle))
                    {
                        AnalyticsEditorLogger.Enable();
                        Close();
                    }
                });
            }, true);
        }
    }
}
