using System;

[Serializable]
public class DActionPosition
{
    public enum TileId
    {
        Authorize,
        Swap,
        Riot,
        Revive,
        Objective
    }

    public TileId? Tile;
    public DPlayer.ID? Player;

    public bool IsValid
    {
        get => (Tile != null ^ Player != null);
    }
    
    public bool IsPlayerTile
    {
        get => Tile == null && Player != null;
    }
    
    public bool IsActionTile
    {
        get => Tile != null && Player == null;
    }

    public DActionPosition(TileId tileId)
    {
        Tile = tileId;
        Player = null;
        
        if (!IsValid)
        {
            throw new System.Exception($"Invalid Action Position: {Tile}, {Player}");
        }
    }

    public DActionPosition(DPlayer.ID playerId)
    {
        Tile = null;
        Player = playerId;
        
        if (!IsValid)
        {
            throw new System.Exception($"Invalid Action Position: {Tile}, {Player}");
        }
    }

    public bool Equals(DActionPosition other)
    {
        return (IsValid && other.IsValid && other.Tile == Tile && other.Player == Player);
    }

    public override string ToString()
    {
        return $"[Tile: {Tile}, Player: {Player}]";
    }
}
