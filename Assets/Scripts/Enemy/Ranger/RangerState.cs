public enum RangerState
{

    Roaming, //Walk around searching for something
    Chasing, //Chasing target, should stay pretty far away.
    Charging, //Charge up a shot, show visual feedback
    Attacking, //Shoot a glob of ink in the direction of the player or building.
    Eating, //Chase Globs if within range to eat.(Globs are enemy remains).
    Fleeing //Run away from target at MAX speed, when Witch appears.
}
