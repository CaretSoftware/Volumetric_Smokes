using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ExtensionsClass {
	
	// Transform
	public static void SetXPos(this Transform t, float x) => t.position = t.position.SetX(x);
	public static void SetYPos(this Transform t, float y) => t.position = t.position.SetY(y);
	public static void SetZPos(this Transform t, float z) => t.position = t.position.SetZ(z);
	
	// Vector3
	public static Vector3 Vector2ToVector3(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);
	public static Vector3 Abs(this Vector3 v3) {
		v3.x = Mathf.Abs(v3.x);
		v3.y = Mathf.Abs(v3.y);
		v3.z = Mathf.Abs(v3.z);
		return v3;
	}
	private static Vector3 SetX(this Vector3 v3, float x) => new Vector3(x, v3.y, v3.z);
	private static Vector3 SetY(this Vector3 v3, float y) => new Vector3(v3.x, y, v3.z);
	private static Vector3 SetZ(this Vector3 v3, float z) => new Vector3(v3.x, v3.y, z);
	
	// Vector2
	public static Vector2 Vector3ToVector2(this Vector3 v3) => new Vector2(v3.x, v3.z);

	// Color
	public static Color Color255(this Color color, Vector3 v3) {
		color.r = v3.x / 255;
		color.g = v3.y / 255;
		color.b = v3.z / 255;
		return color;
	}
	public static Color Color255(this Color color, Vector4 v4) {
		color.r = v4.x / 255;
		color.g = v4.y / 255;
		color.b = v4.z / 255;
		color.a = v4.z / 255;
		return color;
	}
	
	// float
	public static bool IsZero(this float f) => Math.Abs(f) < float.Epsilon;
	// double
	public static bool IsZero(this double d) => Math.Abs(d) < double.Epsilon;

		// Gizmos
	public static void DrawFrustum(Vector3 position, Quaternion rotation, Camera cam, float farClippingPlane = default, float nearClippingPlane = default) {
		Matrix4x4 prevMatrix = Gizmos.matrix;
		
		Gizmos.matrix = Matrix4x4.TRS(
			position,
			rotation, 
			Vector3.one);
		
		Gizmos.DrawFrustum(
			position,
			cam.fieldOfView, 
			cam.farClipPlane, 
			cam.nearClipPlane, 
			cam.aspect);
		
		Gizmos.matrix = prevMatrix;
	}
	public static void DrawWireCircle(Vector3 center, float radius, int segments = 20, Quaternion rotation = default(Quaternion)) {
		DrawWireArc(center,radius,360,segments,rotation);
	}
	public static void DrawWireArc(Vector3 center, float radius, float angle, int segments = 20, Quaternion rotation = default(Quaternion))
	{
		var old = Gizmos.matrix;
		
		Gizmos.matrix = Matrix4x4.TRS(center, rotation,Vector3.one);
		Vector3 from = Vector3.forward * radius;
		var step = Mathf.RoundToInt(angle / segments);
		for (int i = 0; i <= angle; i += step) {
			var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
			Gizmos.DrawLine(from, to);
			from = to;
		}

		Gizmos.matrix = old;
	}
}
