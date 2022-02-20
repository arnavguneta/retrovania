using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [HideInInspector]
    public float speed;

    private Rigidbody2D myBody;
    private Animator anim;
    public Attributes attributes;
    private Player player;
    public HealthBar healthBar;

    private string ATTACK_ANIMATION = "Attack";
    private string DEATH_ANIMATION = "Die";
    private string DMG_ANIMATION = "Damage";

    private string GROUND_TAG = "Ground";
    private string PLAYER_TAG = "Player";
    private string SLASH_TAG = "Slash";

    public bool isDead = false;

    [System.Serializable]
    public class Attributes
    {
        private int hp;
        private int currentHP;
        private int attack;
        private int xp;
        private int level;

        private HealthBar healthBar;

        public Attributes(HealthBar healthBar)
        {
            level = 1;
            hp = 20;
            currentHP = 10;
            attack = 3;
            xp = 10;
            this.healthBar = healthBar;
        }

        public int getHP() {
            return hp;
        }

        public void increaseHP(int hp) {
            this.hp += hp;
            healthBar.SetMaxHealth(this.hp);
        }
        
        public int getCurrentHP() {
            return currentHP;
        }

        public void setCurrentHP(int hp) {
            this.currentHP = hp;
        }

        public void increaseCurrentHP(int hp) {
            this.currentHP += hp;
            healthBar.SetHealth(this.currentHP);
        }

        public int getAttack() {
            return attack;
        }

        public void increaseAttack(int attack) {
            this.attack += attack;
        }

        public int getXP() {
            return xp;
        }

        public void setXP(int xp) {
            this.xp = xp;
        }

        public void increaseXP(int xp) {
            this.xp += xp;
        }

        public int getLevel() {
            return level;
        }

        public void increaseLevel(int level) {
            this.level += level;
            for(int i = 0; i < level; i++) levelUP();
        }

        private void levelUP() {
            increaseHP((System.Math.Min(this.level, 10) * 15));
            setCurrentHP(this.hp);
            increaseAttack((System.Math.Min(this.level, 10) * 2));
            this.debugStats();
        }

        public void debugStats() {
            Debug.Log($"MONSTER HP: {hp}, ATTACK: {attack}");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag(PLAYER_TAG).GetComponent<Player>();
        attributes = new Attributes(healthBar);
        attributes.increaseLevel(player.attributes.getLevel() - 1);
        healthBar.SetMaxHealth(attributes.getHP());
        healthBar.SetHealth(attributes.getCurrentHP());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") && !isDead)
            myBody.velocity = new Vector2(speed, myBody.velocity.y);
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG) && !isDead) {
            myBody.velocity = new Vector2(0f, myBody.velocity.y);
            anim.SetBool(ATTACK_ANIMATION, true);
        } else if (isDead) {
            anim.SetBool(ATTACK_ANIMATION, false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG)) 
            anim.SetBool(ATTACK_ANIMATION, false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG) && !isDead)
            anim.SetBool(ATTACK_ANIMATION, true);
        else if (collision.gameObject.CompareTag(SLASH_TAG) && !isDead) {
            anim.SetBool(ATTACK_ANIMATION, false); 
            Destroy(collision.gameObject);
            player = GameObject.FindWithTag(PLAYER_TAG).GetComponent<Player>();
            bool isCrit = ((int) Random.Range(0.0f, 100.0f)) <= player.attributes.getCritChance();
            attributes.increaseCurrentHP((isCrit) ? -player.attributes.getAttack() * 2 : -player.attributes.getAttack());
            if (attributes.getCurrentHP() <= 0) {
                isDead = true;
                myBody.velocity = new Vector2(0f, myBody.velocity.y);
                anim.SetBool(DEATH_ANIMATION, true);
                player.attributes.increaseXP(attributes.getHP());
                player.attributes.increaseCurrentHP((int) (attributes.getHP() / 2));
                Destroy(gameObject, 1);
            } else {
                anim.SetBool(DMG_ANIMATION, true);
            }
        }
        else
            anim.SetBool(ATTACK_ANIMATION, false); 

        if (collision.gameObject.name.Equals("Right Collector")) 
            transform.position = new Vector3(-65f, transform.position.y, transform.position.z);
        if (collision.gameObject.name.Equals("Left Collector")) 
            transform.position = new Vector3(65f, transform.position.y, transform.position.z);  
    }
} // class

















