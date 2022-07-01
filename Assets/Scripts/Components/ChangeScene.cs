using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ChangeScene : IComponentData
{
    public bool Value;
}