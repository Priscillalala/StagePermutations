using UnityEngine.SceneManagement;

namespace StagePermutations.village;

[RegisterPermutation("village", "Shattered Abodes", "Arch Paths Variation", description = "A set of smaller temple arches will sometimes appear")]
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
        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [LVArc_ArchToggle.gameObject],
            minEnabled = 0,
            maxEnabled = 1,
        });
    }
}
