// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class player_script : MonoBehaviour {

//     private Vector3 direction;
//     Rigidbody2D rb;
//     public float force;

//     public void Start(){
//         rb = GetComponent<Rigidbody2D>();
//         force = 10f;
//         direction = new Vector3 (-1,0);
//         rb.AddForce(direction);
//     }


//     public void Update() {
//         if (Input.GetKey("left")) {
//             direction = new Vector3(-1,0);
//             rb.AddForce(direction * force);
//         }
//         else if (Input.GetKey("right")) {
//             direction = new Vector3(1,0);
//             rb.AddForce(direction * force);
//         }
//     }

//     void OnCollisionEnter2D(Collision2D other) {
//         if (other.gameObject.tag == "walls") {
//             rb.velocity = Vector3.zero;
//             rb.AddForce(direction);
//         }
//     }

// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tasks to improve/complete :
/// 1. set input restrictions - DONE
/// 2. provide death animation instead of Destroy(gameobject) - In - process
/// </summary>

public class player_script : MonoBehaviour
{
    LineTrail lineTrail;
    Vector2 touchStartPos;              //position where the touch began
    Vector2 queuedDirection;
    Vector2 motionDirection;
    Vector2 destination;                //point on the wall that the ball will hit when the ball collidies
    bool isStart;                       //bool value to seex if the game has started - game starts after the user inputs a touch swipe and releases the hand
    int leftOrRight;                   //left motion is 0 and right motion is 1 (coded as to make this variable store only 0 and 1)
    float timeSinceLastCollision = new float();
    public float distanceFromOriginToWallCollider;  //shortest distance from walls' collider to origin (i.e. positive x-coordinate of either collider)
    public GameObject directionDot;     //the dot sprite that shows the touch direction
    public Text scoreText;
    public float scrollSpeed;
    float minimumTimeUntilCollision;
    public GameObject particleGenerator;
    

    // Use this for initialization
    void Start()
    {
        lineTrail = GetComponent<LineTrail>();
        isStart = true;
        directionDot.transform.localScale = new Vector3(0, 0, 0);   //the dots are invisible until the second input is given
        minimumTimeUntilCollision = distanceFromOriginToWallCollider * Mathf.Tan(30 * Mathf.Deg2Rad) / scrollSpeed;
        //the above value is the minimum time required to cross half of the screen
        //change the angle (i.e 30) to the new lowest limit when the input restriction is changed
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastCollision += Time.deltaTime;
        UpdateDestination();
        CalculateDirection();
        if (!isStart)                       //only start moving when the user has input the direction
            Movement();
    }

    void CalculateDirection()
    {
        if (Input.touchCount > 0)
        {
            Touch touchInput = Input.GetTouch(0);
            if (touchInput.phase == TouchPhase.Began)
            {
                touchStartPos = touchInput.position;                       //register the begining of a swipe
            }
            else if (touchInput.phase == TouchPhase.Moved)                  //input direction can be changed until the the user disjoints the touch
            {
                queuedDirection = touchInput.position - touchStartPos;      //calculate direction vector
                queuedDirection = InputRestriction(queuedDirection);
                //changing dot transform information but not in the first input
                if (!isStart)
                {
                    float zAngle = Mathf.Atan2(queuedDirection.y, queuedDirection.x);
                    directionDot.transform.position = new Vector3(destination.x, destination.y, 1);
                    directionDot.transform.eulerAngles = new Vector3(0, 0, zAngle * Mathf.Rad2Deg);
                    directionDot.transform.localScale = new Vector3(20, 0.04f, 1);
                }
            }
            else if (touchInput.phase == TouchPhase.Ended)                        //input is finalised in this stage
            {
                directionDot.transform.localScale = new Vector3(0, 0, 0);       //when the input is complete, the dots vanish - the dots are
                                                                                //only shown to make it easier for the user to make decisions

                //call pseudo OnCollisionEnter2D function when the first input is provided
                if (isStart)
                {
                    motionDirection = queuedDirection;
                    leftOrRight = (int)(0.5 + Mathf.Sign(motionDirection.x));       //value is 0 when moving to the left and 1 when moving to the right
                    queuedDirection = new Vector2(-motionDirection.x, motionDirection.y);
                    CalculateDestination();
                    isStart = false;                                  //once the first input is given, the starting phase is over
                    lineTrail.afterCollision(motionDirection);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "tiles")
        {
            InstantiatedTiles collisionObject = collision.gameObject.GetComponent<InstantiatedTiles>();
            if (timeSinceLastCollision > minimumTimeUntilCollision)
            {
                if (collisionObject.score == 0)
                {
                    lineTrail.DeleteTrail();
                    Destroy(gameObject);
                    return;
                }
                int playerScore = int.Parse(scoreText.text);
                playerScore += collisionObject.score;
                scoreText.text = playerScore.ToString();
            }
            else
            {
                collisionObject.RevertCollision();
                Debug.Log("Multiple collision detected");
                return;
            }
            ContactPoint2D contact = collision.GetContact(0);
            BallParticles particles = particleGenerator.GetComponent<BallParticles>();
            particles.triggerParticles(contact.point);            
        }
        motionDirection = queuedDirection;                  //once collision with walls or tiles occures, the ball's direction 
                                                            //is set to be the previously queued direction
        queuedDirection = new Vector2(-motionDirection.x, motionDirection.y);       //default value for queuedDirection is the angle of
                                                                                    //the motion when the coefficient of restitution is 1
        CalculateDestination();
        leftOrRight = (leftOrRight + 1) % 2;                //changes value from 0 to 1 or vice versa when changing the direction of motion
        lineTrail.afterCollision(motionDirection);
        timeSinceLastCollision = 0;
    }

    void Movement()
    {
        float velocityX = scrollSpeed * motionDirection.x / motionDirection.y;
        Vector3 change = new Vector3(velocityX * Time.deltaTime, 0, 0);
        transform.position += change;
    }

    void CalculateDestination()
    {
        //the formula below is a forced formula that I came up with. It will work on our cases but not in every hypothetical cases
        destination.y = transform.position.y + Mathf.Abs((motionDirection.y / motionDirection.x) * (distanceFromOriginToWallCollider - 0.5f + Mathf.Abs(transform.position.x)));
        //in the above equation, we have to make sure that the radius of the ball is almost 0.5 units
        destination.x = distanceFromOriginToWallCollider * Mathf.Sign(motionDirection.x);    //if the direction is obtuse, the destination will be negative
    }

    void UpdateDestination()
    {
        destination.y -= scrollSpeed * Time.deltaTime;
    }


    //mapping 0-90 domain of angles to 30-60, linearly
    //the restrictions should be symmetric around x=y line
    //i.e 45-15=30 and 45+15=60
    //else, the code needs to be changed
    Vector2 InputRestriction(Vector2 input)
    {
        float inputAngleDeg = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        //////the following code makes sure the range of the domain is either (-90,270] or [-90,270)
        inputAngleDeg += 360 * leftOrRight * (int)(0.5 - Mathf.Sign(inputAngleDeg));
        float angleAfterChecking;
        if (inputAngleDeg < (90 * leftOrRight + 0))
            angleAfterChecking = 90 * leftOrRight + 0;
        else if (inputAngleDeg > (90 * leftOrRight + 90))
            angleAfterChecking = 90 * leftOrRight + 90;
        else
            angleAfterChecking = inputAngleDeg;
        float finalAngle = ((15 * (angleAfterChecking - (45 + 90 * leftOrRight))) / 45) + 45 + 90 * leftOrRight;
        return new Vector2(1 / Mathf.Tan(finalAngle * Mathf.Deg2Rad), 1);
    }
}