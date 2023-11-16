using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] Vector3 displacement;
    [SerializeField] float angle;
    [SerializeField] AXIS rotationAxis;
    [SerializeField] float wheelScale = 1.0f;
    [SerializeField] Mesh wheelMesh;

    private List<GameObject> wheels = new List<GameObject>();
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;

    void Start()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        baseVertices = mesh.vertices;

        newVertices = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            newVertices[i] = baseVertices[i];
        }

        // Instantiate four wheels as child objects of the car
        for (int i = 0; i < 4; i++)
        {
            InstantiateWheel(i);
        }
    }

    void Update()
    {
        DoTransform();
        UpdateWheelTransforms();
    }

    void InstantiateWheel(int index)
    {
        GameObject wheel = new GameObject("Wheel" + (index + 1));
        wheel.transform.parent = transform; // Make it a child of the car

        wheel.AddComponent<MeshFilter>().mesh = wheelMesh; // Use the specified wheel mesh
        wheel.AddComponent<MeshRenderer>(); // You might need to set materials or other renderer properties

        // Set the local position of the wheel based on the index and car's displacement
        float offsetX = (index % 2 == 0) ? 1f : -1f;
        float offsetZ = (index < 2) ? 1.235f : -1.235f;
        wheel.transform.localPosition = new Vector3(offsetX, 0.395f, offsetZ) + displacement;

        // Set the local scale of the wheel based on the wheelScale variable
        wheel.transform.localScale = Vector3.one * wheelScale;

        // Ensure the wheel's local rotation is set to identity
        wheel.transform.localRotation = Quaternion.identity;

        // Add the wheel to the list
        wheels.Add(wheel);
    }

    void UpdateWheelTransforms()
    {
        foreach (var wheel in wheels)
        {
            // Update the local position of each wheel based on the initial position and car's displacement
            wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, 0.395f, wheel.transform.localPosition.z) + displacement;

            // Rotate the wheel based on the car's rotation
            wheel.transform.localRotation = Quaternion.Euler(0, angle * Time.time, 0);
        }
    }

    void DoTransform()
    {
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x * Time.deltaTime, displacement.y * Time.deltaTime, displacement.z * Time.deltaTime);

        Matrix4x4 moveOrigin = HW_Transforms.TranslationMat(-displacement.x, -displacement.y, -displacement.z);

        Matrix4x4 moveObject = HW_Transforms.TranslationMat(displacement.x, displacement.y, displacement.z);

        Matrix4x4 rotate = HW_Transforms.RotateMat(angle * Time.time, rotationAxis);

        Matrix4x4 composite = moveObject * rotate * moveOrigin;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector4 tmp = new Vector4(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);

            newVertices[i] = composite * tmp;
        }

        // Assign the new vertices to the mesh
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
    }
}
