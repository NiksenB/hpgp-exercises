using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;

partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false; // Disable the system after first run
        

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();
        state.Dependency = new SpawnCubesJob
        {
            ECB = ecb,
        }.ScheduleParallel(state.Dependency);

        // var data = SystemAPI.GetSingleton<ConfigComponent>();
        // var instances = state.EntityManager.Instantiate(data.prefab, data.spawnCount, Allocator.Temp);
        // var random = new Random(42);
        // foreach (var entity in instances)
        // {
        //     var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
        //     transform.ValueRW.Position = random.NextFloat3((new float3(10, 10, 10)));
        // }

        // foreach (var (transform, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>())
        // {
        //     transform.ValueRW.Position.y = moveComponent.ValueRO.speed * math.sin ((float)SystemAPI.Time.ElapsedTime -(float)0.1*transform.ValueRW.Position.y);
        // }
        //
        // foreach (var (transform, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>().WithAll<RotateTag>())
        // {
        //     transform.ValueRW.Rotation.value -= new float4(0.25f, 0.25f, 0.25f, 1.0f);
        // }
    }
}

[BurstCompile]
partial struct SpawnCubesJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    private void Execute([ChunkIndexInQuery] int chunkIndex, in ConfigComponent config)
    {
        int n = config.spawnCount;
        for (int i = 0; i < n * n * n; i++)
        {
            var e = ECB.Instantiate(chunkIndex, config.prefab);
            float x = (i % n) * 2f;
            float y = ((i / n) % n) * 2f;
            float z = (i / (n * n)) * 2f;

            ECB.AddComponent(chunkIndex, e, LocalTransform.FromPosition(new float3(x, y, z)));

            Random r = new Random((uint)(i+1));
            if (r.NextBool())
            {
                ECB.AddComponent(chunkIndex, e, new RotateTag());
            }
        }
    }
}