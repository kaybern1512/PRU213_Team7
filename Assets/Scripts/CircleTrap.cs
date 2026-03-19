using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTrap : MonoBehaviour
{
    public Transform diemA;
    public Transform diemB;

    public float tocDoDiChuyen = 3f;
    public float tocDoXoay = 200f;

    private Vector3 diemMucTieu;

    void Start()
    {
        diemMucTieu = diemA.position;
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            diemMucTieu,
            tocDoDiChuyen * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, diemMucTieu) < 0.1f)
        {
            // đổi mục tiêu
            if (diemMucTieu == diemA.position)
            {
                diemMucTieu = diemB.position;
            }
            else
            {
                diemMucTieu = diemA.position;
            }
        }
    }

    void Rotate()
    {
        transform.Rotate(0, 0, tocDoXoay * Time.deltaTime);
    }
}