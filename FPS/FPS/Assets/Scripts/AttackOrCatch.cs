using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOrCatch : MonoBehaviour
{
    /*
    GameObject prefab;
    


    public Vector3 shootStartPoint;
    PlayerMove playerMove;

    public Vector3 vo;

    private void Start()
    {
        //playerMove = GetComponent<PlayerMove>();
    }

    /*private void Update()
    {
        if(playerMove.attack == true)
        {
            //shootStartPoint = transform.position + Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 0, 0.6f) + new Vector3(0, 0.3f, 0);
            //vo = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * playerMove.shootVelocity;
        }
    }*/

    public void ShootAttack(Vector3 shootStartPoint, float shootVelocity)
    {
        GameObject prefab = Resources.Load("projectile") as GameObject;
        GameObject projectile = Instantiate(prefab) as GameObject;
        projectile.transform.position = shootStartPoint;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * shootVelocity;
    }

    public void ShootCatch(Vector3 shootStartPoint, float shootVelocity)
    {
        GameObject prefab = Resources.Load("catchProjectile") as GameObject;
        GameObject projectile = Instantiate(prefab) as GameObject;
        projectile.transform.position = shootStartPoint;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(0, 1, 1) * shootVelocity;
    }
    /*public Vector3 CalculatePosInTime(Vector3 vo, float time)
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;

        Vector3 result = shootStartPoint + Vxz * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + shootStartPoint.y;

        result.y = sY;
        return result;
    }

    public void VisualizeLine(Vector3 vo)
    {
        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(vo, i / (float)lineSegment);
            lineVisual.SetPosition(i, pos);
        }
    }*/
}
