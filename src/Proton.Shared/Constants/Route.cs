namespace Proton.Shared.Contants;

public sealed class Route : IEquatable<Route>
{
    public string Value { get; }

    private Route(string value)
    {
        Value = value;
    }

    public static readonly Route RaceCreator = new("race-creator");
    public static readonly Route RaceMainMenuList = new("racing_menu_list");
    public static readonly Route Auth = new("auth");
    public static readonly Route RaceCountdown = new("race-countdown");
    public static readonly Route RacePrepare = new("race-prepare");
    public static readonly Route RaceFinishCountdown = new("race-finish-countdown");
    public static readonly Route CharacterCreator = new("character-creator");
    public static readonly Route Speedometer = new("speedometer");
    public static readonly Route RaceHud = new("race-hud");
    public static readonly Route RaceFinishScoreboard = new("race-finish-scoreboard");
    public static readonly Route RaceStartCountdown = new("race-start-countdown");

    public override bool Equals(object? obj) => Equals(obj as Route);

    public bool Equals(Route? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Route lhs, Route rhs)
    {
        if (lhs is null)
        {
            return rhs is null;
        }
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Route lhs, Route rhs) => !(lhs == rhs);
}
