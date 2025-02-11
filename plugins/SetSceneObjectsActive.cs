namespace StagePermutations;

public class SetSceneObjectsActive : MonoBehaviour
{
    public List<GameObject> objectsToActivate = [];
    public List<GameObject> objectsToDeactivate = [];

    public void OnEnable()
    {
        foreach (GameObject toActivate in objectsToActivate)
        {
            if (toActivate)
            {
                toActivate.SetActive(true);
            }
        }
        foreach (GameObject toDeactivate in objectsToDeactivate)
        {
            if (toDeactivate)
            {
                toDeactivate.SetActive(false);
            }
        }
    }
}
