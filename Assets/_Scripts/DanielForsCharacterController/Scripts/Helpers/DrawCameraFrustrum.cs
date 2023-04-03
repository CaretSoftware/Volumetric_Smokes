using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DrawCameraFrustrum : MonoBehaviour
{
    Camera m_camera;

    void OnDrawGizmos()
    {
        if (m_camera == null)
        {
            m_camera = gameObject.GetComponent<Camera>();
        }

        Color tempColor = Gizmos.color;
        Matrix4x4 tempMat = Gizmos.matrix;
        if (this.m_camera.orthographic)
        {
            Camera c = m_camera;
            var size = c.orthographicSize;
            Gizmos.DrawWireCube(Vector3.forward * (c.nearClipPlane + (c.farClipPlane-c.nearClipPlane)/2)
                , new Vector3(size * 2.0f, size * 2.0f * c.aspect, c.farClipPlane-c.nearClipPlane));
        }
        else
        {
            Camera c = m_camera;
            Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);
        }
        Gizmos.color = tempColor;
        Gizmos.matrix = tempMat;
    }
    
    // private void OnDrawGizmos() {
    //     
    //     Gizmos.color = Color.white;
    //
    //     Gizmos.matrix = Matrix4x4.TRS( 
    //         m_camera.transform.position,
    //         m_camera.transform.rotation, 
    //         new Vector3(1.0f, 1.0f, 1.0f) );
    //
    //     Gizmos.DrawFrustum(
    //         Vector3.zero,
    //         m_camera.fieldOfView, 
    //         12.0f, 
    //         .3f, 
    //         m_camera.aspect);
    // }
}