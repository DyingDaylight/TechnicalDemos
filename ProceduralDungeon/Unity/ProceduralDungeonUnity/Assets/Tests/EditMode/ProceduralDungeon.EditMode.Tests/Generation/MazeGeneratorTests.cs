using System;
using System.Collections;
using Generation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Tests.EditMode.ProceduralDungeon.EditMode.Tests
{
    public class MazeGeneratorTests
    {
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_ProducesConnectedMaze(MazeAlgorithm algorithm)
        {
        }
        
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_ProducesMazeWithoutCycles(MazeAlgorithm algorithm)
        {
        }
        
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_WithRectangularMap_ProducesValidMaze(MazeAlgorithm algorithm)
        {
        }
        
        private MazeGenerator CreateGenerator(MazeAlgorithm algorithm, int seed)
        {
            switch (algorithm)
            {
                case MazeAlgorithm.RecursiveBacktracker:
                    return new RecursiveBacktracker(seed);

                case MazeAlgorithm.RandomizedPrim:
                    return new RandomizedPrim(seed);

                case MazeAlgorithm.RandomizedKruskal:
                    return new RandomizedKruskal(seed);

                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm));
            }
        }
    }
}