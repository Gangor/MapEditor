using System.Drawing;
using System;
using System.Collections.Generic;
using MapEditor.Models;

namespace MapEditor.Modules
{
    public static class _2DUtils
	{
		public static Point AdjustPoint(Point point, float zoom, bool imprecision, bool tile)
		{
			var position = GetPointRotate180FlipY(new PointF((int)(point.X / zoom), (int)(point.Y / zoom)));

			position.X = (int)Math.Round(position.X * 7.875f);
			position.Y = (int)Math.Round(position.Y * 7.875f);

			if (imprecision)
			{
				position.X *= 8;
				position.Y *= 8;
			}

			if (tile)
			{
				position.X /= Global.tileLenght;
				position.Y /= Global.tileLenght;
			}

			return new Point((int)position.X, (int)position.Y);
		}

		public static PointF AdjustPoint(PointF point, float zoom, bool imprecision, bool tile)
		{
			var position = GetPointRotate180FlipY(new PointF((point.X / zoom), (point.Y / zoom)));

			position.X *= 7.875f;
			position.Y *= 7.875f;

			if (imprecision)
			{
				position.X *= 8;
				position.Y *= 8;
			}

			if (tile)
			{
				position.X /= Global.tileLenght;
				position.Y /= Global.tileLenght;
			}

			return position;
		}

		public static PointF AdjustPoint(PointF point, int segmentNumber, float x, float y)
		{
			var partX = MapWorker.Instance.X * 16128;
			var partY = MapWorker.Instance.Y * 16128;

			var segmentX = segmentNumber % 64;
			var segmentY = segmentNumber / 64;

			var segmentFX = segmentX * Global.tileLenght * 6;
			var segmentFY = segmentY * Global.tileLenght * 6;

			var tempX = x + segmentFX + partX;
			var tempY = y + segmentFY + partY;

			return new PointF(tempX, tempY);
		}

		public static PointF[] AdjustPolygonPoint(List<PointF> polygon, float zoom, bool imprecision, bool tile)
		{
			var points = new PointF[polygon.Count];

			for (var i = 0; i < polygon.Count; i++)
			{
				var position = GetPointRotate180FlipY(new PointF((polygon[i].X / zoom), (polygon[i].Y / zoom)));

				position.X *= 7.875f;
				position.Y *= 7.875f;

				if (imprecision)
				{
					position.X *= 8;
					position.Y *= 8;
				}

				if (tile)
				{
					position.X /= Global.tileLenght;
					position.Y /= Global.tileLenght;
				}

				points[i] = position;
			}

			return points;
		}

		public static PointF GetCenterPoint(PointF point1, PointF point2)
		{
			var centerX = (Math.Max(point1.X, point2.X) - Math.Min(point1.X, point2.X)) / 2 + Math.Min(point1.X, point2.X);
			var centerY = (Math.Max(point1.Y, point2.Y) - Math.Min(point1.Y, point2.Y)) / 2 + Math.Min(point1.Y, point2.Y);
			return new PointF(centerX, centerY);
		}

		public static PointF GetCenterPolygon(List<PointF> points) { return GetCenterPolygon(points.ToArray()); }

		public static PointF GetCenterPolygon(Polygon2 polygon) { return GetCenterPolygon(PolygonToPoint(polygon)); }

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

			if (centerX < 0)
			{
				centerX = -centerX;
				centerY = -centerY;
			}
			return new PointF(centerX, centerY);
		}

		public static Point GetPointRotate180FlipY(Point point)
		{
			var newPoint = new Point(point.X, point.Y);

			newPoint.X = MapManager.Width - newPoint.X;
			newPoint.Y = MapManager.Height - newPoint.Y;
			newPoint.X = MapManager.Width - newPoint.X;
			return newPoint;
		}

		public static PointF GetPointRotate180FlipY(PointF pointF)
		{
			var newPoint = new PointF(pointF.X, pointF.Y);

			newPoint.X = MapManager.Width - newPoint.X;
			newPoint.Y = MapManager.Height - newPoint.Y;
			newPoint.X = MapManager.Width - newPoint.X;
			return newPoint;
		}

