using Core;
using NUnit.Framework;
using Session;

namespace Tests.Session
{
    public class SessionDataTests
    {
        [SetUp]
        public void SetUp()
        {
            SessionData.Clear();
        }
        
        [Test]
        public void Clear_RemovesAllSessionData()
        {
            DungeonMap map = new DungeonMap(5, 5);
            SessionData.SetDungeonMap(map);

            SessionData.Clear();

            Assert.IsNull(SessionData.ConsumeDungeonMap());
        }
        
        [Test]
        public void ConsumeDungeonMap_ReturnsMapAndRemovesItFromSession()
        {
            DungeonMap map = new DungeonMap(5, 5);
            SessionData.SetDungeonMap(map);

            IReadOnlyDungeonMap result = SessionData.ConsumeDungeonMap();

            Assert.AreSame(map, result);
            Assert.IsNull(SessionData.ConsumeDungeonMap());
        }
        
        [TearDown]
        public void TearDown()
        {
            SessionData.Clear();
        }
    }
}