namespace StagePermutations.rootjungle;

[RegisterPermutation("rootjungle", "Sundered Grove", "Mushroom Platform Variation", description = "A mushroom platform on Sundered Grove will sometimes disappear")]
public class ShroomShelf : PermutationBehaviour, StagePermutationsProvider.IStaticContent
{
    const string GATE_NAME = "OnShroomShelf";

    public IEnumerator LoadAsync(IProgress<float> progressReceiver)
    {
        var rootjungleGroundNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/rootjungle/rootjungleGroundNodesNodegraph.asset");
        var rootjungleAirNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/rootjungle/rootjungleAirNodesNodegraph.asset");

        List<NodeGraph.NodeIndex> dest = [];

        yield return rootjungleGroundNodesNodegraph;
        NodeGraph groundNodegraph = rootjungleGroundNodesNodegraph.Result;
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(63, 30, 90), 30f, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME);
        dest.Clear();

        yield return rootjungleAirNodesNodegraph;
        NodeGraph airNodegraph = rootjungleAirNodesNodegraph.Result;
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(63, 30, 90), 30f, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME);
    }

    public override SceneObjectToggleGroup FindToggleGroupController(Scene scene, IDictionary<string, GameObject> rootObjects)
    {
        if (rootObjects.TryGetValue("SceneInfo", out GameObject sceneInfo))
        {
            return sceneInfo.transform.Find("SceneObjectToggleGroup")?.GetComponent<SceneObjectToggleGroup>();
        }
        return null;
    }

    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Props", out GameObject propsHolder))
        {
            return;
        }
        if (!propsHolder.transform.TryFind("GROUP: Mushrooms/RJShroomBig", out Transform RJShroomBig))
        {
            return;
        }
        GameObject disabled = new GameObject("RJShroomBigDisabled");
        disabled.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME;
        disabled.SetActive(false);
        if (rootObjects.TryGetValue("HOLDER: Foliage", out GameObject foliageHolder) && foliageHolder.transform.TryFind("GROUP: ShroomFoliage", out Transform shroomFoliageGroup))
        {
            disabled.AddComponent(out SetSceneObjectsActive setSceneObjectsActive);

            foreach (Transform shroomFoliage in shroomFoliageGroup)
            {
                if ((shroomFoliage.position - new Vector3(63, 30, 90)).sqrMagnitude <= 1600f)
                {
                    setSceneObjectsActive.objectsToDeactivate.Add(shroomFoliage.gameObject);
                }
            }
        }

        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [RJShroomBig.gameObject, disabled],
            minEnabled = 1,
            maxEnabled = 1,
        });
    }
}
