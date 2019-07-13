using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Whistle.Actors;

public class NavAgent {

    //This whole class here is meant for managing navigation operations as efficiently as possible, using multithreading.
    //If you're developing AI for characters with pathfinding, you should probably use this.

    public NavMesh mesh;
    public List<Vector2> navpath;
    public int operations;
    public Skill skill;

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
            if (navpath.Count > 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public struct Skill {
        public bool canFly;
        public bool canClimb;
        public bool canSwim;
    }

    public NavAgent(NavMesh mesh, Skill skill) {
        //The constructor. 
        //If you want to start running operations for AI with a character, give them an agent using this: AIAgent namegoeshere = new AIAgent(NavMesh.SceneNavMesh);

        this.mesh = mesh;
        this.skill = skill;
        navpath = new List<Vector2>();
        operations = 0;
    }

    public void GenerateNewPath(Vector2 start, Vector2 end) {
        //This method starts generating a new path under the agent. It can then be accessed for whatever you need to be doing.

        operations++;

        Thread navParse = new Thread(() => Build(start, end));
        navParse.Start();
    }

    public void ResetPath() {
        navpath = new List<Vector2>();
    }

    public int ParsePathForDirection(Vector2 positionFrom, out Vector2 output) {

        //This method returns a direction to go in based on the path that's been generated, and the Vector2 given. Helpful if you just need to know where the character should go next according to where they are on the path.

        float shortestDistance = Mathf.Infinity;
        int closestNode = 0;
        for (int i = 0; i < navpath.Count; i++) {
            float distance = Vector2.Distance(positionFrom, navpath[i]);
            if (distance < shortestDistance) {
                closestNode = i;
                shortestDistance = distance;
            }
        }

        if (closestNode < navpath.Count) {
            output = navpath[closestNode + 1] - navpath[closestNode];
        }
        else {
            output = Vector2.zero;
        }
        return closestNode;
    }

    public int ParsePathForDirection(int index, out Vector2 output) {

        //Does the above, but returns the direction to go in based on the input index instead.

        if (index < navpath.Count - 1) {
            output = navpath[index + 1] - navpath[index];
        }
        else {
            output = Vector2.zero;
        }
        return index;
    }

    private void Build(Vector2 start, Vector2 end) {
        //This method is intended to be executed outside the main thread, in its own. When executed, a path is built from GetPath and registered into the NavAgent.

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        Vector2[] path = mesh.GetPath(start, end, skill);
        if (path != null) {
            for (int i = path.Length; i > 0; i--) {
                navpath.Add(path[i - 1]);
            }
        }

        sw.Stop();

        operations--;
    }
}
