using Unity.Entities;

[GenerateAuthoringComponent]
struct SpeedData : IComponentData
{
    public float PreferredSpeed;
    public float BrakeSpeed;
}