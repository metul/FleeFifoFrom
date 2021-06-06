public class AuthorizeCommand : ActionCommand
{
    private readonly DMeeple _meeple;
    private readonly DPosition _position;

    public AuthorizeCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, DMeeple meeple) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Authorize;
        _meeple = meeple;
        _position = meeple.Position.Current;
    }

    public override void Execute()
    {   
        base.Execute();

        if (_meeple.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) _meeple).Authorize(_playerId);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Authorize(_playerId);
            
            // TODO: who gets honor with player != token owner
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Earn(honor);
        }
        
        // TODO save remove this
        // _player.SaveMeeple(_meeple);

        // Call function CheckPriority()
        // if return == 1 then proceed
        // else Player.GetDisgrace, then proceed
        // S.R. Should we use a GameCommand class that contains helper commands
        // Notes for StoreAway commmand. Will need to redirect piece to correct owner       
        
        //TempStack.add(meeple);
        // if (meeple is Knight)
        //{   
            //Add the Knight to the player to whom the knight belongs
            //meeple.owner.add(meeple);
            //TempStack.remove(meeple)
            
            //Increment honor if player moved foreign knight
            //if (meeple.owner!=CurrentPlayer)
            //{
            //    CurrentPlayer.SelectedWorker.Player.Disgrace();
            //}
        //}

        //Add all the remaining meeples in the stack to the current player pool
        //Since foreign knights already removed, only current player pieces are left
        //CurrentPlayer.Meeples.add(TempStack);
    }

    public override void Reverse()
    {
        base.Reverse();
        
        if (_meeple.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) _meeple).Deauthorize(_position);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Deauthorize(_position, _playerId);
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Lose(honor);
        }
    }

    public override bool IsFeasibile()
    {
        return true;
        //TODO Step 1: Start loop from tile 1
        /* Psuedo
        if(piece.exists)
        {
            if (piece.injured)
            {
                injuredPiecesOnRow++;
            }
            else
            {   
                //TODO: i.e. there exists at least one non-injured piece with line of access
                this.ActionPossible = true;
                break;
            }
            if (injuredPiecesOnRow >= RowSize)
            {
                this.ActionPossible = false;
                break;
            }            
        }
        else(move to next tile)

        //TODO: end loop
        */
    }
}
