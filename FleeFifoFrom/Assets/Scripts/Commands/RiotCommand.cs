using System.Collections.Generic;
using System.Linq;

public class RiotCommand : ActionCommand
{
    private List<Tile> _path;
    // private List<Meeple.State> _originalStates;

    public RiotCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, List<Tile> path) : base(issuerID, playerId, worker)
    {
        _path = path;
    }

    public override void Execute()
    {
        base.Execute();
        // _originalStates = new List<Meeple.State>();

        //meeple = selected knight
        //follower = low prio
        //LowPrio = List<Meeple>
        //for(set of all meeples)
        //if Meeple.Prio == Low
        //LowPrio.add(Meeple)

        // Debug
        // foreach (var tile in _path)
        // {
        //     _originalStates.Add(tile.Meeple.CurrentState);
        //     tile.Meeple.CurrentState = Meeple.State.Injured;
        // }

        //TempStack.add(meeple);
        //for each follower TempStack.add(follower);

        //meeple.owner.add(meeple);
        //TempStack.remove(meeple)

        //TODO: Increment honor if player moved foreign knight
        /*if (meeple.owner!=CurrentPlayer)
        {
            CurrentPlayer.SelectedWorker.Player.Disgrace();
        }*/

        //TODO: Add all the remaining meeples in the stack to the current player pool
        //Since foreign knights already removed, only current player pieces are left
        //CurrentPlayer.Meeples.add(TempStack);


    }

    public override void Reverse()
    {
        base.Reverse();
        // Set each tile back to original state
        // foreach (var (tile, index) in _path.Select((element, index) => (element, index)))
        //     tile.Meeple.CurrentState = _originalStates[index];
    }

    public override bool IsFeasibile()
    {
        //TODO Step 1: Start loop from tile 1
        /* pseudo
        if (piece.exists)
        {
            if (piece.injured)
            {
                injuredPiecesOnRow++;
            }
            elseif (piece is Knight)
            {
                //TODO: i.e. there exists a knight with line of access
                this.ActionPossible = true;
                break;
            }
            if (injuredPiecesOnRow >= RowSize)
            {   
                //An entire row is blocked
                this.ActionPossible = false;
                break;
            }
        }
        else (move to next tile)

        //TODO: end loop
        */
        return true;
    }


}
