using System;
using UnityEngine;

public abstract class Character
{
    public int hp { get; set; }
    public float speed { get; set; }

    public Rigidbody2D body { get; set; }
    public Animator animator { get; set; }
    public BoxCollider2D boxCollider2D { get; set; }
    public Transform transform { get; set; }
    public float horizontalInput { get; set; }

    public Character(GameObject gameObject)
    {
        hp = 3;
        body = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        transform = gameObject.transform;
    }
    internal void Update()
    {
        InputHandle();
        //FlipSprite();
    }

    //private void FlipSprite()
    //{
    //    throw new NotImplementedException();
    //}

    protected abstract void InputHandle();

}
