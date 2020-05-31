using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMMapViewer.Model.Entitites
{
    class LengthWeight : Weight
    {
        private readonly double length;

        public LengthWeight(double length)
        {
            this.length = length;
        }

        public LengthWeight(Node first, Node second)
        {
            this.length = Math.Sqrt(Math.Pow(first.pos.X - second.pos.X, 2) + Math.Pow(first.pos.Y - second.pos.Y, 2));
        }

        public override double Calculate()
        {
            return length;
        }
    }
}
