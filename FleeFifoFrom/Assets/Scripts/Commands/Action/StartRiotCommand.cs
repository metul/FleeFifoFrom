using UnityEngine;

public class StartRiotCommand : ActionCommand 
{
  private DMeeple _meeple;

  public StartRiotCommand(
    ulong issuerID,
    DPlayer player,
    DWorker worker,
    DMeeple meeple
  ) : base(issuerID, player, worker)
  {
    _actionId = DActionPosition.TileId.Riot;
    _meeple = meeple;
  }

  public override void Execute()
  {
    base.Execute();
    GameState.Instance.PlayerById(_worker.Owner)?.Honor.Lose();
  }

  public override void Reverse()
  {
    base.Reverse();
    GameState.Instance.PlayerById(_worker.Owner)?.Honor.Earn();
  }

  public override bool IsFeasible()
  {
    return base.IsFeasible() && _meeple.GetType() == typeof(DKnight);
  }
}