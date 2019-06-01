using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BuildApp
{
    private static string m_AppName = PlayerSettings.productName;//RealConfig.GetRealFram().m_AppName;
    public static string m_AndroidPath = Application.dataPath + "/../BuildTarget/Android/";
    public static string m_IOSPath = Application.dataPath + "/../BuildTarget/IOS/";
    public static string m_WindowsPath = Application.dataPath + "/../BuildTarget/Windows/";

    [MenuItem("Build/标准包")]
    public static void Build()
    {
        //打ab包
        BundleEditor.Build();
        //生成可执行程序
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        Copy(abPath, Application.streamingAssetsPath);
        string savePath = "";
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            PlayerSettings.Android.keystorePass = "sikiocean";
            PlayerSettings.Android.keyaliasPass = "sikiocean";
            PlayerSettings.Android.keyaliasName = "android.keystore";
            PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + "/realfram.keystore";
            savePath = m_AndroidPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".apk";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            savePath = m_IOSPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows|| EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
        {
            savePath = m_WindowsPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}/{1}.exe", DateTime.Now, m_AppName);
        }

        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        DeleteDir(Application.streamingAssetsPath);
    }

    private static string[] FindEnableEditorrScenes()
    {
        List<string> editorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            editorScenes.Add(scene.path);
        }
        return editorScenes.ToArray();
    }

    private static void Copy(string srcPath, string targetPath)
    {
        try
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            string scrdir = Path.Combine(targetPath, Path.GetFileName(srcPath));
            if (Directory.Exists(srcPath))
                scrdir += Path.DirectorySeparatorChar;
            if (!Directory.Exists(scrdir))
            {
                Directory.CreateDirectory(scrdir);
            }

            string[] files = Directory.GetFileSystemEntries(srcPath);
            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    Copy(file, scrdir);
                }
                else
                {
                    File.Copy(file, scrdir + Path.GetFileName(file), true);
                }
            }

        }
        catch
        {
            Debug.LogError("无法复制：" + srcPath + "  到" + targetPath);
        }
    }

    public static void DeleteDir(string scrPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(scrPath);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo info in fileInfo)
            {
                if (info is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(info.FullName);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    #region 打包机调用打包PC版本
    public static void BuildPC()
    {
        //打ab包
        BundleEditor.Build();
        BuildSetting buildSetting = GetPCBuildSetting();
        string suffix = SetPcSetting(buildSetting);
        //生成可执行程序
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        //清空生成的文件夹
        DeleteDir(m_WindowsPath);
        Copy(abPath, Application.streamingAssetsPath);
        string dir = m_AppName + "_PC" + suffix + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
        string name = string.Format("/{0}.exe", m_AppName);
        string savePath = m_WindowsPath + dir + name;
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        DeleteDir(Application.streamingAssetsPath);
        WriteBuildName(dir);
    }

    /// <summary>
    /// 根据jenkins的参数读取到buildsetting里
    /// </summary>
    /// <returns></returns>
    static BuildSetting GetPCBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
            if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Debug"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.Debug);
                }
            }
        }
        return buildSetting;
    }

    /// <summary>
    /// 根据读取的数据在unity里面设置对应
    /// </summary>
    static string SetPcSetting(BuildSetting setting)
    {
        string suffix = "_";
        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }
        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.macOS.buildNumber = setting.Build;
            suffix += "_" + setting.Build;
        }
        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
        }
        if (setting.Debug)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            suffix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }
        return suffix;
    }
    #endregion

    #region 打包Android
    public static void BuildAndroid()
    {
        //打ab包
        BundleEditor.Build();
        PlayerSettings.Android.keystorePass = "sikiocean";
        PlayerSettings.Android.keyaliasPass = "sikiocean";
        PlayerSettings.Android.keyaliasName = "android.keystore";
        PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + "/realfram.keystore";
        BuildSetting buildSetting = GetAndoridBuildSetting();
        string suffix = SetAndroidSetting(buildSetting);
        //生成可执行程序
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        //清空生成的文件夹
        DeleteDir(m_AndroidPath);
        Copy(abPath, Application.streamingAssetsPath);
        string savePath = m_AndroidPath + m_AppName + "_Andorid" + suffix + string.Format("_{0:yyyy_MM_dd_HH_mm}.apk", DateTime.Now);
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        DeleteDir(Application.streamingAssetsPath);
    }

    static BuildSetting GetAndoridBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
            if (str.StartsWith("Place"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Place = (Place)Enum.Parse(typeof(Place), tempParam[1], true);
                }
            }
            else if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Debug"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.Debug);
                }
            }
            else if (str.StartsWith("MulRendering"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                }
            }
            else if (str.StartsWith("IL2CPP"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.IL2CPP);
                }
            }
        }
        return buildSetting;
    }

    static string SetAndroidSetting(BuildSetting setting)
    {
        string suffix = "_";
        if (setting.Place != Place.None)
        {
            //代表了渠道包
            string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbol + ";" + setting.Place.ToString());
            suffix += setting.Place.ToString();
        }

        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }
        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.Android.bundleVersionCode = int.Parse(setting.Build);
            suffix += "_" + setting.Build;
        }
        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
            //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
        }

        if (setting.MulRendering)
        {
            PlayerSettings.MTRendering = true;
            suffix += "_MTR";
        }
        else
        {
            PlayerSettings.MTRendering = false;
        }

        if (setting.IL2CPP)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            suffix += "_IL2CPP";
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        }

        if (setting.Debug)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            suffix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }
        return suffix;
    }
    #endregion

    #region 打包IOS
    public static void BuildIOS()
    {
        //打ab包
        BundleEditor.Build();
        BuildSetting buildSetting = GetIOSBuildSetting();
        string suffix = SetIOSSetting(buildSetting);

        //生成可执行程序
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        //清空生成的文件夹
        DeleteDir(m_IOSPath);
        Copy(abPath, Application.streamingAssetsPath);
        string name = m_AppName + "_IOS" + suffix + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
        string savePath = m_IOSPath + name;
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        DeleteDir(Application.streamingAssetsPath);
        WriteBuildName(name);
    }

    static BuildSetting GetIOSBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs();
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
           if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("MulRendering"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                }
            }
            else if (str.StartsWith("DynamicBatching"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.DynamicBatching);
                }
            }
        }
        return buildSetting;
    }

    static string SetIOSSetting(BuildSetting setting)
    {
        string suffix = "_";

        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }
        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.iOS.buildNumber = setting.Build;
            suffix += "_" + setting.Build;
        }
        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
            //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
        }

        if (setting.MulRendering)
        {
            PlayerSettings.MTRendering = true;
            suffix += "_MTR";
        }
        else
        {
            PlayerSettings.MTRendering = false;
        }

        if (setting.DynamicBatching)
        {
            suffix += "_Dynamic";
        }
        else
        {

        }

        return suffix;
    }
    #endregion

    public static void WriteBuildName(string name)
    {
        FileInfo fileInfo = new FileInfo(Application.dataPath + "/../buildname.txt");
        StreamWriter sw = fileInfo.CreateText();
        sw.WriteLine(name);
        sw.Close();
        sw.Dispose();
    }
}

public class BuildSetting
{
    //版本号
    public string Version = "";
    //build次数
    public string Build = "";
    //程序名称
    public string Name = "";
    //是否debug
    public bool Debug = true;
    //渠道
    public Place Place = Place.None;
    //多线程渲染
    public bool MulRendering = true;
    //是否IL2CPP
    public bool IL2CPP = false;
    //是否开启动态合批
    public bool DynamicBatching = false;
}

public enum Place
{
    None =0,
    Xiaomi,
    Bilibili,
    Huawei,
    Meizu,
    Weixin,
}
