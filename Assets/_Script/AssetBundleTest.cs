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
			StartCoroutine(LoadAssetBundle("test", loadCallBack1));
			StartCoroutine(LoadAssetBundle("player", loadCallBack2));
		} autoY += buttonH;

		//if (GUI.Button(new Rect(0, autoY, 200, buttonH), "LoadAssetBundle2"))
		//{
		//	StartCoroutine(LoadAssetBundle("player", loadCallBack2));
		//} autoY += buttonH;

		if (GUI.Button(new Rect(0, autoY, 200, buttonH), "Clear"))
		{
			ClearAll();
			Resources.UnloadUnusedAssets();
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

	IEnumerator LoadAssetBundle(string package, AssetBundleLoadDelegate loadCallBack)
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

	void ClearAll()
	{
		foreach (GameObject obj in objectList)
		{
			Destroy(obj);
		}
		objectList.Clear();
	}
}
