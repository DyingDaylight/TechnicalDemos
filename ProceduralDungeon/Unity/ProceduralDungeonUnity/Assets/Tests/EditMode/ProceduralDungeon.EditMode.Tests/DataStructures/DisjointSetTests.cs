using System;
using DataStructures;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.ProceduralDungeon.EditMode.Tests.DataStructures
{
    public class DisjointSetTests
    {
        [Test]
        public void MakeSet_CreatesSeparateSet()
        {
            // Arrange
            DisjointSet disjointSet = new DisjointSet();
            Vector2Int item = new Vector2Int(1, 2);

            // Act
            disjointSet.MakeSet(item);

            // Assert
            Assert.AreEqual(item, disjointSet.Find(item));
        }
        
        [Test]
        public void Union_CombinesSets()
        {
            // Arrange
            DisjointSet disjointSet = new DisjointSet();
            Vector2Int a = new Vector2Int(1, 2);
            Vector2Int b = new Vector2Int(3, 4);

            disjointSet.MakeSet(a);
            disjointSet.MakeSet(b);

            // Act
            disjointSet.Union(a, b);

            // Assert
            Assert.AreEqual(disjointSet.Find(a), disjointSet.Find(b));
        }
        
        [Test]
        public void Union_CombinesMultipleSets()
        {
            // Arrange
            DisjointSet disjointSet = new DisjointSet();
            Vector2Int a = new Vector2Int(1, 2);
            Vector2Int b = new Vector2Int(3, 4);
            Vector2Int c = new Vector2Int(5, 6);

            disjointSet.MakeSet(a);
            disjointSet.MakeSet(b);
            disjointSet.MakeSet(c);

            // Act
            disjointSet.Union(a, b);
            disjointSet.Union(b, c);

            // Assert
            Assert.AreEqual(disjointSet.Find(a), disjointSet.Find(b));
            Assert.AreEqual(disjointSet.Find(a), disjointSet.Find(c));
        }
        
        [Test]
        public void Find_ItemNotInSet_ThrowsArgumentException()
        {
            // Arrange
            DisjointSet disjointSet = new DisjointSet();
            Vector2Int item = new Vector2Int(1, 2);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                disjointSet.Find(item);
            });
        }
    }
}