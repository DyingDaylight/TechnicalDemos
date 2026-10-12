
namespace Session
{
    public static class SessionData
    {
        private static DungeonSessionData sessionData;
        
        public static void SetDungeonSessionData(DungeonSessionData sessionData)
        {
            SessionData.sessionData = sessionData;
        }

        public static DungeonSessionData ConsumeSessionData()
        {
            DungeonSessionData sessionData = SessionData.sessionData;
            SessionData.sessionData = null;
            return sessionData;
        }

        public static void Clear()
        {
            sessionData = null;
        }
    }
}