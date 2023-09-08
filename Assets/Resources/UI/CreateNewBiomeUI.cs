using RoomNs;
using System;
using System.IO;
using TMPro;
using UnityEngine;

public class CreateNewBiomeUI : MonoBehaviour {

    [SerializeField] private GameObject nameField;
    private TMP_InputField biomeNameField;

    private void Awake() {
        
    }

    // Start is called before the first frame update
    void Start() {
        biomeNameField = nameField.GetComponent<TMP_InputField>();
        // biomeNameField.onValueChanged.AddListener(delegate { OnNameChange(biomeNameField.text); });
        biomeNameField.onValueChanged.AddListener((value) => OnNameChange(value));
    }

    private void OnDestroy() {
        biomeNameField.onValueChanged.RemoveAllListeners();
    }

    private void OnNameChange(string value) {
        try {
            string folderName = char.ToUpper(value[0]) + value[1..];
            string newDirectory = GlobalConfig.Instance.DirectoryResourceFolder + GlobalConfig.Instance.PrefabRoomsPath + " / " + folderName;
            if (!Directory.Exists(newDirectory)) {
                CreateDirectories(newDirectory);
            }
        } catch (IOException ex) {
            Console.WriteLine(ex.Message);
           }
    }

    private void CreateDirectories(string path) {
        Directory.CreateDirectory(path);
        CreateShapeFolder(path); 
    }

    private void CreateShapeFolder(string path) {
        foreach (RoomShapeEnum shape in Enum.GetValues(typeof(RoomShapeEnum))) {
            string shapeName = Enum.GetName(typeof(RoomShapeEnum), shape);
            Directory.CreateDirectory(path + "/" + shapeName);
        }
    }
}
