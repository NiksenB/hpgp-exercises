using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

class MoveBaker : MonoBehaviour
{
    public float speed = 2f;
    
    class Baker : Baker<MoveBaker>
    {
        public override void Bake(MoveBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveComponent()
            {
                speed = authoring.speed,
            });
        }
    }
}

struct MoveComponent : IComponentData
{
    public float speed;
}