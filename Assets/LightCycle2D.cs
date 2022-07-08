using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightCycle2D : MonoBehaviour
{
    public float speed = 1.0f;
    public Color color = Color.gray;
    public Transform wayPointPrefab;
    public Transform trailPrefab;
    public Transform currentTrail;
    public Direction initialDirection = Direction.Right;
    public Direction currentDirection = Direction.None;
    public bool isLocal;
    public Player owner;

    public Transform lastWayPoint;
    public Transform lastWayPointTrail;
    public int lastWayPointFrameCount = 0;
    public List<Transform> wayPoints = new List<Transform>();
    public List<Transform> trails = new List<Transform>();

    private ParticleSystem particle;
    private new SpriteRenderer renderer;
    private Light light;

    private bool shouldMuteki = true;


    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<SpriteRenderer>();
        
        // 色をセットする。
        renderer.color = color;
        var pmain = particle.main;
        pmain.startColor = color;
        light = transform.Find("Light").GetComponent<Light>();
        light.color = color;

        currentTrail = Instantiate(trailPrefab);
        currentTrail.GetComponent<SpriteRenderer>().color = color;
        trails.Add(currentTrail);
        currentTrail.name = nameof(currentTrail);
        ChangeDirection(initialDirection);

        StartCoroutine(SetMutekiOff());
    }

    IEnumerator SetMutekiOff()
    {
        yield return new WaitForSeconds(0.3f);
        shouldMuteki = false;
    }

    void Update()
    {
        // 機体を移動する。
        transform.Translate(speed * Time.deltaTime * Vector3.right);
        // 軌跡を伸ばす。
        LocateTrail(currentTrail, transform.position, lastWayPoint.position, trailPrefab.localScale);
    }

    public void Abort()
    {
        Destroy(gameObject);
        foreach (var wp in wayPoints) Destroy(wp.gameObject);
        foreach (var t in trails) Destroy(t.gameObject);
    }

    public void OnDead()
    {
        // 衝突を無効にする。
        GetComponent<CircleCollider2D>().enabled = false;
        foreach (var t in trails)
        {
            t.GetComponent<BoxCollider2D>().enabled = false;
        }

        particle.Play();
        speed = 0;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var initialLightRange = light.range;
        var targets = trails
            .Concat(wayPoints)
            .Concat(new[] { transform })
            .Select(t => t.GetComponent<SpriteRenderer>())
            .ToArray();

        var duration = 0.5f;
        var elapsed = 0.0f;
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            var ratio = elapsed / duration;
            // 光を弱める。
            light.range = Mathf.Lerp(initialLightRange, 0, ratio);

            // 機体と軌跡を徐々に透明にする。
            var alpha = Mathf.Lerp(1, 0, ratio);
            for (int i = 0; i < targets.Length; i++)
            {
                var c = targets[i].color;
                c.a = alpha;
                targets[i].color = c;
            }
        }
        // オブジェクトを消す。
        Abort();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ネットワーク機体なら、自己申告で接触処理を行うのでここでは処理しない。
        if (!isLocal)
        {
            return;
        }

        // 曲がった直後の場合は直前の軌跡に衝突するので無視する。
        var isLastTrail =
            lastWayPointTrail != null &&
            lastWayPointTrail.gameObject == collision.gameObject;
        if (isLastTrail && Time.frameCount - lastWayPointFrameCount < 60)
        {
            return;
        }

        // スポーン直後は衝突判定なしにする。
        if (shouldMuteki)
        {
            return;
        }

        // ローカル機体なので、とりあえずownerに伝えて後始末を行う。
        owner.OnLocalDead();
        OnDead();
    }

    public void ChangeDirection(Direction d)
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
        wp.GetComponent<SpriteRenderer>().color = color;
        wp.transform.position = position;
        wayPoints.Add(wp);
        lastWayPoint = wp;
        lastWayPointFrameCount = Time.frameCount;

        // 前のWayPointとの間にTrailを設置する。
        if (prevWp == null) return;

        var trail = Instantiate(trailPrefab);
        trail.GetComponent<SpriteRenderer>().color = color;
        trails.Add(trail);
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
}
