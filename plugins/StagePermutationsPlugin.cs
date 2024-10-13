using BepInEx.Configuration;
using RoR2.Navigation;
using System.Security;
using System.Security.Permissions;
using UnityEngine.SceneManagement;
using SearchableAttribute = HG.Reflection.SearchableAttribute;

[module: UnverifiableCode]
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: SearchableAttribute.OptIn]

namespace StagePermutations;

[BepInPlugin(GUID, NAME, VERSION)]
public class StagePermutationsPlugin : BaseUnityPlugin
{
    public const string
        GUID = "groovesalad." + NAME,
        NAME = "StageVariety",
        VERSION = "1.0.0";

    public static Dictionary<PermutationBehaviour, RegisterPermutationAttribute> Permutations { get; private set; }

    public ILookup<string, PermutationBehaviour> permutationsLookup;

    public void Awake()
    {
        List<RegisterPermutationAttribute> attributes = [];
        SearchableAttribute.GetInstances(attributes);
        Permutations = attributes
            .Where(x => Config.Bind(x.configSection, $"Enable {x.name}", true, x.description != null ? new ConfigDescription(x.description) : null).Value)
            .ToDictionary(x => (PermutationBehaviour)Activator.CreateInstance((Type)x.target));
        permutationsLookup = Permutations.ToLookup(x => x.Value.targetSceneName, x => x.Key);

        SceneManager.sceneLoaded += OnSceneLoaded;
#if DEBUG
        On.RoR2.Navigation.NodeGraph.GenerateLinkDebugMesh += NodeGraph_GenerateLinkDebugMesh;
#endif
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
    {
        if (permutationsLookup.Contains(newScene.name))
        {
            Dictionary<string, GameObject> rootObjects = [];
            foreach (GameObject rootObject in newScene.GetRootGameObjects())
            {
                rootObjects[rootObject.name] = rootObject;
            }
            foreach (PermutationBehaviour permutation in permutationsLookup[newScene.name])
            {
                SceneObjectToggleGroup toggleGroupController = permutation.FindToggleGroupController(newScene, rootObjects);
                permutation.Apply(newScene, rootObjects, toggleGroupController);
            }
        }
    }

#if DEBUG
    private static Mesh NodeGraph_GenerateLinkDebugMesh(On.RoR2.Navigation.NodeGraph.orig_GenerateLinkDebugMesh orig, NodeGraph self, HullMask hullMask)
    {
        using WireMeshBuilder wireMeshBuilder = new WireMeshBuilder();
        NodeGraph.Link[] array = self.links;
        for (int i = 0; i < array.Length; i++)
        {
            NodeGraph.Link link = array[i];
            if (((uint)link.hullMask & (uint)hullMask) == 0)
            {
                continue;
            }
            Vector3 position = self.nodes[link.nodeIndexA.nodeIndex].position;
            Vector3 position2 = self.nodes[link.nodeIndexB.nodeIndex].position;
            Vector3 val = (position + position2) * 0.5f;
            bool jump = ((uint)link.jumpHullMask & (uint)hullMask) != 0;
            Color color = jump ? Color.cyan : Color.green;
            if (link.gateIndex != 0)
            {
                color = self.openGates[link.gateIndex] ? (jump ? Color.gray : Color.blue) : (jump ? Color.magenta : Color.red);
            }
            if (jump)
            {
                Vector3 apexPos = val;
                apexPos.y = position.y + link.minJumpHeight;
                int num2 = 8;
                Vector3 p = position;
                for (int j = 1; j <= num2; j++)
                {
                    if (j > num2 / 2)
                    {
                        color.a = 0.1f;
                    }
                    Vector3 quadraticCoordinates = self.GetQuadraticCoordinates((float)j / (float)num2, position, apexPos, position2);
                    wireMeshBuilder.AddLine(p, color, quadraticCoordinates, color);
                    p = quadraticCoordinates;
                }
            }
            else
            {
                Color c = color;
                c.a = 0.1f;
                wireMeshBuilder.AddLine(position, color, (position + position2) * 0.5f, c);
            }
        }
        return wireMeshBuilder.GenerateMesh();
    }
#endif

    [SystemInitializer]
    private static IEnumerator Init()
    {
        List<IEnumerator> initAsyncCoroutines = Permutations.Keys.OfType<IAsyncInit>().Select(x => SafeCoroutineWrapper(x.Init())).ToList();
        while (initAsyncCoroutines.Count > 0)
        {
            for (int i = initAsyncCoroutines.Count - 1; i >= 0; i--)
            {
                IEnumerator coroutine = initAsyncCoroutines[i];
                if (coroutine.MoveNext())
                {
                    yield return coroutine.Current;
                }
                else
                {
                    initAsyncCoroutines.RemoveAt(i);
                }
            }
        }

        static IEnumerator SafeCoroutineWrapper(IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
            {
                switch (coroutine.Current)
                {
                    case IEnumerator inner:
                        while (inner.MoveNext()) yield return inner.Current;
                        break;
                    case AsyncOperation asyncOperation:
                        while (!asyncOperation.isDone) yield return null;
                        break;
                    default:
                        yield return coroutine.Current;
                        break;
                }
            }
        }
    }

    public interface IAsyncInit
    {
        public IEnumerator Init();
    }
}