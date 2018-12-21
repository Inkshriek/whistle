using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMesh : MonoBehaviour {

    [SerializeField] public NavRect[] mesh;
    //[SerializeField] public Vector2 nodeSpacing;
    //[SerializeField] public Bounds nodeBounds;
    //private Dictionary<Vector2, NavRectFlag> nodeGraph = new Dictionary<Vector2, NavRectFlag>();
    public bool forceStop = false;

    void OnValidate() {
        //Checks to make sure the mesh is initialized correctly and has at least one element upon the script's creation.
        if (mesh == null) {
            mesh = new NavRect[1];
            mesh[0] = new NavRect("Rect 0", true, NavRectFlag.Normal, new Vector2(-1, 1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1));
        }
    }

    void Awake() {

    }

    void Update() {

    }

    /*
    public void GenerateNodeGraph() {
        //Generates a node graph based on the navigation mesh, nodeSpacing, and nodeBounds. This is what gets used for pathfinding.

        try {
            nodeGraph = new Dictionary<Vector2, NavRectFlag>();
            float currentPosX = nodeBounds.min.x;
            float currentPosY = nodeBounds.min.y;

            while (currentPosY <= nodeBounds.max.y) {

                while (currentPosX <= nodeBounds.max.x) {
                    nodeGraph.Add(new Vector2(currentPosX, currentPosY), PointIntersecting(new Vector2(currentPosX, currentPosY)));

                    NavRectFlag flag;

                    nodeGraph.TryGetValue(new Vector2(currentPosX, currentPosY), out flag);
                    Debug.Log("Node Position: " + new Vector2(currentPosX, currentPosY) + " | Node Flag: " + flag);
                    currentPosX += nodeSpacing.x;
                }
                currentPosY += nodeSpacing.y;
                currentPosX = nodeBounds.min.x;
            }
        }
        catch {

        }

        Debug.Log(nodeGraph.Count);
        Debug.Log("Node graph successfully generated!");
    }
    */

    public Vector2 FindDirectionToGo(Vector2 start, Vector2 end, Accuracy accuracy, float nodeDensity) { 
        //This finds a normalized direction to move in, in order to take a step to reach the end destination.

        if (forceStop) {
            return Vector2.zero;
        }
        try {
            //Realigning the input vectors to the node graph.
            start = new Vector2(Mathf.Round(start.x / nodeDensity) * nodeDensity, Mathf.Round(start.y / nodeDensity) * nodeDensity);
            end = new Vector2(Mathf.Round(end.x / nodeDensity) * nodeDensity, Mathf.Round(end.y / nodeDensity) * nodeDensity);

            Debug.Log(start + " till " + end);

            //Deciding how to calculate this, based on accuracy.
            switch (accuracy) {
                case Accuracy.Low:

                    List<Node> openLocs = new List<Node>();
                    List<Node> closedLocs = new List<Node>();
                    Node[] adjacentNodes;
                    bool evaluating = true;

                    Node current = new Node(start);
                    openLocs.Add(current);
                    int i = 0;

                    while (evaluating) {
                        i++;
                        if (i > 1000) {
                            Debug.LogError("The NavMesh pathfinding algorithm has been force stopped due to too many iterations. Please restart the game.");
                            forceStop = true;
                            return Vector2.zero;
                            
                        }

                        float lowestFcost = Mathf.Infinity;

                        foreach (Node node in openLocs) {
                            float Gcost = Vector2.Distance(node.loc, start);
                            float Hcost = Vector2.Distance(node.loc, end);
                            float Fcost = Gcost + Hcost;

                            if (Fcost < lowestFcost) {
                                current = node;
                                lowestFcost = Mathf.Min(lowestFcost, Fcost);
                            }
                        }

                        openLocs.Remove(current);
                        closedLocs.Add(current);

                        if (current.loc == end) {
                            evaluating = false;
                            break;
                        }

                        adjacentNodes = new Node[] {
                            GetNodeFromMesh(new Vector2(current.loc.x - nodeDensity, current.loc.y)),
                            GetNodeFromMesh(new Vector2(current.loc.x - nodeDensity, current.loc.y + nodeDensity)),
                            GetNodeFromMesh(new Vector2(current.loc.x, current.loc.y + nodeDensity)),
                            GetNodeFromMesh(new Vector2(current.loc.x + nodeDensity, current.loc.y + nodeDensity)),
                            GetNodeFromMesh(new Vector2(current.loc.x + nodeDensity, current.loc.y)),
                            GetNodeFromMesh(new Vector2(current.loc.x + nodeDensity, current.loc.y - nodeDensity)),
                            GetNodeFromMesh(new Vector2(current.loc.x, current.loc.y - nodeDensity)),
                            GetNodeFromMesh(new Vector2(current.loc.x - nodeDensity, current.loc.y - nodeDensity)),
                        };

                        foreach (Node node in adjacentNodes) {
                            if (!IsTraversible(node.flag) || IsInGroup(node.loc, closedLocs)) {
                                continue;
                            }

                            float Gcost = Vector2.Distance(node.loc, start);
                            float Hcost = Vector2.Distance(node.loc, end);
                            float Fcost = Gcost + Hcost;

                            if (Fcost < lowestFcost || !IsInGroup(node.loc, openLocs)) {
                                node.parent = current;
                                if (!IsInGroup(node.loc, openLocs)) {
                                    openLocs.Add(node);
                                }
                            }
                        }
                    }

                    //Returning direction found.
                    Node processingNode = closedLocs[closedLocs.Count - 1];
                    List<Node> path = new List<Node>();

                    bool gettingPath = true;

                    while (gettingPath) {
                        if (processingNode.parent != null) {
                            path.Add(processingNode);
                        }
                        else {
                            gettingPath = false;
                        }
                    }

                    Vector2 difference = path[path.Count - 1].loc - start;
                    difference.Normalize();
                    return difference;

                case Accuracy.Medium:

                    break;
                case Accuracy.High:

                    break;
            }

            return Vector2.zero;
        }
        catch {
            Debug.LogError("The node graph couldn't be parsed! Did you forget to generate it?");

            return Vector2.zero;
        }
    }

    /*
    public Vector2[] FindFullPath(Vector2 start, Vector2 destination) {
        try {

        }
        catch {
            Debug.LogError("The node graph couldn't be parsed! Did you forget to generate it?");
        }
    }
    */
    

    [System.Serializable]
    public struct NavRect {
        //The structure for all rectangles making up the navigation mesh. Be aware they are intended to be convex only, and will break if the points make a degenerate rectangle.

        public string name;
        public bool display;
        public NavRectFlag flag;

        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;

        public NavRect(string name, bool display, NavRectFlag flag, Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            this.name = name;
            this.display = display;
            this.flag = flag;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public Vector2[] Points {
            //Just returns all four points of the NavRect at once.
            get {
                Vector2[] ret = { a, b, c, d };
                return ret;
            }
        }

        public Vector2 Position {
            //An easy way of getting the "midpoint", or position, of the NavRect for the editor.
            get {
                Vector2 max = new Vector2(Mathf.Max(a.x, b.x, c.x, d.x), Mathf.Max(a.y, b.y, c.y, d.y));
                Vector2 min = new Vector2(Mathf.Min(a.x, b.x, c.x, d.x), Mathf.Min(a.y, b.y, c.y, d.y));

                return Vector2.Lerp(max, min, 0.5f);
            }
            set {
                Vector2 lastpos = Position;
                Vector2 difference = value - lastpos;

                a += difference;
                b += difference;
                c += difference;
                d += difference;
            }
        }
    }

    private class Node {
        public Vector2 loc;
        public NavRectFlag flag;
        public Node parent;

        public Node(Vector2 loc) {
            this.loc = loc;
        }
    }

    private Node GetNodeFromMesh(Vector2 loc) {
        /*
        NavRectFlag flag;

        nodeGraph.TryGetValue(new Vector2(loc.x, loc.y), out flag);
        return new Node(loc, flag);
        */

        Node node = new Node(loc);
        node.flag = PointIntersecting(loc);
        return node;
        

    }

    private bool IsTraversible(NavRectFlag flag) {
        switch (flag) {
            case NavRectFlag.Normal:
                return true;
            default:
                return false;
        }
    }

    private bool IsInGroup(Vector2 loc, List<Node> set) {
        foreach (Node node in set) {
            if (node.loc == loc) {
                return true;
            }
        }

        return false;
    }

    public enum NavRectFlag {
        //The various flags used for labeling NavRects. Helps with AI, and is displayed real nice too in the editor.
        Normal,
        Transient,
        Dangerous,
        Nothing
    }

    public enum Accuracy {
        //A few levels in how accurate to allow the pathfinding algorithm to be.
        Low,
        Medium,
        High
    }

    public NavRectFlag PointIntersecting(Vector2 point) {
        //This is used to determine if a point lies within the navigation mesh.

        foreach (NavRect rect in mesh) {
            bool eval = true;
            bool[] checks = {
                CheckLeftOfLine(point, rect.b, rect.a),
                CheckLeftOfLine(point, rect.a, rect.c),
                CheckLeftOfLine(point, rect.c, rect.d),
                CheckLeftOfLine(point, rect.d, rect.b)
            };
            for (int i = 0; i < checks.Length; i++) {

                if (!checks[i]) {
                    eval = false;
                }
            }

            if (eval) {
                return rect.flag;
            }
        }

        return NavRectFlag.Nothing;
    }

    public bool PointIntersecting(Vector2 point, NavRectFlag flag) {
        //Same as above, though also uses a flag as a filter so we only return "true" for specific NavRects.

        foreach (NavRect rect in mesh) {
            if (rect.flag != flag) {
                continue;
            }

            bool eval = true;
            bool[] checks = {
                CheckLeftOfLine(point, rect.b, rect.a),
                CheckLeftOfLine(point, rect.a, rect.c),
                CheckLeftOfLine(point, rect.c, rect.d),
                CheckLeftOfLine(point, rect.d, rect.b)
            };
            for (int i = 0; i < checks.Length; i++) {

                if (!checks[i]) {
                    eval = false;
                }
            }

            if (eval) {
                return true;
            }
        }

        return false;
    }

    public NavRectFlag ColliderIntersecting(BoxCollider2D collider) {
        //This is used to determine if a collider happens to intersect any NavRect in the navigation mesh.

        Vector2[] points = {
            new Vector2 (collider.bounds.min.x, (collider.bounds.min.y)),
            new Vector2 (collider.bounds.min.x, (collider.bounds.max.y)),
            new Vector2 (collider.bounds.max.x, (collider.bounds.max.y)),
            new Vector2 (collider.bounds.max.x, (collider.bounds.min.y))
        };

        foreach (NavRect rect in mesh) {
            int overlaps = 0;

            overlaps += CheckAreaOverlaps(points, rect.Points);
            overlaps += CheckAreaOverlaps(rect.Points, points);
            
            if (overlaps == points.Length + rect.Points.Length) {
                return rect.flag;
            }
        }

        return NavRectFlag.Nothing;
    }

    public bool ColliderIntersecting(BoxCollider2D collider, NavRectFlag flag) {
        //Same as above, only again using a flag as a filter.

        Vector2[] points = {
            new Vector2 (collider.bounds.min.x, (collider.bounds.min.y)),
            new Vector2 (collider.bounds.min.x, (collider.bounds.max.y)),
            new Vector2 (collider.bounds.max.x, (collider.bounds.max.y)),
            new Vector2 (collider.bounds.max.x, (collider.bounds.min.y))
        };

        foreach (NavRect rect in mesh) {

            if (rect.flag != flag) {
                continue;
            }
            
            int overlaps = 0;

            overlaps += CheckAreaOverlaps(points, rect.Points);
            overlaps += CheckAreaOverlaps(rect.Points, points);

            if (overlaps == points.Length + rect.Points.Length) {
                return true;
            }
        }

        return false;
    }

    private bool CheckLeftOfLine(Vector2 point, Vector2 a, Vector2 b) {
        //Shorthand way of checking if something is to the left side of a line. Just for mathing.

        return ((b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x)) > 0;
    }

    private int CheckAreaOverlaps(Vector2[] a, Vector2[] b) {
        //Shorthand (kinda) way of checking if a set of points [a] fulfills certain requirements for "overlapping" another set of points [b]. Needs to be run twice, swapping the parameters, to be accurate.

        int overlaps = 0;

        for (int i = 0; i < a.Length; i++) {

            int next = i + 1;
            if (next == a.Length) {
                next = 0;
            }

            Vector2 line = a[next] - a[i];

            line.Normalize();
            Vector2 lineP = Vector2.Perpendicular(line);

            float projectionMaxA = Mathf.NegativeInfinity;
            float projectionMinA = Mathf.Infinity;
            for (int e = 0; e < a.Length; e++) {
                projectionMaxA = Mathf.Max(Vector2.Dot(lineP, a[e]), projectionMaxA);
                projectionMinA = Mathf.Min(Vector2.Dot(lineP, a[e]), projectionMinA);
            }

            float projectionMaxB = Mathf.NegativeInfinity;
            float projectionMinB = Mathf.Infinity;
            for (int e = 0; e < b.Length; e++) {
                projectionMaxB = Mathf.Max(Vector2.Dot(lineP, b[e]), projectionMaxB);
                projectionMinB = Mathf.Min(Vector2.Dot(lineP, b[e]), projectionMinB);
            }

            if (!(projectionMinA > projectionMaxB || projectionMaxA < projectionMinB)) {
                overlaps++;
            }
        }

        return overlaps;
    }

    public static NavMesh SceneNavMesh {
        //Very definitely shorthand way of getting the navigation mesh for the current scene.

        get {
            return (NavMesh)FindObjectOfType(typeof(NavMesh));
        }
    }
}

