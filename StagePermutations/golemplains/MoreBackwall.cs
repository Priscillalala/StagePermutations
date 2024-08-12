namespace StagePermutations.golemplains;

[RegisterPermutation("golemplains", "Titanic Plains", "More Backwall")]
public class MoreBackwall : PermutationBehaviour, StagePermutationsProvider.IStaticContent
{
    const string GATE_NAME = "BlockedByFullColossusHead";

    public IEnumerator LoadAsync(IProgress<float> progressReceiver)
    {
        var golemplainsGroundNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/golemplains/golemplainsGroundNodesNodegraph.asset");
        var golemplainsAirNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/golemplains/golemplainsAirNodesNodegraph.asset");

        List<NodeGraph.NodeIndex> dest = [];

        yield return golemplainsGroundNodesNodegraph;
        NodeGraph groundNodegraph = golemplainsGroundNodesNodegraph.Result;
        groundNodegraph.blockMap.GetItemsInBounds(new Bounds(new Vector3(-70, -140, -362), new Vector3(100, 40, 55)), dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(50, -120, -380), 100, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(-33.5f, -152.7f, -332.2f), 3, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME);
        dest.Clear();

        yield return golemplainsAirNodesNodegraph;
        NodeGraph airNodegraph = golemplainsAirNodesNodegraph.Result;
        airNodegraph.blockMap.GetItemsInBounds(new Bounds(new(-70, -140, -362), new(100, 40, 60)), dest);
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(50, -120, -380), 100, dest);
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(-96, -122, -397), 20, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME);
    }

    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject.CreatePrimitive(PrimitiveType.Sphere);
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