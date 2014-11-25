using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//@Author NightWolf
//This is a file from Project $safeprojectname$

namespace Crystal.WorldServer.Engines.Path
{
    public class Cell : IPathNode<Object>
    {
        public int ID;
        private Database.Records.MapRecords _map;

        public Cell(int id, Database.Records.MapRecords map)
        {
            _map = map;
            ID = id;
            try
            {
                X = Pathfinding.GetCellXCoord(id, map.Width);
                Y = Pathfinding.GetCellYCoord(id, map.Width);
            }
            catch (Exception e)
            {

            }
        }

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public Point Coor
        {
            get
            {
                return new Point(X, Y);
            }
        }

        public int Available
        {
            get;
            set;
        }

        public Point Squarify()
        {
            return new Point(Coor.X - Coor.Y / 2, Coor.Y + (Coor.X - Coor.Y / 2));
        }

        public bool IsWalkable(Object unused)
        {
            return this.Available != 0;
        }

        public int layerObject2Num { get; set; }

        public int layerObject2Interactive { get; set; }

        public int layerObject1Num { get; set; }
    }
}
