using UnityEngine;
using UnityEngine.InputSystem;
public class RealMovementScript : MonoBehaviour
{
   // public SpriteRenderer playerSprite;
    public CharacterController controller;
    public float speed = 6f;

    //public Animator playerAnim;

  //  float currenthorizontalValue;
   // float newhorizontalValue;
    Vector3 velocity;

   // [SerializeField] private InputActionReference interactionReference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
  
    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move.normalized * speed * Time.deltaTime);

        //velocity.y += gravity * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);
        //controller.Move(velocity*Time.deltaTime);


       

    }

}
