using System.Collections.Generic;
using MLAPI.Serialization;

public class RiotAuthorizeCommand : ActionCommand, INetworkSerializable
{
    protected DKnight _knight;
    protected DPosition _from;
    protected DMeeple[] _coriotors;
    protected Dictionary<ushort, DPosition> _coriotorPositions;

    // Default constructor needed for serialization
    public RiotAuthorizeCommand() {}
    
    public RiotAuthorizeCommand(
        ulong issuerID,
        DPlayer player,
        DKnight knight,
        DPosition to
    ) : base(issuerID, player, null)
    {
        _knight = knight;
        _from = _knight.Position.Current;
        _coriotors = GameState.Instance.AllAtPositions(
            _from.Predecessors(),
            m => m.GetType().IsSubclassOf(typeof(DVillager)) && !m.IsInjured()
        );

        _coriotorPositions = new Dictionary<ushort, DPosition>();
        foreach (var coriotor in _coriotors)
        {
            _coriotorPositions[coriotor.ID] = coriotor.Position.Current;
        }
    }

    public override void Execute()
    {
        base.Execute();

        var honor = _knight.Authorize(_player.Id);
        GameState.Instance.PlayerById(_player.Id)?.Honor.Earn(honor);

        foreach (var coriotor in _coriotors)
        {
            ((DVillager) coriotor).Authorize(_player.Id);

            // done rioting
            coriotor.IsRioting.Current = false;
        }
    }

    public override void Reverse()
    {
        base.Reverse();

        foreach (var coriotor in _coriotors)
        {
            ((DVillager) coriotor).Deauthorize(_coriotorPositions[coriotor.ID]);

            // no rioting for no one, 
            coriotor.IsRioting.Current = false;
        }

        var honor = _knight.Deauthorize(_from, _player.Id);
        GameState.Instance.PlayerById(_player.Id)?.Honor.Lose(honor);
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible();
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        // TODO (Anas-Mert) serialize RiotAuthorizeCommand
        
        base.NetworkSerialize(serializer);
        
        ushort knightID = ushort.MaxValue;
        if (!serializer.IsReading)
            knightID = _knight.ID;
        
        serializer.Serialize(ref knightID);
        
        if (serializer.IsReading)
            _knight = (DKnight)RegistryManager.Instance.Request(knightID);
        
        if (serializer.IsReading)
        {
            _from = new DPosition();
        }
        
        _from.NetworkSerialize(serializer);

        int coriotersLength = 0;
        if (!serializer.IsReading)
            coriotersLength = _coriotors.Length;

        serializer.Serialize(ref coriotersLength);

        if (serializer.IsReading)
            _coriotors = new DMeeple[coriotersLength];

        for (int i = 0; i < coriotersLength; i++)
        {
            ushort meepleID = ushort.MaxValue;
            if (!serializer.IsReading)
                meepleID = _coriotors[i].ID;

            serializer.Serialize(ref meepleID);

            if (serializer.IsReading)
                _coriotors[i] = (DMeeple)RegistryManager.Instance.Request(meepleID); // TODO (metul): Do we need further type casting down the line (e.g. villager)?
        }

        if (serializer.IsReading)
        {
            _coriotorPositions = new Dictionary<ushort, DPosition>();
            foreach (var corioter in _coriotors)
                _coriotorPositions[corioter.ID] = corioter.Position.Current;
        }
    }
}