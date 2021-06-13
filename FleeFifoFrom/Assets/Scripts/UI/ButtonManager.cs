using MLAPI;
using MLAPI.Logging;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Worker _workerPrefab;
    [SerializeField] private PlayerTile _playerTilePrefab;

    [SerializeField] private GameObject _actionCanvas;
    [SerializeField] private GameObject _resetCanvas;
    
    // TODO include priority canvas
    [SerializeField] private GameObject _priorityCanvas;

    [SerializeField] private Button[] _actionButtons;
    [SerializeField] private Button[] _resetButtons;
    [SerializeField] private Button _villagerButton;
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
        
        StateManager.OnStateUpdate += state => UpdateInteractability();
        GameState.Instance.OnTurnChange += turn =>
        {
            UpdateInteractability();
            CommandProcessor.Instance.ClearStack();
            _fieldManager.EndTurnReset();
            // TODO highlight current player
            Debug.Log($"New turn started: {GameState.Instance.TurnPlayer().Name}'s {turn}");
        };

        GameState.Instance.OnUndo += () => UpdateInteractability();
        PlayerManager.Instance.OnRequiredPlayersReached += () => UpdateInteractability(); // MARK: Unsubscribe?
    }

    private void UpdateInteractability()
    {
        // DEBUG
        if (NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsServer)
        {
            Debug.Log("DEBUG");
            if (PlayerManager.Instance.NetworkPlayerIDs[NetworkManager.Singleton.LocalClientId] == GameState.Instance.TurnPlayer().Id)
                NetworkLog.LogInfoServer($"Client {NetworkManager.Singleton.LocalClientId} " +
                    $"(Player {PlayerManager.Instance.NetworkPlayerIDs[NetworkManager.Singleton.LocalClientId]}) says: 'this my turn g'");
            else
                NetworkLog.LogInfoServer($"Client {NetworkManager.Singleton.LocalClientId} " +
                    $"(Player {PlayerManager.Instance.NetworkPlayerIDs[NetworkManager.Singleton.LocalClientId]}) says: 'ima wait'");
        }
        switch (StateManager.CurrentState)
        {
            case StateManager.State.CountermandDrawCard:
                EnableElements(false, false, false);
                break;
            case StateManager.State.CountermandSelectCard:
                EnableElements(false, false, false);
                break;
            case StateManager.State.PoachSelectWorker:
                EnableElements(false, true, false, true);
                // _actionCanvas.SetActive(true);
                break;
            case StateManager.State.PoachSelectCard:
                EnableElements(false, false, false);
                break;
            case StateManager.State.Recall:
                EnableElements(true, false, false);
                // _actionCanvas.SetActive(true);
                break;
            case StateManager.State.Cooperate:
                EnableElements(false, true, false, true);
                break;
            case StateManager.State.PayForAction:
                EnableElements(false, false, true);
                break;
            case StateManager.State.Default:
                EnableElements(false, false, false, buttons: true);
                break;
            default:
                EnableElements(false, false, false);
                break;
        }
    }

    private void EnableElements(bool tiles, bool worker, bool playerWorker, bool opponentTileWorkers = false, bool buttons = false)
    {
        foreach (var actionTile in _actionTiles)
        {
            actionTile.Interactable = tiles;
            if(opponentTileWorkers)
                actionTile.SetOpponentWorkerInteractable(worker, GameState.Instance.TurnPlayer());
            else
                actionTile.SetWorkerInteractable(worker);
            
        }

        foreach (var playerTile in _playerTiles)
        {
            var currentPlayer = GameState.Instance.TurnPlayer().Id == playerTile.Id;
            playerTile.SetWorkerInteractable(playerWorker && currentPlayer);
        }
        
        var actionButtons = buttons 
                     && GameState.Instance.TurnActionPossible 
                     && GameState.Instance.TurnType == GameState.TurnTypes.ActionTurn;

        foreach (var actionButton in _actionButtons)
            actionButton.interactable = actionButtons;

        var resetButtons = buttons 
                           && GameState.Instance.TurnActionPossible 
                           && GameState.Instance.TurnType == GameState.TurnTypes.ResetTurn;
        
        foreach (var resetButton in _resetButtons)
            resetButton.interactable = resetButtons;

        _villagerButton.interactable = buttons && GameState.Instance.TurnType == GameState.TurnTypes.ResetTurn;
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
        switch (StateManager.CurrentState)
        {
            case StateManager.State.Recall:
                CommandProcessor.Instance.ExecuteCommand(new RecallCommand(0, actionTile.Id));
                StateManager.CurrentState = StateManager.State.Default;
                break;
            default:
                Debug.LogWarning("This is not a state in which action tile interaction is allowed!");
            break;
        }
    }

    public void OnWorkerClick(Worker worker)
    {
        switch (StateManager.CurrentState)
        {
            case StateManager.State.Cooperate:
                CommandProcessor.Instance.ExecuteCommand(new CooperateCommand(
                    0, GameState.Instance.TurnPlayer().Id, worker.Core));
                StateManager.CurrentState = StateManager.State.Default;
                break;
            case StateManager.State.PoachSelectWorker:
                CommandProcessor.Instance.ExecuteCommand(new PoachCommand(
                    0, GameState.Instance.TurnPlayer().Id, worker.Core));
                StateManager.CurrentState = StateManager.State.Default;
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
        StateManager.CurrentState = StateManager.State.Authorize;
    }

    public void Swap()
    {
        StateManager.CurrentState = StateManager.State.Swap1;
    }

    public void Riot()
    {
        StateManager.CurrentState = StateManager.State.RiotChooseKnight;
    }

    public void Revive()
    {
        StateManager.CurrentState = StateManager.State.Revive;
    }

    public void Objective()
    {
        Debug.Log("TODO: Player chose the objective action");
    }

    public void Countermand()
    {
        StateManager.CurrentState = StateManager.State.CountermandDrawCard;
    }

    public void Reprioritize()
    {
        StateManager.CurrentState = StateManager.State.Reprioritize;
    }

    public void Retreat()
    {
        StateManager.CurrentState = StateManager.State.RetreatChooseKnight;
    }

    public void Recall()
    {
        StateManager.CurrentState = StateManager.State.Recall;
    }

    public void Cooperate()
    {
        StateManager.CurrentState = StateManager.State.Cooperate;
    }

    public void Poach()
    {
        StateManager.CurrentState = StateManager.State.PoachSelectWorker;
    }

    public void Villager()
    {
        StateManager.CurrentState = StateManager.State.Villager;
    }

    public void Undo()
    {
        if(StateManager.CurrentState == StateManager.State.Default)
            CommandProcessor.Instance.Undo();
        else
            StateManager.CurrentState = StateManager.State.Default;
    }

    public void EndTurn()
    {
        GameState.Instance.RotateTurn();
    }
    
    #endregion
}