using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public abstract class Interactable : MonoBehaviour
{
    protected Unit unit;

        /// <summary>
        /// Called when a player enters the trigger zone of the interactable object.
        /// </summary>
        /// <param name="other">The collider of the object that entered the trigger zone.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Unit>(out Unit u))
        {
            unit = u; // Retrieves the PlayerController component from the player.
            if (unit != null) OnPlayerEnter(); // If a player is detected, triggers the OnPlayerEnter method.
        }
    }

    /// <summary>
    /// Called when the player exits the trigger zone of the interactable object.
    /// </summary>
    /// <param name="other">The collider of the object that exited the trigger zone.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Unit>(out Unit u))
        {
            OnPlayerExit(); // Calls the OnPlayerExit method to handle player exit.
            unit = null; // Nullifies the player reference when the player exits the trigger.
        }
    }

    /// <summary>
    /// Virtual method that can be overridden in derived classes to define behavior when the player enters the interactable object.
    /// </summary>
    protected virtual void OnPlayerEnter() { }

    /// <summary>
    /// Virtual method that can be overridden in derived classes to define behavior when the player exits the interactable object.
    /// </summary>
    protected virtual void OnPlayerExit() { }
}