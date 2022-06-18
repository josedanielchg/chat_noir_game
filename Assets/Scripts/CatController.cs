using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class CatController : MonoBehaviour
{
    //Stores input from the PlayerInput
    private Vector2 movementInput;

    private Vector3 direction;

    bool hasMoved;

    public Animator animator;
    public static string LEFT_DOWN_ANIMATION = "left_down_animation";
    public static string LEFT_TOP_ANIMATION = "left_top_animation";
    public static string LEFT_ANIMATION = "left_animation";
    private bool animationOn = false;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        animator.SetBool(LEFT_DOWN_ANIMATION, false);
        animator.SetBool(LEFT_TOP_ANIMATION, false);
        animator.SetBool(LEFT_ANIMATION, false);
        
    }

    void Update()
    {
        if (movementInput.x == 0)
        {
            hasMoved = false;
        }
        else if (movementInput.x != 0 && !hasMoved)
        {
            hasMoved = true;

            bool flipper = movementInput.x < 0;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipper ? 0f : 180f, 0f));
            if(Time.timeScale == 1)
            {
                GetMovementDirection();
            }
            
        }

  

    }

    public void resetAnimation()
    {
        animator.SetBool(LEFT_DOWN_ANIMATION, false);
        animator.SetBool(LEFT_TOP_ANIMATION, false);
        animator.SetBool(LEFT_ANIMATION, false);
    }

    public void GetMovementDirection()
    {
        if (movementInput.x < 0)
        {
            if (movementInput.y > 0)
            {
                direction = new Vector3(-0.5f, 0.8f);
                resetAnimation();
                animator.SetBool(LEFT_TOP_ANIMATION, true);
            }
            else if (movementInput.y < 0)
            {
                direction = new Vector3(-0.5f, -0.8f);
                resetAnimation();
                animator.SetBool(LEFT_DOWN_ANIMATION, true);
            }
            else
            {
                direction = new Vector3(-1, 0, 0);
                resetAnimation();
                animator.SetBool(LEFT_ANIMATION, true);
            }

            System.Threading.Thread.Sleep(100);
            
            transform.position += direction;

        }
        else if (movementInput.x > 0)
        {
            if (movementInput.y > 0)
            {
                direction = new Vector3(0.5f, 0.8f);
                animator.SetBool(LEFT_TOP_ANIMATION, true);
            }
            else if (movementInput.y < 0)
            {
                direction = new Vector3(0.5f, -0.8f);
                animator.SetBool(LEFT_DOWN_ANIMATION, true);
            }
            else
            {
                direction = new Vector3(1, 0, 0);
                animator.SetBool(LEFT_ANIMATION, true);
            }

            transform.position += direction;
            
        }
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position -= direction;
    }
}
