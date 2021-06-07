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
        
        StateManager.OnStateUpdate += UpdateInteractability;
        GameState.Instance.OnTurnChange += turn =>
        {
            var isActionTurn = turn == GameState.TurnTypes.ActionTurn;
            _actionCanvas.SetActive(isActionTurn);
            _resetCanvas.SetActive(!isActionTurn);
            UpdateInteractability();
            CommandProcessor.Instance.ClearStack();
            _fieldManager.EndTurnReset();
            // TODO highlight current player
            Debug.Log($"New turn started: {GameState.Instance.TurnPlayer().Name}'s {turn}");
        };

        GameState.Instance.OnUndo += () => UpdateInteractability();
        
        // starting with an action turn
        _resetCanvas.SetActive(false);
    }

    private void UpdateInteractability()
    {
        switch (StateManager.GameState)
        {
            case StateManager.State.CountermandDrawCard:
                EnableElements(false, false, false);
                break;
            case StateManager.State.CountermandSelectCard:
                EnableElements(false, false, false);
                break;
            case StateManager.State.PoachSelectWorker:
                EnableElements(false, true, false, true);
                break;
            case StateManager.State.PoachSelectCard:
                EnableElements(false, false, false);
                break;
            case StateManager.State.Recall:
                EnableElements(true, false, false);
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
                CommandProcessor.Instance.ExecuteCommand(new CooperateCommand(
                    0, GameState.Instance.TurnPlayer().Id, worker.Core));
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.PoachSelectWorker:
                CommandProcessor.Instance.ExecuteCommand(new PoachCommand(
                    0, GameState.Instance.TurnPlayer().Id, worker.Core));
                StateManager.GameState = StateManager.State.Default;
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
        StateManager.GameState = StateManager.State.Authorize;
    }

    public void Swap()
    {
        StateManager.GameState = StateManager.State.Swap1;
    }

    public void Riot()
    {
        StateManager.GameState = StateManager.State.RiotChooseKnight;
    }

    public void Revive()
    {
        StateManager.GameState = StateManager.State.Revive;
    }

    public void Objective()
    {
        Debug.Log("TODO: Player chose the objective action");
    }

    public void Countermand()
    {
        StateManager.GameState = StateManager.State.CountermandDrawCard;
    }

    public void Reprioritize()
    {
        StateManager.GameState = StateManager.State.Reprioritize;
    }

    public void Retreat()
    {
        StateManager.GameState = StateManager.State.RetreatChooseKnight;
    }

    public void Recall()
    {
        StateManager.GameState = StateManager.State.Recall;
    }

    public void Cooperate()
    {
        StateManager.GameState = StateManager.State.Cooperate;
    }

    public void Poach()
    {
        StateManager.GameState = StateManager.State.PoachSelectWorker;
    }

    public void Villager()
    {
        StateManager.GameState = StateManager.State.Villager;
    }

    public void Undo()
    {
        if(StateManager.GameState == StateManager.State.Default)
            CommandProcessor.Instance.Undo();
        else
            StateManager.GameState = StateManager.State.Default;
    }

    public void EndTurn()
    {
        GameState.Instance.RotateTurn();
    }
    
    #endregion
}