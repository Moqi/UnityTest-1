using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleTest : MonoBehaviour {

	private static string localRoot =
#if UNITY_EDITOR
		"file://" + Application.streamingAssetsPath;
#elif UNITY_ANDROID
		Application.streamingAssetsPath;
#else
		"file://" + Application.streamingAssetsPath;
#endif

	private static string bundleExt = "assetbundle";

	delegate IEnumerator AssetBundleLoadDelegate(AssetBundle assetBundle);
	List<GameObject> objectList = new List<GameObject>();

	int totalTask = 0;
	int loadTask = 0;
	float asyncTaskStartTime = 0;

	Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();


	void Awake () {
		Screen.SetResolution(600, 960, true);
	}

	void OnGUI ()
	{
		float autoY = 0f;
		float buttonH = 50f;
		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "LoadResources"))
		{
			LoadResources();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "LoadAssetBundle"))
		{
			OnAsyncTaskStart();
			StartCoroutine(LoadABResource("test", loadCallBack1));
			StartCoroutine(LoadABResource("player", loadCallBack2));
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "LoadAssetBundleWithBuffer"))
		{
			OnAsyncTaskStart();
			StartCoroutine(LoadABResourceWithBuffer("test", loadCallBack1));
			StartCoroutine(LoadABResourceWithBuffer("player", loadCallBack2));
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "ClearAssetBundleBuffer"))
		{
			ResetAssetBundleBuffer();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "ClearObjects"))
		{
			ClearObjects();
		} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "Reset"))
		{
			ClearObjects();
			Resources.UnloadUnusedAssets();
			ResetAssetBundleBuffer();
		} autoY += buttonH;
	}

	void LoadResources()
	{
		float startTime = Time.realtimeSinceStartup;

		Object preAtomBall = Resources.Load("pre_AtomBall");
		objectList.Add(Instantiate(preAtomBall) as GameObject);

		Object preSpikeBall = Resources.Load("pre_SpikeBall");
		objectList.Add(Instantiate(preSpikeBall) as GameObject);

		Object prePlayer = Resources.Load("pre_Player");
		objectList.Add(Instantiate(prePlayer) as GameObject);

		float endTime = Time.realtimeSinceStartup;
		Debug.Log(string.Format("LoadResources Cost: {0}", endTime - startTime));
	}

	IEnumerator LoadABResource(string package, AssetBundleLoadDelegate loadCallBack)
	{
		while (!Caching.ready)
			yield return null;

		string loadPath = string.Format("{0}/{1}.{2}",
			localRoot, package, bundleExt);
		Debug.Log(loadPath);

		using (WWW www = WWW.LoadFromCacheOrDownload(loadPath, 1))
		{
			yield return www;

			AssetBundle assetBundle = www.assetBundle;

			yield return StartCoroutine(loadCallBack(assetBundle));

			assetBundle.Unload(false);

			www.Dispose();
		}
		yield return null;
	}

	IEnumerator loadCallBack1(AssetBundle assetBundle)
	{
		Object preAtomBall = assetBundle.Load("pre_AtomBall");
		objectList.Add(Instantiate(preAtomBall) as GameObject);
		OnLoadDone();
		yield return null;

		Object preSpikeBall = assetBundle.Load("pre_SpikeBall");
		objectList.Add(Instantiate(preSpikeBall) as GameObject);
		OnLoadDone();
		yield return null;
	}

	IEnumerator loadCallBack2(AssetBundle assetBundle)
	{
		Object prePlayer = assetBundle.Load("pre_Player");
		objectList.Add(Instantiate(prePlayer) as GameObject);
		OnLoadDone();
		yield return null;
	}

	void OnAsyncTaskStart()
	{
		totalTask = 3;
		loadTask = 0;
		asyncTaskStartTime = Time.realtimeSinceStartup;
	}

	void OnLoadDone()
	{
		loadTask += 1;
		if (loadTask >= totalTask)
		{
			float endTime = Time.realtimeSinceStartup;
			Debug.Log(string.Format("LoadAssetBundle Cost: {0}", endTime - asyncTaskStartTime));
		}
	}

	IEnumerator LoadAssetBundle(string package)
	{
		while (!Caching.ready)
			yield return null;

		string loadPath = string.Format("{0}/{1}.{2}",
			localRoot, package, bundleExt);
		Debug.Log(loadPath);

		using (WWW www = WWW.LoadFromCacheOrDownload(loadPath, 1))
		{
			yield return www;

			AssetBundle assetBundle = www.assetBundle;

			assetBundleDict[package] = assetBundle;

			www.Dispose();
		}
		yield return null;
	}

	IEnumerator LoadABResourceWithBuffer(string package, AssetBundleLoadDelegate loadCallBack)
	{
		if (!assetBundleDict.ContainsKey(package))
			yield return StartCoroutine(LoadAssetBundle(package));

		AssetBundle assetBundle = null;
		assetBundleDict.TryGetValue(package, out assetBundle);
		if (assetBundle == null) yield break;

		yield return StartCoroutine(loadCallBack(assetBundle));
	}

	void ResetAssetBundleBuffer()
	{
		foreach (KeyValuePair<string, AssetBundle> itor in assetBundleDict)
		{
			(itor.Value).Unload(false);
		}
		assetBundleDict.Clear();
	}

	void ClearObjects()
	{
		foreach (GameObject obj in objectList)
		{
			Destroy(obj);
		}
		objectList.Clear();
	}
}
