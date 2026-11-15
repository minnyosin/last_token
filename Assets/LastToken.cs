using System.Collections.Generic;
using UnityEngine;

public sealed class LastToken : MonoBehaviour
{
    [SerializeField] GameObject[] levels;
    [SerializeField] Camera mazeCamera;

    readonly HashSet<Vector2Int> floorTiles = new();
    readonly HashSet<Vector2Int> walls = new();
    readonly HashSet<Vector2Int> shutters = new();
    readonly Dictionary<Vector2Int, Vector2Int> boosts = new();
    readonly HashSet<Vector2Int> slime = new();
    Vector2Int boostDirection;
    int boostSteps;
    float slowUntil;
    float cameraHalfWidth;

    int RobotCount => robotMeshes[1] == null ? 1 : 2;
    int stage;
    GameObject gateLight;
    Vector2Int trigger, gate, switchTile;
    Vector2Int firstDoor, secondDoor;
    GameObject firstDoorMesh, secondDoorMesh;
    bool doorsSwapped;
    bool cutoffArmed;
    readonly Dictionary<Vector2Int, GameObject> fuses = new();
    int collectedFuses;
    float cutoffEnds;
    bool ExitOpen => score >= 5 && collectedFuses == fuses.Count;
    float cutoffStarts;

    static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    readonly Dictionary<Vector2Int, GameObject> tokens = new();
    readonly Vector2Int[] robots = new Vector2Int[2];
    readonly Vector2Int[] homes = new Vector2Int[2];
    readonly GameObject[] robotMeshes = new GameObject[2];
    Vector2Int player, start, exit, anchor;
    GameObject playerMesh, exitMesh, marker;
    bool charged, won, lost;
    int score;
    float elapsed, nextMove, nextChase;

    void Start()
    {
        // Use Unity's normal camera defaults, then copy the scene camera's view.
        var view = mazeCamera.transform;
        mazeCamera.enabled = false;
        mazeCamera = new GameObject("Play Camera").AddComponent<Camera>();
        mazeCamera.transform.SetPositionAndRotation(view.position, view.rotation);
        mazeCamera.orthographic = true;
        mazeCamera.orthographicSize = 6.8f;
        mazeCamera.clearFlags = CameraClearFlags.SolidColor;
        mazeCamera.backgroundColor = new Color(.07f, .03f, .12f);
        LoadStage(0);
    }

    void LoadStage(int index)
    {
        // Put the actors back before switching away from this level.
        if (playerMesh != null)
        {
            playerMesh.transform.position = World(start, .4f);
            for (int i = 0; i < RobotCount; i++)
                robotMeshes[i].transform.position = World(homes[i], .38f);
        }
        stage = index;
        for (int i = 0; i < levels.Length; i++)
            levels[i].SetActive(i == stage);

        floorTiles.Clear();
        walls.Clear();
        shutters.Clear();
        boosts.Clear();
        slime.Clear();
        int rightEdge = 0;
        tokens.Clear();
        fuses.Clear();
        robotMeshes[1] = null;
        gateLight = firstDoorMesh = secondDoorMesh = null;

        // These objects are placed in the scene. Move or duplicate them there.
        foreach (var piece in levels[stage].GetComponentsInChildren<Transform>(true))
        {
            var tile = GridPosition(piece.position);
            string kind = piece.name.Split('(')[0].TrimEnd();
            switch (kind)
            {
                case "Floor": floorTiles.Add(tile); rightEdge = Mathf.Max(rightEdge, tile.x); break;
                case "Wall": walls.Add(tile); break;
                case "Coin": tokens[tile] = piece.gameObject; break;
                case "Fuse": fuses[tile] = piece.gameObject; break;
                case "Shutter": shutters.Add(tile); break;
                case "Boost": boosts[tile] = GridPosition(piece.right); break;
                case "Slime": slime.Add(tile); break;
                case "Player": playerMesh = piece.gameObject; start = tile; break;
                case "Chaser": robotMeshes[0] = piece.gameObject; homes[0] = tile; break;
                case "Interceptor": robotMeshes[1] = piece.gameObject; homes[1] = tile; break;
                case "Exit": exitMesh = piece.gameObject; exit = tile; break;
                case "Rewind Marker": marker = piece.gameObject; break;
                case "Gate A": firstDoorMesh = piece.gameObject; firstDoor = tile; break;
                case "Gate B": secondDoorMesh = piece.gameObject; secondDoor = tile; break;
                case "Gate Switch": switchTile = tile; break;
                case "Interceptor Trigger": trigger = tile; break;
                case "Interceptor Target": gate = tile; gateLight = piece.gameObject; break;
            }
        }
        cameraHalfWidth = rightEdge / 2f + 1.2f;
        var cameraPosition = mazeCamera.transform.position;
        cameraPosition.x = rightEdge / 2f;
        mazeCamera.transform.position = cameraPosition;
        ResetRound();
    }

