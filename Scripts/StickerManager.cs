using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour
{
    public static List<int> edges;
    public static List<List<Vector3>> allBorderPoints;
    public static List<List<Vector3>> islandBorderPoints;
    public static List<Vector3> allPoints;

    public static Color[] bitmap;

    public static List<Vector3> corners;

    public static List<Node> allNodes;
    public static List<Node> availableNodes;
    public static List<Node> activeNodes;
    public static Node currentNode;
    public static List<Vector3> nodePoses;
    public static int brokenNodeCount;

    public static string currentState;

    public List<string> stickerCategories;
    public List<string> lineArtNames;

    public  string currentCategory;
    public string selectedLineArtName;
    public List<string> currentLineArtNames;
    public List<Texture2D> currentLineArtTextures;

    public PolygonCollider2D polygonCollider;
    public RuntimeAlphaMeshCollider runtimeAlphaMeshCollider;

    public float lineTolerance;
    public float lineSmoothSegmentLength;
    public bool smooth;

    public float stickerSize;

    public LineRenderer[] lineRenderers;
    public float lineZPos;
    public MeshFilter stickerMeshFilter;
    public MeshFilter sticker_OuterMeshFilter;
    public float borderEdgeLength;
    public float triangulationMinimumArea;
    public float triangulationMaximumArea;

    public SplineMesh.Spline[] borderSplines;
    public float splineHandleLengthRatio;

    public static int splineID;
    public static float currentScissorsSpeed;
    public Animator scissorsAnim;
    public Animator scissorsAppearAnim;
    public float scissorsSpeed;

    public Rigidbody nodeRootRigidbody;
    public Transform nodeParent;
    public Node nodePrefab;
    public float jointSpring; 
    public float jointDamper;
    public float jointSpring_Debug;
    public float jointDamper_Debug;
    public float breakForce;

    public float jointSpringStick;
    public float jointDamperStick;

    public static float strokeWidth;
    public static Texture2D lineArtTexture2D;
    public static Texture2D colorArtTexture2D;
    public static Texture2D finalArtTexture2D;
    public static Texture2D stickerTexture2D;
    public Transform strokeStart;
    public Transform strokeEnd;
    public Transform strokeBody;
    public float lineStrokeWidth;
    public float colorStrokeWidth;
    public float cutOutStrokeWidth;
    public RenderTexture drawRenderTexture;
    public Material lineArtMat;
    public Material colorArtMat;
    public Material finalArtMat;
    public Material finalArtMat_Transparent;
    public Material cutoutArtMat;
    public Material strokeMat;
    public Color colorArtColor;
    public GameObject lineArtObj;
    public GameObject colorArtObj;
    public GameObject finalArtObj;
    public Transform finalArtVertical;
    public Transform finalArtHorizontal;
    public Transform finalArtPeel;
    public float finalArtMoveDuration;
    public float finalArtRotateDelay;
    public SpriteRenderer finalArtSpriteRenderer;
    public GameObject drawStrokeObjs;
    int drawLayer;

    public MeshFilter finalArtFillMeshFilter;
    public MeshFilter finalArtMeshFilter;
    public MeshRenderer finalArtMeshRenderer;

    public float rotateToPeelDuration;

    public Camera drawCam;
    public GameObject drawMask;

    public Texture2D transparentTexture;

    public float coverOffset;

    public float lineRendererTileUnit;

    public Deform.BendDeformer cutoutBGBendDeformer;
    public Transform cutoutBGBendDeformerStartPos;
    public Transform cutoutBGBendDeformerStopPos;
    public float cutoutBGBendDeformerAngle;
    public float cutoutBGBendDeformerMoveDuration;
    public float cutoutBGBendDeformerRollDuration;
    public float cutoutBGBendDeformerDelay;
    public float scissorHidingDuration;
    public float scissorHidePos;

    public float stickingGravity;
    public float stickingDuration;
    public float normalGravity;

    public Material customerMat;
    public float customerFogAmount;
    public float customerFadeDuration;

    public MeshFilter itemMeshFilter;
    public MeshCollider itemMeshCollider;
    public Mesh[] itemMeshes;
    public Transform itemRotator;
    public Animator itemAnim;
    public Transform itemCustomerPos;
    public Transform itemStickPos;
    public Transform tableSurface;

    public Animator customerAnim;
    public float customerGreetingDelay;

    public GameObject drawModule;

    public float finalArtFadeDuration;
    public Color finalArtFadeColor;

    public float triangulationCheckDelay;

    public GameObject artSelectingModule;
    public MeshRenderer[] artPieces;
    public Material[] artPieceMats;
    public Transform[] artPieceOriginalPoses;
    public Transform artPieceSelectedPos;
    public float artSelectScaleUpDuration;

    public string lineArtFolder;

    public TMPro.TextMeshProUGUI orderText;

    public static int currentColorID;
    public static List<Color> colors;
    public Image[] colorImgs;
    public Animator[] colorBtnAnims;

    public SkinnedMeshRenderer customerSkinnedMeshRenderer;
    public Mesh[] customerMeshes;
    public List<int> availableCustomerMeshIDs;

    public float activateNodeForce;

    char[] splitChar = { '_' };

    public static bool jointSpringDebug;
    public Toggle jointSpringToggle;

    private void Awake()
    {
        edges = new List<int>();
        allBorderPoints = new List<List<Vector3>>();
        for (int i = 0; i < 18; i++)
            allBorderPoints.Add(new List<Vector3>());
        allPoints = new();
        islandBorderPoints = new List<List<Vector3>>();
        for (int i = 0; i < 18; i++)
            islandBorderPoints.Add(new List<Vector3>());

        allNodes = new();
        availableNodes = new();
        activeNodes = new();

        nodePoses = new();

        colors = new();
        for (int i = 0; i < colorImgs.Length; i++)
            colors.Add(colorImgs[i].color);

        currentLineArtNames = new();
        for (int i = 0; i < 4; i++)
            currentLineArtNames.Add("");

        currentLineArtTextures = new();
        for (int i = 0; i < 4; i++)
            currentLineArtTextures.Add(new Texture2D(2,2));

        availableCustomerMeshIDs = new();
        for (int i = 0; i < customerMeshes.Length; i++)
            availableCustomerMeshIDs.Add(i);

        corners = new();
        GenerateSquareBorder();
        drawLayer = LayerMask.NameToLayer("Draw");
        HideStroke();
        lineArtMat.mainTexture = null;

#if UNITY_EDITOR
        LoadLineArtPaths();
#endif
        LoadStickerCategories();

        jointSpringToggle.isOn = false;
    }

    void LoadLineArtPaths()
    {
        lineArtNames = new();
        string[] names = System.IO.Directory.GetFiles("Assets/Resources/" + lineArtFolder);
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = names[i][26..].Replace(".png", "");
            if (!names[i].ToLower().Contains("meta")
                && !names[i].ToLower().Contains("db")
                && !names[i].ToLower().Contains("ds_s"))
                lineArtNames.Add(names[i]);
        }
    }

    void LoadStickerCategories()
    {
        stickerCategories = new();

        for (int i = 0; i < lineArtNames.Count; i++)
        {
            string[] cats = lineArtNames[i].Split(splitChar);
            for (int k = 0; k < cats.Length; k++)
            {
                if (cats[k].Length >= 3 && !stickerCategories.Contains(cats[k]))
                    stickerCategories.Add(cats[k]);
            }
        }
    }
    void SpawnAnOrder()
    {
        currentCategory = stickerCategories[Random.Range(0, 100) % stickerCategories.Count];
        SetArtPieces(currentCategory);
        orderText.text = currentCategory.Replace("-", " ").ToUpper();
    }
    void SetArtPieces(string category)
    {
        artSelectingModule.SetActive(false);
        List<int> artIndexes = new();
        for (int i = 0; i < lineArtNames.Count; i++)
            artIndexes.Add(i);
        int artPieceIndex = Random.Range(0, 100) % 4;
        int artNameIndex = FindALineArtWithCategory(category, artIndexes);
        currentLineArtNames[artPieceIndex] = lineArtNames[artNameIndex];
        currentLineArtTextures[artPieceIndex] = Resources.Load<Texture2D>(lineArtFolder + "/" + lineArtNames[artNameIndex]);
        artPieceMats[artPieceIndex].mainTexture = currentLineArtTextures[artPieceIndex];
        artPieces[artPieceIndex].transform.SetPositionAndRotation(artPieceOriginalPoses[artPieceIndex].position, artPieceOriginalPoses[artPieceIndex].rotation);
        artPieces[artPieceIndex].transform.localScale = artPieceOriginalPoses[artPieceIndex].localScale;
        artIndexes.Remove(artNameIndex);

        for (int i = 0; i < artPieces.Length; i++)
        {
            if (i != artPieceIndex)
            {
                int index = artIndexes[Random.Range(0, 1000) % artIndexes.Count];
                artIndexes.Remove(index);
                currentLineArtNames[i] = lineArtNames[index];
                currentLineArtTextures[i] = Resources.Load<Texture2D>(lineArtFolder + "/" + lineArtNames[index]);
                artPieceMats[i].mainTexture = currentLineArtTextures[i];
                artPieces[i].transform.SetPositionAndRotation(artPieceOriginalPoses[i].position, artPieceOriginalPoses[i].rotation);
                artPieces[i].transform.localScale = artPieceOriginalPoses[i].localScale;
            }
        }
    }
    int FindALineArtWithCategory(string cat, List<int> remainIndices)
    {
        int index = -1;
        while (index == -1 || !lineArtNames[index].Contains(cat))
            index = remainIndices[Random.Range(0, 100) % remainIndices.Count];
        return index;
    }

    private void LateUpdate()
    {
        if (activeNodes.Count > 0)
        {
            if (!currentState.Contains("Sticking"))
                UpdateCoverMesh();
            else if (currentState == "Sticking")
                UpdateFinalArtMesh();
        }
    }

    public void Init()
    {
        currentState = "Start";
        currentScissorsSpeed = 0.0f;

        brokenNodeCount = 0;
        availableNodes.Clear();
        activeNodes.Clear();
        for (int i = 0; i < allNodes.Count; i++)
            allNodes[i].Reset();

        polygonCollider.gameObject.SetActive(false);

        finalArtFillMeshFilter.mesh = null;
        finalArtMeshFilter.mesh = null;
        finalArtMeshRenderer.material = finalArtMat;
        finalArtFillMeshFilter.transform.rotation = finalArtHorizontal.rotation;

        for (int i = 0; i < lineRenderers.Length; i++)
            lineRenderers[i].positionCount = 0;

        cutoutBGBendDeformer.Angle = 0.0f;
        cutoutBGBendDeformer.transform.position = cutoutBGBendDeformerStartPos.position;

        itemAnim.transform.localEulerAngles = 90.0f * (Vector3.left + Vector3.forward);

        HideStroke();

        SelectAnItem();
        StartCoroutine(ShowCustomerAndItem_());

        GetACustomer();

        drawModule.SetActive(false);

        SpawnAnOrder();

        Camera.main.cullingMask = LayerMask.GetMask("Default");
        finalArtObj.transform.parent = drawModule.transform;
        finalArtObj.transform.SetPositionAndRotation(finalArtHorizontal.position, finalArtHorizontal.rotation);
    }

    void GetACustomer()
    {
        int id = availableCustomerMeshIDs[Random.Range(0, 1000) % availableCustomerMeshIDs.Count];
        availableCustomerMeshIDs.Remove(id);
        if (availableCustomerMeshIDs.Count == 0)
        {
            for (int i = 0; i < customerMeshes.Length; i++)
                availableCustomerMeshIDs.Add(i);
        }
        customerSkinnedMeshRenderer.sharedMesh = customerMeshes[id];
    }

    public void UpdateCoverMesh()
    {
        nodePoses.Clear();
        for (int i = 0; i < activeNodes.Count; i++)
            nodePoses.Add(activeNodes[i].transform.localPosition);
        stickerMeshFilter.mesh.SetVertices(nodePoses);
        stickerMeshFilter.mesh.RecalculateNormals();
    }
    public void UpdateFinalArtMesh()
    {
        nodePoses.Clear();
        for (int i = 0; i < activeNodes.Count; i++)
            nodePoses.Add(activeNodes[i].transform.localPosition);
        finalArtMeshFilter.mesh.SetVertices(nodePoses);
        finalArtMeshFilter.mesh.RecalculateNormals();
    }

    public void SpawnNodes(Mesh mesh)
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
            PutANodeAt(mesh.vertices[i]);
        for (int i = 0; i < activeNodes.Count; i++)
        {
            activeNodes[i].AddFixedJoint();
            activeNodes[i].theRigidbody.isKinematic = true;
        }
        for (int i = 0; i < mesh.triangles.Length; i+= 3)
        {
            activeNodes[mesh.triangles[i]].AddSpringJoint(activeNodes[mesh.triangles[i + 1]].theRigidbody);
            activeNodes[mesh.triangles[i + 1]].AddSpringJoint(activeNodes[mesh.triangles[i + 2]].theRigidbody);
            activeNodes[mesh.triangles[i + 2]].AddSpringJoint(activeNodes[mesh.triangles[i]].theRigidbody);
        }
    }
    public void ActivateNodes(float vel)
    {
        for (int i = 0; i < activeNodes.Count; i++)
        {
            activeNodes[i].theRigidbody.isKinematic = false;
            activeNodes[i].theRigidbody.velocity = vel * Physics.gravity.normalized;
            activeNodes[i].theRigidbody.angularVelocity = Vector3.zero;
        }
    }
    public void DeactivateNodes()
    {
        for (int i = 0; i < activeNodes.Count; i++)
            activeNodes[i].theRigidbody.isKinematic = true;
    }
    public void ResetNodePositions()
    {
        DeactivateNodes();
        nodeParent.parent = finalArtMeshFilter.transform;
        nodeParent.localPosition = Vector3.zero;
        nodeParent.localRotation = Quaternion.identity;
        for (int i = 0; i < activeNodes.Count; i++)
        {
            activeNodes[i].transform.localPosition = finalArtMeshFilter.mesh.vertices[i];
            activeNodes[i].transform.localRotation = Quaternion.identity;
            activeNodes[i].theRigidbody.velocity = Vector3.zero;
            activeNodes[i].theRigidbody.angularVelocity = Vector3.zero;

            for (int k = 0; k < activeNodes[i].springJoints.Count; k++)
            {
                activeNodes[i].springJoints[k].spring = jointSpringStick;
                activeNodes[i].springJoints[k].damper = jointDamperStick;
            }
        }
    }
    public void StickIt()
    {
        if (currentState != "Sticking")
        {
            currentState = "Sticking";
            StartCoroutine(StickIt_());
            Managers.sceneManager.HideUI(Managers.sceneManager.btnStickCanvasGroup);
            for (int i = 0; i < lineRenderers.Length; i++)
                lineRenderers[i].positionCount = 0;
        }
    }
    IEnumerator StickIt_()
    {
        Physics.gravity = 9.81f * Camera.main.transform.forward;
        ResetNodePositions();

        for (float t = 0.0f; t <= finalArtFadeDuration * 0.68f + 0.128f; t += Time.deltaTime)
        {
            finalArtMat_Transparent.color = Color.Lerp(finalArtFadeColor, Color.white, t / (finalArtFadeDuration * 0.68f));
            yield return null;
        }
        finalArtMeshRenderer.material = finalArtMat;

        ActivateNodes(activateNodeForce);

        yield return new WaitForSeconds(stickingDuration);

        currentState = "DoneSticking";

        DeactivateNodes();
        finalArtObj.transform.parent = itemMeshFilter.transform;
        nodeParent.parent = null;

        Physics.gravity = normalGravity * Vector3.down;

        StartCoroutine(GiveItemToTheCustomer_());
    }

    void GenerateSquareBorder()
    {
        int step = Mathf.RoundToInt(stickerSize / borderEdgeLength);
        float newLength = stickerSize / step;
        Vector3 pos = stickerSize * 0.5f * (Vector3.left + Vector3.up);
        corners.Add(pos);
        for (int i = 0; i < step; i++)
        {
            pos += newLength * Vector3.right;
            corners.Add(pos);
        }
        for (int i = 0; i < step; i++)
        {
            pos += newLength * Vector3.down;
            corners.Add(pos);
        }
        for (int i = 0; i < step; i++)
        {
            pos += newLength * Vector3.left;
            corners.Add(pos);
        }
        for (int i = 0; i < step - 1; i++)
        {
            pos += newLength * Vector3.up;
            corners.Add(pos);
        }
    }

    public void Triangulate()
    {
        stickerMeshFilter.mesh.Clear();
        while (stickerMeshFilter.mesh.vertexCount == 0)
        {
            andywiecko.BurstTriangulator.Triangulator triangulator = new andywiecko.BurstTriangulator.Triangulator(4096, Unity.Collections.Allocator.Persistent);
            triangulator.Settings.RefineMesh = true;
            triangulator.Settings.RestoreBoundary = true;
            triangulator.Settings.ConstrainEdges = true;
            triangulator.Settings.MinimumArea = triangulationMinimumArea;
            triangulator.Settings.MaximumArea = triangulationMaximumArea;
            triangulator.Settings.ValidateInput = false;

            // STICKER MESH
            // Points
            Unity.Mathematics.float2[] points2D_sticker = new Unity.Mathematics.float2[allPoints.Count];
            for (int i = 0; i < allPoints.Count; i++)
                points2D_sticker[i] = new Unity.Mathematics.float2(allPoints[i].x, allPoints[i].y);
            var points_sticker = new Unity.Collections.NativeArray<Unity.Mathematics.float2>(points2D_sticker, Unity.Collections.Allocator.Persistent);
            triangulator.Input.Positions = points_sticker;
            // Constraint Edges
            var constraintEdges_sticker = new Unity.Collections.NativeArray<int>(allPoints.Count * 2, Unity.Collections.Allocator.Persistent);
            int index = 0;
            int startIndex = 0;
            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                startIndex = index;
                for (int k = 0; k < allBorderPoints[i].Count - 1; k++)
                {
                    constraintEdges_sticker[index * 2] = index;
                    constraintEdges_sticker[index * 2 + 1] = index + 1;
                    index++;
                }
                constraintEdges_sticker[index * 2] = index;
                constraintEdges_sticker[index * 2 + 1] = startIndex;
                index++;
            }
            triangulator.Input.ConstraintEdges = constraintEdges_sticker;
            triangulator.Run();
            List<Vector3> vertices = new();
            List<Vector2> uvs = new();
            for (int i = 0; i < triangulator.Output.Positions.Length; i++)
            {
                vertices.Add(new Vector3(triangulator.Output.Positions[i].x, 0.0f, triangulator.Output.Positions[i].y));
                uvs.Add(new Vector2((vertices[^1].x + stickerSize * 0.5f) / stickerSize, (vertices[^1].y + stickerSize * 0.5f) / stickerSize));
            }
            List<int> tris = new();
            for (int i = 0; i < triangulator.Output.Triangles.Length; i++)
                tris.Add(triangulator.Output.Triangles[i]);
            stickerMeshFilter.mesh.Clear();
            stickerMeshFilter.mesh.SetVertices(vertices);
            stickerMeshFilter.mesh.SetTriangles(tris.ToArray(), 0);
            stickerMeshFilter.mesh.SetUVs(0, uvs);
            stickerMeshFilter.mesh.RecalculateNormals();

            stickerMeshFilter.gameObject.SetActive(false);
        }
    }
    bool paused = false;
    public void Continue()
    {
        paused = false;
    }
    public void Retriangulate()
    {
        stickerMeshFilter.mesh.Clear();
        while (stickerMeshFilter.mesh.vertexCount == 0)
        {
            andywiecko.BurstTriangulator.Triangulator triangulator = new(4096, Unity.Collections.Allocator.Persistent);
            triangulator.Settings.RefineMesh = true;
            triangulator.Settings.RestoreBoundary = true;
            triangulator.Settings.ConstrainEdges = true;
            triangulator.Settings.MinimumArea = triangulationMinimumArea;
            triangulator.Settings.MaximumArea = triangulationMaximumArea;
            triangulator.Settings.ValidateInput = false;

            // STICKER MESH
            // Points
            int pointCount = 0;
            for (int i = 0; i < islandBorderPoints.Count; i++)
                pointCount += islandBorderPoints[i].Count;
            Unity.Mathematics.float2[] points2D_sticker = new Unity.Mathematics.float2[pointCount];
            int index = 0;
            for (int i = 0; i < islandBorderPoints.Count; i++)
            {
                for (int k = 0; k < islandBorderPoints[i].Count; k++)
                {
                    points2D_sticker[index] = new Unity.Mathematics.float2(islandBorderPoints[i][k].x, islandBorderPoints[i][k].y);
                    index++;
                }
            }
            var points_sticker = new Unity.Collections.NativeArray<Unity.Mathematics.float2>(points2D_sticker, Unity.Collections.Allocator.Persistent);
            triangulator.Input.Positions = points_sticker;
            // Constraint Edges
            var constraintEdges_sticker = new Unity.Collections.NativeArray<int>(pointCount * 2, Unity.Collections.Allocator.Persistent);
            index = 0;
            int startIndex = 0;
            for (int i = 0; i < islandBorderPoints.Count; i++)
            {
                startIndex = index;
                for (int k = 0; k < islandBorderPoints[i].Count - 1; k++)
                {
                    constraintEdges_sticker[index * 2] = index;
                    constraintEdges_sticker[index * 2 + 1] = index + 1;
                    index++;
                }
                constraintEdges_sticker[index * 2] = index;
                constraintEdges_sticker[index * 2 + 1] = startIndex;
                index++;
            }

            triangulator.Input.ConstraintEdges = constraintEdges_sticker;
            triangulator.Run();
            List<Vector3> vertices = new();
            List<Vector2> uvs = new();
            for (int i = 0; i < triangulator.Output.Positions.Length; i++)
            {
                vertices.Add(new Vector3(triangulator.Output.Positions[i].x, 0.0f, triangulator.Output.Positions[i].y));
                uvs.Add(new Vector2((vertices[^1].x + stickerSize * 0.5f) / stickerSize, (vertices[^1].y + stickerSize * 0.5f) / stickerSize));
            }
            List<int> tris = new();
            for (int i = 0; i < triangulator.Output.Triangles.Length; i++)
                tris.Add(triangulator.Output.Triangles[i]);
            stickerMeshFilter.mesh.Clear();
            stickerMeshFilter.mesh.SetVertices(vertices);
            stickerMeshFilter.mesh.SetTriangles(tris.ToArray(), 0);
            stickerMeshFilter.mesh.SetUVs(0, uvs);
            stickerMeshFilter.mesh.RecalculateNormals();

            stickerMeshFilter.gameObject.SetActive(false);
        }
    }

    void GetAllPoints()
    {
        allPoints.Clear();
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            Vector2[] points = polygonCollider.GetPath(i);
            allBorderPoints[i].Clear();
            for (int k = 0; k < points.Length; k++)
                allBorderPoints[i].Add(points[k] * stickerSize);

            LineUtility.Simplify(allBorderPoints[i], lineTolerance, allBorderPoints[i]);
            if (smooth)
            {
                Vector3[] smoothedPoints = LineSmoother.SmoothLine(allBorderPoints[i].ToArray(), lineSmoothSegmentLength);
                allBorderPoints[i].Clear();
                for (int k = 0; k < smoothedPoints.Length; k++)
                    allBorderPoints[i].Add(smoothedPoints[k]);
            }
            for (int k = 0; k < allBorderPoints[i].Count; k++)
                allPoints.Add(allBorderPoints[i][k]);
        }
    }

    int AlreadyHaveEdge(List<int> edges, int p0, int p1)
    {
        for (int i = 0; i < edges.Count; i += 2)
        {
            if ((edges[i] == p0 && edges[i + 1] == p1) || (edges[i] == p1 && edges[i + 1] == p0))
                return i;
        }
        return -1;
    }
    List<int> RemoveEdge(List<int> edges, int p0_, int p1_)
    {
        int index = AlreadyHaveEdge(edges, p0_, p1_);
        if (index != -1)
        {
            edges.RemoveAt(index);
            edges.RemoveAt(index);
        }
        return edges;
    }
    Vector2 FindNextEdge(List<int> edges, int havePoint, int withoutPoint)
    {
        for (int i = 0; i < edges.Count; i += 2)
        {
            if (edges[i] == havePoint && edges[i + 1] != withoutPoint)
                return havePoint * Vector2.right + edges[i + 1] * Vector2.up;
            else if (edges[i + 1] == havePoint && edges[i] != withoutPoint)
                return havePoint * Vector2.right + edges[i] * Vector2.up;
        }
        return Vector2.zero;
    }
    List<List<int>> SortBorderEdges(List<int> edges)
    {
        List<int> remainEdges = new(edges);
        List<List<int>> sortedEdges = new();

        while (remainEdges.Count > 0)
        {
            sortedEdges.Add(new List<int>());
            sortedEdges[^1].Add(remainEdges[0]);
            sortedEdges[^1].Add(remainEdges[1]);
            remainEdges = RemoveEdge(remainEdges, sortedEdges[^1][^1], sortedEdges[^1][^2]);
            while (sortedEdges[^1][^1] != sortedEdges[^1][0])
            {
                Vector2 edge = FindNextEdge(remainEdges, sortedEdges[^1][^1], sortedEdges[^1][^2]);
                sortedEdges[^1].Add((int)edge.x);
                sortedEdges[^1].Add((int)edge.y);
                remainEdges = RemoveEdge(remainEdges, sortedEdges[^1][^1], sortedEdges[^1][^2]);
            }
        }
        return sortedEdges;
    }
    List<List<Vector3>> GetIslandBorderPoints(Mesh mesh)
    {
        List<List<Vector3>> borderVertices = new();
        List<int> borderEdges = new();
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            if (AlreadyHaveEdge(borderEdges, mesh.triangles[i], mesh.triangles[i + 1]) == -1)
            {
                borderEdges.Add(mesh.triangles[i]);
                borderEdges.Add(mesh.triangles[i + 1]);
            }
            else
                borderEdges = RemoveEdge(borderEdges, mesh.triangles[i], mesh.triangles[i + 1]);
            if (AlreadyHaveEdge(borderEdges, mesh.triangles[i + 1], mesh.triangles[i + 2]) == -1)
            {
                borderEdges.Add(mesh.triangles[i + 1]);
                borderEdges.Add(mesh.triangles[i + 2]);
            }
            else
                borderEdges = RemoveEdge(borderEdges, mesh.triangles[i + 1], mesh.triangles[i + 2]);
            if (AlreadyHaveEdge(borderEdges, mesh.triangles[i + 2], mesh.triangles[i]) == -1)
            {
                borderEdges.Add(mesh.triangles[i + 2]);
                borderEdges.Add(mesh.triangles[i]);
            }
            else
                borderEdges = RemoveEdge(borderEdges, mesh.triangles[i + 2], mesh.triangles[i]);
        }

        var sortedEdges = SortBorderEdges(borderEdges);
        for (int i = 0; i < sortedEdges.Count; i++)
        {
            borderVertices.Add(new List<Vector3>());
            for (int k = 0; k < sortedEdges[i].Count; k += 2)
                borderVertices[^1].Add(mesh.vertices[sortedEdges[i][k]]);
        }

        return borderVertices;
    }

    bool IsBorderEdge(int[] t, int a, int b)
    {
        int count = 0;
        for (int i = 0; i < t.Length; i += 3)
        {
            if ((t[i] == a && t[i + 1] == b)
                || (t[i] == b && t[i + 1] == a)
                || (t[i] == a && t[i + 2] == b)
                || (t[i] == b && t[i + 2] == a)
                || (t[i + 1] == a && t[i + 2] == b)
                || (t[i + 1] == b && t[i + 2] == a))
            {
                count++;
                if (count == 2)
                    return false;
            }
        }
        return true;
    }

    void CreateSplinesFromIslandPoints()
    {
        for (int k = 0; k < islandBorderPoints.Count; k++)
        {
            if (borderSplines[k].nodes.Count > islandBorderPoints[k].Count)
            {
                while (borderSplines[k].nodes.Count > islandBorderPoints[k].Count)
                    borderSplines[k].RemoveNode(borderSplines[k].nodes[0]);
            }
            else if (borderSplines[k].nodes.Count < islandBorderPoints[k].Count)
            {
                while (borderSplines[k].nodes.Count < islandBorderPoints[k].Count)
                    borderSplines[k].AddNode(new SplineMesh.SplineNode(Vector3.zero, Vector3.up));
            }
            for (int i = 0; i < islandBorderPoints[k].Count; i++)
                borderSplines[k].nodes[i].Position = islandBorderPoints[k][i].x * Vector3.right + islandBorderPoints[k][i].z * Vector3.forward;
            Vector3 dir = Vector3.zero;
            Vector3 v;
            for (int i = 1; i < borderSplines[k].nodes.Count - 1; i++)
            {
                v = borderSplines[k].nodes[(i + 1) % borderSplines[k].nodes.Count].Position - borderSplines[k].nodes[(i + borderSplines[k].nodes.Count - 1) % borderSplines[k].nodes.Count].Position;
                dir = v.normalized * splineHandleLengthRatio;
                borderSplines[k].nodes[i].Direction = borderSplines[k].nodes[i].Position + dir;
                borderSplines[k].nodes[i].Up = dir.x * Vector3.down + dir.z * Vector3.right;
            }
            v = borderSplines[k].nodes[1].Position - borderSplines[k].nodes[^2].Position;
            dir = v.normalized * splineHandleLengthRatio;
            borderSplines[k].nodes[0].Direction = borderSplines[k].nodes[0].Position + dir;
            borderSplines[k].nodes[^1].Direction = borderSplines[k].nodes[^1].Position + dir;
            borderSplines[k].nodes[0].Up = borderSplines[k].nodes[^1].Up = dir.x * Vector3.down + dir.z * Vector3.right;
        }
    }

    public void StartCutting()
    {
        splineID = 0;
        SplineMesh.CurveSample sample = borderSplines[0].GetSampleAtDistance(0.001f);
        CharacterManager.lastPos = sample.location;
        strokeStart.position = CharacterManager.lastPos;
        strokeEnd.position = CharacterManager.lastPos;
        scissorsAppearAnim.transform.SetPositionAndRotation(CharacterManager.lastPos, Quaternion.LookRotation(sample.tangent, Vector3.up));
        scissorsAnim.SetTrigger("Idle");
        StartCoroutine(StartCutting_());

        Managers.cameraManager.SwitchtToCam(Managers.cameraManager.cutCam);
    }
    IEnumerator StartCutting_()
    {
        yield return new WaitForSeconds(Managers.cameraManager.transformTime * 0.5f);

        scissorsAppearAnim.SetTrigger("Appear");

        Managers.characterManager.StartHaptic();

        float dist;
        currentState = "Cutting";
        while (splineID < islandBorderPoints.Count)
        {
            dist = 0.0f;
            while (dist < borderSplines[splineID].Length)
            {
                if (CharacterManager.validCutStart)
                {
                    dist = Mathf.Clamp(dist + Time.deltaTime * currentScissorsSpeed, 0.0f, borderSplines[splineID].Length);
                    SplineMesh.CurveSample sample = borderSplines[splineID].GetSampleAtDistance(dist);
                    scissorsAppearAnim.transform.SetPositionAndRotation (sample.location, Quaternion.LookRotation(sample.tangent, Vector3.up));
                }
                
                yield return null;
            }
            scissorsAppearAnim.transform.position = borderSplines[splineID].GetSampleAtDistance(0.0f).location;
            yield return null;
            yield return null;
            lineRenderers[splineID].positionCount = 0;
            splineID++;
            if (splineID < islandBorderPoints.Count)
            {
                strokeStart.position = borderSplines[splineID].GetSampleAtDistance(borderSplines[splineID].Length).location;
                strokeEnd.position = strokeStart.position;
                CharacterManager.lastPos = strokeStart.position;
            }
        }
        currentState = "DoneCutting";
        scissorsAppearAnim.SetTrigger("Disappear");
        drawStrokeObjs.SetActive(false);

        Managers.characterManager.StopHaptic();
        Managers.soundManager.scissors.Stop();

        CharacterManager.canTouch = false;

        StartCoroutine(BendCutoutBG_());
    }

    public void PutANodeAt(Vector3 pos)
    {
        if (availableNodes.Count > 0)
        {
            currentNode = availableNodes[0];
            availableNodes.RemoveAt(0);
        }
        else
        {
            currentNode = Instantiate(nodePrefab, nodeParent);
            allNodes.Add(currentNode);
        }
        activeNodes.Add(currentNode);
        currentNode.transform.localPosition = pos;
    }

    public void HideStroke()
    {
        strokeStart.position = Vector3.left * 68.0f;
        strokeEnd.position = Vector3.left * 68.0f;
        strokeBody.position = Vector3.left * 68.0f;
    }
    public void SetStrokeWidth(float w)
    {
        strokeWidth = w;
        strokeStart.localScale = strokeEnd.localScale = Vector3.one * strokeWidth;
    }
    public void ClearRenderTexture(RenderTexture rtex)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = rtex;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }

    public void SelectLineArt()
    {
        itemAnim.SetTrigger("Disappear");
        Managers.sceneManager.HideUI(Managers.sceneManager.btnStartColoringCanvasGroup);
        Managers.sceneManager.HideUI(Managers.sceneManager.orderCanvasGroup);
        Managers.sceneManager.ShowUI(Managers.sceneManager.selectArtworkTitleCanvasGroup, 0.268f);
        Managers.cameraManager.SwitchtToCam(Managers.cameraManager.drawCam);
        currentState = "SelectingArt";
        artSelectingModule.SetActive(true);
        drawModule.SetActive(false);
    }

    public void SetLineArt(int index)
    {
        if (currentState != "Coloring")
        {
            //CharacterManager.canTouch = false;

            drawModule.SetActive(true);

            //Managers.cameraManager.SwitchtToCam(Managers.cameraManager.drawCam);
            Managers.sceneManager.ShowUI(Managers.sceneManager.colorBtnCanvasGroup, 0.0f);
            StartCoroutine(ResetColors_());

            currentState = "Coloring";

            drawStrokeObjs.SetActive(true);
            drawMask.SetActive(true);
            //finalArtObj.SetActive(false);
            finalArtFillMeshFilter.mesh = null;
            finalArtSpriteRenderer.gameObject.SetActive(false);
            colorArtObj.SetActive(true);
            finalArtSpriteRenderer.sprite = null;

            lineArtObj.layer = LayerMask.NameToLayer("Default");
            colorArtObj.layer = LayerMask.NameToLayer("Default");

            ClearRenderTexture(drawRenderTexture);
            lineArtMat.mainTexture = artPieceMats[index].mainTexture;
            colorArtMat.mainTexture = null;
            cutoutArtMat.mainTexture = transparentTexture;
            cutoutArtMat.SetTexture("_AdditionalAlpha", transparentTexture);
            //strokeMat.color = lineArtColor;

            StartDrawColorArt();
            Managers.sceneManager.ShowUI(Managers.sceneManager.btnStopColoringCanvasGroup, 2.98f);
            //StartCoroutine(FadeCustomer_());
        }
    }
    public void StartDrawColorArt()
    {
        SetStrokeWidth(colorStrokeWidth);
        ClearRenderTexture(drawRenderTexture);
        colorArtMat.mainTexture = drawRenderTexture;
        strokeMat.color = colorArtColor;
        lineArtObj.SetActive(true);
    }
    
    public void StopDraw()
    {
        if (currentState == "Coloring")
        {
            CharacterManager.canTouch = false;

            currentState = "DoneColoring";
            StartCoroutine(StopDraw_());
            Managers.sceneManager.HideUI(Managers.sceneManager.btnStopColoringCanvasGroup);

            StartCoroutine(MakeSureWeDidTriangulationRight_());
        }
    }
    IEnumerator StopDraw_()
    {
        Managers.sceneManager.HideUI(Managers.sceneManager.colorBtnCanvasGroup);

        drawMask.SetActive(false);
        HideStroke();
        SetStrokeWidth(cutOutStrokeWidth);

        // Get color texture
        GetTexture2DFromRenderTexture(out colorArtTexture2D, drawRenderTexture);
        colorArtMat.mainTexture = colorArtTexture2D;

        lineArtObj.layer = drawLayer;
        colorArtObj.layer = drawLayer;
        Camera.main.cullingMask = LayerMask.GetMask("Default", "Draw");

        yield return null;

        // Get first phase final texture
        GetTexture2DFromRenderTexture(out finalArtTexture2D, drawRenderTexture);
        finalArtSpriteRenderer.gameObject.SetActive(true);
        finalArtSpriteRenderer.sprite = Sprite.Create(finalArtTexture2D, new Rect(0.0f, 0.0f, finalArtTexture2D.width, finalArtTexture2D.height), Vector2.one * 0.5f, 100, 1, SpriteMeshType.FullRect);
        lineArtObj.SetActive(false);
        colorArtObj.SetActive(false);

        yield return null;
        yield return null;
        yield return null;

        // Get final texture with outline
        GetTexture2DFromRenderTexture(out finalArtTexture2D, drawRenderTexture);
        finalArtMat.mainTexture = finalArtTexture2D;

        // Prepare for cutting alpha
        ClearRenderTexture(drawRenderTexture);
        cutoutArtMat.SetTexture("_AdditionalAlpha", drawRenderTexture);

        // Resize texture for the triangulation
        polygonCollider.gameObject.SetActive(true);
        yield return null;
        yield return null;
        stickerTexture2D = Resize(finalArtTexture2D, 512, 512);
        bitmap = stickerTexture2D.GetPixels();
        // Create the polygon collider to get the borders
        runtimeAlphaMeshCollider.m_UsedTexture = stickerTexture2D;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        // Created needed data
        GetAllPoints();
        Triangulate();
        islandBorderPoints = GetIslandBorderPoints(stickerMeshFilter.mesh); // This is the border without holes
        //Retriangulate();
        CreateSplinesFromIslandPoints();
        SpawnNodes(stickerMeshFilter.mesh);

        // Line Renderer for the cutting part
        SetupLineRenderers(null, Color.black);

        polygonCollider.gameObject.SetActive(false);

        // Fill in the holes in finalArt
        finalArtFillMeshFilter.mesh = stickerMeshFilter.mesh;

        yield return null;

        // Get the final art texture without holes
        GetTexture2DFromRenderTexture(out finalArtTexture2D, drawRenderTexture);
        finalArtMat.mainTexture = finalArtTexture2D;
        finalArtMat_Transparent.mainTexture = finalArtTexture2D;
        finalArtMat_Transparent.color = Color.white;
        finalArtSpriteRenderer.gameObject.SetActive(false);
        finalArtFillMeshFilter.mesh = null;

        // Assign final art mesh
        finalArtMeshFilter.mesh = stickerMeshFilter.mesh;
        RecalculateFinalArtUV();
        Camera.main.cullingMask = LayerMask.GetMask("Default");

        nodeParent.gameObject.SetActive(false);

        StartCutting();
    }

    void SetupLineRenderers(Transform tran, Color col)
    {
        if (tran == null)
        {
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                float len;
                if (i < islandBorderPoints.Count)
                {
                    len = 0.0f;
                    lineRenderers[i].positionCount = islandBorderPoints[i].Count + 1;
                    for (int k = 0; k < islandBorderPoints[i].Count; k++)
                    {
                        lineRenderers[i].SetPosition(k, islandBorderPoints[i][k].x * Vector3.right + islandBorderPoints[i][k].z * Vector3.forward + lineZPos * Vector3.up);
                        len += (islandBorderPoints[i][k] - islandBorderPoints[i][(k + 1) % islandBorderPoints[i].Count]).magnitude;
                    }
                    lineRenderers[i].SetPosition(islandBorderPoints[i].Count, islandBorderPoints[i][0].x * Vector3.right + islandBorderPoints[i][0].z * Vector3.forward + lineZPos * Vector3.up);
                    lineRenderers[i].material.mainTextureScale = lineRendererTileUnit * len * Vector2.right + Vector2.up;
                    lineRenderers[i].material.color = col;
                }
                else
                    lineRenderers[i].positionCount = 0;
            }
        }
        else
        {
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                float len;
                if (i < islandBorderPoints.Count)
                {
                    len = 0.0f;
                    lineRenderers[i].positionCount = islandBorderPoints[i].Count + 1;
                    for (int k = 0; k < islandBorderPoints[i].Count; k++)
                    {
                        lineRenderers[i].SetPosition(k, tran.TransformPoint(islandBorderPoints[i][k].x * Vector3.right + islandBorderPoints[i][k].z * Vector3.forward + lineZPos * Vector3.up));
                        len += (islandBorderPoints[i][k] - islandBorderPoints[i][(k + 1) % islandBorderPoints[i].Count]).magnitude;
                    }
                    lineRenderers[i].SetPosition(islandBorderPoints[i].Count, tran.TransformPoint(islandBorderPoints[i][0].x * Vector3.right + islandBorderPoints[i][0].z * Vector3.forward + lineZPos * Vector3.up));
                    lineRenderers[i].material.mainTextureScale = lineRendererTileUnit * len * Vector2.right + Vector2.up;
                    lineRenderers[i].material.color = col;
                }
                else
                    lineRenderers[i].positionCount = 0;
            }
        }
    }

    void RecalculateFinalArtUV()
    {
        Vector2[] newUV = new Vector2[finalArtMeshFilter.mesh.uv.Length];
        for (int i = 0; i < newUV.Length; i++)
        {
            newUV[i] = (finalArtMeshFilter.mesh.vertices[i].x + stickerSize * 0.5f) / stickerSize * Vector2.right
                + (finalArtMeshFilter.mesh.vertices[i].z + stickerSize * 0.5f) / stickerSize * Vector2.up;
        }
        finalArtMeshFilter.mesh.SetUVs(0, newUV);
    }

    public void GetTexture2DFromRenderTexture(out Texture2D tex2D, RenderTexture rt)
    {
        tex2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex2D.Apply();
    }

    Texture2D Resize(Texture2D tex2D, int targetX, int targetY)
    {
        RenderTexture rt = new(targetX, targetY, 32);
        RenderTexture.active = rt;
        Graphics.Blit(tex2D, rt);
        Texture2D result = new(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
    Texture2D Flip(Texture2D tex2D)
    {
        Texture2D result = new(tex2D.width, tex2D.height);
        for (int w = 0; w < tex2D.width; w++)
        {
            for (int h = 0; h < tex2D.height; h++)
            {
                result.SetPixel(tex2D.width - w - 1, h, tex2D.GetPixel(w, h));
            }
        }
        result.Apply();
        return result;
    }
    Texture2D Expand(Texture2D tex2D, int expand)
    {
        Texture2D result = new(tex2D.width + 2 * expand, tex2D.height + 2 * expand);
        for (int w = 0; w < tex2D.width + 2 * expand; w++)
        {
            for (int h = 0; h < tex2D.height + 2 * expand; h++)
            {
                if (w >= expand && h >= expand && w < tex2D.width + expand && h < tex2D.height + expand)
                    result.SetPixel(w, h, tex2D.GetPixel(w - expand, h - expand));
                else
                    result.SetPixel(w, h, Color.clear);
            }
        }
        result.Apply();
        return result;
    }

    IEnumerator RotateFinalArt_()
    {
        yield return new WaitForSeconds(finalArtRotateDelay);

        CharacterManager.canTouch = false;

        cutoutArtMat.mainTexture = finalArtTexture2D;

        for (float t = 0.0f; t <= finalArtMoveDuration; t += Time.deltaTime)
        {
            finalArtObj.transform.SetPositionAndRotation(Vector3.Lerp(finalArtHorizontal.position, finalArtVertical.position, t / finalArtMoveDuration),
                Quaternion.Lerp(finalArtHorizontal.rotation, finalArtVertical.rotation, t / finalArtMoveDuration));
            Camera.main.transform.SetPositionAndRotation(Vector3.Lerp(Camera.main.transform.position, Managers.cameraManager.peelCam.position, t / finalArtMoveDuration),
                Quaternion.Lerp(Camera.main.transform.rotation, Managers.cameraManager.peelCam.rotation, t / finalArtMoveDuration));
            yield return null;
        }
        finalArtObj.transform.SetPositionAndRotation(finalArtVertical.position, finalArtVertical.rotation);
        Camera.main.transform.SetPositionAndRotation(Managers.cameraManager.peelCam.position, Managers.cameraManager.peelCam.rotation);

        StartCoroutine(RotateToPeel_());
    }

    IEnumerator RotateToPeel_()
    {
        yield return new WaitForSeconds(finalArtRotateDelay);

        CharacterManager.canTouch = false;

        Quaternion midRotation = Quaternion.Lerp(finalArtVertical.rotation, finalArtPeel.rotation, 0.5f);
        for (float t = 0.0f; t <= rotateToPeelDuration * 0.5f; t += Time.deltaTime)
        {
            finalArtObj.transform.rotation = Quaternion.Lerp(finalArtVertical.rotation, midRotation, t / (rotateToPeelDuration * 0.5f));
            yield return null;
        }
        finalArtObj.transform.rotation = midRotation;

        //SpawnNodes(stickerMeshFilter.mesh);
        stickerMeshFilter.transform.parent = finalArtMeshFilter.transform;
        stickerMeshFilter.transform.localPosition = coverOffset * Vector3.down;
        stickerMeshFilter.transform.localRotation = Quaternion.identity;
        stickerMeshFilter.gameObject.SetActive(true);

        for (float t = 0.0f; t <= rotateToPeelDuration * 0.5f; t += Time.deltaTime)
        {
            finalArtObj.transform.rotation = Quaternion.Lerp(midRotation, finalArtPeel.rotation, t / (rotateToPeelDuration * 0.5f));
            yield return null;
        }
        finalArtObj.transform.rotation = finalArtPeel.rotation;

        //yield return null;
        nodeParent.parent = finalArtMeshFilter.transform;
        nodeParent.localPosition = Vector3.zero;
        nodeParent.localRotation = Quaternion.identity;
        nodeParent.gameObject.SetActive(true);

        yield return null;

        CharacterManager.canTouch = true;
        currentState = "Peeling";
        ActivateNodes(0.0f);
        StartCoroutine(WaitingForPeeling_());
    }

    IEnumerator RotateToStick_()
    {
        nodeParent.parent = null;
        stickerMeshFilter.transform.parent = null;
        currentState = "PrepareToStick";
        CharacterManager.canTouch = false;

        Managers.cameraManager.SwitchtToCam(Managers.cameraManager.stickCam);

        yield return new WaitForSeconds(finalArtRotateDelay);

        for (float t = 0.0f; t <= rotateToPeelDuration * 0.5f; t += Time.deltaTime)
        {
            finalArtObj.transform.rotation = Quaternion.Lerp(finalArtPeel.rotation, finalArtVertical.rotation, t / (rotateToPeelDuration * 0.5f));
            yield return null;
        }
        finalArtObj.transform.rotation = finalArtVertical.rotation;

        //SetupVertexPoints(finalArtMeshFilter.mesh);

        yield return new WaitForSeconds(0.268f);
        itemRotator.SetPositionAndRotation(itemStickPos.position, itemStickPos.rotation);
        itemAnim.SetTrigger("Appear");

        StartCoroutine(FadeFinalArt_());

        CharacterManager.canTouch = true;
    }

    IEnumerator WaitingForPeeling_()
    {
        while (brokenNodeCount < activeNodes.Count)
            yield return null;

        StartCoroutine(RotateToStick_());
    }

    public void SetColorViaUI()
    {
        int id = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        if (id != currentColorID)
            SelectColor(id);
    }
    void SelectColor(int id_)
    {
        strokeMat.color = colors[id_];
        colorBtnAnims[currentColorID].SetTrigger("Deselect");
        colorBtnAnims[id_].SetTrigger("Select");
        currentColorID = id_;
    }
    IEnumerator ResetColors_()
    {
        yield return null;
        yield return null;
        currentColorID = 0;
        strokeMat.color = colors[0];
        colorBtnAnims[0].SetTrigger("Select");
        for (int i = 1; i < colorBtnAnims.Length; i++)
            colorBtnAnims[i].SetTrigger("Deselect");
    }

    IEnumerator BendCutoutBG_()
    {
        yield return new WaitForSeconds(cutoutBGBendDeformerDelay);

        bool rotated = false;
        for (float t = 0.0f; t <= cutoutBGBendDeformerMoveDuration + 0.128f; t += Time.deltaTime)
        {
            cutoutBGBendDeformer.transform.position = Vector3.Lerp(cutoutBGBendDeformerStartPos.position, cutoutBGBendDeformerStopPos.position, t / cutoutBGBendDeformerMoveDuration);
            cutoutBGBendDeformer.Angle = Mathf.Lerp(0.0f, cutoutBGBendDeformerAngle, t / cutoutBGBendDeformerRollDuration);

            if (!rotated && t >= cutoutBGBendDeformerRollDuration)
            {
                rotated = true;
                StartCoroutine(RotateFinalArt_());
            }

            yield return null;
        }
    }

    Mesh CreateWrapMesh(Mesh mesh, float push)
    {
        Mesh wrapMesh = new();
        Vector3[] vertices = new Vector3[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
            vertices[i] = mesh.vertices[i] + push * mesh.normals[i];
        wrapMesh.SetVertices(vertices);
        wrapMesh.SetTriangles(mesh.triangles, 0);
        wrapMesh.SetNormals(mesh.normals);
        return wrapMesh;
    }

    public void SelectAnItem()
    {
        Mesh itemMesh = itemMeshes[Random.Range(0, 1000) % itemMeshes.Length];
        itemMeshFilter.mesh = itemMesh;
        itemMeshCollider.sharedMesh = itemMesh;
        itemMeshFilter.transform.localRotation = Quaternion.identity;
        CalculateNewItemCustomerPos();
    }

    IEnumerator ShowCustomerAndItem_()
    {
        itemRotator.SetPositionAndRotation(itemCustomerPos.position, itemCustomerPos.rotation);
        itemAnim.SetTrigger("Appear");
        yield return new WaitForSeconds(customerGreetingDelay);
        customerAnim.SetTrigger("Greet");
        Managers.sceneManager.ShowUI(Managers.sceneManager.btnStartColoringCanvasGroup, 0.468f);
        Managers.sceneManager.ShowUI(Managers.sceneManager.orderCanvasGroup, 0.0f);
    }

    IEnumerator GiveItemToTheCustomer_()
    {
        stickerMeshFilter.mesh = null;

        for (float t = 0.0f; t <= Managers.cameraManager.transformTime + 0.128f; t += Time.deltaTime)
        {
            if (!jointSpringDebug)
                Camera.main.transform.SetPositionAndRotation(Vector3.Lerp(Managers.cameraManager.stickCam.position, Managers.cameraManager.startCam.position, t / Managers.cameraManager.transformTime),
                    Quaternion.Lerp(Managers.cameraManager.stickCam.rotation, Managers.cameraManager.startCam.rotation, t / Managers.cameraManager.transformTime));
            else
                Camera.main.transform.SetPositionAndRotation(Vector3.Lerp(Managers.cameraManager.stickCam.position, Managers.cameraManager.startCamDebug.position, t / Managers.cameraManager.transformTime),
                    Quaternion.Lerp(Managers.cameraManager.stickCam.rotation, Managers.cameraManager.startCamDebug.rotation, t / Managers.cameraManager.transformTime));
            itemRotator.SetPositionAndRotation(Vector3.Lerp(itemStickPos.position, itemCustomerPos.position, t / Managers.cameraManager.transformTime),
                Quaternion.Lerp(itemStickPos.rotation, itemCustomerPos.rotation, t / Managers.cameraManager.transformTime));
            yield return null;
        }

        itemAnim.SetTrigger("Rotate");

        yield return new WaitForSeconds(0.468f);

        if (selectedLineArtName.Contains(currentCategory))
            customerAnim.SetTrigger("Done");
        else
            customerAnim.SetTrigger("Disappoint");

        Managers.levelManager.FinishLevel();
    }

    IEnumerator FadeFinalArt_()
    {
        CharacterManager.canTouch = false;

        SetupLineRenderers(finalArtMeshFilter.transform, Color.white);

        finalArtMeshRenderer.material = finalArtMat_Transparent;
        for (float t = 0.0f; t <= finalArtFadeDuration + 0.128f; t += Time.deltaTime)
        {
            finalArtMat_Transparent.color = Color.Lerp(Color.white, finalArtFadeColor, t / finalArtFadeDuration);
            yield return null;
        }

        Managers.sceneManager.ShowUI(Managers.sceneManager.btnStickCanvasGroup, 0.468f);
    }

    IEnumerator MakeSureWeDidTriangulationRight_()
    {
        yield return new WaitForSeconds(triangulationCheckDelay);
        if (finalArtMeshFilter.mesh == null || finalArtMeshFilter.mesh.vertexCount == 0)
            StopDraw();
    }

    public void SelectArtPiece(int index_)
    {
        if (currentState == "SelectingArt")
        {
            currentState = "";
            selectedLineArtName = currentLineArtNames[index_];
            Managers.sceneManager.HideUI(Managers.sceneManager.selectArtworkTitleCanvasGroup);
            StartCoroutine(SelectArtPiece_(index_));
        }
    }
    IEnumerator SelectArtPiece_(int index)
    {
        for (int i = 0; i < artPieces.Length; i++)
        {
            if (i != index)
                artPieces[i].transform.localScale = Vector3.zero;
        }

        for (float t = 0.0f; t <= artSelectScaleUpDuration + 0.128f; t += Time.deltaTime)
        {
            artPieces[index].transform.SetPositionAndRotation(Vector3.Lerp(artPieces[index].transform.position, artPieceSelectedPos.position, t / artSelectScaleUpDuration),
                Quaternion.Lerp(artPieces[index].transform.rotation, artPieceSelectedPos.rotation, t / artSelectScaleUpDuration));
            artPieces[index].transform.localScale = Vector3.Lerp(artPieces[index].transform.localScale, artPieceSelectedPos.localScale, t / artSelectScaleUpDuration);
            yield return null;
        }

        artSelectingModule.SetActive(false);
        SetLineArt(index);
    }

    void CalculateNewItemCustomerPos()
    {
        itemCustomerPos.position += (tableSurface.position.y + itemMeshFilter.mesh.bounds.size.z * 0.5f - itemCustomerPos.position.y) * Vector3.up;
    }

    public void ToggleJointSpring()
    {
        jointSpringDebug = jointSpringToggle.isOn;
    }
}
