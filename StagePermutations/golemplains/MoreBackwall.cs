namespace StagePermutations.golemplains;

[RegisterPermutation("golemplains", "Titanic Plains", "More Backwall")]
public class MoreBackwall : PermutationBehaviour, StagePermutationsProvider.IStaticContent
{
    const string GATE_NAME = "BlockedByFullColossusHead";

    public IEnumerator LoadAsync(IProgress<float> progressReceiver)
    {
        yield break;
        /*var golemplains2GroundNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/golemplains2/golemplains2GroundNodesNodegraph.asset");
        var golemplains2AirNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/golemplains2/golemplains2AirNodesNodegraph.asset");

        List<NodeGraph.NodeIndex> dest = [];

        yield return golemplains2GroundNodesNodegraph;
        NodeGraph groundNodegraph = golemplains2GroundNodesNodegraph.Result;
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(236.6f, 43.9f, 45.2f), 5f, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(245.7f, 45f, 45f), 5f, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(251f, 45f, 35f), 5f, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(250f, 43f, 25.6f), 5f, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME);
        dest.Clear();
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(-28.8f, 47.6f, -154f), 30f, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME_FULL);
        dest.Clear();

        yield return golemplains2AirNodesNodegraph;
        NodeGraph airNodegraph = golemplains2AirNodesNodegraph.Result;
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(235f, 60f, 33f), 20f, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME);
        dest.Clear();
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(-23f, 70f, -155f), 30f, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME_FULL);*/
    }


    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        GameObject backwall = Object.Instantiate(Addressable<GameObject>("RoR2/Base/dampcave/DCTerrainBackwall.prefab"));
        backwall.transform.localPosition = new Vector3(253f, -42f, -310f);
        backwall.transform.localEulerAngles = new Vector3(270, 112, 0);
        backwall.GetComponent<MeshRenderer>().sharedMaterial = Addressable<Material>("RoR2/Base/golemplains/matGPTerrain.mat");
        backwall.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME;
        backwall.SetActive(false);
        if (rootObjects.TryGetValue("HOLDER: Ruined Pieces", out GameObject ruinedPiecesHolder) && ruinedPiecesHolder.transform.TryFind("GPRuinedRing1 (3)", out Transform ruinedRing))
        {
            SetSceneObjectsActive setSceneObjectsActive = backwall.AddComponent<SetSceneObjectsActive>();
            setSceneObjectsActive.objectsToDeactivate = [ruinedRing.gameObject];
        }
        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [backwall],
            minEnabled = 0,
            maxEnabled = 1,
        });
    }
}