using UnityEngine;

public class RectangleGenerator : MonoBehaviour
{
    public float length = 2f;
    public float width = 1f;
    public Material material;

    public Vector3 rectangleRotation; // Add rotation property

    void OnDrawGizmos()
    {
        DrawRectangle();
    }

    void DrawRectangle()
    {
        Vector3[] rectVertices = new Vector3[4];
        float halfLength = length * 0.5f; // Length (horizontal)
        float halfWidth = width * 0.5f;  // Width (vertical)
        
        rectVertices[0] = transform.position + new Vector3(halfLength, halfWidth, 0);
        rectVertices[1] = transform.position + new Vector3(-halfLength, halfWidth, 0);
        rectVertices[2] = transform.position + new Vector3(-halfLength, -halfWidth, 0);
        rectVertices[3] = transform.position + new Vector3(halfLength, -halfWidth, 0);

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Draw the rectangle
        for (int i = 0; i < rectVertices.Length; i++)
        {
            GL.Vertex3(rectVertices[i].x, rectVertices[i].y, rectVertices[i].z);
            GL.Vertex3(rectVertices[(i + 1) % rectVertices.Length].x, rectVertices[(i + 1) % rectVertices.Length].y, rectVertices[(i + 1) % rectVertices.Length].z);
        }

        GL.End();
        GL.PopMatrix();
    }
}