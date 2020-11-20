using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{

    private List<GameObject> pointsPool;
    private List<int> targetPointsIndexes;
    private int awaitedPointIndex = -1;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Camera cam = null;

    [SerializeField]
    private Canvas canvas = null;

    [SerializeField]
    private GameObject Pool = null;

    [SerializeField]
    private GameObject Point = null;

    private LineRenderer line;

    [SerializeField]
    private Slider slider = null;    

    private bool sliderTouched = false;
    
    void Start()
    {

        pointsPool = new List<GameObject>();

        pointsPool.Add(Point);

        targetPointsIndexes = new List<int>();

        transform.position = new Vector2(0, 0);

        line = GetComponent<LineRenderer>();

        transform.localScale = canvas.transform.localScale;

    }

    private void Update()
    {        

        if (Input.touchCount == 1)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {

                if (sliderTouched)
                {

                    sliderTouched = false;

                    return;

                }

                Vector3 touchPosition = new Vector3();
                touchPosition.x = Input.touches[0].position.x;
                touchPosition.y = Input.touches[0].position.y;
                touchPosition.z = cam.nearClipPlane;

                awaitedPointIndex = GetPointIndex(touchPosition);                

                DrawLine();

            }
            else if (Input.touches[0].phase == TouchPhase.Moved)
            {

                if (awaitedPointIndex == -1)
                {

                    return;

                }

                Vector3 touchPosition = new Vector3();
                touchPosition.x = Input.touches[0].position.x;
                touchPosition.y = Input.touches[0].position.y;
                touchPosition.z = cam.nearClipPlane;

                pointsPool[awaitedPointIndex].transform.position = cam.ScreenToWorldPoint(touchPosition);

                DrawLine();

            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {

                if (awaitedPointIndex == -1)
                {

                    return;

                }

                targetPointsIndexes.Add(awaitedPointIndex);
                awaitedPointIndex = -1;

                DrawLine();

            }

        }
        else if (Input.GetMouseButtonDown(0))
        {

            if (sliderTouched)
            {

                sliderTouched = false;

                return;

            }

            Vector3 mousePosition = new Vector3();
            mousePosition.x = Input.mousePosition.x;
            mousePosition.y = Input.mousePosition.y;
            mousePosition.z = cam.nearClipPlane;

            awaitedPointIndex = GetPointIndex(mousePosition);            

            DrawLine();

        }
        else if (Input.GetMouseButton(0))
        {

            if (awaitedPointIndex == -1)
            {

                return;

            }

            Vector3 mousePosition = new Vector3();
            mousePosition.x = Input.mousePosition.x;
            mousePosition.y = Input.mousePosition.y;
            mousePosition.z = cam.nearClipPlane;

            pointsPool[awaitedPointIndex].transform.position = cam.ScreenToWorldPoint(mousePosition);

            DrawLine();

        }
        else if (Input.GetMouseButtonUp(0))
        {

            if (awaitedPointIndex == -1)
            {

                return;

            }

            targetPointsIndexes.Add(awaitedPointIndex);
            awaitedPointIndex = -1;

            DrawLine();

        }

    }

    void FixedUpdate()
    {

        if (targetPointsIndexes.Count > 0)
        {

            transform.position = Vector2.MoveTowards(transform.position, pointsPool[targetPointsIndexes[0]].transform.position, speed);

            if (transform.position.x == pointsPool[targetPointsIndexes[0]].transform.position.x 
                && transform.position.y == pointsPool[targetPointsIndexes[0]].transform.position.y)
            {

                pointsPool[targetPointsIndexes[0]].GetComponent<Point>().Reached();                
                targetPointsIndexes.RemoveAt(0);                                

            }

            DrawLine();

        }

    }

    private void DrawLine()
    {

        if (targetPointsIndexes.Count == 0)
        {

            if (awaitedPointIndex != -1)
            {

                line.positionCount = 2;

                line.SetPosition(0, transform.position);
                line.SetPosition(1, pointsPool[awaitedPointIndex].transform.position);

                return;

            }

            line.positionCount = 0;

            return;

        }        

        if (awaitedPointIndex != -1)
        {

            line.positionCount = targetPointsIndexes.Count + 2;

        }
        else
        {

            line.positionCount = targetPointsIndexes.Count + 1;

        }

        line.SetPosition(0, transform.position);

        for (int i = 1; i <= targetPointsIndexes.Count; ++i)
        {

            line.SetPosition(i, pointsPool[targetPointsIndexes[i - 1]].transform.position);

        }

        if (awaitedPointIndex != -1)
        {

            line.SetPosition(line.positionCount - 1, pointsPool[awaitedPointIndex].transform.position);

        }

    }

    public void ChangeSpeed()
    {

        speed = slider.value;

    }

    public void SliderPointerDown()
    {

        sliderTouched = true;

    }

    public int GetPointIndex(Vector3 mousePosition)
    {

        if (targetPointsIndexes.Count == pointsPool.Count)
        {

            pointsPool.Add(Instantiate(Point, cam.ScreenToWorldPoint(mousePosition), Quaternion.identity, Pool.transform));
            pointsPool.Last().GetComponent<Point>().IsEnable = true;

            return pointsPool.Count - 1;

        }

        for (int i = 0; i < pointsPool.Count; ++i)
        {

            if (!pointsPool[i].GetComponent<Point>().IsEnable)
            {

                pointsPool[i].GetComponent<Point>().Set(cam.ScreenToWorldPoint(mousePosition));

                return i;

            }

        }

        return 0;

    }

}
