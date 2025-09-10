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
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    void Awake()
    {
        parts = new FractalPart[depth][];

        // root
        parts[0] = new FractalPart[1];

		for (int i = 0, arrLength = 1; i < parts.Length; i++, arrLength *= 5) {
			parts[i] = new FractalPart[arrLength];
		}

        float scale = 1f;
        parts[0][0] = CreatePart(0, 0, scale);
        for (int level = 1; level < parts.Length; level++) {
            scale *= 0.5f;
			FractalPart[] levelParts = parts[level];
			for (int part = 0; part < levelParts.Length; part += 5) {
				for (int child = 0; child < 5; child++) {
					levelParts[part+child] = CreatePart(level, child, scale);
				}
			}
		}
    }

    void Update()
    {
        for (int level = 1; level < parts.Length; level++)
        {
            FractalPart[] parentParts = parts[level - 1];
            FractalPart[] levelParts = parts[level];
            for (int part = 0; part < levelParts.Length; part++)
            {
                Transform parentTransform = parentParts[part / 5].transform;
                FractalPart p = levelParts[part];
                p.transform.localRotation = parentTransform.localRotation * p.rotation;
                p.transform.localPosition =	parentTransform.localPosition
                                            + parentTransform.localRotation
                                            * (1.5f * p.transform.localScale.x * p.direction);
            }
        }
    }

    FractalPart CreatePart(int levelIndex, int childIndex, float scale)
    {
        var go = new GameObject("Fractal Part " + levelIndex + " C" + childIndex);
        go.transform.localScale = Vector3.one * scale;
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = material;
        return new FractalPart {
			direction = directions[childIndex],
			rotation = rotations[childIndex],
			transform = go.transform
		};
    }
}