namespace Proton.Shared.Contants;

public sealed class Route
{
    public string Value { get; }

    private Route(string value)
    {
        Value = value;
    }

    public static readonly Route RaceMainMenu = new("race-main-menu");
    public static readonly Route RaceCreator = new("race-creator");
    public static readonly Route RaceMainMenuList = new("racing_menu_list");
    public static readonly Route Auth = new("auth");
}