		public static Point[] GetPointsRotate180FlipY(Point[] points) 
		{
			var newPoints = (Point[])points.Clone();
			for (int i = 0; i < newPoints.Length; i++)
			{
				newPoints[i].X = MapManager.Width - newPoints[i].X;
				newPoints[i].Y = MapManager.Height - newPoints[i].Y;
				newPoints[i].X = MapManager.Width - newPoints[i].X;
			}
			return newPoints;
		}

		public static PointF[] GetPointsRotate180FlipY(PointF[] points)
		{
			var newPoints = (PointF[])points.Clone();
			for (int i = 0; i < newPoints.Length; i++)
			{
				newPoints[i].X = MapManager.Width - newPoints[i].X;
				newPoints[i].Y = MapManager.Height - newPoints[i].Y;
				newPoints[i].X = MapManager.Width - newPoints[i].X;
			}
			return newPoints;
		}

		public static Polygon2 PointToPolygon(PointF[] points)
		{
			var polygon = new Polygon2();

			foreach (var point in points)
			{
				polygon.Points.Add(new K2DPosition((int)point.X, (int)point.Y));
			}

			return polygon;
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

		public static PointF[] PolygonToPoint(Polygon2 polygon)
		{
			var points = new PointF[polygon.Points.Count];
			for (var i = 0; i < polygon.Points.Count; i++)
			{
				points[i] = new PointF(polygon.Points[i].X, polygon.Points[i].Y);
			}

			return points;
		}

		public static PointF UnAdjustPoint(Point point, float zoom, bool imprecision, bool tile)
		{
			var position = new PointF((point.X / zoom), (point.Y / zoom));

			if (tile)
			{
				position.X *= Global.tileLenght;
				position.Y *= Global.tileLenght;
			}

			if (imprecision)
			{
				position.X /= 8;
				position.Y /= 8;
			}

			position.X = (int)Math.Round(position.X / 7.875f);
			position.Y = (int)Math.Round(position.Y / 7.875f);

			return GetPointRotate180FlipY(position);
		}

		public static PointF UnAdjustPoint(PointF point, float zoom, bool imprecision, bool tile)
		{
			var position = new PointF((point.X / zoom), (point.Y / zoom));

			if (tile)
			{
				position.X *= Global.tileLenght;
				position.Y *= Global.tileLenght;
			}

			if (imprecision)
			{
				position.X /= 8;
				position.Y /= 8;
			}

			position.X /= 7.875f;
			position.Y /= 7.875f;

			return GetPointRotate180FlipY(position);
		}

		public static PointF UnAdjustPoint(PointF point, int segmentNumber, float x, float y)
		{
			var partX = MapWorker.Instance.X * 16128;
			var partY = MapWorker.Instance.Y * 16128;
			
			var tempX = x - partX;
			var tempY = y - partY;

			var segmentX = tempX / Global.tileLenght % 64;
			var segmentY = segmentNumber / Global.tileLenght / 64;

			var segmentFX = segmentX * Global.tileLenght * 6;
			var segmentFY = segmentY * Global.tileLenght * 6;

			return new PointF(tempX, tempY);
		}

		public static PointF[] UnAdjustPolygonPoint(List<PointF> polygon, float zoom, bool imprecision, bool tile)
		{
			var data = new PointF[polygon.Count];

			for (int i = 0; i < polygon.Count; i++)
			{
				data[i] = new PointF((polygon[i].X / zoom), (polygon[i].Y / zoom));

				if (tile)
				{
					data[i].X *= Global.tileLenght;
					data[i].Y *= Global.tileLenght;
				}

				if (imprecision)
				{
					data[i].X /= 8;
					data[i].Y /= 8;
				}

				data[i].X /= 7.875f;
				data[i].Y /= 7.875f;
			}

			return GetPointsRotate180FlipY(data);
		}
	}
}
