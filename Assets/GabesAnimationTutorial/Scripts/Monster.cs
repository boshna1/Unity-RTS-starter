using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{

    //3 states: Patrolling, Moving, Attacking
    //Moving overrides patrolling and attacking.

    private Animator myAnimator;
    private NavMeshAgent ai;

    private const int IdleAnims = 2;

    [SerializeField] private float maxHealth;
    private float health;
    private Coroutine stateRoutine;
    [SerializeField] private Rigidbody fireBallPrefab;
    [SerializeField] private Transform Firepoint;

    private Vector3 LookAtPoint;

    private EAiState aiState = EAiState.Idle;
    [SerializeField] private float boredTimer = 1;

    private enum EAiState
    {
        Idle = 1, 
        Wander = 2, 
        CommandMove = 4,
        CommandAttack = 8
    
    }

    private WaitForSeconds IdleTimer;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        ai = GetComponent<NavMeshAgent>();
        health = maxHealth;
        IdleTimer = new WaitForSeconds(boredTimer);
        EnterIdle();
    }

    private IEnumerator Idle()
    {
        yield return IdleTimer;
        
        if (aiState == EAiState.Idle)
        {
            bool findloc;
            RaycastHit hit;
            do
            {
                Vector3 randomSphere = (Vector3)(Random.insideUnitCircle * 10);
                randomSphere.z = randomSphere.y;
                randomSphere.y = 1000;
                findloc = Physics.Raycast(randomSphere, -Vector3.up, out hit, 2000, StaticUtilities.GroundLayer);
            }
            while (!findloc);

            MoveToTarget(hit.point);
            aiState = EAiState.Wander;
        }
    }

    private void EnterIdle()
    {
        aiState = EAiState.Idle;
        StopRotating();

        stateRoutine = StartCoroutine(Idle());
        
    }

    private void Update()
    {
        Vector3 velocity = transform.InverseTransformVector(ai.velocity);
        
        myAnimator.SetFloat(StaticUtilities.XSpeedAnimId, velocity.x);
        myAnimator.SetFloat(StaticUtilities.YSpeedAnimId, velocity.z);

        if ((aiState & (EAiState.Idle | EAiState.CommandAttack)) == 0 && ai.remainingDistance <= ai.stoppingDistance)
        {
            EnterIdle();
        }
    }

    public void MoveToTarget(Vector3 hitInfoPoint)
    {
        aiState = EAiState.CommandMove;
        ai.SetDestination(hitInfoPoint);
        ai.isStopped = false;
    }

    public void ChangeIdleState()
    {
        int rngIndex = Random.Range(0, 2);
        myAnimator.SetFloat(StaticUtilities.IdleAnimId, rngIndex);
    }

    public  void TryAttack(RaycastHit hitObject)
    {
        //rotate to face object
        ai.isStopped = true;
        aiState = EAiState.CommandAttack;
        StopRotating();
        LookAtPoint = (hitObject.point - Firepoint.position).normalized;
        stateRoutine = StartCoroutine(RotateToTarget());
        
    }

    private IEnumerator RotateToTarget()
    {
        float angle;
        
        myAnimator.SetBool(StaticUtilities.IsTurningAnimId, true);
        do
        {
            angle = Vector3.Dot(transform.right, LookAtPoint);
            myAnimator.SetFloat(StaticUtilities.TurnAnimId, angle);
            yield return null;
        } while (Mathf.Abs(angle) >= 0.01f);
        myAnimator.SetBool(StaticUtilities.IsTurningAnimId, false);
        stateRoutine = null;
        Attack();
        

    }

    private void StopRotating()
    {
        if (stateRoutine != null) StopCoroutine(stateRoutine);
        myAnimator.SetBool(StaticUtilities.IsTurningAnimId, false);
    }
    private void Attack()
    {
        myAnimator.SetTrigger(StaticUtilities.AttackAnimId);

    }

    public void SpawnFireBall()
    {
        
        Rigidbody projectile = Instantiate(fireBallPrefab, Firepoint.position, Quaternion.identity);
        projectile.AddForce(LookAtPoint * 1, ForceMode.Impulse);
        EnterIdle();
    }
}
