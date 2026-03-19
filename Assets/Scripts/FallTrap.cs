using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallTrap : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool daRoi = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // ban đầu đứng yên
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !daRoi)
        {
            rb.isKinematic = false; // bắt đầu rơi
            daRoi = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // player chết
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}