using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float doubleJumpwithHorizontalSpeed = 10f;
    [SerializeField] float doubleJumpOnlyVerticalSpeed = 7f;
    [SerializeField] Vector2 deathKick = new Vector2(0, 20f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    [Header("SFXs")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] float jumpSFXVolume = 0.3f;
    [SerializeField] AudioClip dieSFX;
    [SerializeField] float dieSFXVolume = 0.5f;

    [Header("Shooting")]
    [SerializeField] int ammoCount = 0;
    [SerializeField] bool canShoot = false;

    [SerializeField] int numOfAmmo;
    [SerializeField] Image[] ammo;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    bool canPlayerJump;
    int jumpCount = 0;
    float gravityScaleAtStart;
    GameSession gameSession;

    AudioSource audioSource;

    bool isAlive = true;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        gameSession = FindObjectOfType<GameSession>();
    }
    
    void Start()
    {
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if(!isAlive) { return; }
        DisplayAmmoCount();
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void DisplayAmmoCount()
    {
        if(ammoCount <= 0)
        {
            for(int i=0; i < ammo.Length; i++)
            {
                ammo[i].enabled = false;
            }
        }
        else
        {
            numOfAmmo = ammoCount;
            for(int i=0; i < ammo.Length; i++)
            {
                if(i < numOfAmmo)
                {
                    ammo[i].enabled = true;
                }
                else
                {
                    ammo[i].enabled = false;
                }
            }
        }
        
    }

    void OnMove(InputValue value)
    {
        if(!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if(!isAlive) { return; }
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if(value.isPressed && (jumpCount < 1))
            {
                bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

                if(playerHasHorizontalSpeed)
                {
                    myRigidbody.velocity += new Vector2(myRigidbody.velocity.x , doubleJumpwithHorizontalSpeed);
                }
                else
                {
                    myRigidbody.velocity += new Vector2(myRigidbody.velocity.x , doubleJumpOnlyVerticalSpeed);
                }
                myAnimator.SetBool("isJumping", true);
                AudioSource.PlayClipAtPoint(jumpSFX, Camera.main.transform.position, jumpSFXVolume);
                jumpCount++;
            }
            myAnimator.SetBool("isJumping", false);
        }
        else
        {
            if(value.isPressed)
            {
                myRigidbody.velocity += new Vector2(myRigidbody.velocity.x, jumpSpeed);
                myAnimator.SetBool("isJumping", true);
            }
            jumpCount = 0;
            AudioSource.PlayClipAtPoint(jumpSFX, Camera.main.transform.position, jumpSFXVolume);
            myAnimator.SetBool("isJumping", false);
        }
    }

    void OnFire(InputValue value)
    {
        if(!isAlive) { return; }
        if(canShoot)
        {
            Instantiate(bullet, gun.position, transform.rotation);
            ammoCount--;
            if(ammoCount <= 0)
            {
                TurnShootingOff();
            }
        }
    }

    public void TurnShootingOn()
    {
        ammoCount = 5;
        canShoot = true;
    }

    public void TurnShootingOff()
    {
        canShoot = false;
    }

    public int GetAmmoCount()
    {
        return ammoCount;
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        
        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void ClimbLadder()
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) 
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
    
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }   

    void Die() 
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            myBodyCollider.enabled = false;
            myFeetCollider.enabled = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
