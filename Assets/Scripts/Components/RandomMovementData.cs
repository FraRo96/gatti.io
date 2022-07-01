using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RandomMovementData : IComponentData
{
    public float3 To;
    public float Speed;
    public float DistToChangePath;
}
