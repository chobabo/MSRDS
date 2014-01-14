using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeExplorer
{
    class Particle
    {
        public int x, y, vx, vy;
        public double weight;

        public Particle()
        {
            x = 0;
            y = 0;
            vx = 0;
            vy = 0;
            weight = 0;
        }

        public Particle(int _x, int _y, int _vx, int _vy, double _w)
        {
            x = _x;
            y = _y;
            vx = _vx;
            vy = _vy;
            weight = _w;
        }

        public double getWeight
        {
            get { return weight; }
        }

        public int get_x
        {
            get { return x; }
        }

        public int get_y
        {
            get { return y; }
        }

        public int get_vx
        {
            get { return vx; }
        }

        public int get_vy
        {
            get { return vy; }
        }

        public void setWeight(double w)
        {
            weight = w;
        }

        public void set_x(int _x)
        {
            x = _x;
        }

        public void set_y(int _y)
        {
            y = _y;
        }

        public void set_vx(int _vx)
        {
            vx = _vx;
        }

        public void set_vy(int _vy)
        {
            vy = _vy;
        }

    }
}
