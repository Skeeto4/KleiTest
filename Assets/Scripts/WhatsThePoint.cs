using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhatsThePoint : MonoBehaviour
{
    public class Line
    {
        public enum EvaluationType { InLine, LargerThan, LesserThan, InLineOrLargerThan, InLineOrLesserThan }
        public enum Type { Vertical, Horizontal, Diagonal }
        public float slope { get; private set; }
        public float intersectionWithY { get; private set; }
        public Point knownPoint_A { get; private set; }
        public Point knownPoint_B { get; private set; }
        public Type type { get; private set; }

        public Line(Point p1, Point p2)
        {
            knownPoint_A = p1;
            knownPoint_B = p2;

            // the function of a line is y = mx + b where x and y is a point within the line, m is the slope (the rotation of the line) and b is the intersection with the Y axis. (this is burned down in my mind since highschool)
            // a way to get the slope of a line is to know 2 points in it, and use this formula: m = (p2.y - p1.y) / (p2.x - p1.x);

            // the line function and slope calculation can fail when we divide by zero, we need to catch those cases, if you observe the slope formula, the risk happens when p2.x == p1.x 
            bool isHorizontalLine = p1.y == p2.y; // if 2 points that belong to a line share the same x coordinate that means the line is horizontal
            bool isVerticalLine = p1.x == p2.x; // if 2 points that belong to a line share the same y coordinate that means the line is vertical

            type = isVerticalLine ? Type.Vertical : isHorizontalLine ? Type.Horizontal : Type.Diagonal; // define the line type.           

            if (type != Type.Vertical) // when is vertical, the formula for slope fails, because we divide by zero. so we make sure we don't do that.
            {
                slope = (p2.y - p1.y) / (p2.x - p1.x);
            }

            // after we got the slope calculated, we find the intersection with the Y axis, also known as 'b'
            // if (y = mx + b) then b= y-mx    once again we face a division, we need to keep in mind that dividing by zero is ilegal.

            switch (type)
            {
                case Type.Horizontal:
                    intersectionWithY = knownPoint_A.y; // if a line is horizontal, you can grab any point in the line, since they all share the same y coordinate, the intersection with y axis is any point's y coordinate
                    break;
                case Type.Vertical:
                    intersectionWithY = 0; // when a line is vertical is a bit complicated, the intersection with Y is not really relevant, it either never intersects or if a point in that line has x coordinates = "0" then is intersected in infinite
                    break;
                case Type.Diagonal:
                default:
                    intersectionWithY = knownPoint_A.y - slope * knownPoint_A.x;
                    break;
            }
        }

        public bool EvaluatePoint(Point point, EvaluationType evaluationType = EvaluationType.InLine) // tests the given point against the line function
        {
            //y = mx + b
            switch (evaluationType)
            {
                default:
                case EvaluationType.InLine: return point.y == slope * point.x + intersectionWithY;
                case EvaluationType.InLineOrLargerThan: return point.y >= slope * point.x + intersectionWithY;
                case EvaluationType.InLineOrLesserThan: return point.y <= slope * point.x + intersectionWithY;
                case EvaluationType.LargerThan: return point.y > slope * point.x + intersectionWithY;
                case EvaluationType.LesserThan: return point.y < slope * point.x + intersectionWithY;
            }
        }
    }
    public struct Point
    {
        public float x;
        public float y;
    }
    public struct Triangle
    {
        public Point p1;
        public Point p2;
        public Point p3;

        private Line[] _lines;
        private Line.EvaluationType[] _linesEvaluationType; // saves the evaluations you need to make with the lines to check if a point is inside the triangle

        public Triangle(Point _p1, Point _p2, Point _p3)
        {
            p1 = _p1;
            p2 = _p2;
            p3 = _p3;
            _lines = null;
            _linesEvaluationType = null;
            DefineLines();
        }
        public bool ContainsPoint(Point point)
        {
            // what i will do is define the triangle as 3 lines, and if the point is withing the correct side of each of the lines then we are in the triangle.
            // I like to use desmos to demonstrate line operations, here is a copy of what I am doing in desmos to attempt to demonstrate the process: https://www.desmos.com/calculator/hjd5dvzrex
            // change the operator of the function y > mx + b if you make it '<' it shows all possible answers for points that are < than that line. 
            DefineLines(); // defines this triangle's lines, if their haven't been defined before.

            bool pointIsInTriangle = true; // begin by assuming it is in the triangle.
            for (int i = 0; pointIsInTriangle && i < _lines.Length; i++) // per each line, while we think we still in the triangle, lets ask if we are in the correct side of the line.
            {
                Line line = _lines[i];
                Line.EvaluationType evalutaionType = _linesEvaluationType[i];
                pointIsInTriangle &= line.EvaluatePoint(point, evalutaionType);
            }

            return pointIsInTriangle;
        }
        private void DefineLines()
        {
            if (_lines == null)
            {
                _lines = new Line[3]
                {
                    new Line(p1,p2),
                    new Line(p2,p3),
                    new Line(p3,p1)
                };

                Point centralPoint = new Point(); // the central position of the triangle
                centralPoint.x = (p1.x + p2.x + p3.x) / 3f;
                centralPoint.y = (p1.y + p2.y + p3.y) / 3f;

                // a line function can be use to know if a point is within the line. but if you change the operator to be lesser than or less than, you can check if a point is in one side of the line or the other side
                // I want to know, per each line that this triangle is made of, I want to know which side of that line contains the center of the triangle.
                // i will save the operator that gave me the desired result in this array: '_linesEvaluationType' to use later.
                _linesEvaluationType = new Line.EvaluationType[_lines.Length]; // now we need to figure out which operation we should use when evaluating points in lines in a way that the line function covers the inside of the triangle
                for (int i = 0; i < _lines.Length; i++)
                {
                    Line line = _lines[i];
                    _linesEvaluationType[i] = Line.EvaluationType.InLineOrLargerThan; // there are two options, assume one and check if it is correct. 
                    if (line.EvaluatePoint(centralPoint, Line.EvaluationType.InLineOrLesserThan))
                    {
                        _linesEvaluationType[i] = Line.EvaluationType.InLineOrLesserThan;
                    }
                }
            }
        }

    }
    public class Country
    {
        public string name;
        public List<Triangle> triangles = new List<Triangle>();
        public bool IsPointInCountry(Point point)
        {
            if (triangles != null)
            {
                bool pointFound = false;
                foreach(Triangle triangle in triangles)
                {
                    pointFound |= triangle.ContainsPoint(point);
                }
                return pointFound;
            }
            return false;
        }
    }
    public class Map
    {
        public List<Country> countries = new List<Country>();
        public Country Intersects(Point point)
        {
            if(countries != null)
            {
                Country matchingCountry = null;
                foreach (Country country in countries)
                {
                    if (matchingCountry == null && country.IsPointInCountry(point))
                    {
                        matchingCountry = country;
                    }
                }
                return matchingCountry;
            }
            return null;
        }
    }
}
