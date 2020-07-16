using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rd;
    Animator enemyAnimator;

    public int attack = 5;
    public Vector2 speed = new Vector2(6, 6);
    public Sprite enemyDeathSprite;
    public Sprite playerDeathSprite; 

    private Player player;
    private Vector2 movement;

    private Text healthText;
    private int health = 100;
    private int maxHealth = 100;
    private bool isDead = false;
    private bool needToWaitAfterHit = false;
    private GameObject playerHealth;
    private Animator playerAnimator;

    float playerPosX;
    float enemyPosX;
    float playerPosY;
    float enemyPosY;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerAnimator = player.GetComponent<Animator>();
        rd = this.GetComponent<Rigidbody2D>();
        enemyAnimator = this.GetComponent<Animator>();
        healthText = this.GetComponentInChildren<Text>();
        playerHealth = GameObject.FindGameObjectWithTag("PlayerHealth");
        HideHealth();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosX = player.transform.position.x;
        enemyPosX = this.transform.position.x;
        playerPosY = player.transform.position.y;
        enemyPosY = this.transform.position.y;

        playerHealth.GetComponent<Text>().text = player.health + "/" + player.maxHealth + " hp";

        if (!this.isDead && !this.needToWaitAfterHit)
        {
            movement = new Vector2(0, 0);

            if (Mathf.Abs(playerPosX - enemyPosX) < 3 && Mathf.Abs(playerPosY - enemyPosY) < 4)
            {
                enemyAnimator.SetBool("goLeft", false);
                enemyAnimator.SetBool("goRight", false);

                PlayerUnderAttack();
            }

            else
            {
                if (enemyPosX >= playerPosX)
                {
                    enemyAnimator.SetBool("goRight", false);
                    enemyAnimator.SetBool("goLeft", true);

                    movement = new Vector2(playerPosX - enemyPosX, playerPosY - enemyPosY);
                }

                else if (enemyPosX < playerPosX)
                {
                    enemyAnimator.SetBool("goLeft", false);
                    enemyAnimator.SetBool("goRight", true);

                    movement = new Vector2(playerPosX - enemyPosX, playerPosY - enemyPosY);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rd.velocity = movement.normalized * speed;
    }

    public void EnemyUnderAttack()
    {
        if (Mathf.Abs(playerPosX - enemyPosX) <= 3.5 && Mathf.Abs(playerPosY - enemyPosY) <= 3.5)
        {
            if (playerPosX >= enemyPosX)
            {
                playerAnimator.Play("man_left_attack");
            }
            else
            {
                playerAnimator.Play("man_right_attack");
            }

            if (this.health > 0)
            {
                this.health -= player.attack;

                if (this.health <= 0)
                {
                    enemyAnimator.Play("deathStart");
                    this.health = 0;
                    StartCoroutine(EnemyDeath());
                }
            }

            healthText.text = health + "/" + this.maxHealth + " hp";
        }
    }

    IEnumerator EnemyDeath()
    {
        this.isDead = true;
        yield return new WaitForSeconds(1);
        this.transform.position.Set(this.transform.position.x, this.transform.position.y - 5, 0);
        this.GetComponent<SpriteRenderer>().sprite = enemyDeathSprite;
        this.GetComponent<CapsuleCollider2D>().enabled = false;
        enemyAnimator.SetBool("isDead", true);
        movement = new Vector2(0, 0);
        this.needToWaitAfterHit = false;

        this.gameObject.GetComponentInParent<EnemiesWave>().SetCurrentEnemiesCount();
    }

    public void PlayerUnderAttack()
    {
        if (!this.needToWaitAfterHit && !this.isDead)
        {
            this.needToWaitAfterHit = true;

            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        //столько времени враг стоит возле игрока, а затем бьёт
        yield return new WaitForSeconds(1f);

        if (playerPosX >= enemyPosX && !this.isDead)
        {
            enemyAnimator.Play("attackRight");
        }
        else if (!this.isDead)
        {
            enemyAnimator.Play("attackLeft");
        }

        //если игрок не успел отойти за отведённое время, то он получает урон
        if (Mathf.Abs(playerPosX - enemyPosX) <= 3 && Mathf.Abs(playerPosY - enemyPosY) <= 3 && !this.isDead && player.health > 0)
        {
            player.health -= this.attack;

            if (player.health <= 0)
            {
                player.health = 0;

                Text message = GameObject.FindGameObjectWithTag("Message").GetComponent<Text>();
                message.text = "Вы проиграли!";
                message.fontSize = 25;
                player.SetIsDead(true);

                playerAnimator.SetBool("isTop", false);
                playerAnimator.SetBool("isDown", false);
                playerAnimator.SetBool("isRight", false);
                playerAnimator.SetBool("isLeft", false);

            }
        }

        yield return new WaitForSeconds(0.5f);

        this.needToWaitAfterHit = false;
    }

    public void ShowHealth()
    {
        if (isDead)
        {
            healthText.text = "Мёртвый враг";
        }
        healthText.gameObject.SetActive(true);
        healthText.fontSize = 17;

    }

    public void HideHealth()
    {
        healthText.gameObject.SetActive(false);
    }
}