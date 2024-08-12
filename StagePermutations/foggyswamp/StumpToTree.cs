namespace StagePermutations.foggyswamp;

[RegisterPermutation("foggyswamp", "Wetland Aspect", "Stump to Tree", description = "Sometimes the large tree stump on Wetland Aspect will be replaced by a full tree")]
public class StumpToTree : PermutationBehaviour
{
    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Tree Trunks w Collision", out GameObject treeTrunksHolder))
        {
            return;
        }
        if (!treeTrunksHolder.transform.TryFind("CompositeTreeTrunk", out Transform CompositeTreeTrunk) || !treeTrunksHolder.transform.TryFind("FSTreeTrunkStumpCollision (5)", out Transform FSTreeTrunkStumpCollision))
        {
            return;
        }
        GameObject treeTrunk = Object.Instantiate(CompositeTreeTrunk.gameObject, CompositeTreeTrunk.parent);
        treeTrunk.SetActive(false);
        treeTrunk.transform.localPosition = new Vector3(-117, -151, -241);
        treeTrunk.transform.Find("FSTreeTrunkEnormousCollision (2)")?.gameObject.SetActive(false);
        if (treeTrunk.transform.TryFind("FSTreeTrunkEnormousCollision", out Transform FSTreeTrunkEnormousCollision))
        {
            FSTreeTrunkEnormousCollision.localPosition = new Vector3(0, -5, 0);
            FSTreeTrunkEnormousCollision.localEulerAngles = new Vector3(5, 340, 7);
            FSTreeTrunkEnormousCollision.localScale = Vector3.one * 4.3f;
        }
        if (treeTrunk.transform.TryFind("FSRootBundleLargeCollision (1)", out Transform FSRootBundleLargeCollision))
        {
            FSRootBundleLargeCollision.localPosition = new Vector3(-3, -8, 6);
            FSRootBundleLargeCollision.localEulerAngles = new Vector3(80, 69, 3);
        }
        if (treeTrunk.transform.TryFind("FSTreeTrunkEnormousNoCollisionSkybox", out Transform FSTreeTrunkEnormousNoCollisionSkybox))
        {
            FSTreeTrunkEnormousNoCollisionSkybox.localPosition = new Vector3(-15, 0, 0);
            FSTreeTrunkEnormousNoCollisionSkybox.localEulerAngles = new Vector3(0, 10, 357);
        }
        if (treeTrunk.transform.TryFind("FSRootBundleSmallNoCollision", out Transform FSRootBundleSmallNoCollision))
        {
            FSRootBundleSmallNoCollision.localPosition = new Vector3(2, 2, -17);
        }
        treeTrunk.AddComponent<DisableOcclusionNearby>().radius = 50f;
        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [FSTreeTrunkStumpCollision.gameObject, treeTrunk],
            minEnabled = 1,
            maxEnabled = 1,
        });
    }
}
