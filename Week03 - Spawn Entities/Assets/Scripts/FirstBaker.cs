using Unity.Entities;
using UnityEngine;

class FirstBaker : MonoBehaviour
{
    public float speed = 2f;
}

class MoverBaker : Baker<FirstBaker>
{
    public override void Bake(FirstBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MoveComponent()
        {
            speed = authoring.speed,
        });
    }
}

struct MoveComponent : IComponentData
{
    public float speed;
}

struct RotateTag : IComponentData
{
    
}