using System.IO;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;

public static class EditorAssetLoader 
{
    private const string SETTINGS_SAVE_FOLDER = "Resources/Ready Player Me/Settings";
    private const string SETTINGS_ASSET_NAME = "ReadyPlayerMeSettings.asset";
    private const string AVATAR_LOADER_ASSET_NAME = "AvatarLoaderSettings.asset";

    private const string CONFIG_SAVE_FOLDER = "Resources/Ready Player Me/Configurations";
    
#if !DISABLE_AUTO_INSTALLER

    public static readonly string ReadyPlayerMeAssetPath = $"Packages/com.readyplayerme.core/Settings/{SETTINGS_ASSET_NAME}";
    public static readonly string AvatarLoaderAssetPath = $"Packages/com.readyplayerme.core/Settings/{AVATAR_LOADER_ASSET_NAME}";
    public const string CONFIG_ASSET_FOLDER = "Packages/com.readyplayerme.core/Avatar Configurations";

#else
    private static readonly string ReadyPlayerMeAssetPath = $"Assets/Ready Player Me/Core/Settings/{SETTINGS_ASSET_NAME}";
    private static readonly string AvatarLoaderAssetPath = $"Assets/Ready Player Me/Core/Settings/{AVATAR_LOADER_ASSET_NAME}";
    private const string CONFIG_ASSET_FOLDER = "Assets/Ready Player Me/Core/Avatar Configurations";
#endif

    private static readonly string[] ConfigNames = {  "Avatar Config Medium", "Avatar Config Low", "Avatar Config High" };

    public static void CreateSettingsAssets()
    {
        CreateAvatarConfigAssets();
        LoadReadyPlayerMeSettings();
    }

    
    public static ReadyPlayerMeSettings LoadReadyPlayerMeSettings()
    {
        
        var absolutePath = $"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}/{SETTINGS_ASSET_NAME}";
        if (File.Exists(absolutePath))
        {
            return AssetDatabase.LoadAssetAtPath<ReadyPlayerMeSettings>(ReadyPlayerMeAssetPath);
        }
        return CreateReadyPlayerMeSettings();
    }
    
    private static ReadyPlayerMeSettings CreateReadyPlayerMeSettings()
    {
        var defaultSettings = AssetDatabase.LoadAssetAtPath<ReadyPlayerMeSettings>(ReadyPlayerMeAssetPath);
        ReadyPlayerMeSettings newSettings = ScriptableObject.CreateInstance<ReadyPlayerMeSettings>();
        newSettings.partnerSubdomain = defaultSettings.partnerSubdomain;
        var loaderSettings = LoadAvatarLoaderSettings();
        newSettings.AvatarLoaderSettings = loaderSettings;
        
        DirectoryUtility.ValidateDirectory($"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}");
        
        AssetDatabase.CreateAsset(newSettings, $"Assets/{SETTINGS_SAVE_FOLDER}/{SETTINGS_ASSET_NAME}");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newSettings;
    }
    
    public static AvatarLoaderSettings LoadAvatarLoaderSettings()
    {
        var absolutePath = $"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}/{SETTINGS_ASSET_NAME}";
        if (File.Exists(absolutePath))
        {
            return AssetDatabase.LoadAssetAtPath<AvatarLoaderSettings>(AvatarLoaderAssetPath);
        }
        return CreateAvatarLoaderSettings();
    }
    
    private static AvatarLoaderSettings CreateAvatarLoaderSettings()
    {
        var defaultSettings = AssetDatabase.LoadAssetAtPath<AvatarLoaderSettings>(AvatarLoaderAssetPath);
        var newSettings = ScriptableObject.CreateInstance<AvatarLoaderSettings>();
        var config = Resources.Load<AvatarConfig>($"Avatar Configurations/Avatar Config Medium");
        newSettings.AvatarConfig = config;
        newSettings.AvatarCachingEnabled = defaultSettings.AvatarCachingEnabled;
        
        DirectoryUtility.ValidateDirectory($"{Application.dataPath}/{SETTINGS_SAVE_FOLDER}");
        
        AssetDatabase.CreateAsset(newSettings, $"Assets/{SETTINGS_SAVE_FOLDER}/{AVATAR_LOADER_ASSET_NAME}");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return newSettings;
    }
    
    public static void CreateAvatarConfigAssets()
    {
        var absolutePath = $"{Application.dataPath}/{CONFIG_SAVE_FOLDER}/Avatar Config Medium.asset";
        if (File.Exists(absolutePath))
        {
            return;
        }
        foreach (var configName in ConfigNames)
        {
            DirectoryUtility.ValidateDirectory($"{Application.dataPath}/{CONFIG_SAVE_FOLDER}");
            
            var defaultSettings = AssetDatabase.LoadAssetAtPath<AvatarConfig>($"{CONFIG_ASSET_FOLDER}/{configName}.asset");
            var newSettings = CopyAvatarConfig(defaultSettings);
            
            AssetDatabase.CreateAsset(newSettings, $"Assets/{CONFIG_SAVE_FOLDER}/{configName}.asset");
            AssetDatabase.SaveAssets();
            
        }
        AssetDatabase.Refresh();
    }

    private static AvatarConfig CopyAvatarConfig(AvatarConfig sourceConfig)
    {
        var newSettings = ScriptableObject.CreateInstance<AvatarConfig>();
        newSettings.Pose = sourceConfig.Pose;
        newSettings.MeshLod = sourceConfig.MeshLod;
        newSettings.MorphTargets = sourceConfig.MorphTargets;
        newSettings.UseHands = sourceConfig.UseHands;
        newSettings.TextureSizeLimit = sourceConfig.TextureSizeLimit;
        return newSettings;
    }
}
