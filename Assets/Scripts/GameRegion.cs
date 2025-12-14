using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

public class GameRegion : MonoBehaviour
{
    public TMP_Dropdown DropdownRegion;
    private List<Region.RegionData> regions;
    public static int selectedRegionId;

    private void Start()
    {
        StartCoroutine(GetRegion());
    }

    private IEnumerator GetRegion()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/api/regions"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Region API Error: " + www.error);
                Debug.Log("HTTP Code: " + www.responseCode);
                Debug.Log("Body: " + www.downloadHandler.text);
            }
            else
            {
                string json = www.downloadHandler.text;
                Region.Response response = JsonConvert.DeserializeObject<Region.Response>(json);

                DropdownRegion.options.Clear();
                regions = new List<Region.RegionData>(response.data);
                foreach (Region.RegionData region in response.data)
                {
                    DropdownRegion.options.Add(new TMP_Dropdown.OptionData(region.name));
                }

                if (DropdownRegion.options.Count > 0)
                {
                    DropdownRegion.SetValueWithoutNotify(0);
                    DropdownRegion.RefreshShownValue();
                    DropdownValueChanged(DropdownRegion);
                }
            }
        }
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        if (regions == null)
        {
            return;
        }

        int index = dropdown.value;
        if (index < 0 || index >= regions.Count)
        {
            Debug.LogWarning("Invalid dropdown index selected.");
            return;
        }

        selectedRegionId = regions[index].regionId;
        Debug.Log("Selected Region ID: " + selectedRegionId);
    }
}
