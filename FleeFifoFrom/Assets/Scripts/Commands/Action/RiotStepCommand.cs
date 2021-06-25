using System.Collections.Generic;
using MLAPI.Serialization;

public class RiotStepCommand : ActionCommand, INetworkSerializable
{
    protected DKnight _knight;
    protected DPosition _to;
    protected DPosition _from;
    protected DMeeple[] _onTheWay;
    protected DMeeple[] _coriotors;
    protected Dictionary<ushort, DPosition> _coriotorPositions;

    // Default constructor needed for serialization
    public RiotStepCommand() : base() { }

    public RiotStepCommand(
        ulong issuerID,
        DPlayer player,
        DKnight knight,
        DPosition to
    ) : base(issuerID, player, null)
    {
        _knight = knight;
        _to = to;
        _from = _knight.Position.Current;
        _onTheWay = GameState.Instance.AllAtPosition(_to);
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

        foreach (var meeple in _onTheWay)
        {
            if (meeple.GetType().IsSubclassOf(typeof(DVillager)))
            {
                ((DVillager) meeple).Injure();
                GameState.Instance.PlayerById(_player.Id)?.Honor.Lose();
            }
        }

        _knight.Position.Current = _to;
        foreach (var coriotor in _coriotors)
        {
            coriotor.Position.Current = _from;
            coriotor.IsRioting.Current = true;
        }
    }

    public override void Reverse()
    {
        base.Reverse();

        foreach (var coriotor in _coriotors)
        {
            coriotor.Position.Current = _coriotorPositions[coriotor.ID];

            // TODO check if they joined recently
            coriotor.IsRioting.Current = false;
        }

        _knight.Position.Current = _from;


        foreach (var meeple in _onTheWay)
        {
            if (meeple.GetType().IsSubclassOf(typeof(DVillager)))
            {
                ((DVillager) meeple).Heal();
                GameState.Instance.PlayerById(_player.Id)?.Honor.Earn();
            }
        }
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
               && GameState.Instance.AllAtPosition(
                   _to,
                   m => m.IsInjured() || m.GetType() == typeof(DKnight)
               ).Length == 0;
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        ushort knightID = ushort.MaxValue;
        if (!serializer.IsReading)
            knightID = _knight.ID;

        serializer.Serialize(ref knightID);

        if (serializer.IsReading)
            _knight = (DKnight)RegistryManager.Instance.Request(knightID);

        if (serializer.IsReading)
        {
            _to = new DPosition();
            _from = new DPosition();
        }

        _to.NetworkSerialize(serializer);
        _from.NetworkSerialize(serializer);

        int onTheWayLength = 0;
        if (!serializer.IsReading)
            onTheWayLength = _onTheWay.Length;

        serializer.Serialize(ref onTheWayLength);

        if (serializer.IsReading)
            _onTheWay = new DMeeple[onTheWayLength];

        for (int i = 0; i < onTheWayLength; i++)
        {
            ushort meepleID = ushort.MaxValue;
            if (!serializer.IsReading)
                meepleID = _onTheWay[i].ID;

            serializer.Serialize(ref meepleID);

            if (serializer.IsReading)
                _onTheWay[i] = (DMeeple)RegistryManager.Instance.Request(meepleID); // TODO (metul): Do we need further type casting down the line (e.g. villager)?
        }

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