using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Location targetLocation;

    Location myLocation;

    [SerializeField] float range;
    [SerializeField] float memoryTime;

    [SerializeField] bool targetVisible;
    bool forgetting = false;

    public bool TargetVisible { get => targetVisible; set => targetVisible = value; }
    public Transform Target { get => target; set => target = value; }
    public Location TargetLocation { get => targetLocation; set => targetLocation = value; }

    void Start()
    {
        myLocation = GetComponentInParent<Location>();
    }

    void Update()
    {
        if (IsTargetVisible())
        {
            if (forgetting)
            {
                StopCoroutine(WaitToForgetEnemy());
            }

            forgetting = false;
            TargetVisible = true;
        }
        else if (TargetVisible && !forgetting)
        {
            forgetting = true;
            StartCoroutine(WaitToForgetEnemy());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    bool IsTargetVisible()
    {
        if (Target == null)
        {
            return false;
        }

        if (TargetLocation == null)
        {
            return false;
        }

        if (!TargetLocation.RoomPosition.Equals(myLocation.RoomPosition))
        {
            return false;
        }

        Vector2 toTarget = Target.position - transform.position;
        if (toTarget.magnitude > range)
        {
            return false;
        }

        return true;
    }

    IEnumerator WaitToForgetEnemy()
    {
        forgetting = true;
        yield return new WaitForSeconds(memoryTime);

        TargetVisible = false;

        forgetting = false;
    }
}
