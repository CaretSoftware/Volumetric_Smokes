using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralAnimation))]
public class SecondOrderEditor : Editor {
    private static Color _inspectorGray = new Color(0.22352941176f, 0.22352941176f, 0.22352941176f);
    private static Color lineColorZero = new Color(.15f, .15f, .15f);
    private static Color lineColorOne = new Color(.3f, .3f, .3f);
    private Texture2D _previousTexture2D;

    private void OnEnable() {
        _previousTexture2D = new Texture2D(1, 1);
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        DrawGraph();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGraph() {
        ProceduralAnimation component = target as ProceduralAnimation;

        Vector2[] dataPoints = component.GetGraph(out float zero, out float one);

        int inspectorWidth = (int)EditorGUIUtility.currentViewWidth;

        DestroyImmediate(_previousTexture2D);
        Texture2D graphTexture = new Texture2D(inspectorWidth, 100);
        _previousTexture2D = graphTexture;
        Color backgroundColor = _inspectorGray;
        Color graphColor = Color.white;

        // Clear the texture with the background color
        Color[] pixels = new Color[graphTexture.width * graphTexture.height];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = backgroundColor;
        }

        graphTexture.SetPixels(pixels);

        // Draw the scalar lines
        float height = graphTexture.height;
        for (int i = 0; i < graphTexture.width; i++) {
            graphTexture.SetPixel(i, (int)(height*zero), lineColorZero);
            graphTexture.SetPixel(i, (int)(height*one), lineColorOne);
        }
        
        // Draw the graph line
        for (int i = 0; i < dataPoints.Length - 1; i++) {
            Vector2 startPoint = new Vector2(
                Mathf.Lerp(0, graphTexture.width, dataPoints[i].x),
                Mathf.Lerp(0, graphTexture.height, dataPoints[i].y)
            );
            Vector2 endPoint = new Vector2(
                Mathf.Lerp(0, graphTexture.width, dataPoints[i + 1].x),
                Mathf.Lerp(0, graphTexture.height, dataPoints[i + 1].y)
            );
            DrawLine(graphTexture, startPoint, endPoint, graphColor);
        }

        graphTexture.Apply();

        // Display the graph in the inspector
        GUILayout.Label(graphTexture);
    }

    private void DrawLine(Texture2D texture, Vector2 p1, Vector2 p2, Color color, int height = 100) {
        var x0 = (int)p1.x;
        var y0 = (int)p1.y;
        var x1 = (int)p2.x;
        var y1 = (int)p2.y;

        int dx = Mathf.Abs(x1 - x0); // horizontal change 
        int dy = Mathf.Abs(y1 - y0); // vertical change

        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;

        Color drawColor = color;
        
        int err = dx - dy;
        int safety = 0;
        while (safety++ < height) {

            bool draw = y0 < texture.height && y0 > 0;
            
            drawColor = draw ? color : _inspectorGray;
            
            texture.SetPixel(x0, y0, drawColor);

            if (x0 == x1 && y0 == y1)
                break;
            
            int e2 = 2 * err;

            if (e2 > -dy) {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx) {
                err += dx;
                y0 += sy;
            }
        }
    }
}
