using UnityEngine;

public class DetectorEscalera : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LadderTop"))
            player.TocarLadderTop(other);

        else if (other.CompareTag("LadderBottom"))
            player.TocarLadderBottom(other);
    }
}
