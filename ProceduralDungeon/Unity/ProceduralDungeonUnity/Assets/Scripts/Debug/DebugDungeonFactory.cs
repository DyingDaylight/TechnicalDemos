using System;
using Core;
using Generation;
using UnityEngine;

namespace DebugTools
{
    public static class DebugDungeonFactory
    {
        private const int DebugSeed = 15;
        private const int DebugWidth = 19;
        private const int DebugHeight = 21;
        private const bool DebugRecordHistory = false;
        
        public static IReadOnlyDungeonMap Create()
        {
            MazeGeneratorFactory factory = new MazeGeneratorFactory();
            MazeGenerator mazeGenerator = factory.Create(MazeAlgorithm.RecursiveBacktracker, DebugSeed);
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
            GenerationResult generationResult = mapGenerator.Generate(DebugWidth, DebugHeight, DebugRecordHistory);
            return generationResult.Map;
        }

        public static IReadOnlyDungeonMap CreateFromAscii(string ascii, out Vector2Int? source)
        {
            if (string.IsNullOrWhiteSpace(ascii))
                throw new ArgumentException("ASCII map cannot be empty.", nameof(ascii));

            string[] rows = ascii.Trim().Replace("\r", "").Split('\n');

            int width = rows[0].Length;
            int height = rows.Length;

            if (width == 0)
                throw new ArgumentException("ASCII map cannot have empty rows.", nameof(ascii));

            DungeonMap map = new DungeonMap(width, height);
            source = null;

            for (int row = 0; row < height; row++)
            {
                if (rows[row].Length != width)
                    throw new ArgumentException( $"ASCII map row {row + 1} has an invalid width.", nameof(ascii));

                int y = height - 1 - row;

                for (int x = 0; x < width; x++)
                {
                    switch (rows[row][x])
                    {
                        case '#':
                            map[x, y] = TileType.Wall;
                            break;

                        case '.':
                            map[x, y] = TileType.Floor;
                            break;

                        case 'S':
                            if (source.HasValue)
                                throw new ArgumentException( "ASCII map must contain exactly one water source.",
                                    nameof(ascii));

                            map[x, y] = TileType.Floor;
                            source = new Vector2Int(x, y);
                            break;

                        default:
                            throw new ArgumentException(
                                $"Invalid character '{rows[row][x]}' at row {row + 1}, column {x + 1}.",
                                nameof(ascii));
                    }
                }
            }

            if (!source.HasValue)
                throw new ArgumentException(
                    "ASCII map must contain a water source (S).",
                    nameof(ascii));

            return map;
        }
    }
}