public class AuthorizeCommand : ActionCommand
{
    private DMeeple _meeple;
    private DPosition _position;
    private DPlayer _player;
    private DWorker _worker;

    public AuthorizeCommand(ulong issuerID, DMeeple meeple, DPlayer player, DWorker worker) : base(issuerID)
    {
        _meeple = meeple;
        _position = meeple.Position.Current;
        _player = player;
        _worker = worker;
    }

    public override void Execute()
    {   
        base.Execute();

        if (_meeple.GetType() == typeof(DVillager))
        {
            ((DVillager) _meeple).Authorize(_player.Id);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Authorize(_player.Id);
            GameState.Instance.PlayerById(_worker.Owner)?.Honor.Earn(honor);
        }
        
        _player.SaveMeeple(_meeple);


        // Call function CheckPriority()
        // if return == 1 then proceed
        // else Player.GetDisgrace, then proceed
        // TODO Authorize: store away piece instead of destroy
        // S.R. Should we use a GameCommand class that contains helper commands
        // TODO: Notes for StoreAway commmand. Will need to redirect piece to correct owner       
        // Destroy(meeple.gameObject);


        // S.R. Old logic, unnecessary as of new FIFO model
        //TempStack.add(meeple);
        /*if(meeple is Child)
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
            }
        }

        //TODO: Add all the remaining meeples in the stack to the current player pool
        //Since foreign knights already removed, only current player pieces are left
        //CurrentPlayer.Meeples.add(TempStack);
        */
    }

    public override void Reverse()
    {
        base.Reverse();

        if (_meeple.GetType() == typeof(DVillager))
        {
            ((DVillager) _meeple).Deauthorize(_position);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Deauthorize(_position, _player.Id);
            GameState.Instance.PlayerById(_worker.Owner)?.Honor.Lose(honor);
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
