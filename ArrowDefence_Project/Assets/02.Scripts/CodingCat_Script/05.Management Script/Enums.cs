namespace ActionCat {
    #region STAGE
    public enum STAGEDIFF {
        NONE = 0,
        EASY = 1,
        NOML = 2,
        HARD = 3,
        WARF = 4,
        HELL = 5
    }

    #endregion

    #region STATE_MACHINE

    public enum MONSTERSTATE
    {
        IDLE = 0,
        MOVE = 1,
        ATTACK = 2,
        DEATH = 3,
    }

    public enum STATETYPE
    {
        IDLE = 0,
        MOVE = 1,
        ATTACK = 2,
        DEATH = 3,
    }

    public enum STATEFLOW
    {
        ENTER,
        UPDATE,
        EXIT,
    }

    public enum SPAWNSTATE {
        NONE,
        BREAK,
        SPAWN,
    }

    #endregion

    #region EQUIPMENT

    public enum ABILITY_TYPE
    {
        //NONE
        NONE = 0,
        //BOW PROPERTY
        DAMAGE = 1,
        CHARGEDAMAGE = 2,
        CRITICALCHANCE = 3,
        CRITICALDAMAGE = 4,
        ARMORPENETRATE = 5,
        //ARROW PROPERTY
        DAMAGEINC = 6,
        ARROWSPEED = 7,
        //ACCESSORY PROPERTY
    }

    #endregion

    #region GAMEMANAGER

    public enum GAMESTATE {
        STATE_NONE,
        STATE_BEFOREBATTLE,
        STATE_INBATTLE,
        STATE_BOSSBATTLE,
        STATE_ENDBATTLE,
        STATE_GAMEOVER,
    }

    #endregion

    #region BATTLE

    public enum STAGETYPE {
        NONE = 0, //throw error
        STAGE_DEV = 1,
        STAGE_FOREST_SECLUDED = 2,
        STAGE_DUNGEON_ENTRANCE = 3,
        FOREST_SECLUDED_E,
        FOREST_SECLUDED_N,
        FOREST_SECLUDED_H,
        DUNGEON_E,
        DUNGEON_N,
        DUNGEON_H
    }

    public enum AUTOSTATE
    {
        NONE,
        WAIT,
        FIND,
        TRAC,
        SHOT,
    }

    public enum PULLINGTYPE
    {
        AROUND_BOW_TOUCH = 0,
        FREE_TOUCH = 1,
        AUTOMATIC = 2
    }

    #endregion

    #region EFFECT

    public enum BOWEFFECTYPE
    {
        NONE = 0,
        IMPACT = 1,
        CHARGED = 2,
        FADE = 3,
    }


    public enum EFFECTORTYPE {
        NONE,
        NEWEFFECT,
        RESTARTER,
    }

    #endregion

    public enum CHARGETYPE {
        NONE,
        KILL,
        TIME,
        ATCK,
    }

    public enum BLUEPRINTTYPE {
        ALL        = 0,
        BOW        = 1,
        ARROW      = 2,
        ARTIFACT   = 3,
        MATERIAL   = 4,
        CONSUMABLE = 5
    }
}


