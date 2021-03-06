﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Factories
{
    public class FactoryMethod
    {
        public enum CoordinateSystem
        {
            Cartesian,
            Polar
        }

        public class Point
        {
            public static Point NewCartesianPoint(double x, double y)
            {
                return new Point(x, y);
            }

            public static Point NewPolarPoint(double rho, double theta)
            {
                return new Point(rho*Math.Cos(theta), rho*Math.Sin(theta));
            }

            private double x, y;

            private Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
            }
        }

        // change to Main to run.
        public static void none(string[] args)
        {
            var point = Point.NewPolarPoint(1.0, Math.PI / 2);
            Console.WriteLine(point);
        }
    }
}
