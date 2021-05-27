public class AuthorizeCommand : ActionCommand
{
    private DMeeple _meeple;
    private DPosition _position;


    public AuthorizeCommand(ulong issuerID, DMeeple meeple) : base(issuerID)
    {
        _meeple = meeple;
        _position = meeple.Position.Current;
    }

    public override void Execute()
    {   
        base.Execute();

        // TODO Authorize: store away piece
        _meeple.Position.Current = null;

        // S.R. Should we use a GameCommand class that contains helper commands
        // TODO: Notes for StoreAway commmand. Will need to redirect piece to correct owner
        //TempStack.add(meeple);
        // Destroy(meeple.gameObject);

        // if(meeple is Child)
        {
            //TODO: Simplied. If no adult exists in the temp stack
            //if(TempStack.contains(Knight) || TempStack.contains(Commoner) || TempStack.contains(Elder))
            //break;
            //else CurrentPlayer.SelectedWorker.Player.Disgrace();
        }

        // if (meeple is Elder)
        {   
            //if CurrentPlayer.PlayedWorkers == 2
            //CurrentPlayer.SelectedWorker.Player.Honor();
            //else CurrentPlayer.SelectedWorker.Player.Disgrace();
        }

        // if (meeple is Knight)
        {   
            //Add the Knight to the player to whom the knight belongs
            //meeple.owner.add(meeple);
            //TempStack.remove(meeple)
            
            //TODO: Increment honor if player moved foreign knight
            /*if (meeple.owner!=CurrentPlayer)
            {
                CurrentPlayer.SelectedWorker.Player.Disgrace();
            }*/
        }

        //TODO: Add all the remaining meeples in the stack to the current player pool
        //Since foreign knights already removed, only current player pieces are left
        //CurrentPlayer.Meeples.add(TempStack);
    }

    public override void Reverse()
    {
        base.Reverse();
        // TODO revert points given to player etc.
        _meeple.Position.Current = _position;
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
