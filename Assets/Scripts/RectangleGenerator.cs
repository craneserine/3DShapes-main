using UnityEngine;

public class RectangleGenerator : MonoBehaviour
{
    public Material material;  // Material for rendering the lines
    public Vector3 center = new Vector3(0, 0, 10);  // Center of the rectangle
    public float length = 2f;  // Length (x-axis)
    public float width = 1f;   // Width (y-axis)
    public float depth = 0.5f; // Depth (z-axis) for 3D effect
    public float focalLength = 5f;  // Focal length for the 3D perspective
    public Vector3 rectangleRotation = Vector3.zero;  // Rotation of the rectangle in 3D space

    private void OnPostRender()
    {
        DrawRectangle();
    }

    private void OnDrawGizmos()
    {
        DrawRectangle();
    }

    private void DrawRectangle()
    {
        if (material == null)
        {
            Debug.LogError("You need to add a material");
            return;
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Calculate half dimensions
        float halfLength = length * 0.5f;
        float halfWidth = width * 0.5f;
        float halfDepth = depth * 0.5f;

        // Define the front corners (before rotation)
        Vector3[] frontCorners = new Vector3[]
        {
            center + new Vector3(-halfLength, halfWidth, -halfDepth),  // Front top left
            center + new Vector3(halfLength, halfWidth, -halfDepth),   // Front top right
            center + new Vector3(halfLength, -halfWidth, -halfDepth),  // Front bottom right
            center + new Vector3(-halfLength, -halfWidth, -halfDepth)  // Front bottom left
        };

        // Define the back corners (with depth)
        Vector3[] backCorners = new Vector3[]
        {
            center + new Vector3(-halfLength, halfWidth, halfDepth),  // Back top left
            center + new Vector3(halfLength, halfWidth, halfDepth),   // Back top right
            center + new Vector3(halfLength, -halfWidth, halfDepth),  // Back bottom right
            center + new Vector3(-halfLength, -halfWidth, halfDepth)  // Back bottom left
        };

        // Apply rotation to both the front and back corners
        Quaternion rotationQuat = Quaternion.Euler(rectangleRotation);
        for (int i = 0; i < 4; i++)
        {
            frontCorners[i] = RotatePointAroundPivot(frontCorners[i], center, rotationQuat);
            backCorners[i] = RotatePointAroundPivot(backCorners[i], center, rotationQuat);
        }

        // Draw the front rectangle edges
        for (int i = 0; i < 4; i++)
        {
            DrawLine(frontCorners[i], frontCorners[(i + 1) % 4]);
        }

        // Draw the back rectangle edges
        for (int i = 0; i < 4; i++)
        {
            DrawLine(backCorners[i], backCorners[(i + 1) % 4]);
        }

        // Draw the connecting edges between the front and back
        for (int i = 0; i < 4; i++)
        {
            DrawLine(frontCorners[i], backCorners[i]);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        // Scale based on the focal length
        float startScale = focalLength / (start.z + focalLength);
        float endScale = focalLength / (end.z + focalLength);

        Vector3 scaledStart = new Vector3(start.x * startScale, start.y * startScale, 0f);
        Vector3 scaledEnd = new Vector3(end.x * endScale, end.y * endScale, 0f);

        // Draw the line in 2D space (Z is set to 0 for 2D)
        GL.Vertex3(scaledStart.x, scaledStart.y, 0);
        GL.Vertex3(scaledEnd.x, scaledEnd.y, 0);
    }

    // Helper function to rotate a point around a pivot in 3D space
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot; // Direction vector from pivot to point
        direction = rotation * direction;  // Apply the rotation
        return pivot + direction;  // Return the rotated point
    }
}