using UnityEngine;

public class PyramidGenerator : MonoBehaviour
{
    public float pyramidBaseLength = 1f;
    public float pyramidHeight = 2f;
    public Material material;

    public Vector3 pyramidRotation; // Rotation property

    void OnDrawGizmos()
    {
        DrawPyramid();
    }

    void DrawPyramid()
    {
        // Apply rotation to the pyramid's vertices
        Quaternion rotation = Quaternion.Euler(pyramidRotation);

        // Calculate the position of the pyramid's top vertex
        Vector3 top = transform.position + rotation * new Vector3(0, pyramidHeight, 0);

        // Calculate the base vertices
        Vector3[] baseVertices = new Vector3[4];
        float halfLength = pyramidBaseLength * 0.5f;

        baseVertices[0] = transform.position + rotation * new Vector3(halfLength, 0, halfLength);
        baseVertices[1] = transform.position + rotation * new Vector3(-halfLength, 0, halfLength);
        baseVertices[2] = transform.position + rotation * new Vector3(-halfLength, 0, -halfLength);
        baseVertices[3] = transform.position + rotation * new Vector3(halfLength, 0, -halfLength);

        // Start drawing
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Draw the pyramid's sides
        for (int i = 0; i < 4; i++)
        {
            GL.Vertex3(top.x, top.y, top.z);
            GL.Vertex3(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z);

            GL.Vertex3(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z);
            GL.Vertex3(baseVertices[(i + 1) % 4].x, baseVertices[(i + 1) % 4].y, baseVertices[(i + 1) % 4].z);
        }

        GL.End();
        GL.PopMatrix();
    }
}
