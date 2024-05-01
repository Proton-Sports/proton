namespace Proton.Shared.Contants;

public sealed class Route
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
    public static readonly Route RaceEndCountdown = new("race-end-countdown");
    public static readonly Route CharacterCreator = new("character-creator");
    public static readonly Route VehicleShop = new("car_shop");
    public static readonly Route ClothShop = new("cloth_shop");
}
