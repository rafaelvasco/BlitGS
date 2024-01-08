using System;

namespace BlitGS.Engine;

public abstract class Disposable : IDisposable
{
    ~Disposable()
    {
        Dispose();
        Console.WriteLine($"Not properly disposed: {GetType()}");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Free();
    }

    protected abstract void Free();
}