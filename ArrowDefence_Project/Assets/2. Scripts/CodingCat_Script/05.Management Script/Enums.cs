
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

public enum ABILITY_TYPE {
    //NONE
    NONE           = 0,
    //BOW PROPERTY
    DAMAGE         = 1,
    CHARGEDAMAGE   = 2,
    CRITICALCHANCE = 3,
    CRITICALDAMAGE = 4,
    ARMORPENETRATE = 5,
    //ARROW PROPERTY
    DAMAGEINC      = 6,
    ARROWSPEED     = 7,
    //ACCESSORY PROPERTY
}

#region GAMEMANAGER

public enum GAMESTATE {
    STATE_BEFOREBATTLE,
    STATE_INBATTLE,
    STATE_BOSSBATTLE,
    STATE_ENDBATTLE,
    STATE_GAMEOVER,
}

#endregion

#region BATTLE

public enum STAGETYPE {
    NONE                   = 0, //Stage Type Not Setting, throw error
    STAGE_DEV              = 1,
    STAGE_FOREST_SECLUDED  = 2,
    STAGE_DUNGEON_ENTRANCE = 3,
}

#endregion

