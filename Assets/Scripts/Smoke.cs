using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; // Avoids ambiguous reference to System.Random

/// <summary>
/// Volumetric smoke flood fill algorithm
/// @Author Patrik Bergsten
/// </summary>
public class Smoke : MonoBehaviour {
    private Vector3Int _grenadePosition;
    [SerializeField, Tooltip("If prefab has collider, make sure prefabs layer is not included in layerMask!")]
    private GameObject smokeParticle;
    [SerializeField]
    private int numSmokeParticlesToSpawn = 1500;
    [SerializeField, Tooltip("What layer smoke collides with")]
    private LayerMask layerMask;
    [SerializeField, Tooltip("Radius for obstacle avoidance")] 
    private float particleRadius = .45f;

    [ContextMenu("Explode")] // click on 'hamburger menu' and choose 'Explode' to run method from editor
    public void Explode() {
        _grenadePosition = transform.position.ToVector3Int();
        StartCoroutine(FloodFill());
    }

    private IEnumerator FloodFill() {
        if (smokeParticle == null) {
            Debug.LogError("Assign Particle prefab as <B>Smoke Particle</B> <I>before</I> running method");
            yield break;
        }
        Vector3Int grenadePosition = _grenadePosition;
        Transform parentTransform = new GameObject("Smoke").transform;
        parentTransform.SetPositionAndRotation(grenadePosition, Quaternion.identity);
        int numSpawnedParticles = 0;
        
        HashSet<Vector3Int> duplicateCheck = new HashSet<Vector3Int>();
        Heap<Node> neighbors = new Heap<Node>(200);
        int updateSteps = 50;

        Node current = new Node(grenadePosition, grenadePosition);
        neighbors.Insert(current);

        while (!neighbors.Empty() && numSpawnedParticles < numSmokeParticlesToSpawn) {

            current = neighbors.DeleteMin();

            if (!duplicateCheck.Contains(current.position) && Unblocked(current.realPosition)) {
                duplicateCheck.Add(current.position);
                
                numSpawnedParticles++;
                if (numSpawnedParticles % updateSteps == 0)
                    yield return new WaitForSeconds(1f/60f);

                InstantiateSmokeParticle(current.realPosition, parentTransform);
                
                Vector3Int[] currentNodesNeighbourDirections = current.NeighbourDirections;
                for (int i = 0; i < currentNodesNeighbourDirections.Length; i++) {
                    Node neighbour = new Node(currentNodesNeighbourDirections[i], _grenadePosition);
                    neighbors.Insert(neighbour);
                }
            }
        }

        bool Unblocked(Vector3 pos) {
            return !Physics.CheckSphere(pos, particleRadius, layerMask);
        }
    }

    private void InstantiateSmokeParticle(Vector3 pos, Transform parentTransform) {
        Instantiate(smokeParticle, pos, Quaternion.identity, parentTransform);
    }
}

public static class Extensions {
    public static Vector3Int ToVector3Int(this Vector3 v3) {
        int x = Mathf.RoundToInt(v3.x);
        int y = Mathf.RoundToInt(v3.y);
        int z = Mathf.RoundToInt(v3.z);
        return new Vector3Int(x, y, z);
    }
}

public class Node : IComparable<Node> {
    private static readonly float SquashHeightValue = 3f;
    private static readonly Vector3Int[] Directions = new Vector3Int[] {
        // top
        new Vector3Int(-1, 1, -1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 1, 1),
        new Vector3Int( 0, 1, -1),
        new Vector3Int( 0, 1, 0),
        new Vector3Int( 0, 1, 1),
        new Vector3Int( 1, 1, -1),
        new Vector3Int( 1, 1, 0),
        new Vector3Int( 1, 1, 1),
        // middle
        new Vector3Int(-1, 0, -1),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, 0, 1),
        new Vector3Int( 0, 0, -1),
        new Vector3Int( 0, 0, 1),
        new Vector3Int( 1, 0, -1),
        new Vector3Int( 1, 0, 0),
        new Vector3Int( 1, 0, 1),
        // bottom
        new Vector3Int(-1, -1, -1),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, -1, 1),
        new Vector3Int( 0, -1, -1),
        new Vector3Int( 0, -1, 0),
        new Vector3Int( 0, -1, 1),
        new Vector3Int( 1, -1, -1),
        new Vector3Int( 1, -1, 0),
        new Vector3Int( 1, -1, 1),
    };
    public readonly Vector3Int position;
    public readonly Vector3 realPosition;
    private readonly Vector3 _grenadePosition;
    public Vector3Int[] NeighbourDirections => GetNeighbours();
    
    public Node(Vector3Int position, Vector3 grenadePosition) {
        this.position = position;
        _grenadePosition = grenadePosition;
        realPosition = grenadePosition + (grenadePosition - this.position) * .5f 
                                       + Random.insideUnitSphere * .5f;
    }

    private Vector3Int[] GetNeighbours() {
        Vector3Int[] neighbors = new Vector3Int[Directions.Length];

        for (int i = 0; i < Directions.Length; i++) {
            neighbors[i] = position + Directions[i];
        }

        return neighbors;
    }
    
    // Compare function is for Heap to decide which position is better
    // closer to origin is better, squash height with SquashHeightValue
    public int CompareTo(Node other) {
        if (position == other.position) return 0;

        Vector3 thisPos = position;
        thisPos.y *= SquashHeightValue;

        Vector3 otherPos = other.position;
        otherPos.y *= SquashHeightValue;
        
        float thisDist = Vector3.SqrMagnitude(thisPos - _grenadePosition);
        float otherDist = Vector3.SqrMagnitude(otherPos - _grenadePosition);

        int distComparison = thisDist.CompareTo(otherDist);
        return distComparison;
    }

    public override bool Equals(object obj) {
        if (obj != null && obj.GetType() == typeof(Node))
            return Equals((Node)obj);
        
        return false;
    }

    public override int GetHashCode() {
        return position.GetHashCode();
    }

    private bool Equals(Node other) {
        return position == other.position;
    }
}
