using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void BuildAndroidAab()
    {
        BuildAndroid(isAab: true);
    }

    public static void BuildAndroidApk()
    {
        BuildAndroid(isAab: false);
    }

    private static void BuildAndroid(bool isAab)
    {
        try
        {
            ApplyAndroidSigningFromEnv();

            string workspace = GetArgValue("-workspace") ?? Directory.GetCurrentDirectory();
            string buildRoot = Path.Combine(workspace, "Build", "Android");
            Directory.CreateDirectory(buildRoot);

            string productName = Application.productName;
            string ext = isAab ? "aab" : "apk";
            string outPath = Path.Combine(buildRoot, $"{productName}.{ext}");

            EditorUserBuildSettings.buildAppBundle = isAab;

            string[] scenes = GetEnabledScenes();
            if (scenes.Length == 0)
            {
                throw new Exception("No enabled scenes in Build Settings. Add scenes and enable them.");
            }

            BuildPlayerOptions opts = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log($"[CI] Build output: {outPath}");
            Debug.Log($"[CI] buildAppBundle: {EditorUserBuildSettings.buildAppBundle}");

            BuildReport report = BuildPipeline.BuildPlayer(opts);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception($"Build failed: {report.summary.result} / errors: {report.summary.totalErrors}");
            }

            Debug.Log("[CI] Build succeeded");
        }
        catch (Exception e)
        {
            Debug.LogError("[CI] Build exception: " + e);
            EditorApplication.Exit(1);
            return;
        }

        EditorApplication.Exit(0);
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes;
        if (scenes == null) return Array.Empty<string>();

        return Array.FindAll(scenes, s => s.enabled)
                    .Select(s => s.path)
                    .ToArray();
    }

    private static string GetArgValue(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private static void ApplyAndroidSigningFromEnv()
    {
        string keystorePath = Environment.GetEnvironmentVariable("CI_KEYSTORE_PATH");
        string keystorePass = Environment.GetEnvironmentVariable("CI_KEYSTORE_PASS");
        string keyaliasName = Environment.GetEnvironmentVariable("CI_KEYALIAS_NAME");
        string keyaliasPass = Environment.GetEnvironmentVariable("CI_KEYALIAS_PASS");

        if (string.IsNullOrWhiteSpace(keystorePath) ||
            string.IsNullOrWhiteSpace(keystorePass) ||
            string.IsNullOrWhiteSpace(keyaliasName) ||
            string.IsNullOrWhiteSpace(keyaliasPass))
        {
            Debug.LogWarning("[CI] Signing env vars missing. Build will proceed without forcing custom keystore (debug signing).");
            return;
        }

        if (!File.Exists(keystorePath))
        {
            throw new FileNotFoundException($"[CI] Keystore file not found: {keystorePath}");
        }

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = keystorePass;

        PlayerSettings.Android.keyaliasName = keyaliasName;
        PlayerSettings.Android.keyaliasPass = keyaliasPass;

        Debug.Log("[CI] Custom keystore signing applied.");
    }
}
