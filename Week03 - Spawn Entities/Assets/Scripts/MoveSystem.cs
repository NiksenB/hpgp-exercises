using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        double elapsedTime = SystemAPI.Time.ElapsedTime;

        // MOVE
        foreach (var (transform, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>())
        {
            state.Dependency = new ShakeCubeJob
            {
                ElapsedTime = elapsedTime,
            }.Schedule(state.Dependency);
            
            state.Dependency = new RotateCubeJob()
            {
                ElapsedTime = elapsedTime,
                DeltaTime = deltaTime,
            }.Schedule(state.Dependency);
        }

        // MOVE AND SHAKE
        // foreach (var (transform, moveComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveComponent>>().WithPresent<RotateTag>())
        // {
        //     state.Dependency = new ShakeAndRotateCubeJob
        //     {
        //         ElapsedTime = elapsedTime,
        //         DeltaTime = deltaTime
        //     }.Schedule(state.Dependency);
        // }
    }
}

[BurstCompile]
internal partial struct ShakeCubeJob : IJobEntity
{
    public double ElapsedTime;

    private void Execute(ref LocalTransform transform, in MoveComponent moveData)
    {
        var newXPosition = math.sin((float)ElapsedTime * moveData.speed) * (float)0.00005;
        transform.Position.x += newXPosition;
    }
}

[BurstCompile]
internal partial struct RotateCubeJob : IJobEntity
{
    public float DeltaTime;
    public double ElapsedTime;

    private void Execute(ref LocalTransform transform, in MoveComponent moveData, in RotateTag tag)
    {
        // var newXPosition = math.sin((float)ElapsedTime * moveData.speed) * DeltaTime;
        // transform.Position.x += newXPosition;
        var newRotation = quaternion.RotateZ(moveData.speed * math.TORADIANS * DeltaTime * (float)0.005);
        transform.Rotation = math.mul(transform.Rotation, newRotation);
    }
}