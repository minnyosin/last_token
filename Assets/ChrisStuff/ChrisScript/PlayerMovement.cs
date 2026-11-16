using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public CharacterController controller;
    public float speed = 6f;

    public Animator playerAnim;
  
    float currenthorizontalValue;
    float newhorizontalValue;
    Vector3 velocity;

   [SerializeField] private InputActionReference interactionReference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnEnable()
    {
        interactionReference.action.Enable();
        interactionReference.action.started += PlayerInteracted;
    }

    private void OnDisable()
    {
        interactionReference.action.started -= PlayerInteracted;
        interactionReference.action.Disable();
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
      
      
        if (move !=Vector3.zero)
        {
            playerAnim.SetBool("isWalking", true);

        }
        else if(move==Vector3.zero) 
        {
            playerAnim.SetBool("isWalking", false);
        }


        if (horizontal != 0)
            playerSprite.flipX = horizontal < 0;

    }
    private void PlayerInteracted(InputAction.CallbackContext context)
    {
        Debug.Log("ispressed");
        controller.enabled = false;
        transform.position = new Vector3(11.9f,0,0);
        controller.enabled = true;

    }
}
