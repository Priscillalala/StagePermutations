namespace StagePermutations.skymeadow;

[RegisterPermutation("skymeadow", "Sky Meadow", "Mauling Rocks Direction Variation", description = "The mauling rocks on Sky Meadow will sometimes fly the opposite direction")]
public class MaulingRocks : PermutationBehaviour
{
    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Mauling Rocks", out GameObject maulingRocksHolder))
        {
            return;
        }
        GameObject altMaulingRocksHolder = Object.Instantiate(maulingRocksHolder);
        altMaulingRocksHolder.SetActive(false);
        foreach (MaulingRockZoneManager manager in altMaulingRocksHolder.GetComponentsInChildren<MaulingRockZoneManager>())
        {
            (manager.startZonePoint1, manager.endZonePoint1) = (manager.endZonePoint1, manager.startZonePoint1);
            (manager.startZonePoint2, manager.endZonePoint2) = (manager.endZonePoint2, manager.startZonePoint2);
        }
        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [maulingRocksHolder, altMaulingRocksHolder],
            minEnabled = 1,
            maxEnabled = 1,
        });
    }
}
