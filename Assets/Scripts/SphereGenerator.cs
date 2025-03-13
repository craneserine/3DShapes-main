using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    public float radius = 1f;
    public int latitudeSegments = 10;
    public int longitudeSegments = 10;
    public Material material;

    public Vector3 sphereRotation; // Add a rotation property

    void OnDrawGizmos()
    {
        if (material == null)
        {
            Debug.LogError("Material is not assigned in the inspector.");
            return;
        }

        DrawSphere();
    }

    public void DrawSphere()
    {
        // Create a quaternion rotation from the sphereRotation vector
        Quaternion rotation = Quaternion.Euler(sphereRotation);

        // Enable drawing with the provided material
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0); // Ensure the material is set for drawing

        // Draw the sphere using latitude and longitude lines
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float latAngle = Mathf.PI * lat / latitudeSegments - Mathf.PI / 2;
            float y = Mathf.Sin(latAngle) * radius;
            float circleRadius = Mathf.Cos(latAngle) * radius;

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float lonAngle = 2 * Mathf.PI * lon / longitudeSegments;
                float x = Mathf.Cos(lonAngle) * circleRadius;
                float z = Mathf.Sin(lonAngle) * circleRadius;

                Vector3 point = transform.position + new Vector3(x, y, z);

                // Apply the rotation to the point
                point = rotation * (point - transform.position) + transform.position;

                // Connect points to form longitude and latitude lines
                if (lat > 0)
                {
                    Vector3 prevPoint = transform.position + new Vector3(Mathf.Cos(lonAngle) * circleRadius, Mathf.Sin(latAngle - Mathf.PI / latitudeSegments) * radius, Mathf.Sin(lonAngle) * circleRadius);
                    prevPoint = rotation * (prevPoint - transform.position) + transform.position;

                    GL.Vertex3(prevPoint.x, prevPoint.y, prevPoint.z);
                    GL.Vertex3(point.x, point.y, point.z);
                }

                if (lon > 0)
                {
                    Vector3 prevPoint = transform.position + new Vector3(Mathf.Cos(lonAngle - Mathf.PI * 2 / longitudeSegments) * circleRadius, Mathf.Sin(latAngle) * radius, Mathf.Sin(lonAngle - Mathf.PI * 2 / longitudeSegments) * circleRadius);
                    prevPoint = rotation * (prevPoint - transform.position) + transform.position;

                    GL.Vertex3(prevPoint.x, prevPoint.y, prevPoint.z);
                    GL.Vertex3(point.x, point.y, point.z);
                }
            }
        }

        GL.End();
        GL.PopMatrix();
    }
}