using UnityEngine;

public class StartingZoneController : MonoBehaviour
{
    public delegate void SomeAction();
    public static event SomeAction ExitingFromStartingZone;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitingFromStartingZone();
            Destroy(gameObject);
        }
    }
}
