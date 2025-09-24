using Unity.Entities;
using UnityEngine;

class RotateBaker : MonoBehaviour
{
    
}

class Baker : Baker<RotateBaker>
{
    public override void Bake(RotateBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RotateTag());
    }
}

struct RotateTag : IComponentData
{
    
}