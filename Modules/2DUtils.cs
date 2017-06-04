using System.Drawing;
using System;

namespace MapEditor.Modules
{
    public static class _2DUtils
    {
        public static PointF GetCenterPolygon(PointF[] points)
        {
            int num_points = points.Length;
            PointF[] pts = new PointF[num_points + 1];
            points.CopyTo(pts, 0);
            pts[num_points] = pts[0];

            float centerX = 0;
            float centerY = 0;
            float secondFactor = 0;

            for (int i = 0; i < num_points; i++)
            {
                secondFactor = pts[i].X * pts[i + 1].Y - pts[i + 1].X * pts[i].Y;
                centerX += (pts[i].X + pts[i + 1].X) * secondFactor;
                centerY += (pts[i].Y + pts[i + 1].Y) * secondFactor;
            }

            float polygon_area = PolygonArea(points);
            centerX /= (6 * polygon_area);
            centerY /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (centerX < 0)
            {
                centerX = -centerX;
                centerY = -centerY;
            }

            return new PointF(centerX, centerY);
        }

        public static float PolygonArea(PointF[] points)
        {
            int num_points = points.Length;

            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (points[(i + 1) % num_points].X - points[i].X) *
                    (points[(i + 1) % num_points].Y + points[i].Y) / 2;
            }
            return area;
        }

        public static PointF GetCenterPoint(PointF point1, PointF point2)
        {
            var centerX = (Math.Max(point1.X, point2.X) - Math.Min(point1.X, point2.X)) / 2 + Math.Min(point1.X, point2.X);
            var centerY = (Math.Max(point1.Y, point2.Y) - Math.Min(point1.Y, point2.Y)) / 2 + Math.Min(point1.Y, point2.Y);
            return new PointF(centerX, centerY);
        }
    }
}
