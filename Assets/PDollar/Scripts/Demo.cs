using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using UnityEngine.Networking;
using System.Linq.Expressions; // add at top



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

        [SerializeField] private Symbol[] symbols; 

        [SerializeField] private bool writingNewGestures = false;

        private EnemyType enemyType = EnemyType.None;
        private Color currentColor = Color.white;

        [SerializeField] private GameObject sphere;
        [SerializeField] private GameObject fromObjectTest;

        private List<Enemy> spawnedEnemies;
        public void AddEnemy(Enemy enemy) => spawnedEnemies.Add(enemy);

        void Start()
        {
            spawnedEnemies = new List<Enemy>();
            platform = Application.platform;
            


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
                        trainingSet.Add(GestureIO.ReadGestureFromXML(xmlContent));//
                    }
                }
            }

            // Create and check custom gestures folder
            string customGesturesFolder = Path.Combine(Application.persistentDataPath, "CustomGestures");
            if (!Directory.Exists(customGesturesFolder))
            {
                Directory.CreateDirectory(customGesturesFolder);
                
            }
print("Loaded user gesture: " + customGesturesFolder);
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

            drawArea = new Rect(0, 0, Screen.width, Screen.height);

            if (writingNewGestures)
                drawArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);

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

            if (recognized)
            {
                recognized = false;
                strokeId = -1;

                points.Clear();

                foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                {

                    //lineRenderer.positionCount = 0;
                    lineRenderer.gameObject.GetComponent<DisappearLine>().Disappear();
                }

                gestureLinesRenderer.Clear();
            }

            if (drawArea.Contains(virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {


                    ++strokeId;

                    Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
 
                    currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();
                    currentGestureLineRenderer.material.color = currentColor;

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



                Symbol currSym = null;

                foreach (Symbol sym in symbols)
                {
                    if (sym.name == gestureResult.GestureClass)
                    {
                        currSym = sym;
                        break;
                    }
                }

                if (gestureResult.Score < 0.8f || currSym == null && gestureResult.GestureClass != "Circle")
                {
                    message = "Gesture not recognized";
                    foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                    {
                        lineRenderer.material.color = Color.red;
                    }
                    return;
                }

                if (currSym != null)
                {
                    enemyType = currSym.killsThese;
                    currentColor = currSym.color;

                    foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                    {
                        lineRenderer.material.color = currentColor;
                    }

                }

                if(gestureResult.GestureClass != "Circle")
                {
                    return;
                }


                int pointCount = 0;
                foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                {
                    pointCount += lineRenderer.positionCount;

                }


                Vector2[] positions = new Vector2[pointCount];

                int currentInitialPos = 0;

                List<Vector3> points3DList = new List<Vector3>();

                Vector3 intersectionPoint = Vector3.zero;

                foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                {
                    int lineRendererPointCount = lineRenderer.positionCount;
                    for (int i = currentInitialPos; i < lineRendererPointCount + currentInitialPos; i++)
                    {
                        Vector3 currPos = lineRenderer.GetPosition(i - currentInitialPos);
                        //Instantiate(sphere, currPos, Quaternion.identity);

                        print("Distance: " + GetDistanceToCameraPlane(lineRenderer.bounds.center, Camera.main));
                        print("Tried to get position: " + (i - currentInitialPos));

                        points3DList.Add(currPos);
                    }

                    currentInitialPos += lineRendererPointCount;
                }

                DestroyEnemies(spawnedEnemies, points3DList);

                //bool isInside = IsPointInPolygon(points3DList.ToArray(), intersectionPoint);
                //print("Object inside: " + isInside);




                //CreateMeshFrom3DPoints(points3DList.ToArray());


                print("Final current initial pos: " + currentInitialPos);


                

            }
        }

        private void DestroyEnemies(List<Enemy> enemies, List<Vector3> points3DList)
        {
            List<Enemy> enemiesToDestroy = new List<Enemy>();
            for (int i = 0; i < enemies.Count; i++)
            {
                print("Enemy: " + enemies[i].name);
                if (enemies[i].enemyType != enemyType)
                {
                    continue;
                }

                if (IsPointInPolygon(points3DList.ToArray(), SpawnSphereOnOffsetCameraPlane(enemies[i].gameObject, 10f)))
                {
                    print("Enemy destroyed: " + enemies[i].name + " at position: " + enemies[i].transform.position);
                    enemiesToDestroy.Add(enemies[i]);


                }
            }

            foreach (Enemy enemy in enemiesToDestroy)
            {
                spawnedEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }

        }

        float GetDistanceToCameraPlane(Vector3 worldPosition, Camera camera)
        {
            Vector3 toPoint = worldPosition - camera.transform.position;
            float distance = Vector3.Dot(toPoint, camera.transform.forward);
            return distance;
        }

        public Vector3 SpawnSphereOnOffsetCameraPlane(GameObject fromObject, float distanceFromCamera = 10f)
        {
            Vector3 origin = fromObject.transform.position;
            Vector3 direction = (GetComponent<Camera>().transform.position - origin).normalized;

            // Plane normal is the same as the camera’s forward vector
            Vector3 planeNormal = GetComponent<Camera>().transform.forward;

            // Plane point is 10 units in front of the camera
            Vector3 planePoint = GetComponent<Camera>().transform.position + planeNormal * distanceFromCamera;

            // Define the plane
            Plane plane = new Plane(planeNormal, planePoint);

            // Ray from the object toward the camera
            Ray ray = new Ray(origin, direction);

            GameObject tempSphere = null;

            Vector3 finalIntersectionPoint = Vector3.zero;

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 intersectionPoint = ray.GetPoint(distance);
                finalIntersectionPoint = intersectionPoint;
                tempSphere = Instantiate(sphere, intersectionPoint, Quaternion.identity);
            }
            else
            {
                print("No intersection with the camera's offset plane.");
            }

            return finalIntersectionPoint;
        }


        void CreateMeshFrom3DPoints(Vector3[] points3D)
        {
            // Project points onto a 2D plane for triangulation (e.g., XY)
            Vector2[] points2D = new Vector2[points3D.Length];
            for (int i = 0; i < points3D.Length; i++)
            {
                points2D[i] = new Vector2(points3D[i].x, points3D[i].y); // You can change projection plane here
            }

            // Triangulate the 2D shape
            Triangulator triangulator = new Triangulator(points2D);
            int[] indices = triangulator.Triangulate();
            print("Generated indices: " + indices.Length);

            // Create the mesh using the original 3D positions
            Mesh mesh = new Mesh();
            mesh.vertices = points3D;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Create GameObject to hold the mesh
            GameObject meshObject = new GameObject("GestureMesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(Rigidbody));

            meshObject.GetComponent<Rigidbody>().isKinematic = true;
            meshObject.GetComponent<MeshFilter>().mesh = mesh;
            meshObject.GetComponent<MeshRenderer>().material = meshMaterial;
            meshObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            meshObject.GetComponent<MeshCollider>().convex = true;
            meshObject.GetComponent<MeshCollider>().isTrigger = true;

            // Optional: Calculate direction from camera to mesh for movement
            Vector3 directionFromCamera = (meshObject.transform.position - Camera.main.transform.position).normalized;


            // Optional: Add movement script
            // MoveForwardForSeconds mover = meshObject.AddComponent<MoveForwardForSeconds>();
            // mover.moveDirection = directionFromCamera;
        }

        public bool IsPointInPolygon(Vector3[] polygonPoints, Vector3 testPoint)
        {
            if (polygonPoints.Length < 3)
                return false;

            // Step 1: Define the polygon plane
            Plane plane = new Plane(polygonPoints[0], polygonPoints[1], polygonPoints[2]);

            // Step 2: Project the testPoint and polygon points onto the plane
            Vector3 projectedTestPoint = plane.ClosestPointOnPlane(testPoint);

            // Step 3: Choose a projection axis to flatten the plane to 2D (e.g., XY, XZ, or YZ)
            // We'll project to the best fitting plane by comparing normal
            Vector3 normal = plane.normal;
            Vector2[] polygon2D = new Vector2[polygonPoints.Length];
            Vector2 point2D;

            // Project onto dominant plane axis (drop smallest component of normal)
            if (Mathf.Abs(normal.z) > Mathf.Abs(normal.x) && Mathf.Abs(normal.z) > Mathf.Abs(normal.y))
            {
                // Project to XY
                for (int i = 0; i < polygonPoints.Length; i++)
                    polygon2D[i] = new Vector2(polygonPoints[i].x, polygonPoints[i].y);
                point2D = new Vector2(projectedTestPoint.x, projectedTestPoint.y);
            }
            else if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            {
                // Project to YZ
                for (int i = 0; i < polygonPoints.Length; i++)
                    polygon2D[i] = new Vector2(polygonPoints[i].y, polygonPoints[i].z);
                point2D = new Vector2(projectedTestPoint.y, projectedTestPoint.z);
            }
            else
            {
                // Project to XZ
                for (int i = 0; i < polygonPoints.Length; i++)
                    polygon2D[i] = new Vector2(polygonPoints[i].x, polygonPoints[i].z);
                point2D = new Vector2(projectedTestPoint.x, projectedTestPoint.z);
            }

            // Step 4: Perform point-in-polygon test (ray-casting algorithm)
            return IsPointInPolygon2D(polygon2D, point2D);
        }

        private bool IsPointInPolygon2D(Vector2[] polygon, Vector2 point)
        {
            bool inside = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if ((polygon[i].y > point.y) != (polygon[j].y > point.y) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) /
                    (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
                j = i;
            }
            return inside;
        }


        void OnGUI()
        {
            //GUI.Box(drawArea, "Draw Area");

            GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

            //if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
            //{
            //    recognized = true;
            //    points2 = points.ToArray();
            //    Gesture candidate = new Gesture(points.ToArray());
            //    Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            //    message = gestureResult.GestureClass + " " + gestureResult.Score;
            //    print("message: " + message + " with: " + points.Count + " number of points: " + points[0].X + " " + points[0].Y);
            //}
            //else
            //if (Input.GetMouseButtonDown(1))
            //{
            //    recognized = true;
            //    points2 = points.ToArray();
            //    Gesture candidate = new Gesture(points.ToArray());

            //    Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            //    message = gestureResult.GestureClass + " " + gestureResult.Score;
            //    print("message: " + message + " with: " + points.Count + " number of points: " + points[0].X + " " + points[0].Y);
            //}



            if(writingNewGestures)
            {
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
}