public enum FlyerState
{
    
    Roaming, //Fly around a given area at 30% speed.
    Nesting, //Chill at their nest, go to roaming every 2 minutes.
    Chasing, //Chasing target.
    Circling, //Circling around Target in the air.
    Attacking, //Attack target. Should swoop down.
    Gathering, //Attack building, go back to nest. Ignored target unless provoked.
    Eating, //Chase Globs if within range to eat.(Globs are enemy remains).
    Fleeing //Run away from target at MAX speed. Either when Alpha dies, or when Witch appears.
}
