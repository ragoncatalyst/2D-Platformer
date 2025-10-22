using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int pointsForCoinPickup = 100;
    bool wasCollected = false;
    // ...existing code... (removed empty Start/Update)

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasCollected) return;

        if (collision != null && collision.CompareTag("Player"))
        {
            wasCollected = true;

            // Add points to the game session (if present)
            var gameSession = FindObjectOfType<GameSession>();
            if (gameSession != null)
            {
                gameSession.AddToScore(pointsForCoinPickup);
            }

            // Play audio at camera position if clip and camera exist
            if (coinPickupSFX != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            }

            Destroy(gameObject);
        }
    }
}
