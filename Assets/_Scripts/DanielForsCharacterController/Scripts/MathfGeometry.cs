using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfGeometry {

	public static bool PointInsideShape(Vector3 p, List<Vector3> lineSegments) {
		Vector3 pInfiniteRight = new Vector3(p.x, p.y, float.MaxValue);
		int numIntersections = 0;
		int i = 0;
		do {
			if (LineSegmentsIntersect(
				    p,
				    pInfiniteRight,
				    lineSegments[i++],
				    lineSegments[i],
				    out Vector3 intersection))
				numIntersections++;
		} while (i < lineSegments.Count);
		
		// wrap around from last point to first point
		if (LineSegmentsIntersect(
			    p,
			    pInfiniteRight,
			    lineSegments[i],
			    lineSegments[0],
			    out Vector3 intersection2))
			numIntersections++;

		return numIntersections % 2 == 0; // if num intersections is an odd number point was outside the shape
	}
	/// <summary>
	/// Test whether two line segments intersect on x-, z-plane. If so, calculate the intersection point.
	/// </summary>
	/// <href>https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments</href>
	/// <param name="p">Vector to the start point of p.</param>
	/// <param name="p2">Vector to the end point of p.</param>
	/// <param name="q">Vector to the start point of q.</param>
	/// <param name="q2">Vector to the end point of q.</param>
	/// <param name="intersection">The Vector3 point of intersection, if any.</param>
	/// <returns>True if an intersection point was found.</returns>
	public static bool LineSegmentsIntersect(Vector3 p, Vector3 p2, Vector3 q, Vector3 q2, out Vector3 intersection) {
		bool intersected = LineSegmentsIntersect(
			p.Vector3ToVector2(), 
			p2.Vector3ToVector2(), 
			q.Vector3ToVector2(), 
			q2.Vector3ToVector2(), 
			out Vector2 intersectionRef);
		
		intersection = intersectionRef.Vector2ToVector3();
		
		return intersected;
	}
	
	/// <summary>
	/// Test whether two line segments intersect on on x-, y-plane. If so, calculate the intersection point.
	/// </summary>
	/// <href>https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments</href>
	/// <param name="p">Vector to the start point of p.</param>
	/// <param name="p2">Vector to the end point of p.</param>
	/// <param name="q">Vector to the start point of q.</param>
	/// <param name="q2">Vector to the end point of q.</param>
	/// <param name="intersection">The Vector2 point of intersection, if any.</param>
	/// <returns>True if an intersection point was found.</returns>
	public static bool LineSegmentsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2, out Vector2 intersection)
	{
		intersection = new Vector2();
		Vector2 r = p2 - p;
		Vector2 s = q2 - q;
		float rxs = Cross(r, s);
		float qpxr = Cross(q - p, r);

		// If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
		if (rxs.IsZero() && qpxr.IsZero())
		{
			// 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
			// then the two lines are overlapping,
			// if (considerCollinearOverlapAsIntersect)
			// 	if ((0 <= (q - p)*r && 
			// 	     (q - p)*r <= r*r) || 
			// 	    (0 <= (p - q)*s && 
			// 	     (p - q)*s <= s*s))
			// 		return true;

			// 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
			// then the two lines are collinear but disjoint.
			// No need to implement this expression, as it follows from the expression above.
			return false;
		}

		// 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
		if (rxs.IsZero() && !qpxr.IsZero())
			return false;

		// t = (q - p) x s / (r x s)
		float t = Cross(q - p, s) / rxs;

		// u = (q - p) x r / (r x s)
		
		float u = Cross((q - p), r) / rxs;

		// 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
		// the two line segments meet at the point p + t r = q + u s.
		if (!rxs.IsZero() && 
		    (0 <= t && 
		     t <= 1) && 
		    (0 <= u && 
		     u <= 1))
		{
			// We can calculate the intersection point using either t or u.
			intersection = p + t * r;

			// An intersection was found.
			return true;
		}

		// 5. Otherwise, the two line segments are not parallel but do not intersect.
		return false;

		float Cross(Vector2 v, Vector2 w) {
			return v.x * w.y - v.y * w.x;
		}
	}
	
	// public class Vector
	// {
	// 	public double X;
	// 	public double Y;
	//
	// 	// Constructors.
	// 	public Vector(double x, double y) { X = x; Y = y; }
	// 	public Vector() : this(double.NaN, double.NaN) {}
	//
	// 	public static Vector operator -(Vector v, Vector w)
	// 	{
	// 		return new Vector(v.X - w.X, v.Y - w.Y);
	// 	}
	//
	// 	public static Vector operator +(Vector v, Vector w)
	// 	{
	// 		return new Vector(v.X + w.X, v.Y + w.Y);
	// 	}
	//
	// 	public static double operator *(Vector v, Vector w)
	// 	{
	// 		return v.X * w.X + v.Y * w.Y;
	// 	}
	//
	// 	public static Vector operator *(Vector v, double mult)
	// 	{
	// 		return new Vector(v.X * mult, v.Y * mult);
	// 	}
	//
	// 	public static Vector operator *(double mult, Vector v)
	// 	{
	// 		return new Vector(v.X * mult, v.Y * mult);
	// 	}
	//
	// 	public double Cross(Vector v)
	// 	{
	// 		return X * v.Y - Y * v.X;
	// 	}
	//
	// 	public override bool Equals(object obj) {
	// 		
	// 		if (obj == null || !(obj is Vector)) return false;
	// 		var v = (Vector)obj;
	// 		return (X - v.X).IsZero() && (Y - v.Y).IsZero();
	// 	}
	// }

	
	/// <summary>
	/// Get intersection point of two lines extending to infinity
	/// </summary>
	/// <param name="posA">pivot position of first line</param>
	/// <param name="dirA">direction of first line</param>
	/// <param name="posB">pivot position of first line</param>
	/// <param name="dirB">direction of second line</param>
	/// <returns>position on xz-plane where lines intersect. y-coordinate is 0</returns>
	public static Vector3 LineLineIntersectionPoint(Vector3 posA, Vector3 dirA, Vector3 posB, Vector3 dirB) {
			Vector2 v2 = LineLineIntersectionPoint( new Vector2(posA.x, posA.z), new Vector2(dirA.x, dirA.z), new Vector2(posB.x, posB.z), new Vector2(dirB.x, dirB.z));
			return new Vector3(v2.x, 0, v2.y);
	}

	/// <summary>
	/// Get intersection point of two infinitely extending lines
	/// </summary>
	/// <param name="posA">pivot position of first line</param>
	/// <param name="dirA">direction of first line</param>
	/// <param name="posB">pivot position of first line</param>
	/// <param name="dirB">direction of second line</param>
	/// <returns>position on xy-plane where lines intersect.</returns>
	public static Vector2 LineLineIntersectionPoint(Vector2 posA, Vector2 dirA, Vector2 posB, Vector2 dirB) {
		// Graphics Gems p.139
		// https://books.google.se/books?id=CCqzMm_-WucC&pg=PA141&lpg=PA141&dq=%22perp+dot+product%22+intersection&source=bl&ots=msmz_6LJff&sig=OzXyR_kcJlpdOuFeYFMMUELuni0&hl=en&sa=X&ei=R0I0Uu_LFsHF2QWI1IH4Ag&redir_esc=y#v=onepage&q=%22perp%20dot%20product%22%20intersection&f=false
		Vector2 direction = posB - posA;
		return posA + (PerpendicularLineDotProduct(direction, dirB) / PerpendicularLineDotProduct(dirA, dirB)) * dirA;

		float PerpendicularLineDotProduct(Vector2 first, Vector2 second) {
			return Vector2.Dot(PerpendicularVector(first), second);
		
		}
		Vector2 PerpendicularVector(Vector2 originalVector) {
			Vector2 returnValue;
			returnValue.x = -originalVector.y;
			returnValue.y = originalVector.x;
			return returnValue;
		}
	}

	/// <summary>
	/// Get intersection point on plane. Returns point even if hit on back of plane.
	/// </summary>
	/// <param name="rayOrigin">origin of ray</param>
	/// <param name="rayDirection">direction of ray - gets normalised</param>
	/// <param name="planePosition">pivot point of plane</param>
	/// <param name="normalDirection">the direction of the face of the plane</param>
	/// <returns>The point of ray intersection on plane. If ray doesn't hit on planes normal - rayOrigin is returned</returns>
	public static Vector3 PlaneIntersectionPoint(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePosition, Vector3 normalDirection) {
		rayDirection.Normalize();
		Plane plane = new Plane(normalDirection, planePosition);
		Ray ray = new Ray(rayOrigin, rayDirection);
	
		if (plane.Raycast(ray, out float distance))
			return rayOrigin + rayDirection * distance;
		else
			return rayOrigin;
	}
	
	/// <summary>
	/// Get intersection point on face of plane. Returns point only if hit on planes normal.
	/// </summary>
	/// <param name="rayOrigin">origin of ray</param>
	/// <param name="rayDirection">direction of ray</param>
	/// <param name="planePosition">pivot point of plane</param>
	/// <param name="normalDirection">the direction of the face of the plane</param>
	/// <returns>The point of ray intersection on plane. If ray doesn't hit plane - rayOrigin is returned</returns>
	public static Vector3 FaceIntersectionPoint(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePosition, Vector3 normalDirection) {
		normalDirection *= -1;
		float denominator = Vector3.Dot(rayDirection, normalDirection);

		if (denominator > 0.00001f) {
			float distance = Vector3.Dot((planePosition - rayOrigin), normalDirection) / denominator;
			return rayOrigin + rayDirection * distance;
		}
		else 
			return rayOrigin;
	}
	
	/// <summary>
	/// Get position on line nearest to point
	/// </summary>
	/// <param name="linePivot"></param>
	/// <param name="lineDir"></param>
	/// <param name="point"></param>
	/// <returns></returns>
	public static Vector3 NearestPointOnLine(Vector3 linePivot, Vector3 lineDir, Vector3 point) {
		lineDir.Normalize();//this needs to be a unit vector
		Vector3 vec = point - linePivot;
		float distance = Vector3.Dot(vec, lineDir);
		return linePivot + lineDir * distance;
	}
	
	/// <summary>
	/// Check to see if a point is inside a triangle of points a,b,c.
	/// Works for either winding order?
	/// </summary>
	/// <author> John Bananas </author>
	/// <href>"https://stackoverflow.com/a/9755252" John Bananas StackOverflow answer</href>
	/// <param name="p">the point</param>
	/// <param name="a">triangle point</param>
	/// <param name="b">triangle point</param>
	/// <param name="c">triangle point</param>
	/// Todo Make overloaded function for x-,y-, or z-facing comparisons
	public static bool PointInsideTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		float asX = p.x-a.x;
		float asZ = p.z-a.z;

		// is the point s to the left of or to the right of both the lines AB and AC? 
		bool pAb = (b.x-a.x)*asZ-(b.z-a.z)*asX > 0;
		if((c.x-a.x)*asZ-(c.z-a.z)*asX > 0 == pAb) return false;
		// since we know that a point inside a trigon (triangle) must be to the same side of AB as BC (and also CA),
		// we check if they differ. If they do, s can't possibly be inside, otherwise s must be inside.
		if((c.x-b.x)*(p.z-b.z)-(c.z-b.z)*(p.x-b.x) > 0 != pAb) return false;

		return true;
	}
}