using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Whistle.Actors;

public class NavAgent {

    //This whole class here is meant for managing navigation operations as efficiently as possible, using multithreading.
    //If you're developing AI for characters with pathfinding, you should probably use this.

    public NavMesh mesh;
    public List<Vector2> navPath;
    public int operations;
    public bool canFly;
    public bool canClimb;
    public bool canSwim;

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
            if (navPath.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public NavAgent(NavMesh mesh, bool canFly, bool canClimb, bool canSwim) {
        //The constructor. 
        //If you want to start running operations for AI with a character, give them an agent using this: AIAgent namegoeshere = new AIAgent(NavMesh.SceneNav);

        this.mesh = mesh;
        this.canFly = canFly;
        this.canClimb = canClimb;
        this.canSwim = canSwim;
        navPath = new List<Vector2>();
        operations = 0;
    }

    public void GenerateNewPath(Vector2 start, Vector2 end) {
        //This method starts generating a new path under the agent. It can then be accessed for whatever you need to be doing.

        operations++;

        Thread navParse = new Thread(() => Build(start, end));
        navParse.Start();
    }

    public void ResetPath() {
        navPath = new List<Vector2>();
    }

    public int ParsePathForDirection(Vector2 positionFrom, out Vector2 output) {

        //This method returns a direction to go in based on the path that's been generated, and the Vector2 given. Helpful if you just need to know where the character should go next according to where they are on the path.

        float shortestDistance = Mathf.Infinity;
        int closestNode = 0;
        for (int i = 0; i < navPath.Count; i++) {
            float distance = Vector2.Distance(positionFrom, navPath[i]);
            if (distance < shortestDistance) {
                closestNode = i;
                shortestDistance = distance;
            }
        }

        if (closestNode < navPath.Count - 1) {
            output = navPath[closestNode + 1] - navPath[closestNode];
        }
        else {
            output = Vector2.zero;
        }
        return closestNode;
    }

    public int ParsePathForDirection(int index, out Vector2 output) {

        //Does the above, but returns the direction to go in based on the input index instead.

        if (index < navPath.Count - 1) {
            output = navPath[index + 1] - navPath[index];
        }
        else {
            output = Vector2.zero;
        }
        return index;
    }

    private void Build(Vector2 start, Vector2 end) {
        //This method is intended to be executed outside the main thread and in its own. When executed, a path is built from GetPath and registered into the NavAgent.

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        NavMesh.AgentSkill skills;
        skills.canClimb = canClimb;
        skills.canFly = canFly;
        skills.canSwim = canSwim;

        Vector2[] path = mesh.GetPath(start, end, skills);
        if (path != null) {
            for (int i = path.Length; i > 0; i--) {
                navPath.Add(path[i - 1]);
            }
        }

        sw.Stop();

        operations--;
    }
}
