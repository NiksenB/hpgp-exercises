using Unity.Entities;
using UnityEngine;

class MyBaker : MonoBehaviour
{
    public int numberOfSeats = 0;
    public float speed = 2f;
}

class MyBakerBaker : Baker<MyBaker>
{
    public override void Bake(MyBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MyPizzaria
        {
            speed = authoring.speed,
            numberOfSeats = authoring.numberOfSeats,
            numberOfPizzas = 0
        });
    }
}

struct MyPizzaria : IComponentData
{
    public int numberOfSeats;
    public float speed;
    public int numberOfPizzas;
}

struct ChosenOneTag : IComponentData
{
    
}