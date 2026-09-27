using System.Collections.Generic;
using Core;

namespace Generation
{
    public class GenerationHistory
    {
        private readonly List<DungeonSnapshot> snapshots = new ();
        
        public int Count => snapshots.Count;
        
        public DungeonSnapshot this[int index] => snapshots[index];
        
        public void Record(DungeonMap map)
        {
            snapshots.Add(new DungeonSnapshot(map));
        }
    }
}