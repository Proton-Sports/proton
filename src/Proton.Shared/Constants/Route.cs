namespace Proton.Shared.Constants;

public sealed class Route : IEquatable<Route>
{
    public string Value { get; }

    private Route(string value)
    {
        Value = value;
    }

    public static readonly Route RaceCreator = new("race-creator");
    public static readonly Route RaceMenu = new("racing_menu_list");
    public static readonly Route Auth = new("auth");
    public static readonly Route RaceCountdown = new("race-countdown");
    public static readonly Route RacePrepare = new("race-prepare");
    public static readonly Route RacePrepareTransition = new("race-prepare-transition");
    public static readonly Route RaceFinishCountdown = new("race-finish-countdown");
    public static readonly Route CharacterCreator = new("character-creator");
    public static readonly Route Speedometer = new("speedometer");
    public static readonly Route RaceHud = new("race-hud");
    public static readonly Route RaceFinishScoreboard = new("race-finish-scoreboard");
    public static readonly Route RaceStartCountdown = new("race-start-countdown");
    public static readonly Route RaceEndTransition = new("race-end-transition");
<<<<<<< HEAD
<<<<<<< HEAD
    public static readonly Route TuningMenu = new("tuning_menu");
=======
    public static readonly Route VehicleShop = new("car_shop");
    public static readonly Route ClothShop = new("cloth_shop");
    public static readonly Route VehicleMenu = new("vehicle-menu");
    public static readonly Route ClothesMenu = new("clothes-menu");
    public static readonly Route AdminPanel = new("admin-panel");
>>>>>>> 10f8164571fb7aec57ac8c49f85f305ccbd1793a
=======
    public static readonly Route VehicleShop = new("car_shop");
    public static readonly Route ClothShop = new("cloth_shop");
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1

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
