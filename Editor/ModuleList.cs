
namespace ReadyPlayerMe
{
    public static class ModuleList
    {
        public static readonly ModuleInfo[] Modules = 
        {
            new ModuleInfo 
            {
                name = "com.atteneder.gltfast",
                gitUrl = "https://github.com/atteneder/glTFast.git",
                branch = ""
            },
            new ModuleInfo 
            {
                name = "com.readyplayer.avatarloader",
                gitUrl = "https://github.com/readyplayerme/Unity-Avatar-Loader.git",
                branch = "feature/editor-window-merge"
            },
            new ModuleInfo 
            {
                name = "com.readyplayerme.webview",
                gitUrl = "https://github.com/readyplayerme/Unity-WebView.git",
                branch = ""
            }

        };
    }
}
