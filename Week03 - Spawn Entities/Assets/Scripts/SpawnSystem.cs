using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct NewISystemScript : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var e = state.EntityManager.CreateEntity();
        state.EntityManager.SetName(e, "myEntity");
        
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>())
        {
            transform.ValueRW.Position.y = moveComponent.ValueRO.speed * math.sin ((float)SystemAPI.Time.ElapsedTime -(float)0.1*transform.ValueRW.Position.y);
        }
        
        foreach (var (transform, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>().WithAll<RotateTag>())
        {
            transform.ValueRW.Rotation.value -= new float4(0.25f, 0.25f, 0.25f, 1.0f);
        }
    }
}
