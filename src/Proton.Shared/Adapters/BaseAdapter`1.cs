using AltV.Net;

namespace Proton.Shared.Adapters;

public abstract class BaseMValueAdapter<T> : IMValueAdapter<T>
{
    public abstract T FromMValue(IMValueReader reader);
    public abstract void ToMValue(T value, IMValueWriter writer);

    public void ToMValue(object obj, IMValueWriter writer)
    {
        if (obj is T value)
        {
            ToMValue(value, writer);
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader)!;
    }
}
