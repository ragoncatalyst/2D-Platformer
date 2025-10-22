using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpPower = 100f;
    [SerializeField] float climbSpeed = 10f;
    bool canFly = false;
    [SerializeField] bool useFlyMode = false;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] float deathTime = 2f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] bool autoFire = false;
    float gravityScaleAtStart;

    bool isAlive = true;
    bool isActive => ActiveControl.Instance.Current == ActiveControl.Actor.Player;
    
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            if (useFlyMode)
            {
                Fly();
            }
            return; 
        }
        // 只有成为受控对象时才处理移动相关
        if (isActive)
        {
            Run();
            ClimbLadder();
            FlipSprite();
            AutoFire();
        }
        else
        {
            // 非受控对象：保持竖直速度（重力与爬梯仍按碰撞决定），水平输入置零
            if (!canFly)
            {
                var v = myRigidbody.velocity;
                v.x = 0f;
                myRigidbody.velocity = v;
                myAnimator.SetBool("isRunning", false);
            }
        }
        // 死亡判定始终有效
        Die();
    }
    void Run()
    {
        if (!canFly)
        {
            Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
            myRigidbody.velocity = playerVelocity;
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
            myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
            myRigidbody.gravityScale = gravityScaleAtStart;
        }
    }
    void Fly()
    {
        if (canFly)
        {
            Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, moveInput.y * runSpeed);
            myRigidbody.velocity = playerVelocity;
            bool playerHasSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon || Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("isRunning", playerHasSpeed);
            myRigidbody.gravityScale = 0;
        }
    }
    void OnMove(InputValue value)
    {
        if (!useFlyMode && !isAlive) { return; }
        if (!isActive) { return; }
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!isActive) { return; }
        if(value.isPressed)
        {
            if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                
                    myRigidbody.velocity += new Vector2(0f, jumpPower);
                
            }
            
        }
    }
    void FlipSprite()
    {
        //if the player is moving -> true
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; 

        // if the player is moving we will flip the sprite
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
        
        
    }
    void ClimbLadder()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            if (isActive)
            {
                Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
                myRigidbody.velocity = climbVelocity;
                bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
                myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
                myRigidbody.gravityScale = 0;
            }
            else if (!canFly)
            {
                // 不受控且处于梯子：按普通状态处理，允许自然下落
                myRigidbody.gravityScale = gravityScaleAtStart;
                myAnimator.SetBool("isClimbing", false);
            }
        }
        else if (!canFly)
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
        }
    }
    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")))
        {
            
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity =new Vector2( Mathf.Sign(-myRigidbody.velocity.x)*deathKick.x,deathKick.y);
            if (useFlyMode)
            {
                canFly = true;
            }
            StartCoroutine(DoDeathStuff());
        }
    }
    IEnumerator DoDeathStuff()
    {
        yield return new WaitForSecondsRealtime(deathTime);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
    void AutoFire()
    {
        if (autoFire)
        {
            if (!isAlive) { return; }
            GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
            
        }
    }
    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        if (!isActive) { return; }
        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
        
    }
}
