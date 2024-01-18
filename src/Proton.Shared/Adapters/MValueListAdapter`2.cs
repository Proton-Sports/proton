using AltV.Net;
using AltV.Net.Elements.Args;

namespace Proton.Shared.Adapters;

public sealed class MValueListAdapter<TModel, TAdapter> : IMValueAdapter<List<TModel>>
    where TAdapter : IMValueAdapter<TModel>, new()
{
    public static readonly MValueListAdapter<TModel, TAdapter> Instance = new();
    private readonly IMValueAdapter<List<TModel>> listAdapter = DefaultMValueAdapters.GetArrayAdapter(new TAdapter());

    public List<TModel> FromMValue(IMValueReader reader)
    {
        return listAdapter.FromMValue(reader);
    }

    public void ToMValue(List<TModel> value, IMValueWriter writer)
    {
        listAdapter.ToMValue(value, writer);
    }

    public void ToMValue(object obj, IMValueWriter writer)
    {
        switch (obj)
        {
            case List<TModel> list:
                {
                    ToMValue(list, writer);
                    break;
                }
            case IEnumerable<TModel> enumerable:
                {
                    ToMValue(enumerable.ToList(), writer);
                    break;
                }
        }
    }

    object IMValueBaseAdapter.FromMValue(IMValueReader reader)
    {
        return FromMValue(reader);
    }
}
