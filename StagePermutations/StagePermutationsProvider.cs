global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Linq;
global using UnityEngine;
global using UnityEngine.Networking;
global using UnityEngine.SceneManagement;
global using UnityEngine.AddressableAssets;
global using UnityEngine.ResourceManagement.AsyncOperations;
global using BepInEx;
global using BepInEx.Configuration;
global using MonoMod.Cil;
global using Mono.Cecil.Cil;
global using RoR2;
global using RoR2.Navigation;
global using HG;
global using Object = UnityEngine.Object;
global using SearchableAttribute = HG.Reflection.SearchableAttribute;
using System.Security.Permissions;
using System.Security;
using RoR2.ContentManagement;
using HG.Coroutines;

[module: UnverifiableCode]
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: SearchableAttribute.OptIn]

namespace StagePermutations;

[BepInPlugin(GUID, NAME, VERSION)]
public class StagePermutationsProvider : BaseUnityPlugin, IContentPackProvider
{
    public const string
        GUID = "groovesalad." + NAME,
        NAME = "StageVariety",
        VERSION = "1.0.0";

    public string identifier => GUID;

    public AssetBundleCreateRequest assetBundleCreateRequest;
    public Dictionary<PermutationBehaviour, RegisterPermutationAttribute> permutations;
    public static ILookup<string, PermutationBehaviour> permutationsLookup;
    //public ContentPack contentPack;

    public void Awake()
    {
        ContentManager.collectContentPackProviders += add => add(this);

        List<RegisterPermutationAttribute> attributes = [];
        SearchableAttribute.GetInstances(attributes);
        permutations = attributes
            .Where(x => Config.Bind(x.configSection, $"Enable {x.name}", true, x.description != null ? new ConfigDescription(x.description) : null).Value)
            .ToDictionary(x => (PermutationBehaviour)Activator.CreateInstance((Type)x.target));
        permutationsLookup = permutations.ToLookup(x => x.Value.targetSceneName, x => x.Key);

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

    public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
    {
        ParallelProgressCoroutine parallelProgressCoroutine = new ParallelProgressCoroutine(new ReadableProgress<float>(args.ReportProgress));
        foreach (IStaticContent staticContent in permutations.Keys.OfType<IStaticContent>())
        {
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
            ReadableProgress<float> progressReceiver = new();
            parallelProgressCoroutine.Add(SafeCoroutineWrapper(staticContent.LoadAsync(progressReceiver)), progressReceiver);
        }
        while (parallelProgressCoroutine.MoveNext()) yield return parallelProgressCoroutine.Current;
    }

    public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
    {
        //ContentPack.Copy(contentPack, args.output);
        yield break;
    }

    public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
    {
        yield break;
    }

    public interface IStaticContent
    {
        public IEnumerator LoadAsync(IProgress<float> progressReceiver);
    }
}