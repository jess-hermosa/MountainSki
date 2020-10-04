using System;
using System.IO;
namespace SnowGliding
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = File.ReadAllText(@"./map.txt");
            var row = input.Split('\n');
            var arrSize = row[0].Trim().Split(' ');

            int rowSize = int.Parse(arrSize[0].Trim());
            int colSize = int.Parse(arrSize[1].Trim());
            int[,] map = new int[rowSize,colSize];
            int j = 0;

            for (var x = 1; x < (row.Length - 1); x++)
            {
                var col = row[x].Trim().Split(' ');

                for (var i = 0; i < col.Length; i++)
                {
                    map[j, i] = int.Parse(col[i]);
                }
                j++;
            }

            int[,] pathMap = new int[6, 6];
            int t = 0;
            int p = 0;

            for(var x=158;x<164;x++)
            {
                for(var y=392;y<398;y++)
                {
                    pathMap[t, p] = map[x, y];
                    p++;
                }
                t++;
                p = 0;
            }
            (int lengthCalculatedPath, string calculatedPath) = FindLongestPath(map, rowSize, colSize);
            var path = calculatedPath.Split(',');

            Console.WriteLine($"Length of calculated path: {lengthCalculatedPath}");
            Console.WriteLine($"Drop of calculated path: {path[0]}");
            Console.WriteLine($"Calculated path: {calculatedPath}");
        }

        static (int, string) FindLongestPath(int[,] map, int rowSize, int colSize)
        {
            int longestPathLength = 1;
            string longestPath = "";
            string[,] visitedPaths = new string[rowSize, colSize];
            int[,] visitedPathLengths = new int[rowSize, colSize];

            for (var i = 0; i < (rowSize - 1); i++)
            {
                for (var x = 0; x < (colSize - 1); x++)
                {
                    (int downHillPathLength, string downHillPath) = GetDownHillPath(map, i, x, visitedPathLengths, visitedPaths);

                    if (downHillPathLength > longestPathLength)
                    {
                        longestPathLength = downHillPathLength;
                        longestPath = downHillPath;
                    }
                    else if (downHillPathLength == longestPathLength && !string.IsNullOrEmpty(longestPath))
                    {
                        var path = downHillPath.Split(',');
                        var currentPath = longestPath.Split(',');
                        if ((int.Parse(path[0]) - int.Parse(path[path.Length-1])) > (int.Parse(currentPath[0]) - int.Parse(currentPath[currentPath.Length - 1])))
                        {
                            longestPathLength = downHillPathLength;
                            longestPath = downHillPath;
                        }
                    }
                }
            }

            return (longestPathLength, longestPath);
        }

        static (int, string) GetDownHillPath(int[,] map, int x, int y, int[,] visitedPathLengths, string[,] visitedPaths)
        {
            if (visitedPathLengths[x, y] != 0)
            {
                return (visitedPathLengths[x,y], visitedPaths[x,y]);
            }

            int rowSize = visitedPathLengths.GetLength(0);
            int colSize = visitedPathLengths.GetLength(1);
            int pathLength = 1;
            int currentLocation = map[x, y];
            string path = $"{currentLocation}";
            int steepestPath = currentLocation;

            if (HasNorthPath(x) && IsDownHill(map[x-1,y], currentLocation))
            {
                (int downHillPathLength, string downHillPath) = GetDownHillPath(map, x-1, y, visitedPathLengths, visitedPaths);
                if (HasLongerDownHillPath(downHillPathLength, pathLength))
                {
                    SetAsLongestAndSteepestPath(downHillPathLength, downHillPath, ref pathLength, ref path, currentLocation, ref steepestPath);
                }
            }

            if (HasSouthPath(x,rowSize-1) && IsDownHill(map[x+1,y], currentLocation))
            {
                (int downHillPathLength, string downHillPath) = GetDownHillPath(map, x+1, y, visitedPathLengths, visitedPaths);
                if (HasLongerDownHillPath(downHillPathLength, pathLength))
                {
                    SetAsLongestAndSteepestPath(downHillPathLength, downHillPath, ref pathLength, ref path, currentLocation, ref steepestPath);
                }
            }

            if (HasWestPath(y) && IsDownHill(map[x,y-1], currentLocation))
            {
                (int downHillPathLength, string downHillPath) = GetDownHillPath(map, x, y-1, visitedPathLengths, visitedPaths);
                if (HasLongerDownHillPath(downHillPathLength, pathLength))
                {
                    SetAsLongestAndSteepestPath(downHillPathLength, downHillPath, ref pathLength, ref path, currentLocation, ref steepestPath);
                }
            }

            if (HasEastPath(y,colSize-1) && IsDownHill(map[x, y + 1], currentLocation))
            {
                (int downHillPathLength, string downHillPath) = GetDownHillPath(map, x, y+1, visitedPathLengths, visitedPaths);
                if (HasLongerDownHillPath(downHillPathLength, pathLength))
                {
                    SetAsLongestAndSteepestPath(downHillPathLength, downHillPath, ref pathLength, ref path, currentLocation, ref steepestPath);
                }
            }

            visitedPathLengths[x, y] = pathLength;
            visitedPaths[x, y] = path;

            return (pathLength, path);
        }

        static void SetAsLongestAndSteepestPath(int downHillPathLength, string downHillPath, ref int pathLength, ref string path, int currentLocation, ref int steepestPath)
        {
            if (pathLength == downHillPathLength + 1)
            {
                var paths = downHillPath.Split(',');
                if (int.Parse(paths[paths.Length - 1]) < steepestPath)
                {
                    path = $"{currentLocation},{downHillPath}";
                    steepestPath = int.Parse(paths[paths.Length - 1]);
                }
            }
            else
            {
                pathLength = downHillPathLength + 1;
                path = $"{currentLocation},{downHillPath}";

                var paths = downHillPath.Split(',');
                steepestPath = int.Parse(paths[paths.Length - 1]);
            }
        }

        static bool HasNorthPath(int x) => x != 0;
        static bool HasSouthPath(int x, int rowLength) => x != rowLength;
        static bool HasWestPath(int y) => y != 0;
        static bool HasEastPath(int y, int columnLength) => y != columnLength;
        static bool IsDownHill(int direction, int currentLocation) => direction < currentLocation;
        static bool HasLongerDownHillPath(int downHillPathLength, int pathLength) => downHillPathLength + 1 >= pathLength;
    }
}
