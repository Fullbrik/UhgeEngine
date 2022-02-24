namespace UhgeEngine.Core;

public class Entity
{
    public bool IsStarted { get; private set; }

    private readonly List<Component> _components = new();

    public void RegisterComponent(Component component)
    {
        if (component.IsStarted || component.HasEntity) return;

        _components.Add(component);
        if (IsStarted) component.Start();
    }

    public virtual void Start()
    {
        IsStarted = true;
        ForEachComponent((c) => c.Start());
    }

    public virtual void Tick(float deltaTime)
    {
        ForEachComponent((c) => c.Tick(deltaTime));
    }

    public virtual void End()
    {
        ForEachComponent((c) => c.End());
    }

    private void ForEachComponent(Action<Component> iter)
    {
        // We copy the list first because it might change during the update.
        var components = new Component[_components.Count];
        _components.CopyTo(components);
        foreach (var component in components)
        {
            iter(component);
        }
    }
}