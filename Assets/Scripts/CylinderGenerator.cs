using UnityEngine;
using System.Collections.Generic;

public class CylinderGenerator : MonoBehaviour
{
    public float radius = 1f;
    public float height = 2f;
    public int segments = 8;
    public Material material;

    public Vector3 cylinderCenter;
    public Vector3 cylinderRotation; // Add rotation property

    void OnDrawGizmos()
    {
        DrawCylinder();
    }

    void DrawCylinder()
    {
        // Create a quaternion from the cylinderRotation vector
        Quaternion rotation = Quaternion.Euler(cylinderRotation);

        float halfHeight = height * 0.5f;
        Vector3 topCenter = transform.position + new Vector3(0, halfHeight, 0);
        Vector3 bottomCenter = transform.position + new Vector3(0, -halfHeight, 0);

        // Apply rotation to the top and bottom centers
        topCenter = rotation * (topCenter - transform.position) + transform.position;
        bottomCenter = rotation * (bottomCenter - transform.position) + transform.position;

        List<Vector3> topCircle = new List<Vector3>();
        List<Vector3> bottomCircle = new List<Vector3>();

        // Generate points for the top and bottom circles and apply rotation
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // Apply rotation to each point on the top and bottom circles
            Vector3 topPoint = rotation * new Vector3(x, 0, z) + topCenter;
            Vector3 bottomPoint = rotation * new Vector3(x, 0, z) + bottomCenter;

            topCircle.Add(topPoint);
            bottomCircle.Add(bottomPoint);
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Draw the sides of the cylinder (connect top and bottom circle)
        for (int i = 0; i < segments; i++)
        {
            GL.Vertex3(topCircle[i].x, topCircle[i].y, topCircle[i].z);
            GL.Vertex3(bottomCircle[i].x, bottomCircle[i].y, bottomCircle[i].z);

            GL.Vertex3(topCircle[i].x, topCircle[i].y, topCircle[i].z);
            GL.Vertex3(topCircle[(i + 1) % segments].x, topCircle[(i + 1) % segments].y, topCircle[(i + 1) % segments].z);

            GL.Vertex3(bottomCircle[i].x, bottomCircle[i].y, bottomCircle[i].z);
            GL.Vertex3(bottomCircle[(i + 1) % segments].x, bottomCircle[(i + 1) % segments].y, bottomCircle[(i + 1) % segments].z);
        }

        GL.End();
        GL.PopMatrix();
    }
}