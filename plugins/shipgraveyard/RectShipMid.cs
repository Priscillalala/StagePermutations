using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace StagePermutations.shipgraveyard;

[RegisterPermutation("shipgraveyard", "Siren`s Call", "Crashed Ship Middle Section Variation", description = "The middle section of the long rectangular crashed ship on Siren`s Call will sometimes appear in a different orientation")]
public class RectShipMid : PermutationBehaviour, StagePermutationsPlugin.IAsyncInit
{
    const string GATE_NAME = "RectShipMid";

    public IEnumerator Init()
    {
        var shipgraveyardGroundNodeNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/shipgraveyard/shipgraveyardGroundNodeNodegraph.asset");
        var shipgraveyardAirNodeNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/shipgraveyard/shipgraveyardAirNodeNodegraph.asset");

        List<NodeGraph.NodeIndex> dest = [];
        Bounds bounds = new Bounds(new Vector3(-3.5f, 39f, 71f), new Vector3(25, 30, 41));
        yield return shipgraveyardGroundNodeNodegraph;
        NodeGraph groundNodegraph = shipgraveyardGroundNodeNodegraph.Result;
        groundNodegraph.blockMap.GetItemsInBounds(bounds, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME);
        dest.Clear();

        yield return shipgraveyardAirNodeNodegraph;
        NodeGraph airNodegraph = shipgraveyardAirNodeNodegraph.Result;
        airNodegraph.blockMap.GetItemsInBounds(bounds, dest);
        EnsureNodesInBounds(airNodegraph, dest, bounds);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME);
    }

    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: Environment", out GameObject environmentHolder))
        {
            return;
        }
        if (!environmentHolder.transform.TryFind("HOLDER: Ship Chunks/RectShipMid", out Transform RectShipMid))
        {
            return;
        }
        GameObject RectShipMidAlt = Object.Instantiate(Addressable<GameObject>("RoR2/Base/shipgraveyard/RectShipMid.prefab"));
        RectShipMidAlt.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME;
        RectShipMidAlt.SetActive(false);
        RectShipMidAlt.AddComponent<DisableOcclusionNearby>().radius = 200f;
        RectShipMidAlt.transform.localPosition = new Vector3(0, 47, 110);
        RectShipMidAlt.transform.localEulerAngles = new Vector3(10, 40, 0);
        RectShipMidAlt.AddComponent(out SetSceneObjectsActive setSceneObjectsActive);

        if (environmentHolder.transform.TryFind("HOLDER: Main Spikes", out Transform mainSpikes))
        {
            Transform overlappingSpike = mainSpikes.GetChild(6);
            if (overlappingSpike)
            {
                setSceneObjectsActive.objectsToDeactivate.Add(overlappingSpike.gameObject);
            }

            Transform newSpikeOriginal = mainSpikes.GetChild(2);
            if (newSpikeOriginal)
            {
                GameObject newSpike = Object.Instantiate(newSpikeOriginal.gameObject, newSpikeOriginal.parent);
                newSpike.transform.localPosition = new Vector3(-40, 10, 94);
                newSpike.transform.localScale = new Vector3(903.583f, 903.586f, 3921.5f);
                newSpike.SetActive(false);
                setSceneObjectsActive.objectsToActivate.Add(newSpike);
            }
        }

        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups, new GameObjectToggleGroup
        {
            objects = [RectShipMid.gameObject, RectShipMidAlt],
            minEnabled = 1,
            maxEnabled = 1,
        });
    }
}
