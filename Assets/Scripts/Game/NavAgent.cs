using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Whistle.Actors;

public class NavAgent {

    //This whole class here is meant for managing navigation operations as efficiently as possible, using multithreading.
    //If you're developing AI for characters with pathfinding, you should probably use this.

    private NavMesh mesh;
    private List<Vector2> navPath;
    private int operations;
    private Dictionary<NavMesh.NavFlag, int> weights;
    private NavMesh.Attributes atts;

    public bool Operating {
        get {
            if (operations > 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public bool PathReady {
        get {
            if (NavPath.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public NavMesh Mesh { get => mesh; set => mesh = value; }
    public Dictionary<NavMesh.NavFlag, int> Weights { get => weights; set => weights = value; }
    public List<Vector2> NavPath { get => navPath; private set => navPath = value; }
    public NavMesh.Attributes Attributes { get => atts; set => atts = value; }

    public NavAgent(NavMesh mesh, bool canClimb, bool canFly, bool canSwim) {
        //The constructor. 
        //If you want to start running operations for AI with a character, give them an agent using this: AIAgent namegoeshere = new AIAgent(NavMesh.SceneNav);

        this.Mesh = mesh;
        this.NavPath = new List<Vector2>();
        this.Attributes = new NavMesh.Attributes(canClimb, canFly, canSwim);
        this.operations = 0;
    }

    private void Build(Vector2 start, Vector2 end) {
        //This method is intended to be executed outside the main thread and in its own. When executed, a path is built from the mesh and registered into the NavAgent.

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        Vector2[] path = Mesh.GetPath(start, end, atts);
        if (path != null) {
            for (int i = path.Length; i > 0; i--) {
                NavPath.Add(path[i - 1]);
            }
        }

        sw.Stop();

        operations--;
    }

    ///<summary>This method starts generating a new path under the agent in a separate thread. It can then be accessed for whatever you need to be doing once <c>PathReady</c> is true.
    ///It won't interrupt a currently running execution unless you define "interrupt" as true.</summary>
    public void GenerateNewPath(Vector2 start, Vector2 end, bool interrupt) {

        if (!Operating || interrupt) {
            ResetPath();
            operations++;
            Thread navParse = new Thread(() => Build(start, end));
            navParse.Start();
        }
    }

    public void ResetPath() {
        NavPath = new List<Vector2>();
    }

    ///<summary>Returns a direction to go in next based on the closest node the given Vector2 is on the NavPath.
    ///Helpful if you just need to know where the character should go relative to where they are on the path so far.</summary>
    public int GetNextDirection(Vector2 positionFrom, out Vector2 output) {

        float shortestDistance = Mathf.Infinity;
        int closestNode = 0;
        for (int i = 0; i < NavPath.Count; i++) {
            float distance = Vector2.Distance(positionFrom, NavPath[i]);
            if (distance < shortestDistance) {
                closestNode = i;
                shortestDistance = distance;
            }
        }

        if (closestNode < NavPath.Count - 1) {
            output = NavPath[closestNode + 1] - NavPath[closestNode];
        }
        else {
            output = Vector2.zero;
        }
        return closestNode;
    }

    ///<summary>Returns a direction to go in next on the NavPath based on the input index.</summary>
    public int GetNextDirection(int index, out Vector2 output) {

        //Does the above, but returns the direction to go in based on the input index instead.

        if (index < NavPath.Count - 1) {
            output = NavPath[index + 1] - NavPath[index];
        }
        else {
            output = Vector2.zero;
        }
        return index;
    }
}
