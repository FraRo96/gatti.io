using Unity.Entities;

[GenerateAuthoringComponent]
public struct PointsData : IComponentData
{
    public int DestroyedAllies;
    public int SavedAllies;
    public int PowerUpPoints;
}

