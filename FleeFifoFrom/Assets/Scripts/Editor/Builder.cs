using UnityEditor;

public class Builder
{
    private static readonly string _mainMenuScenePath = "Assets/Scenes/MainMenu.unity";
    private static readonly string _localScenePath = "Assets/Scenes/Game.unity";
    private static readonly string _networkScenePath = "Assets/Scenes/NetworkScene.unity";
    private static readonly string _buildPathServerWindows = "Builds/Windows/Server/FleeFiFoFrom_Server.exe";
    private static readonly string _buildPathClientWindows = "Builds/Windows/Client/FleeFiFoFrom_Client.exe";
    private static readonly string _buildPathServerLinux = "Builds/Linux/Server/FleeFiFoFrom_Server.exe";
    private static readonly string _buildPathClientLinux = "Builds/Linux/Client/FleeFiFoFrom_Server.exe";

    [MenuItem("Build/Windows/BuildAll")]
    public static void BuildAllWindows()
    {
        BuildServerWindows();
        BuildClientWindows();
    }

    [MenuItem("Build/Linux/BuildAll")]
    public static void BuildAllLinux()
    {
        BuildServerLinux();
        BuildClientLinux();
    }

    [MenuItem("Build/Windows/BuildServer")]
    public static void BuildServerWindows()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _networkScenePath },
            locationPathName = _buildPathServerWindows,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.EnableHeadlessMode
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/Windows/BuildClient")]
    public static void BuildClientWindows()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _mainMenuScenePath, _localScenePath, _networkScenePath }, // MARK: Local scene path might be unnecessary
            locationPathName = _buildPathClientWindows,
            target = BuildTarget.StandaloneWindows64,
            //options = BuildOptions.Development // MARK: Remove for release
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/Linux/BuildServer")]
    public static void BuildServerLinux()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _networkScenePath },
            locationPathName = _buildPathServerLinux,
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.EnableHeadlessMode
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/Linux/BuildClient")]
    public static void BuildClientLinux()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { _mainMenuScenePath, _localScenePath, _networkScenePath },
            locationPathName = _buildPathClientLinux,
            target = BuildTarget.StandaloneLinux64,
            //options = BuildOptions.Development
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