    static Vector2Int GridPosition(Vector3 world) =>
        new(Mathf.RoundToInt(world.x), Mathf.RoundToInt(-world.z));

    static Vector3 World(Vector2Int tile, float height) => new(tile.x, height, -tile.y);

    void ResetRound()
    {
        player = start;
        homes.CopyTo(robots, 0);
        foreach (var token in tokens.Values) token.SetActive(true);
        foreach (var fuse in fuses.Values) fuse.SetActive(true);
        collectedFuses = 0;
        won = lost = charged = false;
        score = 0;
        elapsed = 0;
        nextMove = 0;
        boostSteps = 0;
        slowUntil = 0;
        nextChase = 4f;
        if (stage == 1) nextChase = .85f;
        if (stage == 2) nextChase = 1.7f;
        cutoffArmed = false;
        doorsSwapped = false;
        DrawState(true);
    }

    Vector2Int Movement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return Vector2Int.down;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) return Vector2Int.up;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) return Vector2Int.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) return Vector2Int.right;
        return Vector2Int.zero;
    }

    bool Walkable(Vector2Int tile) =>
        floorTiles.Contains(tile) && !walls.Contains(tile) &&
        (firstDoorMesh == null || tile != (doorsSwapped ? firstDoor : secondDoor));

    bool CanStep(Vector2Int from, Vector2Int to, bool playerMove)
    {
        if (!Walkable(to)) return false;
        bool crossesShutter = shutters.Contains(from) || shutters.Contains(to);
        return !crossesShutter || (playerMove && charged && to == from + Vector2Int.down);
    }

    bool RobotAt(Vector2Int position) =>
        position == robots[0] || (RobotCount == 2 && position == robots[1]);

    void Update()
    {
        var camera = mazeCamera;
        camera.orthographicSize = Mathf.Max(6.8f, cameraHalfWidth / camera.aspect);

        for (int i = 0; i < levels.Length; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                LoadStage(i);
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) ResetRound();
        if (won && Input.GetKeyDown(KeyCode.Return))
        {
            LoadStage((stage + 1) % levels.Length);
            return;
        }
        if (won || lost) return;

        var direction = Movement();
        bool rewind = charged && Input.GetKeyDown(KeyCode.Space);
        if (elapsed == 0 && direction == Vector2Int.zero && !rewind) return;
        elapsed += Time.deltaTime;

        MovePlayer(direction, rewind);
        CheckPlayerTile();
        won = !lost && ExitOpen && player == exit;
        MoveRobots();
        DrawState(false);
    }

    void MovePlayer(Vector2Int direction, bool rewind)
    {
        if (rewind)
        {
            boostSteps = 0;
            player = anchor;
            charged = false;
            playerMesh.transform.position = World(player, .4f);
            nextMove = elapsed + .11f;
        }
        else if (elapsed >= nextMove)
        {
            bool boosting = boostSteps > 0;
            var step = direction;
            if (boosting) step = boostDirection;
            if (step != Vector2Int.zero && CanStep(player, player + step, true))
            {
                player += step;
                if (boosting) boostSteps--;
                else if (boosts.TryGetValue(player, out var arrow) && step == arrow)
                {
                    boostDirection = arrow;
                    boostSteps = 2;
                }

                if (slime.Contains(player)) slowUntil = elapsed + .85f;
                float stepDelay = .13f;
                if (elapsed < slowUntil) stepDelay = .23f;
                if (boostSteps > 0) stepDelay = .055f;
                nextMove = elapsed + stepDelay;
                if (firstDoorMesh != null && player == switchTile && !RobotAt(doorsSwapped ? secondDoor : firstDoor))
                    doorsSwapped = !doorsSwapped;
            }
            else if (boosting)
            {
                boostSteps = 0;
            }
        }
    }

    void CheckPlayerTile()
    {
        lost = RobotAt(player);
        if (!lost && tokens.TryGetValue(player, out var token) && token.activeSelf)
        {
            token.SetActive(false);
            anchor = player;
            charged = true;
            score++;
        }
        if (!lost && fuses.TryGetValue(player, out var fuse) && fuse.activeSelf)
        {
            fuse.SetActive(false);
            collectedFuses++;
        }
        if (stage == 2 && !cutoffArmed && player == trigger)
        {
            cutoffArmed = true;
            cutoffStarts = elapsed + .65f;
            cutoffEnds = cutoffStarts + 2.4f;
        }
    }

    void MoveRobots()
    {
        if (!lost && !won && elapsed >= nextChase)
        {
            float robotDelay = .43f;
            if (stage == 1) robotDelay = .21f;
            if (stage == 2) robotDelay = .27f;
            nextChase = elapsed + robotDelay;
            var otherRobot = new Vector2Int(-1, -1);
            if (RobotCount == 2) otherRobot = robots[1];
            robots[0] = Chase(robots[0], otherRobot, player);
            if (stage == 1 && RobotCount == 2 && robots[0] != player)
                robots[1] = Chase(robots[1], robots[0], player);
            else if (stage == 2 && cutoffArmed && elapsed >= cutoffStarts && robots[0] != player)
            {
                // The purple robot heads to the junction before chasing the player.
                var target = player;
                if (elapsed < cutoffEnds) target = gate;
                robots[1] = Chase(robots[1], robots[0], target);
            }
            lost = RobotAt(player);
        }
    }

    Vector2Int Chase(Vector2Int from, Vector2Int blocked, Vector2Int target)
    {
        var queue = new Queue<(Vector2Int tile, Vector2Int first)>();
        var seen = new HashSet<Vector2Int> { from, blocked };
        queue.Enqueue((from, from));
        while (queue.Count > 0)
        {
            var (tile, first) = queue.Dequeue();
            if (tile == target) return first;
            foreach (var direction in Directions)
            {
                var next = tile + direction;
                if (CanStep(tile, next, false) && seen.Add(next))
                    queue.Enqueue((next, tile == from ? next : first));
            }
        }
        return from;
    }

    void DrawState(bool snap)
    {
        Place(playerMesh, player, .4f, snap);
        for (int i = 0; i < RobotCount; i++) Place(robotMeshes[i], robots[i], .38f, snap);
        if (gateLight != null)
        {
            bool flashing = cutoffArmed && elapsed < cutoffStarts;
            gateLight.SetActive(!flashing || Mathf.FloorToInt(elapsed * 16) % 2 == 0);
        }
        if (firstDoorMesh != null)
        {
            firstDoorMesh.SetActive(doorsSwapped);
            secondDoorMesh.SetActive(!doorsSwapped);
        }
        marker.SetActive(charged);
        marker.transform.position = World(anchor, .03f);
        exitMesh.GetComponent<Renderer>().material.color = ExitOpen ? Color.green : Color.red;
    }

    static void Place(GameObject mesh, Vector2Int tile, float height, bool snap)
    {
        var target = World(tile, height);
        mesh.transform.position = snap ? target : Vector3.MoveTowards(mesh.transform.position, target, 14 * Time.deltaTime);
    }

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
        GUI.Label(new Rect(0, 8, Screen.width, 30),
            $"{stage + 1}. {levels[stage].name}   |   {score}/{tokens.Count} tokens   |   {collectedFuses}/{fuses.Count} fuses   |   {elapsed:0.0}s   |   {(charged ? "REWIND READY" : "NO REWIND")}", style);
        string message = "Take the pink fuses  •  Space to rewind  •  R to retry";
        if (won)
        {
            message = "ESCAPED  •  Enter for next stage  •  R to retry";
            if (stage == levels.Length - 1)
                message = "ALL STAGES CLEAR  •  Enter for warm-up  •  R to retry";
        }
        else if (lost)
            message = "CAUGHT  •  R to retry";
        else if (ExitOpen)
            message = "EXIT OPEN  •  Leave now or grab the rest";
        else if (stage == 2 && cutoffArmed && elapsed < cutoffStarts)
            message = "WARNING  •  Purple robot is heading to the flashing tile";
        else if (elapsed < slowUntil)
            message = "SLIME  •  Movement slowed briefly";
        else if (stage == 1)
            message = "Follow the green arrow to dash  •  Space to rewind";
        else if (!charged)
            message = "Grab a token to charge rewind and enter a pink shutter";
        else if (stage == 2 && cutoffArmed && elapsed >= cutoffEnds)
            message = "Purple robot is now chasing you  •  Space to rewind";
        GUI.Label(new Rect(0, Screen.height - 38, Screen.width, 32), message, style);
    }
}





