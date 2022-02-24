namespace UhgeEngine.Core;

public class Component
{
    public bool IsStarted { get; private set; }

    public Entity Entity
    {
        get => _entity ?? throw new NullReferenceException("Could not get entity associated with component");
        internal set => _entity = value;
    }

    public bool HasEntity => _entity != null;

    private Entity? _entity;

    public virtual void Start()
    {
        IsStarted = true;
    }

    public virtual void Tick(float deltaTime)
    {
    }

    public virtual void End()
    {
        IsStarted = false;
    }
}