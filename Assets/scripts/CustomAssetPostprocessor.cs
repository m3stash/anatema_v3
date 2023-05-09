using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//public class CustomAssetModificationProcessor : UnityEditor.AssetModificationProcessor {
//    private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath) {
//        Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
//        if (destinationPath.Contains("Rooms/")) {
//            Debug.Log("--------OnWillMoveAsset---------");
//        }
//        AssetMoveResult assetMoveResult = AssetMoveResult.DidNotMove;

//        var count = Directory.GetFiles(Application.dataPath + "/Resources/Prefabs/Rooms/Cave/Easy/1x1");
//        Debug.Log("COUNNNNNNNT" + count);
//        int nbrItems = 0;
//        foreach (var item in count) {
//            if (!item.Contains(".meta")) {
//                nbrItems++;
//            }
//        }
//        Debug.Log("TOTAL" + nbrItems);
//        string json = "{Rooms: { caves: " + nbrItems + "}}";

//        File.WriteAllText(Application.dataPath + "/Resources/count.json", json);
//        Debug.Log("////////////////////////" + Application.dataPath);

//        // Perform operations on the asset and set the value of 'assetMoveResult' accordingly.
//        return assetMoveResult;
//    }
//    public static string[] OnWillSaveAssets(string[] paths) {
//        foreach (string path in paths) {
//            if (path.EndsWith(".prefab")) {
//                //var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//                // Do something
//                Debug.Log("--------OnWillSaveAssets---------" + path);
//            }
//        }
//        return paths;
//    }

//    public static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao) {
//        Debug.Log("deleted - unity callback: " + AssetPath);
//        Debug.Log("--------OnWillDeleteAsset---------" + AssetPath);
//        return AssetDeleteResult.DidNotDelete;
//    }
//}

class CustomAssetPostprocessor : AssetPostprocessor {
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload) {
        foreach (string str in importedAssets) {
            OnReimportedAssets(str);
        }

        foreach (string str in deletedAssets) {
            OnDeletedAssets(str);
        }

        for (int i = 0; i < movedAssets.Length; i++) {
            OnMovedAssets(movedAssets[i], movedFromAssetPaths[i]);
            // Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }

        if (didDomainReload) {
            Debug.Log("Domain has been reloaded");
        }
    }

    static void OnReimportedAssets(string path) {
        if (path.Contains("Rooms")) {
            Debug.Log("Reimported Asset: " + path);
        }
    }

    static void OnDeletedAssets(string path) {
        if (path.Contains("Rooms")) {
            Debug.Log("Deleted Asset: " + path);
        }
    }

    static void OnMovedAssets(string movedAssets, string movedFromAssetPaths) {
        if (movedAssets.Contains("Prefabs/Rooms")) {
            Debug.Log("----"+ movedAssets);
            /*string[] prefabsCount = Directory.GetFiles(Application.dataPath + "/Resources/Prefabs/Rooms/Cave/Easy/1x1");
            int nbrItems = 0;
            foreach (var item in prefabsCount) {
                if (!item.Contains(".meta")) {
                    nbrItems++;
                }
            }*/
            //string json = "{Rooms: { Caves: " + nbrItems + "}}";
            // string pattern = ".*[^[a - zA - Z0 - 9.prefab]";
            /*Debug.Log(movedAssets.Split(' '));
            Debug.Log(movedAssets.Split(" "));
            Debug.Log(movedAssets.Split(movedAssets.Split("Rooms/").Last()));
            string[] toto = movedAssets.Split(" ");
            string infos = movedAssets.Split("Rooms/").Last();
            string[] paths = infos.Split("/");
            foreach (var path in paths) {
                if (!path.Contains(".prefab")) {

                } else {

                }
                Debug.Log("------------- " + path);
            }*/
            //File.WriteAllText(Application.dataPath + "/Resources/rooms_prefab_config.json", json);
            //Debug.Log("______TOTAL_______" + nbrItems);
            //Debug.Log("______movedFromAssetPaths_______" + movedFromAssetPaths);
            //Debug.Log("______>> movedToAssets_______" + movedAssets);
        }
    }
}