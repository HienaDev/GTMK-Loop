using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using UnityEngine.Networking; // add at top



namespace PDollarGestureRecognizer
{
    public class Demo : MonoBehaviour
    {
        public Transform gestureOnScreenPrefab;

        private List<Gesture> trainingSet = new List<Gesture>();

        private List<Point> points = new List<Point>();
        private Point[] points2 = new Point[500];
        private int strokeId = -1;

        private Vector3 virtualKeyPosition = Vector2.zero;
        private Rect drawArea;

        private RuntimePlatform platform;
        private int vertexCount = 0;

        private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
        private LineRenderer currentGestureLineRenderer;

        //GUI
        private string message;
        private bool recognized;
        private string newGestureName = "";

        public Material meshMaterial;

        [SerializeField] private bool writingNewGestures = false;

        void Start()
        {
            platform = Application.platform;
            drawArea = new Rect(0, 0, Screen.width, Screen.height);
            drawArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);

            // Load pre-made gestures from StreamingAssets
            string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "Gestures");
            print("path " + streamingAssetsPath);//
            if (Application.platform == RuntimePlatform.Android)
            {
                // On Android, StreamingAssets is inside a compressed APK, so use UnityWebRequest to read files.
                StartCoroutine(LoadGesturesFromStreamingAssets(streamingAssetsPath));
            }
            else
            {
                if (Directory.Exists(streamingAssetsPath))
                {
                    string[] files = Directory.GetFiles(streamingAssetsPath, "*.xml");
                    foreach (string file in files)
                    {
                        string xmlContent = File.ReadAllText(file);
                        trainingSet.Add(GestureIO.ReadGestureFromXML(xmlContent));
                    }
                }
            }

            // Create and check custom gestures folder
            string customGesturesFolder = Path.Combine(Application.persistentDataPath, "CustomGestures");
            if (!Directory.Exists(customGesturesFolder))
            {
                Directory.CreateDirectory(customGesturesFolder);
            }

            // Load user custom gestures from custom folder
            string[] customGestureFiles = Directory.GetFiles(customGesturesFolder, "*.xml");
            foreach (string filePath in customGestureFiles)
            {
                trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
                print("Loaded user gesture: " + filePath);
            }



        }


        IEnumerator LoadGesturesFromStreamingAssets(string path)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                print("Failed to load gestures from StreamingAssets: " + www.error);
            }
            else
            {
                string[] files = Directory.GetFiles(path, "*.xml");
                foreach (string file in files)
                {
                    string xmlPath = Path.Combine(path, Path.GetFileName(file));
                    UnityWebRequest fileRequest = UnityWebRequest.Get(xmlPath);
                    yield return fileRequest.SendWebRequest();

                    if (fileRequest.result == UnityWebRequest.Result.Success)
                    {
                        string xmlContent = fileRequest.downloadHandler.text;
                        trainingSet.Add(GestureIO.ReadGestureFromXML(xmlContent));
                    }
                }
            }
        }

        void Update()
        {

            if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                }
            }

            if (drawArea.Contains(virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (recognized)
                    {
                        recognized = false;
                        strokeId = -1;

                        points.Clear();

                        foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                        {

                            lineRenderer.positionCount = 0;
                            Destroy(lineRenderer.gameObject);
                        }

                        gestureLinesRenderer.Clear();
                    }

                    ++strokeId;

                    Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
                    currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                    gestureLinesRenderer.Add(currentGestureLineRenderer);

                    vertexCount = 0;
                }

                if (Input.GetMouseButton(0))
                {
                    points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                    currentGestureLineRenderer.positionCount = ++vertexCount;
                    currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                recognized = true;

                points2 = points.ToArray();
                Gesture candidate = new Gesture(points.ToArray());
                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

                message = gestureResult.GestureClass + " " + gestureResult.Score;
                print("message3: " + message + " with: " + points.Count + " number of points: " + points[0]);

                PolygonCollider2D col = currentGestureLineRenderer.GetComponent<PolygonCollider2D>();
                print(col);
                int pointCount = 0;
                foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                {
                    pointCount += lineRenderer.positionCount;

                }


                Vector2[] positions = new Vector2[pointCount];

                int currentInitialPos = 0;

                foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                {
                    int lineRendererPointCount = lineRenderer.positionCount;
                    for (int i = currentInitialPos; i < lineRendererPointCount + currentInitialPos; i++)
                    {
                        Vector3 currPos = lineRenderer.GetPosition(i - currentInitialPos);
                        print("Tried to get position: " + (i - currentInitialPos));
                        positions[i] = new Vector2(currPos.x, currPos.y);

                    }

                    currentInitialPos += lineRendererPointCount;
                }

                print("Final current initial pos: " + currentInitialPos);


                CreateMeshFromPoints(positions);

                col.points = positions;
            }
        }

        void CreateMeshFromPoints(Vector2[] points2D)
        {
            // Triangulate the 2D shape
            Triangulator triangulator = new Triangulator(points2D);
            int[] indices = triangulator.Triangulate();

            // Convert Vector2 to Vector3 (flat on XY plane)
            Vector3[] vertices = new Vector3[points2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(points2D[i].x, points2D[i].y, 0);
            }

            // Create the mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Create GameObject
            GameObject meshObject = new GameObject("GestureMesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(Rigidbody));
            meshObject.GetComponent<Rigidbody>().isKinematic = true; // optional, if you don't want physics to affect it	
            meshObject.GetComponent<MeshFilter>().mesh = mesh;
            meshObject.GetComponent<MeshRenderer>().material = meshMaterial;
            meshObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            meshObject.GetComponent<MeshCollider>().convex = true; // optional
            meshObject.GetComponent<MeshCollider>().isTrigger = true; // optional, if you want it to be a trigger
            meshObject.AddComponent<MoveForwardForSeconds>();

            meshObject.transform.localScale *= 1.2f;
        }

        void OnGUI()
        {
            //GUI.Box(drawArea, "Draw Area");

            GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

            if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
            {
                recognized = true;
                points2 = points.ToArray();
                Gesture candidate = new Gesture(points.ToArray());
                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

                message = gestureResult.GestureClass + " " + gestureResult.Score;
                print("message: " + message + " with: " + points.Count + " number of points: " + points[0].X + " " + points[0].Y);
            }
            else
            if (Input.GetMouseButtonDown(1))
            {
                recognized = true;
                points2 = points.ToArray();
                Gesture candidate = new Gesture(points.ToArray());

                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

                message = gestureResult.GestureClass + " " + gestureResult.Score;
                print("message: " + message + " with: " + points.Count + " number of points: " + points[0].X + " " + points[0].Y);
            }

            GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
            newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

            if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
            {
                string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
                GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

                trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

                newGestureName = "";

                points.Clear();
            }
        }
    }
}