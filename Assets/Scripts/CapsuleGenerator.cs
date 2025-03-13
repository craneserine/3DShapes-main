using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CapsuleGenerator : MonoBehaviour
{
    public float radius = 1.0f;
    public float height = 2f;
    public int segments = 8;
    public Material material;

    public Vector3 capsuleRotation; // Rotation property

    void OnDrawGizmos()
    {
        DrawCapsule();
    }

    void DrawCapsule()
    {
        // Create a rotation quaternion from the capsuleRotation vector
        Quaternion rotation = Quaternion.Euler(capsuleRotation);

        // Apply rotation to the cylinder and hemispheres
        DrawCylinder(rotation);
        DrawHemisphere(rotation, 1); // Top hemisphere
        DrawHemisphere(rotation, -1); // Bottom hemisphere
    }

    void DrawCylinder(Quaternion rotation)
    {
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
            topCircle.Add(rotation * (new Vector3(x, 0, z) + topCenter));
            bottomCircle.Add(rotation * (new Vector3(x, 0, z) + bottomCenter));
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

    void DrawHemisphere(Quaternion rotation, int direction)
    {
        // Hemisphere position is either top or bottom, based on the direction parameter
        Vector3 spherePosition = transform.position + new Vector3(0, direction * height * 0.5f, 0);

        // Apply rotation to the hemisphere's position
        spherePosition = rotation * (spherePosition - transform.position) + transform.position;

        float hemisphereRadius = radius;
        int latitudeSegments = 5; // Use fewer segments to make it look more pill-like
        int longitudeSegments = 5;

        // Draw the hemisphere and connect it to the cylinder
        DrawHemisphereGeometry(spherePosition, hemisphereRadius, latitudeSegments, longitudeSegments, direction, rotation);
    }

    void DrawHemisphereGeometry(Vector3 position, float radius, int latitudeSegments, int longitudeSegments, int direction, Quaternion rotation)
    {
        List<Vector3> vertices = new List<Vector3>();

        // Start drawing hemisphere vertices
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float latAngle = Mathf.PI * lat / latitudeSegments;
            float y = Mathf.Cos(latAngle) * radius;
            float circleRadius = Mathf.Sin(latAngle) * radius;

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float lonAngle = 2 * Mathf.PI * lon / longitudeSegments;
                float x = Mathf.Cos(lonAngle) * circleRadius;
                float z = Mathf.Sin(lonAngle) * circleRadius;

                Vector3 point = position + new Vector3(x, y, z);
                vertices.Add(rotation * (point - position) + position); // Apply rotation to each point

                // Connect points to form latitude and longitude lines
                if (lat > 0)
                {
                    GL.Vertex3(vertices[(lat - 1) * (longitudeSegments + 1) + lon].x, vertices[(lat - 1) * (longitudeSegments + 1) + lon].y, vertices[(lat - 1) * (longitudeSegments + 1) + lon].z);
                    GL.Vertex3(vertices[lat * (longitudeSegments + 1) + lon].x, vertices[lat * (longitudeSegments + 1) + lon].y, vertices[lat * (longitudeSegments + 1) + lon].z);
                }

                if (lon > 0)
                {
                    GL.Vertex3(vertices[lat * (longitudeSegments + 1) + lon].x, vertices[lat * (longitudeSegments + 1) + lon].y, vertices[lat * (longitudeSegments + 1) + lon].z);
                    GL.Vertex3(vertices[lat * (longitudeSegments + 1) + (lon - 1)].x, vertices[lat * (longitudeSegments + 1) + (lon - 1)].y, vertices[lat * (longitudeSegments + 1) + (lon - 1)].z);
                }
            }
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Draw hemisphere using vertices
        foreach (var vertex in vertices)
        {
            GL.Vertex3(vertex.x, vertex.y, vertex.z);
        }

        GL.End();
        GL.PopMatrix();

        // Connect hemisphere to the cylinder's ends by drawing additional lines
        ConnectHemisphereToCylinder(position, radius, longitudeSegments, direction, rotation);
    }

    void ConnectHemisphereToCylinder(Vector3 position, float radius, int longitudeSegments, int direction, Quaternion rotation)
    {
        // Get the center of the cylinder's top or bottom
        Vector3 cylinderCenter = transform.position + new Vector3(0, direction * height * 0.5f, 0);

        // Apply rotation to cylinder's center
        cylinderCenter = rotation * (cylinderCenter - transform.position) + transform.position;

        // Connect the top/bottom of the hemisphere to the cylinder’s edge
        List<Vector3> circle = new List<Vector3>();

        for (int i = 0; i < longitudeSegments; i++)
        {
            float angle = 2 * Mathf.PI * i / longitudeSegments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            circle.Add(position + new Vector3(x, 0, z));
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Draw lines from the hemisphere's outer circle to the cylinder's edge
        for (int i = 0; i < longitudeSegments; i++)
        {
            GL.Vertex3(circle[i].x, circle[i].y, circle[i].z);
            GL.Vertex3(cylinderCenter.x, cylinderCenter.y, cylinderCenter.z);
        }

        GL.End();
        GL.PopMatrix();
    }
}