using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCycle2D : MonoBehaviour
{
    public float speed = 1.0f;
    public Transform wayPointPrefab;
    public Transform trailPrefab;
    public Transform currentTrail;
    public Direction initialDirection = Direction.Right;
    public Direction currentDirection = Direction.None;

    public Transform lastWayPoint;
    public Transform lastWayPointTrail;
    public int lastWayPointFrameCount = 0;
    public List<Transform> wayPoints = new List<Transform>();

    private ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();

        currentTrail = Instantiate(trailPrefab);
        currentTrail.name = nameof(currentTrail);
        ChangeDirection(initialDirection);
    }

    void Update()
    {
        UpdateDirection();

        transform.Translate(Vector3.right * speed * Time.deltaTime);
        LocateTrail(currentTrail, transform.position, lastWayPoint.position, trailPrefab.localScale);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 
        var isLastTrail = collision.gameObject == lastWayPointTrail.gameObject;
        if (isLastTrail && Time.frameCount - lastWayPointFrameCount < 60)
        {
            return;
        }
        Debug.Log(Time.frameCount - lastWayPointFrameCount);

        particle.Play();
    }

    private void UpdateDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeDirection(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeDirection(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) ChangeDirection(Direction.Right);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangeDirection(Direction.Left);
    }
    private void ChangeDirection(Direction d)
    {
        if (d == currentDirection) return;

        var rotation = d switch
        {
            Direction.Up => new Vector3(0, 0, 90),
            Direction.Down => new Vector3(0, 0, -90),
            Direction.Left => new Vector3(0, 0, -180),
            Direction.Right => new Vector3(0, 0, 0),
            _ => throw new System.NotImplementedException(),
        };

        currentDirection = d;
        transform.eulerAngles = rotation;
        PutWayPoint(transform.position);
    }

    private void PutWayPoint(Vector3 position)
    {
        var prevWp = lastWayPoint;

        // WayPointを設置する。
        var wp = Instantiate(wayPointPrefab);
        wp.transform.position = position;
        wayPoints.Add(wp);
        lastWayPoint = wp;
        lastWayPointFrameCount = Time.frameCount;

        // 前のWayPointとの間にTrailを設置する。
        if (prevWp == null) return;

        var trail = Instantiate(trailPrefab);
        LocateTrail(trail, prevWp.position, wp.position, trail.localScale);
        lastWayPointTrail = trail;
    }

    private static void LocateTrail(Transform trail, Vector3 wp1, Vector3 wp2, Vector3 defaultScale)
    {
        var distance = wp1 - wp2;
        trail.transform.position = wp1 - distance / 2;
        var scale = defaultScale;
        if (Mathf.Abs(distance.x) > 0.001) scale.x = distance.x;
        if (Mathf.Abs(distance.y) > 0.001) scale.y = distance.y;
        if (Mathf.Abs(distance.z) > 0.001) scale.z = distance.z;
        trail.transform.localScale = scale;
    }

    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }
}
