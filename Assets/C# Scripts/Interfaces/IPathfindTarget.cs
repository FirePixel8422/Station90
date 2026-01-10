using Unity.Mathematics;


public interface IPathfindTarget
{
    public int GridFloorId { get; }
    public float3 Position { get; }
}