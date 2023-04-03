using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public static class EditorHelpers {
	
	private static readonly Color UnityGrey = new Color(56f / 255f, 56f / 255f, 56f / 255f);
	
	public static void DrawGird(Vector3 startPos, Vector3 endPos, SceneView sceneView) {
		
		CompareFunction prevZTest = Handles.zTest;
		Camera cam = sceneView.camera;
		const float maxLength = 250f;

		// Calculate bounds
		float xSign = startPos.x < endPos.x ? +1 : -1 ;
		float zSign = startPos.z < endPos.z ? +1 : -1 ;
		
		// limit length of x and z to avoid lag and crashes
		if (xSign < 0)
			endPos.x = Mathf.Max(endPos.x, startPos.x - maxLength);
		else
			endPos.x = Mathf.Min(endPos.x, startPos.x + maxLength);

		if (zSign < 0)
			endPos.z = Mathf.Max(endPos.z, startPos.z - maxLength);
		else
			endPos.z = Mathf.Min(endPos.z, startPos.z + maxLength);

		float sx = startPos.x;
		float sz = startPos.z;
		float ex = endPos.x;
		float ez = endPos.z;

		// assign bound limits
		float minX = Mathf.Min(sx, ex);
		float maxX = Mathf.Max(sx, ex);
		float minZ = Math.Min(sz, ez);
		float maxZ = Math.Max(sz, ez);
		
		// Draw visible grid
		Handles.zTest = CompareFunction.LessEqual;
		Handles.color = new Color(.7f, .7f, 1f, 1f);
		
		for (float x = sx; x >= minX && x <= maxX; x += xSign) {
			Vector3 sPos = new Vector3(x, 0, sz);
			Vector3 ePos = new Vector3(x, 0, ez);
			Handles.DrawAAPolyLine(6.0f,sPos, ePos);
		}
		
		for (float z = sz; z >= minZ && z <= maxZ; z += zSign) {
			Vector3 sPos = new Vector3(sx, 0, z);
			Vector3 ePos = new Vector3(ex, 0, z);
			Handles.DrawAAPolyLine(6.0f,sPos, ePos);
		}
		
		// Draw faded grid behind objects
		Handles.zTest = CompareFunction.Greater;
		Handles.color = new Color(1, 1, 1, .08f);
		
		for (float x = sx; x >= minX && x <= maxX; x += xSign) {
			Vector3 sPos = new Vector3(x, 0, sz);
			Vector3 ePos = new Vector3(x, 0, ez);
			Handles.DrawAAPolyLine(6.0f,sPos, ePos);
		}
		
		for (float z = sz; z >= minZ && z <= maxZ; z += zSign) {
			Vector3 sPos = new Vector3(sx, 0, z);
			Vector3 ePos = new Vector3(ex, 0, z);
			Handles.DrawAAPolyLine(6.0f,sPos, ePos);
		}
		
		// Draw bounds of grid
		Handles.zTest = CompareFunction.Always;

		Vector3[] corners = new Vector3[] {
			new Vector3(ex,0,sz),
			new Vector3(ex,0,ez),
			new Vector3(ex,0,ez),
			new Vector3(sx,0,ez),
			new Vector3(sx,0,ez),
			new Vector3(sx,0,sz),
			new Vector3(sx,0,sz),
			new Vector3(ex,0,sz),
		};
		
		Color colorBlue = new Color(.5f, .5f, 1f, 1f);
		Color colorRed = new Color(1f, .5f, .5f, 1f);
		Color[] c = new Color[] {
			colorBlue,
			colorBlue,
			colorRed,
			colorRed,
			new Color(.7f, .7f, 1f, 1f), // colorBlue,
			new Color(.7f, .7f, 1f, 1f), // colorBlue,
			new Color(.7f, .7f, 1f, 1f), // colorRed,
			new Color(.7f, .7f, 1f, 1f), // colorRed,
		};
		
		for (int i = 0; i < corners.Length; ) {
			Handles.color = c[i];
			Handles.DrawAAPolyLine(6.0f, corners[i++], corners[i++]);	
		}

		// Draw Dimension Labels at mouse position (endPos)
		Quaternion cameraRotation = Quaternion.Inverse(cam.transform.rotation);
		
			// offset depending on direction of grid
		Vector3 labelOffsetX = cameraRotation * (new Vector3(-xSign, .3f, zSign) * 50);
		Vector3 labelOffsetZ = cameraRotation * new Vector3(xSign, .3f, -zSign) * 50;
		
			// prevent label being too close to the camera and being culled
		labelOffsetX.z = 0; 
		labelOffsetZ.z = 0;
		
			// calculate screen position with constant pixel offset
		Vector3 screenPointX = cam.ScreenToWorldPoint(cam.WorldToScreenPoint(endPos) + labelOffsetX);
		Vector3 screenPointZ = cam.ScreenToWorldPoint(cam.WorldToScreenPoint(endPos) + labelOffsetZ);

		// Initialise GUIStyles for text colors and outline
		GUIStyle blue = new GUIStyle();
		blue.normal.textColor = new Color(0,0,1, .3f);
		blue.alignment = TextAnchor.MiddleCenter;
		blue.fontStyle = FontStyle.Normal;
		GUIStyle red = new GUIStyle();
		red.normal.textColor = new Color(1,0,0, .3f);
		red.alignment = TextAnchor.MiddleCenter;
		red.fontStyle = FontStyle.Normal;
		GUIStyle bold = new GUIStyle();
		bold.normal.textColor = Color.white;
		bold.alignment = TextAnchor.MiddleCenter;
		bold.fontStyle = FontStyle.Bold;
		
			// Draw white "outline"
		Handles.Label(screenPointX, $"{Mathf.Abs(sx - ex):F1}", bold);
			// Draw red fill
		Handles.Label(screenPointX, $"{Mathf.Abs(sx - ex):F1}", red);
			// Draw white "outline"
		Handles.Label(screenPointZ, $"{Mathf.Abs(sz - ez):F1}", bold);
			// Draw blue fill
		Handles.Label(screenPointZ, $"{Mathf.Abs(sz - ez):F1}", blue);
		
		// reset handles color
		Handles.color = Color.white;
		Handles.zTest = prevZTest;
	}
	
	public static void DrawCurve(EditorWindow editorWindow, Func<float, float> curveFunction) {

		Rect rect;
		Color lastColor = Handles.color;

		GUILayout.Space(10);
		GUIStyle guiStyle = new GUIStyle();
		guiStyle.fixedWidth = 290;
		guiStyle.fixedHeight = 75;
		guiStyle.padding = new RectOffset(15, 0, 0, 0);
		EditorGUILayout.BeginVertical(guiStyle);
		rect = GUILayoutUtility.GetRect(400, 75);
		EditorGUI.DrawRect(rect, UnityGrey);

		float yMin = float.MaxValue;
		float yMax = float.MinValue;
		float step = 1 / editorWindow.position.width;

		// adjust bounds to highest and lowest values
		for (float i = 0; i <= 1; i += step) {
			float t = curveFunction(i);
			yMin = Mathf.Min(yMin, t);
			yMax = Mathf.Max(yMax, t);
		}

		// yMin = Mathf.Clamp(yMin, -1, 1);
		// yMax = Mathf.Clamp(yMax, -1, 1);

		// draw 0 axis
		if (0 <= yMax && 0 >= yMin)
			DrawDottedLine(0, 2, Color.gray);
		// draw 1 axis
		if (1 - 0.05f <= yMax)
			DrawDottedLine(1, 5, Color.gray);
		// draw -1 axis
		if (-1 + 0.05f >= yMin)
			DrawDottedLine(-1, 5, Color.gray);

		void DrawDottedLine(float val, float lineSpacing, Color c) {
			Handles.color = c;
			GUIStyle axisTextStyle = new GUIStyle();
			axisTextStyle.normal.textColor = Color.gray;
			
			Handles.DrawDottedLine(
				new Vector3(
					rect.xMin,
					rect.yMax - ((val - yMin) / (yMax - yMin)) * rect.height,
					0),
				new Vector3(
					rect.xMin + rect.width,
					rect.yMax - ((val - yMin) / (yMax - yMin)) * rect.height,
					0),
				lineSpacing);

			string str = $"{val}";
			Handles.Label(
				new Vector3(
					5 - (5 * (str.Length - 1)),
					rect.yMax - ((val - yMin) / (yMax - yMin)) * rect.height - 5), 
				str,
				axisTextStyle);
		}

		Handles.color = Color.white;
		
		Vector3 prevPos = new Vector3(0, curveFunction(0), 0);
		for (float t = 0; t < 1; t += step) {
			Vector3 pos = new Vector3(t, curveFunction(t), 0);
			Handles.DrawAAPolyLine(
				2,
				new Vector3(
					rect.xMin + prevPos.x * rect.width, 
					rect.yMax - ((prevPos.y - yMin) / (yMax - yMin)) * rect.height, 
					0), 
				new Vector3( 
					rect.xMin + pos.x * rect.width,
					rect.yMax - ((pos.y - yMin) / (yMax - yMin)) * rect.height,
					0));
			prevPos = pos;
		}
		
		// draw xAxis
		Handles.color = Color.gray;
		Handles.DrawLine(
			new Vector3(
				rect.xMin, 
				rect.yMin),
			new Vector3(
				rect.xMin,
				rect.yMax));
		
		// draw yAxis
		Handles.color = Color.gray;
		Handles.DrawLine(
			new Vector3(
				rect.xMin, 
				rect.yMax),
			new Vector3(
				rect.xMax,
				rect.yMax));

		Handles.color = lastColor;
		EditorGUILayout.EndVertical();
		GUILayout.Space(10);
	}
	
	public static int DrawTabButtons(string[] labels, int btn) {
		
		Rect defaultRect = EditorGUILayout.BeginVertical();
		Rect buttonRect = defaultRect;
		
		// Add Padding to rects
		defaultRect.x += 4f;
		buttonRect.x = defaultRect.x;
		
		defaultRect.height = 120f;
		defaultRect.width = 60f;
		
		GUIStyle buttonSelected = GUIStyle.none;
		buttonSelected.normal.textColor = Color.white;
		buttonSelected.alignment = TextAnchor.UpperCenter;
		
		buttonRect.width = 50;
		buttonRect.height = 20;
		buttonRect.y += 6;
		for (int i = 0; i < labels.Length; i++) {
			if (GUI.Button(buttonRect, labels[i], btn == i ? buttonSelected : EditorStyles.miniButtonLeft))
				btn = i;
			buttonRect.y += 20;
		}
		
		DrawSelectedButton(btn, buttonRect.height);

		//Now the Key Part, add a space the size of the texture, so that any other GUILayout draw calls
		//will be placed below the texture
		float padding = 4.0f;
		GUILayout.Space(defaultRect.height + padding);
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(defaultRect.width);
		EditorGUILayout.EndHorizontal();
		return btn;
		
		void DrawSelectedButton(int buttonPos, float buttonHeight) {
			Color def = Handles.color;
			Handles.color = Color.black;
			float thickness = 0;
			float r = 3f;
			float selectedButtonPosYMin = buttonPos * buttonHeight + buttonHeight - 15;
			float selectedButtonPosYMax = buttonPos * buttonHeight + buttonHeight + 5;
			
			// Draw Upper Tab Line
			Handles.color = Color.gray;
			Handles.DrawLine(
				new Vector3(
					defaultRect.xMin + 2, 
					defaultRect.yMin + selectedButtonPosYMin),
				new Vector3(
					defaultRect.xMin + buttonRect.width ,
					defaultRect.yMin + selectedButtonPosYMin),
				thickness);
			Handles.color = Color.black;
			// Draw Lower Tab Line
			Handles.DrawLine(
				new Vector3(
					defaultRect.xMin + 2, 
					defaultRect.yMin + selectedButtonPosYMax),
				new Vector3(
					defaultRect.xMin + buttonRect.width ,
					defaultRect.yMin + selectedButtonPosYMax),
				thickness);
			// Draw Side Tab Line
			Handles.color = Color.gray;
			Handles.DrawLine(
				new Vector3(
					defaultRect.xMin + 0, 
					defaultRect.yMin + selectedButtonPosYMin + r),
				new Vector3(
					defaultRect.xMin + 0,
					defaultRect.yMin + selectedButtonPosYMax - r),
				thickness);
			// Draw Upper Tab Curve
			Handles.DrawWireArc( new Vector3(defaultRect.xMin + r, defaultRect.yMin + selectedButtonPosYMin + r),
				-Vector3.forward,
				Vector3.down, 
				90,
				r,
				thickness
			);
			Handles.color = Color.black;
			// Draw Lower Tab Curve
			Handles.DrawWireArc( new Vector3(defaultRect.xMin + r, defaultRect.yMin + selectedButtonPosYMax - r),
				-Vector3.forward,
				Vector3.left, 
				90,
				r,
				thickness
			);
			// Draw Upper Tab Separator
			Handles.color = Color.gray;
			Handles.DrawLine(
				new Vector3(
					defaultRect.xMin + buttonRect.width, 
					defaultRect.yMin),
				new Vector3(
					defaultRect.xMin + buttonRect.width,
					defaultRect.yMin + selectedButtonPosYMin),
				thickness);
			// Draw Lower Tab Separator
			Handles.DrawLine(
				new Vector3(
					defaultRect.xMin + buttonRect.width, 
					defaultRect.yMin + selectedButtonPosYMax),
				new Vector3(
					defaultRect.xMin + buttonRect.width,
					defaultRect.yMax),
				thickness);
			// Draw Upper Rect Delineation
			Handles.color = Color.gray;
			Handles.DrawLine(
				new Vector3(
					defaultRect.xMin + buttonRect.width, 
					defaultRect.yMin),
				new Vector3(
					 defaultRect.xMin + buttonRect.width + 2,
					defaultRect.yMin),
				thickness);
			Handles.color = def;
		}
		
		// GUI.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
		// GUI.DrawTexture(boxRect, Texture2D.whiteTexture);
	}
	
	public static void DrawAlertMessage(string message, MessageType messageType) {
		EditorGUILayout.HelpBox(message, messageType);
	}
}
