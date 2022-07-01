using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FollowGameObject : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject Follower;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        FollowEntity followEntity = Follower.GetComponent<FollowEntity>();
        conversionSystem.DeclareDependency(gameObject, followEntity);
        followEntity.EntityToFollow = entity;
        conversionSystem.DeclareDependency(gameObject, Follower);        
    }
}
