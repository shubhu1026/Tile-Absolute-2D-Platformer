using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D enemyRigidBody;
    BoxCollider2D turnCollider;
    int health = 100;

    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        turnCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        enemyRigidBody.velocity = new Vector2(moveSpeed, 0);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        moveSpeed *= -1;
        FlipEnemySprite();
    }

    void FlipEnemySprite()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        enemyRigidBody.velocity = new Vector2(moveSpeed, 0);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Bullet")
        {
            health -= 50;
            Destroy(other.gameObject);
        }
    }
}
