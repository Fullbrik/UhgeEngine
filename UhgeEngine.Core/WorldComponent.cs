namespace UhgeEngine.Core;

public class WorldComponent : Component
{
    public WorldComponent? Parent { get; set; }
    
    public Vector3 Position { get; set; }
    public Rotator Rotation { get; set; }
    public Vector3 Scale { get; set; }

    public WorldComponent[] Children
    {
        init
        {
            foreach (var component in value)
            {
                AttachChild(component);
            }
        }
    }

    private readonly List<WorldComponent> _children = new();

    public void AttachChild(WorldComponent component)
    {
        if (_children.Contains(component)) return;
        
        _children.Add(component);
        component.Parent = this;
    }
}