namespace StagePermutations.foggyswamp;

[RegisterPermutation("foggyswamp", "Wetland Aspect", "More Ruin Rings", description = "Two more floating ruin rings will sometimes appear on Wetland Aspect")]
public class MoreRings : PermutationBehaviour
{
    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Tree Trunks w Collision", out GameObject treeTrunksHolder))
        {
            return;
        }
        if (!treeTrunksHolder.transform.TryFind("FSTreeTrunkLongCollision (1)/FSRuinRingCollision", out Transform FSRuinRingCollision))
        {
            return;
        }
        GameObject ruinRing1 = Object.Instantiate(FSRuinRingCollision.gameObject, new Vector3(-5, 0, 26), Quaternion.Euler(5, 0, 90));
        ruinRing1.SetActive(false);
        ruinRing1.transform.localScale = Vector3.one * 22f;

        GameObject ruinRing2 = Object.Instantiate(FSRuinRingCollision.gameObject, new Vector3(-39, 65, -82), Quaternion.Euler(0, 0, 90));
        ruinRing2.SetActive(false);
        ruinRing2.transform.localScale = Vector3.one * 22f;

        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [ruinRing1, ruinRing2],
            minEnabled = 0,
            maxEnabled = 2,
        });
    }
}
