
#region STAGE
public enum STAGEDIFF {
    NONE     = 0,
    EASY     = 1,
    NORMAL   = 2,
    HARD     = 3,
    WARFIELD = 4,
    HELL     = 5 
}

#endregion

#region STATE_MACHINE

public enum MONSTERSTATE {
    IDLE   = 0,
    MOVE   = 1,
    ATTACK = 2,
    DEATH  = 3,
}

public enum STATETYPE {
    IDLE   = 0,
    MOVE   = 1,
    ATTACK = 2,
    DEATH  = 3,
}

public enum STATEFLOW {
    ENTER,
    UPDATE,
    EXIT,
}

#endregion
