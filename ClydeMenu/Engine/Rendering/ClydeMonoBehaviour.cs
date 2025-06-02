namespace ClydeMenu.Engine.Rendering;

using System;

public abstract class ClydeMonoBehaviour : IDisposable //: MonoBehaviour
{
    public bool IsAlive { get; set; } = true;
        
    public virtual void Dispose() => throw new NotImplementedException();
}