namespace Example.Construction;

public sealed class PrimitiveSubject
{
    private readonly int _count;

    public PrimitiveSubject(int count)
    {
        _count = count;
    }

    public int Measure()
    {
        return _count;
    }
}
