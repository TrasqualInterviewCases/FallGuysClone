using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [SerializeField]
    float followDistance = 18f;
    [SerializeField]
    float strafeSpeed = .1f;

    Vector3 targetPos;

    RaycastHit hit;

    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;

    }

    private void Update()
    {
        targetPos = new Vector3(target.position.x, transform.position.y, (target.position.z - followDistance));
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * strafeSpeed);
    }

    private void CheckForObjectsInTheWay()
    {
        //Vector3 rayStart = player.transform.position + Vector3.up * 0.8f;
        Vector3 rayEnd = new Vector3(target.position.x, transform.position.y, (target.position.z - followDistance));
        //Physics.Raycast(rayStart, transform.position, out hit, Mathf.Infinity);
        //if (hit.transform != null)
        //{
        //    Debug.LogWarning(hit.transform.name);
        //    targetPos = Vector3.Lerp(rayStart,hit.point,0.2f);
        //}
            targetPos = rayEnd;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(player.transform.position + Vector3.up * .8f, transform.position);
    //}
}
