using ORMMap.VectorTile.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMMapViewer.Model.Entitites
{
    public class Node
    {
        private const int maxDistance = 2;
        public Vector2<int> pos;
        public Dictionary<Node, Weight> relatives = new Dictionary<Node, Weight>();

        public Node(int x, int y)
        {
            pos = new Vector2<int>(x, y);
        }

        public override bool Equals(object obj)
        {
            return obj is Node node &&
                   Math.Abs(pos.X - node.pos.X) < maxDistance &&
                   Math.Abs(pos.Y - node.pos.Y) < maxDistance;
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + pos.X.GetHashCode();
            hashCode = hashCode * -1521134295 + pos.Y.GetHashCode();
            return hashCode;
        }

        public void TakeRelatives(Node other)
        {
            foreach (Node otherNode in other.relatives.Keys)
            {
                foreach (Node thisNode in relatives.Keys)
                {
                    if (otherNode.Equals(thisNode))
                    {
                        thisNode.relatives.Concat(otherNode.relatives.Where(x => !thisNode.relatives.ContainsKey(x.Key)));
                    }
                }
            }
        }

        public void UpdateRelatives()
        {
            foreach (Node node in relatives.Keys)
            {
                if (!node.relatives.ContainsKey(this))
                {
                    node.relatives.Add(this, new LengthWeight(this, node));
                }
            }
        }
    }
}
