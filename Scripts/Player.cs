using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rd;

    public Vector2 speed = new Vector2(6, 6);
    private Vector2 movement;

    Animator animator;
    private InvItem[] objects;
    private GameObject player;
    private InvItem nearest;
    private bool isDead = false;

    public int maxHealth = 100;
    public int health = 100;
    public int attack = 20;

    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
    }

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (!isDead)
        {

            float minDistance = Mathf.Infinity;
            objects = GameObject.FindObjectsOfType<InvItem>();

            foreach (InvItem gameObject in objects)
            {
                float objPosX = gameObject.transform.position.x;
                float objPosY = gameObject.transform.position.y;

                float playerPosX = player.transform.position.x;
                float playerPosY = player.transform.position.y;

                float currentDistance = Mathf.Sqrt((objPosX - playerPosX) * (objPosX - playerPosX) +
                    (objPosY - playerPosY) * (objPosY - playerPosY));

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    nearest = gameObject;
                }
            }

            if (nearest != null && Input.GetKeyDown(KeyCode.F) && !nearest.getAnimator().GetBool("needToTake"))
            {
                nearest.TakeObject();
            }

            if (nearest != null && !nearest.getAnimator().GetBool("needToTake"))
            {
                animator.SetBool("isTop", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isRight", false);
                animator.SetBool("isLeft", false);
            }

            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && nearest != null && !nearest.getAnimator().GetBool("needToTake"))
            {
                animator.SetBool("isTop", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isRight", false);
                animator.SetBool("isLeft", true);
            }

            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && nearest != null && !nearest.getAnimator().GetBool("needToTake"))
            {
                animator.SetBool("isTop", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isLeft", false);
                animator.SetBool("isRight", true);
            }

            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && nearest != null && !nearest.getAnimator().GetBool("needToTake"))
            {
                animator.SetBool("isLeft", false);
                animator.SetBool("isRight", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isTop", true);
            }

            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && nearest != null && !nearest.getAnimator().GetBool("needToTake"))
            {
                animator.SetBool("isLeft", false);
                animator.SetBool("isRight", false);
                animator.SetBool("isTop", false);
                animator.SetBool("isDown", true);
            }

            if (nearest != null && !nearest.getAnimator().GetBool("needToTake"))
            {
                float inputX = Input.GetAxis("Horizontal");
                float inputY = Input.GetAxis("Vertical");

                movement = new Vector2(speed.x * inputX, speed.y * inputY);
            }
        }
    }

    private void FixedUpdate()
    {
        if (nearest != null && !nearest.getAnimator().GetBool("needToTake") && !isDead)
        {
            rd.velocity = movement;
        }
    }
}