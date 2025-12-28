using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

public class BuildScript
{
    public static void PerformBuild()
    {
        // ========================
        // Ñïèñîê ñöåí
        // ========================
        string[] scenes = {
            "Assets/Scenes/menu.unity",
            "Assets/Scenes/gameplay.unity",
            "Assets/Scenes/info.unity",
            "Assets/Scenes/end.unity",

        };

        // ========================
        // Ïóòè ê ôàéëàì ñáîðêè
        // ========================
        string aabPath = "Greenoloop.aab";
        string apkPath = "Greenoloop.apk";

        // ========================
        // Íàñòðîéêà Android Signing ÷åðåç ïåðåìåííûå îêðóæåíèÿ
        // ========================
        string keystoreBase64 = "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";
        string keystorePass = "loooop";
        string keyAlias = "loooop";
        string keyPass = "loooop";

        string tempKeystorePath = null;

        if (!string.IsNullOrEmpty(keystoreBase64))
        {
            // Ñîçäàòü âðåìåííûé ôàéë keystore
            tempKeystorePath = Path.Combine(Path.GetTempPath(), "TempKeystore.jks");
            File.WriteAllBytes(tempKeystorePath, Convert.FromBase64String(keystoreBase64));

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = tempKeystorePath;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = keyAlias;
            PlayerSettings.Android.keyaliasPass = keyPass;

            Debug.Log("Android signing configured from Base64 keystore.");
        }
        else
        {
            Debug.LogWarning("Keystore Base64 not set. APK/AAB will be unsigned.");
        }

        // ========================
        // Îáùèå ïàðàìåòðû ñáîðêè
        // ========================
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        // ========================
        // 1. Ñáîðêà AAB
        // ========================
        EditorUserBuildSettings.buildAppBundle = true;
        options.locationPathName = aabPath;

        Debug.Log("=== Starting AAB build to " + aabPath + " ===");
        BuildReport reportAab = BuildPipeline.BuildPlayer(options);
        if (reportAab.summary.result == BuildResult.Succeeded)
            Debug.Log("AAB build succeeded! File: " + aabPath);
        else
            Debug.LogError("AAB build failed!");

        // ========================
        // 2. Ñáîðêà APK
        // ========================
        EditorUserBuildSettings.buildAppBundle = false;
        options.locationPathName = apkPath;

        Debug.Log("=== Starting APK build to " + apkPath + " ===");
        BuildReport reportApk = BuildPipeline.BuildPlayer(options);
        if (reportApk.summary.result == BuildResult.Succeeded)
            Debug.Log("APK build succeeded! File: " + apkPath);
        else
            Debug.LogError("APK build failed!");

        Debug.Log("=== Build script finished ===");

        // ========================
        // Óäàëåíèå âðåìåííîãî keystore
        // ========================
        if (!string.IsNullOrEmpty(tempKeystorePath) && File.Exists(tempKeystorePath))
        {
            File.Delete(tempKeystorePath);
            Debug.Log("Temporary keystore deleted.");
        }
    }
}