using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] float coinPickupSFXVolume = 0.3f;
    [SerializeField] int pointsForCoinPickup = 100;
    int count = 0;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player" && count < 1)
        {
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position, coinPickupSFXVolume);
            FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
            Destroy(gameObject);
            count++;
        }
    }
}
