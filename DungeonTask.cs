using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            var pathFromPlayerToExit =
                BfsTask.FindPaths(map, map.Exit, new[] {map.InitialPosition}).FirstOrDefault();

            // если нет пути от игрока к выходу, возвращаем пустой путь
            if (pathFromPlayerToExit == null)
            {
                return !CheckIfExitInOneStepFromStart(map.InitialPosition, map.Exit)
                    ? Array.Empty<MoveDirection>()
                    : GetMoveDirectionsFromPoints(new List<Point> {map.InitialPosition, map.Exit});
            }

            // если нет сундуков, то возвращаем путь от игрока к выходу
            if (map.Chests.Length == 0)
                return GetMoveDirectionsFromPoints(pathFromPlayerToExit.ToList());

            SinglyLinkedList<Point> pathFromChestToPlayer = null;
            List<Point> shortestPath = null;

            foreach (var chest in map.Chests)
            {
                // если кратчайший путь от игрока до выхода включает в себя сундук, то сразу возвращаем его
                if (pathFromPlayerToExit.Contains(chest))
                    return GetMoveDirectionsFromPoints(pathFromPlayerToExit.ToList());

                pathFromChestToPlayer = BfsTask.FindPaths(map, chest, new[] {map.InitialPosition}).FirstOrDefault();
                var pathFromChestToExit = BfsTask.FindPaths(map, map.Exit, new[] {chest}).FirstOrDefault();

                if (shortestPath == null)
                {
                    shortestPath = pathFromChestToPlayer?.Concat(pathFromChestToExit?.Skip(1) ?? Array.Empty<Point>()).ToList();
                    continue;
                }

                if (pathFromChestToPlayer?.Length + pathFromChestToExit?.Length < shortestPath.Count)
                    shortestPath = pathFromChestToPlayer.Concat(pathFromChestToExit.Skip(1)).ToList();
            }

            // если игрок не может дойти ни до одного сундука, то идет сразу к выходу
            if (pathFromChestToPlayer == null)
            {
                return !CheckIfExitInOneStepFromStart(map.InitialPosition, map.Exit)
                    ? GetMoveDirectionsFromPoints(pathFromPlayerToExit.ToList())
                    : GetMoveDirectionsFromPoints(new List<Point> {map.InitialPosition, map.Exit});
            }

            return GetMoveDirectionsFromPoints(shortestPath);
        }

        private static bool CheckIfExitInOneStepFromStart(Point initialPosition, Point exit)
        {
            return (Math.Abs(initialPosition.X - exit.X) == 1 && Math.Abs(initialPosition.Y - exit.Y) == 0)
                   || Math.Abs(initialPosition.X - exit.X) == 0 && Math.Abs(initialPosition.Y - exit.Y) == 1;
        }

        private static MoveDirection[] GetMoveDirectionsFromPoints(List<Point> shortest)
        {
            var result = new MoveDirection[shortest.Count - 1];

            for (var i = 1; i < shortest.Count; i++)
                result[i - 1] = Walker.ConvertOffsetToDirection((Size) (shortest[i] - (Size) shortest[i - 1]));

            return result;
        }
    }
}