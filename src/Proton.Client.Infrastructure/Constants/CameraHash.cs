using AltV.Net.Client;
using Proton.Client.Core.Interfaces;

namespace Proton.Client.Infrastructure.Constants;

public class CameraHash : ICameraHash
{
    public uint Hash { get; }

    private CameraHash(uint hash)
    {
        Hash = hash;
    }

    public static readonly CameraHash Scripted = new(Alt.Hash("DEFAULT_SCRIPTED_CAMERA"));
}
