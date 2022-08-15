public enum CrawlerState
{
    Roaming, //Walk around a given area at 30% speed.
    Following, //Following "alpha", not player.
    Chasing, //Chasing player.
    Attacking, //Attack target.
    Eating, //Chase Globs if within range to eat.(Globs are enemy remains).
    Running //Run away from target at MAX speed. Either when Alpha dies, or when Witch appears.
}
