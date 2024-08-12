namespace StagePermutations;

static class Prefab
{
    private static readonly Transform _prefabParent;

    static Prefab()
    {
        _prefabParent = new GameObject(StagePermutationsProvider.GUID + "_Prefabs").transform;
        _prefabParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_prefabParent.gameObject);
        On.RoR2.Util.IsPrefab += (orig, gameObject) => orig(gameObject) || gameObject.transform.parent == _prefabParent;
    }

    public static GameObject Create(string name)
    {
        GameObject prefab = new GameObject(name);
        prefab.transform.SetParent(_prefabParent);
        return prefab;
    }

    public static GameObject Create(string name, params Type[] components)
    {
        GameObject prefab = new GameObject(name, components);
        prefab.transform.SetParent(_prefabParent);
        return prefab;
    }

    public static GameObject Clone(GameObject original, string name)
    {
        GameObject prefab = Object.Instantiate(original, _prefabParent);
        prefab.name = name;
        if (prefab.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            networkIdentity.m_AssetId.Reset();
        }
        return prefab;
    }
}
