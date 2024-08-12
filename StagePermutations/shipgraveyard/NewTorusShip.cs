namespace StagePermutations.shipgraveyard;

[RegisterPermutation("shipgraveyard", "Siren`s Call", "New Donut Ship", description = "A new crashed ship will sometimes appear in the middle of Siren`s Call")]
public class NewTorusShip : PermutationBehaviour, StagePermutationsProvider.IStaticContent
{
    const string GATE_NAME = "BlockedByTorusShip";

    public IEnumerator LoadAsync(IProgress<float> progressReceiver)
    {
        var shipgraveyardGroundNodeNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/shipgraveyard/shipgraveyardGroundNodeNodegraph.asset");
        var shipgraveyardAirNodeNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/shipgraveyard/shipgraveyardAirNodeNodegraph.asset");
        
        List<NodeGraph.NodeIndex> dest = [];

        yield return shipgraveyardGroundNodeNodegraph;
        NodeGraph groundNodegraph = shipgraveyardGroundNodeNodegraph.Result;
        groundNodegraph.blockMap.GetItemsInSphere(new Vector3(-15, 3, 14), 10, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME);
        dest.Clear();

        yield return shipgraveyardAirNodeNodegraph;
        NodeGraph airNodegraph = shipgraveyardAirNodeNodegraph.Result;
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(-15, 15, 11), 15, dest);
        airNodegraph.blockMap.GetItemsInSphere(new Vector3(18, 15, 59), 15, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME);
    }

    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        GameObject TorusShipRound = Object.Instantiate(Addressable<GameObject>("RoR2/Base/shipgraveyard/TorusShip Round.prefab"));
        TorusShipRound.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME;
        TorusShipRound.SetActive(false);
        TorusShipRound.transform.localPosition = new Vector3(2, 5, 41);
        TorusShipRound.transform.localEulerAngles = new Vector3(340, 300, 230);
        if (rootObjects.TryGetValue("HOLDER: Environment", out GameObject environmentHolder) && environmentHolder.transform.TryFind("TO BE CONVERTED!!!!!!!!!!/HOLDER: Main Rocks", out Transform mainRocks))
        {
            TorusShipRound.AddComponent(out SetSceneObjectsActive setSceneObjectsActive);
            setSceneObjectsActive.objectsToDeactivate.Add(mainRocks.GetChild(27).gameObject);
        }
        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [TorusShipRound],
            minEnabled = 0,
            maxEnabled = 1,
        });
    }
}
