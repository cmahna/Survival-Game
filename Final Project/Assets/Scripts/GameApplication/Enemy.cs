﻿using UnityEngine;
using System.Collections;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
    public int playerDamage;                            //The amount of health points to subtract from the player when attacking.
    private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    private Transform target;                           //Transform to attempt to move toward each turn.
    //public bool isLongRange;                            // Determine whether or not enemy has weapon
    private Weapon weapon;
    public float attackDistance;                        //Distance to start attack
    //private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
    public AudioClip attackSound1;
    public AudioClip attackSound2;

    void Awake()
    {           
            // Retrieve the weapon only once
            weapon = GetComponent<Weapon>();        
    }

    //Start overrides the virtual Start function of the base class.
    protected override void Start()
    {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);
        //GameManager.instance.updateEnemiesToSpawn();        

        //Get and store a reference to the attached Animator component.
        animator = GetComponent<Animator>();

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        target = GameObject.FindGameObjectWithTag("Character").transform;

        //Call the start function of our base class MovingObject.
        base.Start();
    }

    public void Die()
    {
        animator.SetTrigger("enemyDeath");
        Destroy(this.gameObject, 2f);
        GameManager.instance.updateEnemiesRemaining();
    }


    //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
    //See comments in MovingObject for more on how base AttemptMove function works.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {        
        //Call the AttemptMove function from MovingObject.
        base.AttemptMove<T>(xDir, yDir);
    }


    //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
    public void MoveEnemy()
    {
        //Declare variables for X and Y axis move directions, these range from -1 to 1.
        //These values allow us to choose between the cardinal directions: up, down, left and right.
        int xDir = 0;
        int yDir = 0;

        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)

            //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            yDir = target.position.y > transform.position.y ? 1 : -1;

        //If the difference in positions is not approximately zero (Epsilon) do the following:
        else
        {
            //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            xDir = target.position.x > transform.position.x ? 1 : -1;
            //Check what direction enemy should face
            if (xDir == -1)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
        AttemptMove<Character>(xDir, yDir);
    }


    //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
    //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
    protected override void OnCantMove<T>(T component)
    {

        /*//Declare hitPlayer and set it to equal the encountered component.
        Health playerHealth = component.GetComponent<Health>();

        //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
        playerHealth.Damage(playerDamage);

        //Set the attack trigger of animator to trigger Enemy attack animation.
        animator.SetTrigger("enemyAttack");*/

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Character")
            animator.SetTrigger("enemyAttack");
    }

        void Update()
    {
        // Auto-fire
        if (weapon != null && weapon.CanAttack && Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            animator.SetTrigger("enemyAttack");            
            weapon.Attack(true);
        }
    }
    
    void FixedUpdate()
    {

        if (Vector3.Distance(transform.position, target.position) > attackDistance)
            MoveEnemy();
    }

}
