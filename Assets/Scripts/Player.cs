using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

// onDeath: anim.SetTrigger("Death");
// onDamage anim.SetTrigger("Hurt");
// onAttack anim.SetTrigger("Attack");

public class Player : MonoBehaviour
{   
    [SerializeField]
    private float moveForce = 5f;

    [SerializeField]
    private float jumpForce = 11f;
    
    private float movementX;
    private bool isGrounded;
    private bool isDead;
    private bool combatIdle;
    private bool isFacingRight;

    private Rigidbody2D myBody;
    private SpriteRenderer sr;
    private Animator anim;
    public Attributes attributes;
    public HealthBar healthBar;
    public AudioClip onLevelUp;
    private AudioSource audioSrc;

    private Text levelDisplay;
    private Text xpDisplay;

    
    private string WALK_ANIMATION = "Walk";
    private string GROUND_TAG = "Ground";
    private string ENEMY_TAG = "Enemy";
    private string R_COLLECTOR = "RCollector";
    private string L_COLLECTOR = "LCollector";
    private string LEVEL_TAG = "Level";
    private string XP_TAG = "XP";

    [System.Serializable]
    public class Attributes
    {
        private int hp;
        private int currentHP;
        private int attack;
        private int critChance;
        private int xp;

        private int level;
        private int requiredXP;

        private Text levelDisplay;
        private Text xpDisplay;
        private HealthBar healthBar;
        private AudioClip onLevelUp; 
        private AudioSource audioSrc;
        
        public Attributes(Text levelDisplay, Text xpDisplay, HealthBar healthBar, AudioClip onLevelUp, AudioSource audioSrc)
        {
            hp = 25;
            currentHP = 25;
            attack = 5;
            critChance = 5;
            xp = 0;
            level = 1;
            requiredXP = 30;
            this.levelDisplay = levelDisplay;
            this.xpDisplay = xpDisplay;
            this.healthBar = healthBar;
            this.onLevelUp = onLevelUp;
            this.audioSrc = audioSrc;
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

        public int getCritChance() {
            return critChance;
        }

        public void increaseCritChance(int cc) {
            this.critChance += cc;
        }

        public int getXP() {
            return xp;
        }

        public void setXP(int xp) {
            this.xp = xp;
        }

        public void increaseXP(int xp) {
            this.xp += xp;
            xpDisplay.text = $"XP: {this.xp} / {this.requiredXP}";
            if (this.xp >= requiredXP) this.increaseLevel(1);
        }

        public int getLevel() {
            return level;
        }

        public void increaseLevel(int level) {
            this.level += level;
            levelDisplay.text = $"LEVEL: {this.level}";
            for(int i = 0; i < level; i++) levelUP();
        }

        public int getRequiredXP() {
            return requiredXP;
        }

        public void increaseRequiredXP(int xp) {
            this.requiredXP += xp;
            xpDisplay.text = $"XP: {this.xp} / {this.requiredXP}";
        }

        private void levelUP() {
            audioSrc.PlayOneShot(onLevelUp, 0.8f);
            setXP(0);
            increaseHP((System.Math.Min(this.level, 10) * 5));
            increaseAttack((System.Math.Min(this.level, 10) * 2));
            increaseCritChance(1);
            increaseRequiredXP((this.level * 30));
            this.debugStats();
        }

        public void debugStats() {
            Debug.Log($"MC HP: {hp}, ATTACK: {attack}, CRIT: {critChance}");
        }
    }

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        audioSrc = GetComponent<AudioSource>();
        levelDisplay = GameObject.FindWithTag(LEVEL_TAG).GetComponent<Text>();
        xpDisplay = GameObject.FindWithTag(XP_TAG).GetComponent<Text>();
        attributes = new Attributes(levelDisplay, xpDisplay, healthBar, onLevelUp, audioSrc);
        healthBar.SetMaxHealth(attributes.getHP());
        healthBar.SetHealth(attributes.getCurrentHP());
        isFacingRight = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // attributes.increaseXP(1);
        // PlayerMoveKeyboard();
        // AnimatePlayer();
        // PlayerJump();

        movementX = Input.GetAxisRaw("Horizontal");
        Debug.Log(movementX);
        // Swap direction of sprite depending on walk direction
        if (movementX > 0) {
            if (!isFacingRight) {
                isFacingRight = true;
                transform.Rotate(0f,180f,0f);
            }
        }
        else if (movementX < 0) {
            if (isFacingRight) {
                isFacingRight = false;
                transform.Rotate(0f,180f,0f);
            }
        }

        // Move
        myBody.velocity = new Vector2(movementX * moveForce, myBody.velocity.y);

        //Set AirSpeed in animator
        anim.SetFloat("AirSpeed", myBody.velocity.y);

        //Attack
        if(Input.GetMouseButtonDown(0)) {
            anim.SetTrigger("Attack");
        }

        //Jump
        else if (Input.GetKeyDown("space") && isGrounded) {
            anim.SetTrigger("Jump");
            isGrounded = false;
            anim.SetBool("Grounded", isGrounded);
            myBody.velocity = new Vector2(myBody.velocity.x, jumpForce);
        }

        else if (movementX > 0) 
            anim.SetInteger("AnimState", 2);
        else if (movementX < 0)
            anim.SetInteger("AnimState", 2);

        //Combat Idle
        else if (combatIdle)
            anim.SetInteger("AnimState", 1);

        //Idle
        else
            anim.SetInteger("AnimState", 0);
    }

    private void FixedUpdate()
    {
        // PlayerJump();
    }

    public void Wait(float seconds, Action action){
        StartCoroutine(_wait(seconds, action));
    }
    IEnumerator _wait(float time, Action callback){
        yield return new WaitForSeconds(time);
        callback();
    }

    void PlayerMoveKeyboard() 
    {
        movementX = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(movementX, 0f, 0f) * moveForce * Time.deltaTime;
    }

    void AnimatePlayer() 
    {
        // right
        if (movementX > 0)
        {
            anim.SetInteger("AnimState", 2);
            sr.flipX = false;
        }
        else if (movementX < 0)
        {
            // left
            anim.SetInteger("AnimState", 2);
            sr.flipX = true;
        }
        else
        {
            anim.SetInteger("AnimState", 0); // idle
        }

        anim.SetFloat("AirSpeed", myBody.velocity.y);

        if (Input.GetMouseButtonDown(0)) {
            anim.SetTrigger("Attack");
        }
    }

    void PlayerJump() 
    {
        if (Input.GetButtonDown("Jump") && isGrounded) {
            anim.SetTrigger("Jump");
            isGrounded = false;
            anim.SetBool("Grounded", isGrounded);
            myBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG)) {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
        }
        
        if (collision.gameObject.CompareTag(ENEMY_TAG)) {
            myBody.velocity = new Vector2(0f, myBody.velocity.y);
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster.isDead) return;
            attributes.increaseCurrentHP(-monster.attributes.getAttack());
            
            if (attributes.getCurrentHP() <= 0) {
                anim.SetTrigger("Death");
                SceneManager.LoadScene("MainMenu");
            }
            else 
                anim.SetTrigger("Hurt");
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(ENEMY_TAG)) {
            myBody.velocity = new Vector2(0f, myBody.velocity.y);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(R_COLLECTOR)) 
            transform.position = new Vector3(-55f, transform.position.y, transform.position.z);
        if (collision.gameObject.CompareTag(L_COLLECTOR)) 
            transform.position = new Vector3(55f, transform.position.y, transform.position.z);
            // Destroy(gameObject);
    }

} // class






























