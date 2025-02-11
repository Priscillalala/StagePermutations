using RoR2.Navigation;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace StagePermutations.goolake;

[RegisterPermutation("goolake", "Abandoned Aqueduct", "Second Aqueduct", description = "A second aqueduct structure will sometimes appear on Abandoned Aqueduct")]
public class SecondAqueduct : PermutationBehaviour, StagePermutationsPlugin.IAsyncInit
{
    const string GATE_NAME = "SecondaryAqueduct";

    public IEnumerator Init()
    {
        var goolakeGroundNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/goolake/goolakeGroundNodegraph.asset");
        var goolakeAirNodegraph = Addressables.LoadAssetAsync<NodeGraph>("RoR2/Base/goolake/goolakeAirNodegraph.asset");

        yield return goolakeGroundNodegraph;
        NodeGraph groundNodegraph = goolakeGroundNodegraph.Result;
        List<NodeGraph.NodeIndex> dest = [];
        Bounds bounds = new Bounds
        {
            min = new Vector3(210, -95, -260),
            max = new Vector3(230, -30, -224),
        };
        groundNodegraph.blockMap.GetItemsInBounds(bounds, dest);
        AssignNodesToGate(groundNodegraph, dest, GATE_NAME);

        yield return goolakeAirNodegraph;
        NodeGraph airNodegraph = goolakeAirNodegraph.Result;
        dest.Clear();
        airNodegraph.blockMap.GetItemsInBounds(bounds, dest);
        AssignNodesToGate(airNodegraph, dest, GATE_NAME);
    }


    public override void Apply(Scene scene, IDictionary<string, GameObject> rootObjects, SceneObjectToggleGroup toggleGroupController)
    {
        if (!rootObjects.TryGetValue("HOLDER: GameplaySpace", out GameObject gameplaySpaceHolder))
        {
            return;
        }
        if (!gameplaySpaceHolder.transform.TryFind("mdlGlDam/GL_AqueductPartial", out Transform GL_AqueductPartial))
        {
            return;
        }
        GameObject toggleableAqueduct = new GameObject("SecondaryAqueductToggle");
        toggleableAqueduct.AddComponent<GateStateSetter>().gateToDisableWhenEnabled = GATE_NAME;
        toggleableAqueduct.SetActive(false);
        toggleableAqueduct.AddComponent(out SetSceneObjectsActive setSceneObjectsActive);
        toggleableAqueduct.transform.localEulerAngles = new Vector3(0, 18.7f, 180f);
        Transform GL_AqueductPartial_2 = Object.Instantiate(GL_AqueductPartial.gameObject, toggleableAqueduct.transform).transform;
        GL_AqueductPartial_2.localPosition = new Vector3(-264.7861f, 94.7f, -158.4718f);
        GL_AqueductPartial_2.localScale = new Vector3(10f, 14.29f, 8f);
        if (GL_AqueductPartial_2.TryFind("GL_Waterfall/FoamOverParticles", out Transform FoamOverParticles))
        {
            FoamOverParticles.localScale = new Vector3(0.8f, 1f, 0.5f);
        }
        if (GL_AqueductPartial_2.TryFind("GL_Waterfall/SplashZone, Directional", out Transform SplashZoneDirectional))
        {
            SplashZoneDirectional.localPosition = SplashZoneDirectional.localPosition with { y = 2.5f };
        }
        if (rootObjects.TryGetValue("HOLDER: Warning Flags", out GameObject warningFlagsHolder))
        {
            setSceneObjectsActive.objectsToDeactivate.Add(warningFlagsHolder.transform.FindGameObject("GLWarningFlag"));
            setSceneObjectsActive.objectsToDeactivate.Add(warningFlagsHolder.transform.FindGameObject("GLWarningFlag (2)"));
        }
        if (gameplaySpaceHolder.transform.TryFind("Gates/BridgeOverGooOff", out Transform BridgeOverGooOff))
        {
            setSceneObjectsActive.objectsToActivate.Add(BridgeOverGooOff.gameObject);
        }
        ArrayUtils.ArrayAppend(ref toggleGroupController.toggleGroups[0].objects, toggleableAqueduct);
    }
}
