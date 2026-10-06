using Core;

namespace Session
{
    public static class SessionData
    {
        private static IReadOnlyDungeonMap dungeonMap;

        public static void SetDungeonMap(IReadOnlyDungeonMap dungeonMap)
        {
            SessionData.dungeonMap = dungeonMap;
        }

        public static IReadOnlyDungeonMap ConsumeDungeonMap()
        {
            IReadOnlyDungeonMap map = dungeonMap;
            dungeonMap = null;
            return map;
        }

        private static void ClearDungeonMap()
        {
            dungeonMap = null;
        }

        public static void Clear()
        {
            ClearDungeonMap();
        }
    }
}