using System.Linq;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Worker _workerPrefab;
    [SerializeField] private PlayerTile _playerTilePrefab;
    
    [SerializeField] private ActionTile[] _actionTiles;
    [SerializeField] private Transform _playerTileAnchor;
    private PlayerTile[] _playerTiles;

    private FieldManager _fieldManager;

    // card & action tile states:
    // objective:   - draw card
    // countermand1 - draw card
    //              - (select card)
    // poach:       - select one opponent worker
    //              - select card
    // recall:      - select action tile
    // cooperate:   - select one opponent worker
    

    private void Start()
    {
        _fieldManager = FindObjectOfType<FieldManager>();
        
        // init player tiles
        _playerTiles = new PlayerTile[GameState.Instance.Players.Length];
        
        for (var i = 0; i < GameState.Instance.Players.Length; i++)
        {
            var playerTile = Instantiate(_playerTilePrefab, _playerTileAnchor);
            playerTile.Initialize(GameState.Instance.Players[i].Id);
            _playerTiles[i] = playerTile;

        }
        
        // init worker
        foreach (var dWorker in GameState.Instance.Workers)
        {
            var worker = Instantiate(_workerPrefab);
            worker.Initialize(dWorker, this);
        }
        
        StateManager.OnStateUpdate += UpdateInteractability;
    }

    private void UpdateInteractability()
    {
        switch (StateManager.GameState)
        {
            case StateManager.State.CountermandDrawCard:
                EnableElements(false, false, false, false);
                break;
            case StateManager.State.CountermandSelectCard:
                EnableElements(false, false, false, true);
                break;
            case StateManager.State.PoachSelectWorker:
                EnableElements(false, true, false, false);
                break;
            case StateManager.State.PoachSelectCard:
                EnableElements(false, false, false, true);
                break;
            case StateManager.State.Recall:
                EnableElements(true, false, false, false);
                break;
            case StateManager.State.Cooperate:
                EnableElements(false, true, false, false);
                break;
            case StateManager.State.PayForAction:
                EnableElements(false, false, true, false);
                break;
            default:
                EnableElements(false, false, false, false);
                break;
        }
    }

    private void EnableElements(bool tiles, bool worker, bool playerWorker, bool cards)
    {
        foreach (var actionTile in _actionTiles)
        {
            actionTile.Interactable = tiles;
            actionTile.SetWorkerInteractable(worker);
        }

        foreach (var playerTile in _playerTiles)
        {
            var currentPlayer = GameState.Instance.TurnPlayer().Id == playerTile.Id;
            playerTile.SetWorkerInteractable(playerWorker && currentPlayer);
        }
    }

    public UiTile ActionTileByPosition(DActionPosition position)
    {
        if (position.IsActionTile)
        {
            return _actionTiles.First(a => a.Id == position.Tile);
        }
        if(position.IsPlayerTile)
        {
            return _playerTiles.First(p => p.Id == position.Player);
        }
        return null;
    }
    
    public void OnActionTileClick(ActionTile actionTile)
    {
        Debug.Log("Action Tile has been clicked");
        switch (StateManager.GameState)
        {
            case StateManager.State.Recall:
                CommandProcessor.Instance.ExecuteCommand(new RecallCommand(0, actionTile.Id));
                StateManager.GameState = StateManager.State.Default;
                break;
            default:
                Debug.LogWarning("This is not a state in which action tile interaction is allowed!");
            break;
        }
    }

    public void OnWorkerClick(Worker worker)
    {
        switch (StateManager.GameState)
        {
            case StateManager.State.Cooperate:
                Debug.Log($"TODO: return worker {worker} to player {worker.Core.Owner}");
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.PoachSelectWorker:
                Debug.Log($"TODO: poach worker {worker} from player {worker.Core.Owner}");
                StateManager.GameState = StateManager.State.PoachSelectCard;
                break;
            case StateManager.State.PayForAction:
                _fieldManager.InvokeAction(StateManager.CurrentlyPayingFor, worker.Core);
                break;
            default:
                Debug.LogWarning("This is not a state in which worker interaction is allowed!");
                break;
        }
    }

    #region ui button onClicks

    public void Authorize()
    {
        Debug.Log("Player chose the authorize action");
        StateManager.GameState = StateManager.State.Authorize;
    }

    public void Swap()
    {
        Debug.Log("Player chose the swap action");
        StateManager.GameState = StateManager.State.Swap1;
    }

    public void Riot()
    {
        Debug.Log("Player chose the riot action");
        StateManager.GameState = StateManager.State.Riot;
    }

    public void Revive()
    {
        Debug.Log("Player chose the revive action");
        StateManager.GameState = StateManager.State.Revive;
    }

    public void Objective()
    {
        Debug.Log("Player chose the objective action");
    }

    public void Countermand()
    {
        Debug.Log("Player chose the countermand action");
        StateManager.GameState = StateManager.State.CountermandDrawCard;
    }

    public void Reprioritize()
    {
        Debug.Log("Player chose the reprioritize action");
        StateManager.GameState = StateManager.State.Reprioritize;
    }

    public void Retreat()
    {
        Debug.Log("Player chose the retreat action");
        StateManager.GameState = StateManager.State.RetreatChooseTile;
    }

    public void Recall()
    {
        Debug.Log("Player chose the recall action");
        StateManager.GameState = StateManager.State.Recall;
    }

    public void Cooperate()
    {
        Debug.Log("Player chose the cooperate action");
        StateManager.GameState = StateManager.State.Cooperate;
    }

    public void Poach()
    {
        Debug.Log("Player chose the poach action");
        StateManager.GameState = StateManager.State.PoachSelectCard;
    }

    public void Villager()
    {
        Debug.Log("Player chose the draw villager action");
        StateManager.GameState = StateManager.State.Villager;
    }

    public void Undo()
    {
        if(StateManager.GameState == StateManager.State.Default)
            CommandProcessor.Instance.Undo();
        
        // TODO add multi step command undo handling
        // return to default state and lose all accumulated progress
        // (-> if tile was selected but not payed yet)
        else
            StateManager.GameState = StateManager.State.Default;
    }

    public void EndTurn()
    {
        Debug.Log("End Player Turn");
        // TODO remove debug
        StateManager.GameState = StateManager.State.PayForAction;
    }
    
    #endregion
}