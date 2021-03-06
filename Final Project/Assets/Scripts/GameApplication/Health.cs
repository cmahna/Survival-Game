﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    // Total hitpoints
    public int hp = 100;    
    // Enemy or player?  
    public bool isEnemy = false;
    private bool dead;

    public Slider healthSlider;
    //public float flashSpeed = 5f;
    //public Color flashColor = new Color(1f, 0f, 0f, 0.1f);

    void Awake()
    {
        dead = false;
    }

    // Inflicts damage and check if the object should be destroyed
    public void Damage(int damageCount)
    {
        if (dead)
            return;
        hp -= damageCount;
        healthSlider.value = hp;
        if (hp <= 0)
        {
            // Dead!
            dead = true;
            if (isEnemy)
            {
                Enemy e = this.gameObject.GetComponent<Enemy>();
                e.Die();
            }
            else
            {
                Character c = this.gameObject.GetComponent<Character>();
                c.Die();
                Debug.Log("You died!");
            }
            
            // GameManager.instance.GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // Is this a shot?
        Shooting shot = otherCollider.gameObject.GetComponent<Shooting>();
        if (shot != null)
        {
            // Avoid friendly fire
            if (shot.isEnemyShot != isEnemy)
            {
                Animator shot_anim = otherCollider.gameObject.GetComponent<Animator>();                                
                shot_anim.SetTrigger("impact");
                Move stop = otherCollider.gameObject.GetComponent<Move>();
                stop.speed.x = 0;                                
                Damage(shot.damage);
                // Destroy the shot                           
                Destroy(shot.gameObject,0.5f); // Remember to always target the game object, otherwise you will just remove the script
            }
        }        
      
    }
}
