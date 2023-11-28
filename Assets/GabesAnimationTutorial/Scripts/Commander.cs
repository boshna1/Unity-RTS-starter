using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Commander : MonoBehaviour
{

    [SerializeField] public Monster[] controlledMonsters = new Monster[20];
    
    private float _rotationDirection;
    private Vector3 _moveDirection;
    private bool enableSpawner;
    public Button button;
    public GameObject Spawner;
    bool MouseSpawned;
    GameObject cursorSpawner;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Initialize(this);
    }

    private void Update()
    {
        Ray hit = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Transform myTransCached  = transform;
        myTransCached.Rotate(Vector3.up,Time.deltaTime *   Settings.Instance.MouseRotateSens * _rotationDirection );
        myTransCached.position += transform.rotation * (Time.deltaTime * Settings.Instance.MouseMoveSense * _moveDirection);
        Physics.Raycast(hit, out RaycastHit hitObject, 100,StaticUtilities.AttackLayerMask);
        Debug.DrawRay(hit.origin,hit.direction * 100,Color.yellow,1);
        if (enableSpawner == true && MouseSpawned == false)
        {
            cursorSpawner = Instantiate(Spawner, hit.origin, Quaternion.identity);
            MouseSpawned = true;
        }
        if (enableSpawner == true && MouseSpawned == true)
        {
            cursorSpawner.transform.position = hitObject.point;
            if (Input.GetMouseButtonDown(0))
            {
                cursorSpawner.transform.position = hitObject.point;
                enableSpawner = false;
                MouseSpawned = false;
            }
        }
        
    }

    public void Attack(Ray camToWorldRay)
    {
        Debug.DrawRay(camToWorldRay.origin, camToWorldRay.direction * 100, Color.red,1);

        if (!Physics.Raycast(camToWorldRay, out RaycastHit hitObject, 100, StaticUtilities.AttackLayerMask)) return;

        foreach (Monster monster in controlledMonsters) 
        {
            monster.TryAttack(hitObject);
        }
    }

    public void MoveTo(Ray camToWorldRay)
    {
        Debug.DrawRay(camToWorldRay.origin, camToWorldRay.direction* 100, Color.blue,1);

        if (!Physics.Raycast(camToWorldRay, out RaycastHit hit, 100, StaticUtilities.MoveLayerMask))
            return;

        foreach (Monster monster in controlledMonsters)
        {
            monster.MoveToTarget(hit.point);
        }
        
    }
    
    
    public void SetRotationDirection(float direction)
    {
        _rotationDirection = direction;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        _moveDirection.x = direction.x;
        _moveDirection.z = direction.y;
    }

    public void enableAddSpawner()
    {
        enableSpawner = true;
        MouseSpawned = false;
    }

    public void addAlly(GameObject newMonster)
    {
        for (int i = 0; i < controlledMonsters.Length; i++) 
        {
            if (controlledMonsters[i] == null)
            {
                controlledMonsters[i] = newMonster.GetComponent<Monster>();
                break;
            }
        }
    }
}
