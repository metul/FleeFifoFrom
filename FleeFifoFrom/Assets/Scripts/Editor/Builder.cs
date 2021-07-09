using UnityEditor;

public class Builder
{
    private static readonly string _scenePath = "Assets/Scenes/NetworkScene.unity";
    private static readonly string _buildPathServer = "Builds/Server/FleeFiFoFrom_Server.exe";
    private static readonly string _buildPathClient = "Builds/Client/FleeFiFoFrom_Client.exe";
    private static readonly string _buildPathServerLinux = "Builds/ServerLinux/FleeFiFoFrom_Server.exe";

    [MenuItem("Build/BuildAll")]
    public static void BuildAll()
    {
        BuildServer();
        BuildClient();
    }

    [MenuItem("Build/BuildServer")]
    public static void BuildServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _scenePath },
            locationPathName = _buildPathServer,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.EnableHeadlessMode
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/BuildClient")]
    public static void BuildClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _scenePath },
            locationPathName = _buildPathClient,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development // MARK: Remove for release
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/BuildServerLinux")]
    public static void BuildServerLinux()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _scenePath },
            locationPathName = _buildPathServerLinux,
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.EnableHeadlessMode
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
