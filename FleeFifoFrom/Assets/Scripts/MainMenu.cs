using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const int LOCAL_SCENE_INDEX = 1;
    private const int ONLINE_SCENE_INDEX = 2;

    public void LoadLocalGame()
    {
        GameState.LocalGame = true;
        SceneManager.LoadScene(LOCAL_SCENE_INDEX);
    }
    
    public void LoadOnlineGame()
    {
        GameState.LocalGame = false;
        
        // TODO dummy-removal: set custom player count
        GameState.PlayerCount = Rules.MAX_PLAYER_COUNT;
        
        SceneManager.LoadScene(ONLINE_SCENE_INDEX);
    }

    public void ChangePlayerCount(int val)
    {
        GameState.PlayerCount = Rules.MAX_PLAYER_COUNT - val;
    }
}
