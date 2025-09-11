using Unity.Burst;
using Unity.Entities;

partial struct NewISystemScript : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var e = state.EntityManager.CreateEntity();
        state.EntityManager.SetName(e, "myEntity");

    }
}
