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
        foreach (var (transform, myPizzaria) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MyPizzaria>>())
        {
            // Time.time increases infinitely
            // 0.1*transform.ValueRW.Position.y moves between -0.5 and 0.5
            // math.sin(...) returns between 1 and -1
            transform.ValueRW.Position.y = myPizzaria.ValueRO.speed * math.sin ((float)SystemAPI.Time.ElapsedTime -(float)0.1*transform.ValueRW.Position.y);
        }
        
        // Chosen ones rotate
        foreach (var (transform, myPizzaria) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MyPizzaria>>().WithAll<ChosenOneTag>())
        {
            transform.ValueRW.Rotation.value -= new float4(0.25f, 0.25f, 0.25f, 1.0f);
        }
    }
}
