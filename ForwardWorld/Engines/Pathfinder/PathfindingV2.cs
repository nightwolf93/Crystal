using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal.WorldServer.Engines.Path;

namespace Crystal.WorldServer.Engines.Pathfinder
{
    public class PathfindingV2
    {
        public MapEngine Map { get; set; }

        public List<Cell> Cells = new List<Cell>();
        public static int CELL_DISTANCE_VALUE = 10;

        private List<Cell> openList = new List<Cell>();
        private List<Cell> closedList = new List<Cell>();

        public PathfindingV2(MapEngine map)
        {
            this.Map = map;
            this.Map.Cells.Values.ToList().ForEach(x => this.Cells.Add(
                new Cell(x.ID, x._map)));
        }

        private void initialize()
        {
            this.openList = new List<Cell>();
            this.closedList = new List<Cell>();
        }

        public Cell GetCell(int cell)
        {
            return this.Cells.FirstOrDefault(x => x.ID == cell);
        }

        public List<Cell> FindShortestPath(int startCell, int endCell, List<int> dynObstacles)
        {
            this.initialize();
            try
            {
                var finalPath = new List<Cell>();
                Cell startNode = this.GetCell(startCell);
                Cell endNode = this.GetCell(endCell);

                this.addToOpenList(this.GetCell(startCell));
                Cell currentNode = null;
                while (this.openList.Count > 0)
                {
                    currentNode = this.getCurrentNode();
                    if (currentNode == endNode)
                        break;

                    this.addToCloseList(currentNode);
                    var neighbours = this.getNeighbours(currentNode, dynObstacles);
                    var maxi = neighbours.Count;
                    for (int i = 0; i < maxi; i++)
                    {
                        var node = neighbours[i];
                        if (this.closedList.Contains(node))
                            continue;

                        var newG = node.Parent.g + CELL_DISTANCE_VALUE;
                        var newH = (Math.Abs(endNode.X - node.X) + Math.Abs(endNode.Y - node.Y));
                        var newF = newH + newG;
                        if (this.openList.Contains(node))
                        {
                            if (newG < node.g)
                            {
                                node.Parent = currentNode;
                                node.g = newG;
                                node.h = newH;
                                node.f = newF;
                            }
                        }
                        else
                        {
                            addToOpenList(node);
                            node.Parent = currentNode;
                            node.g = newG;
                            node.h = newH;
                            node.f = newF;
                        }
                    }
                }

                if (this.openList.Count == 0)
                    return finalPath;

                var lastNode = this.openList.FirstOrDefault(x => x.ID == endCell);
                while (lastNode != startNode)
                {
                    finalPath.Add(lastNode);
                    lastNode = lastNode.Parent;
                }

                finalPath.Reverse();
                return finalPath;
            }
            catch (Exception e)
            {
                Utilities.ConsoleStyle.Error("Can't find path .. " + e.ToString());
                return new List<Cell>();
            }
        }

        private void addToCloseList(Cell cell)
        {
            this.openList.Remove(cell);
            this.closedList.Add(cell);
        }

        private void addToOpenList(Cell cell)
        {
            this.closedList.Remove(cell);
            this.openList.Add(cell);
        }

        private Cell getCurrentNode()
        {
            var tmpList = new List<Cell>();
            var maximum = this.openList.Count;
            var minF = 1000000;
            Cell curNode = null;
            for (int i = 0; i < maximum; i++)
            {
                var node = this.openList[i];
                if (node.f < minF)
                {
                    minF = node.f;
                    curNode = node;
                }
            }
            return curNode;
        }

        private List<Cell> getNeighbours(Cell cell, List<int> dyn)
        {
            var neigh = new List<Cell>();
            var tmpCell = Pathfinding.GetJoinCell(cell.ID, this.Map.Map);
            foreach (var c in tmpCell)
            {
                if (this.Map.IsAvailableCell(c) && !dyn.Contains(c)) { neigh.Add(this.GetCell(c)); }
            }
            return neigh;
        }
    }
}
