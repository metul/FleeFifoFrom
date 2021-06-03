using UnityEngine;

public class StartRiotCommand : ActionCommand 
{
  private DMeeple _meeple;

  public StartRiotCommand(
    ulong issuerID,
    DPlayer.ID playerId,
    DWorker worker,
    DMeeple meeple
  ) : base(issuerID, playerId, worker)
  {
    _actionId = DActionPosition.TileId.Riot;
    _meeple = meeple;
  }

  public override void Execute()
  {
    base.Execute();
    GameState.Instance.PlayerById(_worker.Owner)?.Honor.Lose(1);
  }

  public override void Reverse()
  {
    base.Reverse();
    GameState.Instance.PlayerById(_worker.Owner)?.Honor.Earn(1);
  }

  public override bool IsFeasibile()
  {
    return _meeple.GetType() == typeof(DKnight);
  }
}