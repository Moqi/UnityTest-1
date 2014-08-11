using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportAssetBundles {

	// Store current texture format for the TextureProcessor.
	public static TextureImporterFormat textureFormat;

	[MenuItem("Assets/Build AssetBundle From Selection - Android")]
	static void ExportResourceAndroid () {
		ExportResource(BuildTarget.Android);		
	}	

	[MenuItem("Assets/Build AssetBundle From Selection - iOS")]
	static void ExportResourceIOS () {
		ExportResource(BuildTarget.iPhone);
	}

	static void ExportResource (BuildTarget target) {
		// Bring up save panel.
		string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");

		if (path.Length != 0) {
			// Build the resource file from the active selection.
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

			foreach (object asset in selection) {
				string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object) asset);
				if (asset is Texture2D) {
					// Force reimport thru TextureProcessor.
					AssetDatabase.ImportAsset(assetPath);
				}
			}

			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, target);
			Selection.objects = selection;
		}
	}
}
