using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CatController : MonoBehaviour
{
    //Stores input from the PlayerInput
    private Vector2 movementInput;
    private Vector3 direction;
    bool hasMoved;

    void Update()
    {
        if (movementInput.x == 0)
        {
            hasMoved = false;
        }
        else if (movementInput.x != 0 && !hasMoved)
        {
            hasMoved = true;
            GetMovementDirection();
        }

        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void GetMovementDirection()
    {
        if (movementInput.x < 0)
        {
            if (movementInput.y > 0)
            {
                direction = new Vector3(-0.5f, 0.8f);
            }
            else if (movementInput.y < 0)
            {
                direction = new Vector3(-0.5f, -0.8f);
            }
            else
            {
                direction = new Vector3(-1, 0, 0);
            }
            transform.position += direction;
        }
        else if (movementInput.x > 0)
        {
            if (movementInput.y > 0)
            {
                direction = new Vector3(0.5f, 0.8f);
            }
            else if (movementInput.y < 0)
            {
                direction = new Vector3(0.5f, -0.8f);
            }
            else
            {
                direction = new Vector3(1, 0, 0);
            }

            transform.position += direction;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position -= direction;
    }
}
