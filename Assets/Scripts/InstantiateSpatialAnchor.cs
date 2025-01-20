using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum LocationType
{
    Gate, Wall, Temple
}

public class InstantiateSpatialAnchor : MonoBehaviour
{
    public LocationType LocationType;

    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform xRSpaceContent;

    private List<Vector3> anchorPosition = new List<Vector3> ();
    private List<Quaternion> anchorRotation = new List<Quaternion> ();

    private SaveAnchorTransform saveFile;

    private string m_Filename = "";
    

    private GameObject anchor;
    [SerializeField] private List<Button> PositionButtons = new List<Button> ();
    [SerializeField] private List<Button> RotationButtons = new List<Button>();
    [SerializeField] TMP_InputField InputField_PositionValue;
    [SerializeField] TMP_InputField InputField_RotationValue;

    private void Start()
    {
        if (LocationType == LocationType.Gate)
            m_Filename = "GateLocation.json";
        else if (LocationType == LocationType.Wall)
            m_Filename = "WallLocation.json";
        else
            m_Filename = "TempleLocation.json";

        if (PositionButtons.Count >= 6)
        {
            PositionButtons[0].onClick.AddListener(() => AdjustPosition(Vector3.right));    // +X
            PositionButtons[1].onClick.AddListener(() => AdjustPosition(Vector3.left));     // -X
            PositionButtons[2].onClick.AddListener(() => AdjustPosition(Vector3.up));       // +Y
            PositionButtons[3].onClick.AddListener(() => AdjustPosition(Vector3.down));     // -Y
            PositionButtons[4].onClick.AddListener(() => AdjustPosition(Vector3.forward));  // +Z
            PositionButtons[5].onClick.AddListener(() => AdjustPosition(Vector3.back));     // -Z
        }
        else
        {
            Debug.LogError("Please assign 6 buttons to the PositionButtons list.");
        }

         // 为旋转按钮绑定事件
        if (RotationButtons.Count >= 6)
        {
            RotationButtons[0].onClick.AddListener(() => AdjustRotation(Vector3.right));    // Rotate around +X
            RotationButtons[1].onClick.AddListener(() => AdjustRotation(Vector3.left));     // Rotate around -X
            RotationButtons[2].onClick.AddListener(() => AdjustRotation(Vector3.up));       // Rotate around +Y
            RotationButtons[3].onClick.AddListener(() => AdjustRotation(Vector3.down));     // Rotate around -Y
            RotationButtons[4].onClick.AddListener(() => AdjustRotation(Vector3.forward));  // Rotate around +Z
            RotationButtons[5].onClick.AddListener(() => AdjustRotation(Vector3.back));     // Rotate around -Z
        }
        else
        {
            Debug.LogError("Please assign 6 buttons to the RotationButtons list.");
        }
    }

    private void AdjustPosition(Vector3 direction)
    {
        if (anchor == null)
        {
            Debug.LogWarning("Anchor is null!");
            return;
        }

        float value = float.Parse(InputField_PositionValue.text);
        anchor.transform.localPosition += direction * value;
    }

    private void AdjustRotation(Vector3 axis)
    {
        if (anchor == null)
        {
            Debug.LogWarning("Anchor is null!");
            return;
        }

        float value = float.Parse(InputField_RotationValue.text);
        anchor.transform.Rotate(axis, value, Space.Self);
    }

    public void InstanciateAnchor()
    {
        if (anchor != null)
            return;

        anchor = Instantiate(anchorPrefab, cameraTransform.position + cameraTransform.forward, Quaternion.identity, xRSpaceContent);
        anchorPosition.Add(anchor.transform.localPosition);
        anchorRotation.Add(anchor.transform.localRotation);
    }

    public void SaveContents()
    {
        saveFile.positions = anchorPosition;
        saveFile.rotations = anchorRotation;

        string jsonstring = JsonUtility.ToJson(saveFile, true);

        string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);

        File.WriteAllText(dataPath, jsonstring);
    }

    public void InstantiateAnchor(Vector3 positon, Quaternion rotation)
    {
        anchor = Instantiate(anchorPrefab, positon, rotation, xRSpaceContent);
        anchorPosition.Add(anchor.transform.localPosition);
        anchorRotation.Add(anchor.transform.rotation);
        SaveContents();
    }

    public void LoadContents()
    {
        anchorPosition.Clear();
        anchorRotation.Clear();

        string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
        Debug.LogFormat("Trying to load file: {0}", dataPath);

        try
        {
            SaveAnchorTransform loadFile = JsonUtility.FromJson<SaveAnchorTransform>(File.ReadAllText(dataPath));

            for (int i = 0; i < loadFile.positions.Count; i++)
            {
                InstantiateAnchor(loadFile.positions[i], loadFile.rotations[i]);
            }

            Debug.Log("Sucessfully loaded file!");
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarningFormat("{0}\n.json file for content storage not found, Created a new file!", e.Message);
            File.WriteAllText(dataPath, "");
        }
        catch (NullReferenceException nullerr)
        {
            Debug.LogWarningFormat("{0}\n.json file for content storage not found, Created a new file", nullerr.Message);
            File.WriteAllText(dataPath, "");
        }
    }
}

[System.Serializable]
public struct SaveAnchorTransform
{
    public List<Vector3> positions;
    public List<Quaternion> rotations;
}
