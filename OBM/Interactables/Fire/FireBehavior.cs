/* Charlie Dye, PACE Team - 2024.01.18

This is the script for the fire spawning */

using UnityEngine;

public class FireBehavior : MonoBehaviour
{

    [Header("Lowest Allowed Elevation")]
    [Tooltip("The lowest elevation that fire can occupy before it is automatically extinguished.")] public float boundDeadline;

    [Header("Audio Sources")]
    [Tooltip("The audio source responsible for the fire crackling.")] public AudioSource cracklingNoise;
    [Tooltip("The audio source responsible for the fire hissing as it is put out.")] public AudioSource extinguishedNoise;

    [Header("Particle System")]
    [Tooltip("The particle system that produces a flame effect.")] public ParticleSystem fireParticles;

    // "Local" extinguisher gameObject variable, assigned from FireSpawner.cs
    [HideInInspector] public GameObject localSprayVariable;

    void FixedUpdate()
    {

        /* If the fire's y-level drops below a specified value, the fire will be destroyed;
        this is a failsafe to prevent any fire from falling forever, softlocking and lagging the game */
        if (gameObject.transform.position.y < boundDeadline) Destroy(gameObject);

    }

    void OnTriggerEnter(Collider extinguisher)
    {
        
        // If the extinguisher collides with the fire, then the fire will be destroyed        
        if (extinguisher == localSprayVariable.GetComponent<Collider>())
        {

            DestroyFire();

        }

    }

    public void DestroyFire()
    {

        // Plays a steam noise and stops the crackling
        cracklingNoise.Stop();
        extinguishedNoise.Play();

        // Stops the particle effect
        fireParticles.Stop();

        // Destroys the GameObject once the steam noise stops
        Destroy(gameObject, extinguishedNoise.clip.length);

    }

}