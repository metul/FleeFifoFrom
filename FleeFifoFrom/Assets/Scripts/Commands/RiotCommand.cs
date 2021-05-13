using System.Collections.Generic;
using System.Linq;

public class RiotCommand : ActionCommand
{
    private List<Tile> _path;
    private List<Meeple.State> _originalStates;

    public RiotCommand(ulong issuerID, List<Tile> path) : base(issuerID)
    {
        _path = path;
    }

    public override void Execute()
    {
        base.Execute();
        _originalStates = new List<Meeple.State>();
        // Debug
        foreach (var tile in _path)
        {
            _originalStates.Add(tile.Meeple.CurrentState);
            tile.Meeple.CurrentState = Meeple.State.Injured;
        }
    }

    public override void Reverse()
    {
        base.Reverse();
        // Set each tile back to original state
        foreach (var (tile, index) in _path.Select((element, index) => (element, index)))
            tile.Meeple.CurrentState = _originalStates[index];
    }
}
