namespace ActionCat
{
    public enum PULLINGTYPE
    {
        AROUND_BOW_TOUCH = 0,
        FREE_TOUCH       = 1,
        AUTOMATIC        = 2
    }


    public class GameSettings
    {
        PULLINGTYPE pullingType = PULLINGTYPE.AROUND_BOW_TOUCH; 

        public PULLINGTYPE PullingType { get => pullingType; }

        public GameSettings() { }
        ~GameSettings() { }

        public void SetPullingType(PULLINGTYPE pullType) => pullingType = pullType;
    }
}
