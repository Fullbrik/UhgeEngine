namespace UhgeEngine.Core;

public class World
{
    private readonly List<Entity> _entities = new();

    public void Tick(float deltaTime)
    {
        var entities = new Entity[_entities.Count];
        _entities.CopyTo(entities);
        foreach (var entity in entities)
        {
            entity.Tick(deltaTime);
        }
    }
}