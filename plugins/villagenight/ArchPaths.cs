using UnityEngine.SceneManagement;

namespace StagePermutations.villagenight;

[RegisterPermutation("villagenight", "Disturbed Impact", "Arch Paths Variation", description = "A set of smaller temple arches will sometimes appear")]
public class ArchPaths : PermutationBehaviour
{
    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("TOGGLE: ArchPaths", out GameObject archPathsToggle))
        {
            return;
        }
        Transform LVArc_ArchToggle = archPathsToggle.transform.Find("LVArc_ArchToggle");
        if (!LVArc_ArchToggle)
        {
            return;
        }
        Addressable("RoR2/DLC2/villagenight/Assets/LVNBrazierNoChain.prefab", out GameObject LVNBrazierNoChain);

        GameObject InstantiateBrazier(Vector3 position, Quaternion rotation)
        {
            GameObject instance = Object.Instantiate(LVNBrazierNoChain, position, rotation, LVArc_ArchToggle);
            instance.layer = LayerIndex.world.intVal;
            instance.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
            return instance;
        }

        InstantiateBrazier(new Vector3(148.8f, 23.4f, -51.5f), Quaternion.Euler(-9, 5, 6));
        InstantiateBrazier(new Vector3(118.3f, 11.2f, -64.7f), Quaternion.Euler(-9, 0, 6));

        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [LVArc_ArchToggle.gameObject],
            minEnabled = 0,
            maxEnabled = 1,
        });
    }
}
