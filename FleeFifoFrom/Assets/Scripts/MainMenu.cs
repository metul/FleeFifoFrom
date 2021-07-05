using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private const int LOCAL_SCENE_INDEX = 1;
    private const int ONLINE_SCENE_INDEX = 2;
    private const int RULES_SCENE_INDEX = 3;
    
    public void LoadLocalGame()
    {
        GameState.LocalGame = true;
        SceneManager.LoadScene(LOCAL_SCENE_INDEX);
    }
    
    public void LoadOnlineGame()
    {
        GameState.LocalGame = false;
        SceneManager.LoadScene(ONLINE_SCENE_INDEX);
    }
    
    public void LoadRules()
    {
        SceneManager.LoadScene(RULES_SCENE_INDEX);
    }

    public void ChangePlayerCount(int val)
    {
        GameState.PlayerCount = Rules.MAX_PLAYER_COUNT - val;
    }
}
