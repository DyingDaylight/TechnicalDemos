using Core;
using Generation;
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
            DungeonSessionData session = new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker);

            SessionData.SetDungeonSessionData(session);
            SessionData.Clear();

            Assert.IsNull(SessionData.ConsumeSessionData());
        }
        
        [Test]
        public void ConsumeSessionData_ReturnsSessionAndClearsStoredData()
        {
            DungeonMap map = new DungeonMap(5, 5);
            DungeonSessionData session = new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker);
            
            SessionData.SetDungeonSessionData(session);

            DungeonSessionData result = SessionData.ConsumeSessionData();

            Assert.AreSame(session, result);
            Assert.IsNull(SessionData.ConsumeSessionData());
        }
        
        [TearDown]
        public void TearDown()
        {
            SessionData.Clear();
        }
    }
}