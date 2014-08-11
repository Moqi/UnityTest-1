using UnityEngine;
using System.Collections;

public class AssetBundleTest : MonoBehaviour {

	private string localRoot = "";
	private string bundleExt = "assetbundle";

	void Start () {
		localRoot = 
#if UNITY_EDITOR
		"file://" + Application.streamingAssetsPath;
#else
		Application.streamingAssetsPath;
#endif

		Screen.SetResolution(600, 960, true);
	}

	void Update()
	{
		;
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
			StartCoroutine(LoadAssetBundle("test"));
		} autoY += buttonH;
	}

	void LoadResources()
	{
		float startTime = Time.realtimeSinceStartup;

		Object preAtomBall = Resources.Load("pre_AtomBall");
		Instantiate(preAtomBall);

		Object preSpikeBall = Resources.Load("pre_SpikeBall");
		Instantiate(preSpikeBall);

		float endTime = Time.realtimeSinceStartup;
		Debug.Log(string.Format("LoadResources Cost: {0}", endTime - startTime));
	}

	IEnumerator LoadAssetBundle(string package)
	{
		float startTime = Time.realtimeSinceStartup;
		while (!Caching.ready)
			yield return null;

		string loadPath = string.Format("{0}/{1}.{2}",
			localRoot, package, bundleExt);
		Debug.Log(loadPath);

		using (WWW www = WWW.LoadFromCacheOrDownload(loadPath, 1))
		{
			yield return www;

			AssetBundle assetBundle = www.assetBundle;

			Object preAtomBall = assetBundle.Load("pre_AtomBall");
			Instantiate(preAtomBall);

			Object preSpikeBall = assetBundle.Load("pre_SpikeBall");
			Instantiate(preSpikeBall);

			assetBundle.Unload(false);

			www.Dispose();
		}
		float endTime = Time.realtimeSinceStartup;
		Debug.Log(string.Format("LoadAssetBundle Cost: {0}", endTime - startTime));
		yield return null;
	}

}
