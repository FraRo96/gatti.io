using Unity.Entities;

[GenerateAuthoringComponent]
public struct IsSavedData : IComponentData
{
    public bool Value;
}
