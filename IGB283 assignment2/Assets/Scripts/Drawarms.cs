using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drawarms : MonoBehaviour
{

    public GameObject child;
    public GameObject control;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;
    public Vector3[] limbVertexLocations;

    public Material material;

    public Mesh mesh;

    public static Matrix3x3 Rotate (float angle)
    {
        // Create a new matrix
        Matrix3x3 matrix = new Matrix3x3();
        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(Mathf.Cos(angle),
        -Mathf.Sin(angle), 0.0f));
        matrix.SetRow(1, new Vector3(Mathf.Sin(angle),
        Mathf.Cos(angle), 0.0f));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));
        // Return the matrix
        return matrix;

    }

    public static Matrix3x3 Translate(Vector3 offset)
    {
        // Create a new matrix
        Matrix3x3 matrix = new Matrix3x3();
        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(1.0f, 0.0f, offset.x));
        matrix.SetRow(1, new Vector3(0.0f, 1.0f, offset.y));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));
        // Return the matrix
        return matrix;
    }

    private void DrawLimb()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = material;
        mesh.Clear();
        mesh.vertices = limbVertexLocations;

        if (limbVertexLocations.Length == 4)
        {
            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            mesh.colors = new Color[] {
            new Color(0f, 0f, 1f, 1.0f),
            new Color(0f, 0f, 1f, 1.0f),
            new Color(0f, 0f, 1f, 1.0f),
            new Color(0f, 0f, 1f, 1.0f)
        };
        }
        else if (limbVertexLocations.Length == 5)
        {
            mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2, 3, 4, 2 };

            mesh.colors = new Color[] {
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f),
            new Color(0.8f, 0.3f, 0.3f, 1.0f)
        };
        }

    }

    public void MoveByOffset(Vector3 offset)
    {
        // Find the translation Matrix
        Matrix3x3 T = Translate(offset);
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }

        mesh.vertices = vertices;

        jointLocation = T.MultiplyPoint(jointLocation);

        if (child != null)
        {
            child.GetComponent<Drawarms>().MoveByOffset(offset);
        }
    }

    public void RotateAroundPoint(Vector3 point, float angle, float lastAngle)
    {
        // Move the point to the origin
        Matrix3x3 T1 = Translate(-point);
        // Undo the last rotation
        Matrix3x3 R1 = Rotate(-lastAngle);
        // Move the point back to the oritinal position
        Matrix3x3 T2 = Translate(point);
        // Perform the new rotation
        Matrix3x3 R2 = Rotate(angle);
        // The final translation matrix
        Matrix3x3 M = T2 * R2 * R1 * T1;


        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);

        if (child != null)
        {
            child.GetComponent<Drawarms>().RotateAroundPoint(
            point, angle, lastAngle);
        }
    }


    private void Awake()
    {
        DrawLimb();

    }

    void Start()
    {
        if (child != null)
        {
            child.GetComponent<Drawarms>().MoveByOffset(jointOffset);

        }
    }   
    // Update is called once per frame
    void Update()
    {
        lastAngle = angle;
        if (control != null)
        {
            angle = control.GetComponent<Slider>().value;
        }
        if (child != null)
        {
            child.GetComponent<Drawarms>().RotateAroundPoint(
            jointLocation, angle, lastAngle);
        }
        // Recalculate the bounds of the mesh
        mesh.RecalculateBounds();


    }
}
