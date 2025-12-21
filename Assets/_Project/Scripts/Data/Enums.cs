namespace AsgardFoundry.Data
{
    /// <summary>
    /// Types of production systems in the game.
    /// </summary>
    public enum SystemType
    {
        Mining,
        Woodcutting,
        Farming,
        Smithing,
        Market
    }

    /// <summary>
    /// Resource types produced and consumed by systems.
    /// </summary>
    public enum ResourceType
    {
        Stone,
        Wood,
        Food,
        Metal,
        Gold,
        Tools
    }

    /// <summary>
    /// Visual/mechanical eras for city progression.
    /// </summary>
    public enum Era
    {
        StoneAge = 1,
        BronzeAge = 2,
        IronAge = 3,
        Medieval = 4
    }
}
