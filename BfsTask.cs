using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeon
{
    public class BfsTask
    {
        public static readonly Point[] Offsets =
            {new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)};
        
        public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
        {
            var visited = new HashSet<Point>();
            var queue = new Queue<SinglyLinkedList<Point>>();
            var currentPoint = new SinglyLinkedList<Point>(start);
            var chestsLocations = GetChestsLocations(chests);

            queue.Enqueue(currentPoint);
            visited.Add(currentPoint.Value);

            while (queue.Count != 0)
            {
                currentPoint = queue.Dequeue();
                foreach (var nextStep in GetPossibleNextSteps(map, visited, currentPoint))
                {
                    if (chestsLocations.Contains(nextStep.Value))
                    {
                        chestsLocations.Remove(nextStep.Value);
                        yield return nextStep;
                        if (chestsLocations.Count == 0)
                            yield break;
                    }
                    queue.Enqueue(nextStep);
                    visited.Add(nextStep.Value);
                }
            }
        }

        private static HashSet<Point> GetChestsLocations(Point[] chests)
        {
            var chestsLocations = new HashSet<Point>();
            foreach (var chest in chests)
                chestsLocations.Add(new Point(chest.X, chest.Y));
            return chestsLocations;
        }

        private static IEnumerable<SinglyLinkedList<Point>> GetPossibleNextSteps(Map map, HashSet<Point> visited,
            SinglyLinkedList<Point> currentPoint)
        {
            var lastPoint = currentPoint.Value;
            var nextSteps = Offsets
                .Where(offset => !visited.Contains(new Point(offset.X + lastPoint.X, offset.Y + lastPoint.Y))
                                 && map.InBounds(offset + (Size) lastPoint)
                                 && map.Dungeon[(offset + (Size) lastPoint).X, (offset + (Size) lastPoint).Y] !=
                                 MapCell.Wall)
                .Select(point => new SinglyLinkedList<Point>(point + (Size) lastPoint, currentPoint));
            return nextSteps;
        }
    }
}