namespace BlitGS.Demo;

public abstract class Scene(string name)
{
    public string Name { get; } = name;

    public abstract void Load();
    
    public abstract void Update(float dt);

    public abstract void FixedUpdate(float dt);
    
    public abstract void Frame(float dt);
    
    
}