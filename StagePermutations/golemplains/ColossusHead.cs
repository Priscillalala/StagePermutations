namespace StagePermutations.golemplains;

[RegisterPermutation("golemplains2", "Titanic Plains", "Colossus Heads")]
public class ColossusHead : PermutationBehaviour, StagePermutationsProvider.IStaticContent
{
    const string GATE_NAME_RUINED = "BlockedByRuinedColossusHead";
    const string GATE_NAME_FULL = "BlockedByFullColossusHead";

    public IEnumerator LoadAsync(IProgress<float> progressReceiver)
    {
        var golemplains2GroundNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/golemplains2/golemplains2GroundNodesNodegraph.asset");
        var golemplains2AirNodesNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/golemplains2/golemplains2AirNodesNodegraph.asset");

        List<NodeGraph.NodeIndex> dest = [];

        yield return golemplains2GroundNodesNodegraph;
        NodeGraph groundNodegraph = golemplains2GroundNodesNodegraph.Result;
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(236.6f, 43.9f, 45.2f), 5f, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(245.7f, 45f, 45f), 5f, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(251f, 45f, 35f), 5f, dest);
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(250f, 43f, 25.6f), 5f, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME_RUINED);
        dest.Clear();
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(-28.8f, 47.6f, -154f), 30f, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME_FULL);
        dest.Clear();

        yield return golemplains2AirNodesNodegraph;
        NodeGraph airNodegraph = golemplains2AirNodesNodegraph.Result;
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(235f, 60f, 33f), 20f, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME_RUINED);
        dest.Clear();
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(-23f, 70f, -155f), 30f, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME_FULL);
    }

    public Material matColossusHeadMossy;

    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!matColossusHeadMossy)
        {
            Addressable("RoR2/Base/golemplains/matGPBoulderHeavyMoss.mat", out Material matGPBoulderHeavyMoss);
            matColossusHeadMossy = new Material(matGPBoulderHeavyMoss);
            matColossusHeadMossy.SetTexture("_NormalTex", Addressable<Texture>("RoR2/Junk/slice2/texColossusHead1Normal.png"));
            matColossusHeadMossy.SetTextureScale("_NormalTex", new Vector2(1, 1));
            matColossusHeadMossy.SetTextureScale("_MainTex", new Vector2(3, 8));
            matColossusHeadMossy.SetTextureScale("_SnowTex", new Vector2(10, 10));
            matColossusHeadMossy.SetFloat("_SnowBias", .14f);
            matColossusHeadMossy.SetFloat("_Depth", .3f);
            matColossusHeadMossy.SetInt("_RampInfo", 3);
        }
        ApplyRuinedHead(rootObjects, toggleGroupController);
        ApplyFullHead(rootObjects, toggleGroupController);
    }

    public void ApplyRuinedHead(IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Ruins", out GameObject ruinsHolder))
        {
            return;
        }
        if (!ruinsHolder.transform.TryFind("GPRuinedRing1 (1)", out Transform ruinedRing))
        {
            return;
        }

        GameObject RaidColossusHeadDestroyed = Object.Instantiate(Addressable<GameObject>("RoR2/DLC1/voidraid/mdlRaidColossusHeadDestroyed.fbx"));
        Transform mdlRaidColossusHeadDestroyedChild = RaidColossusHeadDestroyed.transform.Find("mdlRaidColossusHeadDestroyed");
        mdlRaidColossusHeadDestroyedChild.localPosition = Vector3.zero;
        mdlRaidColossusHeadDestroyedChild.localEulerAngles = Vector3.zero;
        mdlRaidColossusHeadDestroyedChild.localScale = Vector3.one * 5f;
        mdlRaidColossusHeadDestroyedChild.GetComponent<MeshRenderer>().sharedMaterial = matColossusHeadMossy;
        mdlRaidColossusHeadDestroyedChild.gameObject.layer = LayerIndex.world.intVal;
        mdlRaidColossusHeadDestroyedChild.gameObject.AddComponent<MeshCollider>();
        RaidColossusHeadDestroyed.transform.localPosition = new Vector3(235f, 65.1f, 30f);
        RaidColossusHeadDestroyed.transform.localEulerAngles = new Vector3(270, 300, 0);
        RaidColossusHeadDestroyed.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME_RUINED;
        RaidColossusHeadDestroyed.SetActive(false);

        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [ruinedRing.gameObject, RaidColossusHeadDestroyed],
            minEnabled = 1,
            maxEnabled = 1,
        });
    }

    public void ApplyFullHead(IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Columns", out GameObject columnsHolder))
        {
            return;
        }
        Transform GPTerrainColumn = columnsHolder.transform.AllChildren().Where(x => x.name == "GPTerrainColumn").ElementAtOrDefault(2);
        if (!GPTerrainColumn)
        {
            return;
        }
        GameObject GPTerrainColumnLower = Object.Instantiate(GPTerrainColumn.gameObject, GPTerrainColumn.parent);
        GPTerrainColumnLower.transform.localPosition = GPTerrainColumn.transform.localPosition with { y = 0 };
        GPTerrainColumnLower.SetActive(false);

        Transform colossusHead1Parent = new GameObject("colossusHead1Parent").transform;
        GameObject ColossusHead1 = Object.Instantiate(Addressable<GameObject>("RoR2/Base/Common/Props/mdlColossusHead1.fbx"), colossusHead1Parent);
        ColossusHead1.GetComponent<MeshRenderer>().sharedMaterial = matColossusHeadMossy;
        ColossusHead1.layer = LayerIndex.world.intVal;
        ColossusHead1.AddComponent<MeshCollider>();
        ColossusHead1.transform.localScale = Vector3.one * 5f;
        ColossusHead1.transform.localEulerAngles = new Vector3(270, 0, 0);
        colossusHead1Parent.localPosition = new Vector3(-35, 38, -152);
        colossusHead1Parent.localEulerAngles = new Vector3(355, 75, 350);
        colossusHead1Parent.gameObject.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME_FULL;
        colossusHead1Parent.gameObject.SetActive(false);
        SetSceneObjectsActive setSceneObjectsActive = colossusHead1Parent.gameObject.AddComponent<SetSceneObjectsActive>();
        setSceneObjectsActive.objectsToDeactivate = [GPTerrainColumn.gameObject];
        setSceneObjectsActive.objectsToActivate = [GPTerrainColumnLower];

        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups[3].objects, colossusHead1Parent.gameObject);
    }
}