using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const string LOCAL_SCENE_NAME = "Game";
    private const string ONLINE_SCENE_NAME = "NetworkScene";

    public void LoadLocalGame()
    {
        GameState.LocalGame = true;
        SceneManager.LoadScene(LOCAL_SCENE_NAME);
    }
    
    public void LoadOnlineGame()
    {
        GameState.LocalGame = false;
        
        // TODO dummy-removal: set custom player count
        GameState.PlayerCount = Rules.MAX_PLAYER_COUNT;
        
        SceneManager.LoadScene(ONLINE_SCENE_NAME);
    }

    public void ChangePlayerCount(int val)
    {
        GameState.PlayerCount = Rules.MAX_PLAYER_COUNT - val;
    }
}
