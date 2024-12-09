using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESpellMinotaur : MonoBehaviour
{
    private float cooldown = 15f;
    private float lastAttackTime = 0f;
    private Animator anim;
    private LifeSystem lifeSystem;
    public GameObject throwEffectPrefab; // Effect when the spell is cast
    public float throwForce = 6f; // Maximum force to throw the target
    public float throwHeight = 2f; // The height to lift the target
    private AudioSource throwSound;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lifeSystem = GetComponent<LifeSystem>();
        throwSound = GetComponents<AudioSource>()[2];
        lastAttackTime = Time.time - cooldown; // Set the last attack time to the past to allow the first attack
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CastESpell();
        }
    }

    // If the player presses the E key, the Minotaur throws the closest entity within range towards the mouse position
    public void CastESpell()
    {
        if (Time.time - lastAttackTime >= cooldown)
        {
            // Find the closest target within a 2 unit range
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);

            GameObject closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (var hitCollider in hitColliders)
            {
                // Check if the target is a valid enemy or minion
                if ((hitCollider.CompareTag("PlayerBlue") || hitCollider.CompareTag("PlayerRed") ||
                    hitCollider.CompareTag("MinionBlue") || hitCollider.CompareTag("MinionRed")) &&
                    hitCollider.gameObject != this.gameObject) // Exclude self
                {
                    float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);

                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = hitCollider.gameObject;
                    }
                }
            }

            if (closestTarget != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 mousePosition = hit.point; // Get the mouse position

                    // Calculate the direction to throw the target
                    Vector3 throwDirection = (mousePosition - closestTarget.transform.position).normalized;

                    // Calculate the throw position with a max distance
                    Vector3 throwPosition = closestTarget.transform.position + throwDirection * Mathf.Min(Vector3.Distance(closestTarget.transform.position, mousePosition), throwForce);

                    // Calculate the throw height
                    throwPosition.y = throwHeight;

                    // Calculate the throw force
                    Vector3 throwForceVector = (throwPosition - closestTarget.transform.position).normalized * throwForce;

                    // Debug.Log("Throwing: " + closestTarget.name + " to " + mousePosition);

                    // Play the throw sound
                    throwSound.Play();
                    // Start the throw coroutine with the calculated direction and position
                    StartCoroutine(ThrowTarget(closestTarget, throwPosition));

                    // Instantiate the throw effect at the Minotaur's position
                    if (throwEffectPrefab != null)
                    {
                        GameObject throwEffect = Instantiate(throwEffectPrefab, transform.position, Quaternion.identity);
                        throwEffect.transform.SetParent(hit.collider.transform); // Parent the effect to the target
                    }
                    else
                    {
                        Debug.LogWarning("Throw Effect Prefab is not assigned!");
                    }

                    lastAttackTime = Time.time; // Update the last attack time
                }
            }
        }
    }

    // Coroutine to handle throwing the target towards the mouse position with a max distance
    IEnumerator ThrowTarget(GameObject target, Vector3 targetPosition)
    {
        if (target == null)
        {
            yield break; // Exit if the target is null
        }

        Vector3 startPosition = target.transform.position;
        float elapsed = 0f;
        float throwDuration = 1f; // Duration of the throw

        while (elapsed < throwDuration)
        {
            // Check if the target still exists
            if (target == null)
            {
                yield break; // Stop the coroutine if the target is destroyed
            }

            // Move the target towards the target position
            float t = elapsed / throwDuration;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            target.transform.position = currentPosition;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (target != null)
        {
            target.transform.position = targetPosition; // Ensure final position if the target still exists
        }
    }
}
