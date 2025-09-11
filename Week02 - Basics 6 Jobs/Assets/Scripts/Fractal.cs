using System;
using Mono.Cecil.Cil;
using UnityEngine;

public class Fractal : MonoBehaviour
{

    [SerializeField, Range(1, 8)]
    int depth = 4;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;
    FractalPart[][] parts;
    Matrix4x4[][] matrices;

    static Vector3[] directions = {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
    };

    struct FractalPart
    {
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
        public float spinAngle;
    }

    ComputeBuffer[] matricesBuffers;

    void OnEnable()
    {
        parts = new FractalPart[depth][];
        matrices = new Matrix4x4[depth][];
        matricesBuffers = new ComputeBuffer[depth];
		int stride = 16 * 4;

        // root
        parts[0] = new FractalPart[1];

        for (int i = 0, arrLength = 1; i < parts.Length; i++, arrLength *= 5)
        {
            parts[i] = new FractalPart[arrLength];
            matrices[i] = new Matrix4x4[arrLength];
            matricesBuffers[i] = new ComputeBuffer(arrLength, stride); // 16 x 4 bytes
		}

        parts[0][0] = CreatePart(0);

        for (int level = 1; level < parts.Length; level++)
        {
            FractalPart[] levelParts = parts[level];
            for (int part = 0; part < levelParts.Length; part += 5)
            {
                for (int child = 0; child < 5; child++)
                {
                    levelParts[part + child] = CreatePart(child);
                }
            }
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            matricesBuffers[i].Release();
        }
        parts = null;
		matrices = null;
		matricesBuffers = null;
    }

    void OnValidate () {
		if (parts != null && enabled) {
			OnDisable();
			OnEnable();
		}
	}

    void Update()
    {
        float spinAngleDelta = 22.5f * Time.deltaTime;

        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle *= spinAngleDelta;
        rootPart.worldRotation = rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f);
        parts[0][0] = rootPart;
        matrices[0][0] = Matrix4x4.TRS(
            rootPart.worldPosition, rootPart.worldRotation, Vector3.one
        );

        float scale = 1f;
        for (int level = 1; level < parts.Length; level++)
        {
            scale *= 0.5f;
            FractalPart[] parentParts = parts[level - 1];
            FractalPart[] levelParts = parts[level];
            Matrix4x4[] levelMatrices = matrices[level];
            for (int part = 0; part < levelParts.Length; part++)
            {
                FractalPart parent = parentParts[part / 5];
                FractalPart p = levelParts[part];
                p.spinAngle += spinAngleDelta;
                p.worldRotation = parent.worldRotation * (p.rotation * Quaternion.Euler(0f, p.spinAngle, 0f));
                p.worldPosition = parent.worldPosition
                                            + parent.worldRotation
                                            * (1.5f * scale * p.direction);
                levelParts[part] = p;
                levelMatrices[part] = Matrix4x4.TRS(
                    p.worldPosition, p.worldRotation, scale * Vector3.one
                );
            }
        }
        
        for (int i = 0; i < matricesBuffers.Length; i++) {
			matricesBuffers[i].SetData(matrices[i]);
		}
    }

    FractalPart CreatePart(int childIndex) => new()
    {
        direction = directions[childIndex],
        rotation = rotations[childIndex],
    };
}