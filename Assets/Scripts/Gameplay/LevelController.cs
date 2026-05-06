using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelController : MonoBehaviour
{
    public List<LevelLayer> layers = new List<LevelLayer>();
    public List<ScrewController> screws;
    public List<ScrewController> screwsActive;
    public Transform layerParent;
    public int currentActiveLayer = 1;

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (layerParent == null)
        {
            layerParent = transform.GetChild(0);
        }

        if (layerParent != null)
        {
            layerParent.GetComponent<SortingGroup>().sortingOrder = -2;
            layerParent.localScale = Vector3.one * 0.9f;
        }

        yield return waitForEndOfFrame;

        layers = GetComponentsInChildren<LevelLayer>(true).ToList();
        screws = new List<ScrewController>();
        screwsActive = new List<ScrewController>();

        int idTextureWood = 1;
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].Init(idTextureWood);

            idTextureWood++;
            if (idTextureWood > 2)
            {
                idTextureWood = 1;
            }
        }

        for (int i = 0; i < layers.Count; i++)
        {
            if (i <= currentActiveLayer)
            {
                layers[i].gameObject.SetActive(true);
                layers[i].ShowLayer();
            }
            else
            {
                layers[i].gameObject.SetActive(false);
            }

            foreach (var shape in layers[i].shapes)
            {
                foreach (var screw in shape.screws)
                {
                    screws.Add(screw);
                }
            }
        }

        if (currentActiveLayer + 1 < layers.Count)
        {
            layers[currentActiveLayer + 1].gameObject.SetActive(true);
            layers[currentActiveLayer + 1].SetSiverShape(false);
        }
    }

    public void ShowNextLayer()
    {
        if (currentActiveLayer + 1 < layers.Count)
        {
            currentActiveLayer++;
            layers[currentActiveLayer].ShowLayer();

            if (currentActiveLayer + 1 < layers.Count)
            {
                layers[currentActiveLayer + 1].gameObject.SetActive(true);
                layers[currentActiveLayer + 1].SetSiverShape(true);
            }
        }
    }

    public void OnLose()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].OnLose();
        }
    }

    public void OnRevive()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].OnRevive();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #region Editor
#if UNITY_EDITOR
    [Header("EDITOR")]
    [SerializeField] private int amountScrewFirstLayer = 12;
    [SerializeField] private int amountScrewSecondLayer = 12;
    [SerializeField] private int amountScrewEachLayer = 12;
    [SerializeField] private AnimationCurve difficultCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [Header("Random Value")]
    [SerializeField] List<ColorEntry> entries = new List<ColorEntry>();
    [SerializeField] List<ColorEnum> listRandomColors = new List<ColorEnum>();
    [SerializeField] List<RandomColorStep> randomSteps = new List<RandomColorStep>();
    [SerializeField] List<int> layerCounts = new List<int>();
    List<ColorEnum> colorPool = new List<ColorEnum>();
    List<ColorEntry> entriesPool = new List<ColorEntry>();
    [SerializeField] List<RandomColorStepLayer> stepsByLayer = new();

    [ContextMenu("Gen Layers")]
    public void GenLayers()
    {
        if (layerParent == null)
        {
            Debug.LogError("Chưa có reference cho layerParent!!!");
            return;
        }

        string levelName = gameObject.name;
        string[] levelNames = levelName.Split('_');
        int level = int.Parse(levelNames[1]);

        TextAsset textAsset = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (textAsset.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(textAsset.bytes), "DeoDucDu0cD@u"));
        }

        ShooterTileData[,] tileData = new ShooterTileData[levelData.shooterGridSize.x, levelData.shooterGridSize.y];
        foreach (var data in levelData.shooterTileDatas)
        {
            tileData[data.pos.x, data.pos.y] = data;
        }

        int totalCapacity = 0;
        List<ColorEnum> allColors = new List<ColorEnum>();
        for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < levelData.shooterGridSize.x; x++)
            {
                ShooterTileData data = tileData[x, y];
                if (data != null && data.shooters != null && data.shooters.Count > 0)
                {
                    foreach (var shooter in data.shooters)
                    {
                        totalCapacity += shooter.capacity;
                        for (int i = 0; i < shooter.capacity; i++)
                        {
                            allColors.Add(shooter.color);
                        }
                    }
                }
            }
        }

        int totalColors = allColors.Count - amountScrewFirstLayer - amountScrewSecondLayer;
        int totalLayers = totalColors / amountScrewEachLayer;

        List<int> amountScrewToSpawns = new List<int>();
        for (int i = 0; i < totalLayers + 2; i++)
        {
            if (i == 0)
            {
                amountScrewToSpawns.Add(amountScrewFirstLayer);
            }
            else if (i == 1)
            {
                amountScrewToSpawns.Add(amountScrewSecondLayer);
            }
            else
            {
                amountScrewToSpawns.Add(amountScrewEachLayer);
            }
        }

        int diff = totalColors - (amountScrewEachLayer * totalLayers);

        if (diff > 0)
        {
            amountScrewToSpawns[amountScrewToSpawns.Count - 1] += diff;
            //while (diff > 0)
            //{
            //    for (int i = amountScrewToSpawns.Count - 1; i >= 0; i--)
            //    {
            //        amountScrewToSpawns[i]++;
            //        diff--;

            //        if (diff == 0)
            //        {
            //            break;
            //        }
            //    }
            //}
        }

        List<LevelLayer> allLayers = Resources.LoadAll<LevelLayer>("Layers").ToList();
        List<LevelLayer> generatedLayers = new List<LevelLayer>();

        Debug.Log($"Tổng số đinh cần gen = {allColors.Count}");

        for (int i = 0; i < amountScrewToSpawns.Count; i++)
        {
            List<LevelLayer> randomLayers = new List<LevelLayer>();
            foreach (var layer in allLayers)
            {
                layer.shapes = layer.GetComponentsInChildren<ShapeController>(true).ToList();
                foreach (var shape in layer.shapes)
                {
                    shape.screws = shape.GetComponentsInChildren<ScrewController>(true).ToList();
                }

                int totalScrew = 0;
                foreach (var shape in layer.shapes)
                {
                    totalScrew += shape.screws.Count;
                }
                if (totalScrew == amountScrewToSpawns[i])
                {
                    randomLayers.Add(layer);
                }
            }

            if (randomLayers.Count == 0)
            {
                Debug.LogError($"Ko có layer nào có số đinh = {amountScrewToSpawns[i]}");
                Debug.LogError("Gen Layers Failed!");
                return;
            }

            LevelLayer randomLayer = randomLayers[Random.Range(0, randomLayers.Count)];

            bool checkLast5Layers = true;
            for (int j = 0; j < randomLayers.Count; j++)
            {
                if (!generatedLayers.Contains(randomLayers[j]))
                {
                    checkLast5Layers = false;
                    break;
                }
            }

            if (!checkLast5Layers)
            {
                while (generatedLayers.Contains(randomLayer))
                {
                    if (randomLayers.Count == 0) break;

                    randomLayers.Remove(randomLayer);

                    if (randomLayers.Count > 0)
                    {
                        randomLayer = randomLayers[Random.Range(0, randomLayers.Count)];
                    }
                }
            }
            else
            {
                List<LevelLayer> last5GeneratedLayers = new List<LevelLayer>();
                for (int j = i - 1; j >= 0 && j > (i - 6); j--)
                {
                    last5GeneratedLayers.Add(generatedLayers[j]);
                }

                while (last5GeneratedLayers.Contains(randomLayer))
                {
                    if (randomLayers.Count == 0) break;

                    randomLayers.Remove(randomLayer);

                    if (randomLayers.Count > 0)
                    {
                        randomLayer = randomLayers[Random.Range(0, randomLayers.Count)];
                    }
                }
            }

            generatedLayers.Add(randomLayer);

            LevelLayer newLayer = (LevelLayer)UnityEditor.PrefabUtility.InstantiatePrefab(randomLayer, layerParent);
            newLayer.gameObject.transform.localPosition = new Vector3(Random.Range(newLayer.offsetMin.x, newLayer.offsetMax.x), Random.Range(newLayer.offsetMin.y, newLayer.offsetMax.y), 0);
            UnityEditor.PrefabUtility.UnpackPrefabInstance(newLayer.gameObject, UnityEditor.PrefabUnpackMode.OutermostRoot, UnityEditor.InteractionMode.UserAction);

            Debug.Log($"Gen layer_{i + 1} với số đinh = {amountScrewToSpawns[i]} success!");
        }

        Debug.Log("Gen Layers Success!");

        layerParent.localPosition = new Vector3(0, 6.0f, 0);

        //RandomScrewColor();
        CalculateTotal();
    }

    [ContextMenu("Set-up")]
    public void SetUp()
    {
        screws = new List<ScrewController>();

        int layerIndex = 1;
        int layerNameIndex = 1;
        int sortingOrder = 0;

        layers = GetComponentsInChildren<LevelLayer>(true).ToList();
        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];

            layer.shapes = layer.GetComponentsInChildren<ShapeController>(true).ToList();
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                ShapeController shape = layer.shapes[j];

                shape.screws = shape.trsfScrew.GetComponentsInChildren<ScrewController>(true).ToList();
                for (int k = 0; k < shape.screws.Count; k++)
                {
                    ScrewController screw = shape.screws[k];
                    screws.Add(screw);
                }
            }

            layer.sortingGroup.sortingOrder = sortingOrder;
            sortingOrder--;

            layer.gameObject.name = $"Layer_{layerNameIndex}";
            layerNameIndex++;

            layer.gameObject.layer = LayerMask.NameToLayer($"Shape{layerIndex}");
            layerIndex++;

            if (layerIndex > 9)
            {
                layerIndex = 1;
            }
        }
    }

    [ContextMenu("Random Screw Color")]
    public void RandomScrewColor()
    {
        entries.Clear();
        listRandomColors.Clear();
        randomSteps.Clear();
        layerCounts.Clear();
        colorPool.Clear();
        entriesPool.Clear();
        stepsByLayer.Clear();

        if (difficultCurve == null)
        {
            Debug.LogError("Chưa set difficultCurve rồi!!");
            return;
        }

        SetUp();

        string levelName = gameObject.name;
        string[] levelNames = levelName.Split('_');
        int level = int.Parse(levelNames[1]);

        TextAsset textAsset = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (textAsset.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(textAsset.bytes), "DeoDucDu0cD@u"));
        }

        levelData.shooterTileDatas.Sort((o1, o2) =>
        {
            return o2.pos.y.CompareTo(o1.pos.y);
        });

        ShooterTileData[,] tileData = new ShooterTileData[levelData.shooterGridSize.x, levelData.shooterGridSize.y];
        foreach (var data in levelData.shooterTileDatas)
        {
            tileData[data.pos.x, data.pos.y] = data;
        }

        int totalCapacity = 0;
        for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < levelData.shooterGridSize.x; x++)
            {
                ShooterTileData data = tileData[x, y];
                if (data != null && data.shooters != null && data.shooters.Count > 0)
                {
                    foreach (var shooter in data.shooters)
                    {
                        totalCapacity += shooter.capacity;
                    }
                }
            }
        }

        TextAsset textAssetColorEntry = Resources.Load<TextAsset>($"ColorEntry/{level}");
        if (textAssetColorEntry != null)
        {
            entries = JsonConvert.DeserializeObject<List<ColorEntry>>(textAssetColorEntry.text);
        }

        if (entries.Count == 0)
        {
            for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < levelData.shooterGridSize.x; x++)
                {
                    ShooterTileData data = tileData[x, y];
                    if (data != null && data.shooters != null && data.shooters.Count > 0)
                    {
                        foreach (var shooter in data.shooters)
                        {
                            ColorEntry colorEntry = new ColorEntry(shooter.capacity, shooter.color);
                            entries.Add(colorEntry);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Đã có sẵn data color entry!");
        }

        int totalEntryCapacity = 0;
        for (int i = 0; i < entries.Count; i++)
        {
            totalEntryCapacity += entries[i].quantity;
        }

        if (totalEntryCapacity != totalCapacity)
        {
            Debug.Log($"Total entry quantity = {totalEntryCapacity}");
            Debug.Log($"Total shooter capacity = {totalCapacity}");
            Debug.Log($"Total screws = {screws.Count}");
            Debug.LogError("Color Entries thiếu");
            return;
        }

        int indexRandomColorCreated = 0;
        int indexColorEntryAdded = 0;

        int maxAmountTrayToRandom;
        if (level < 16)
        {
            maxAmountTrayToRandom = 5;
        }
        else if (level < 25)
        {
            maxAmountTrayToRandom = 6;
        }
        else
        {
            maxAmountTrayToRandom = 7;
        }

        while (indexRandomColorCreated < totalCapacity)
        {
            float t = ((float)indexRandomColorCreated) / totalCapacity;                 // điểm cần lấy trên trục X
            float valueDifficult = difficultCurve.Evaluate(t); // giá trị Y tại t

            int amountPoolShouldCreate = Mathf.Max(1, Mathf.RoundToInt(valueDifficult * maxAmountTrayToRandom));
            int amountEntryToAdd = amountPoolShouldCreate - entriesPool.Count;
            if (amountEntryToAdd > 0)
            {
                for (int i = 0; i < amountEntryToAdd; i++)
                {
                    if (indexColorEntryAdded < entries.Count)
                    {
                        ColorEntry colorEntry = new ColorEntry(entries[indexColorEntryAdded].quantity, entries[indexColorEntryAdded].color);
                        entriesPool.Add(colorEntry);
                        for (int j = 0; j < colorEntry.quantity; j++)
                        {
                            colorPool.Add(colorEntry.color);
                        }
                        indexColorEntryAdded++;
                    }
                }
            }

            if (colorPool.Count > 1)
            {
                colorPool.Shuffle(colorPool.Count);
                Debug.Log(colorPool.Count);
                for (int i = 0; i < colorPool.Count; i++)
                {
                    Debug.Log(colorPool[i]);
                }
            }
            listRandomColors.Add(colorPool[0]);

            for (int i = 0; i < entriesPool.Count; i++)
            {
                if (entriesPool[i].color == colorPool[0])
                {
                    entriesPool[i].quantity--;
                    if (entriesPool[i].quantity == 0)
                    {
                        entriesPool.RemoveAt(i);
                    }
                    break;
                }
            }
            colorPool.RemoveAt(0);

            indexRandomColorCreated++;
            Debug.Log(indexRandomColorCreated);
        }

        for (int i = 0; i < listRandomColors.Count; i++)
        {
            if (i >= screws.Count)
            {
                break;
            }

            screws[i].color = listRandomColors[i];
            screws[i].Rebuild();
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                ShapeController shape = layer.shapes[j];
                List<ColorEnum> screwColors = new List<ColorEnum>();
                for (int k = 0; k < shape.screws.Count; k++)
                {
                    ScrewController screw = shape.screws[k];
                    screwColors.Add(screw.color);
                }
                ColorEnum shapeColor = GetTheMostColorInList(screwColors);
                shape.color = shapeColor;
                shape.Rebuild();
            }
        }

        if (totalCapacity != screws.Count)
        {
            Debug.LogError("Random Screw Color Fail");
            Debug.Log($"Total shooter capacity ({totalCapacity}) not equal to screws ({screws.Count})");
        }
        else
        {
            Debug.Log("Random Screw Color Success");
        }
    }

    [ContextMenu("Random Screw Color Sequence")]
    public void RandomScrewColorSequence()
    {
        entries.Clear();
        listRandomColors.Clear();
        randomSteps.Clear();
        layerCounts.Clear();
        colorPool.Clear();
        entriesPool.Clear();
        stepsByLayer.Clear();

        if (difficultCurve == null)
        {
            Debug.LogError("Chưa set difficultCurve rồi!!");
            return;
        }

        SetUp();

        string levelName = gameObject.name;
        string[] levelNames = levelName.Split('_');
        int level = int.Parse(levelNames[1]);

        TextAsset textAsset = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (textAsset.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(textAsset.bytes), "DeoDucDu0cD@u"));
        }

        levelData.shooterTileDatas.Sort((o1, o2) =>
        {
            return o2.pos.y.CompareTo(o1.pos.y);
        });

        ShooterTileData[,] tileData = new ShooterTileData[levelData.shooterGridSize.x, levelData.shooterGridSize.y];
        foreach (var data in levelData.shooterTileDatas)
        {
            tileData[data.pos.x, data.pos.y] = data;
        }

        int totalCapacity = 0;
        for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < levelData.shooterGridSize.x; x++)
            {
                ShooterTileData data = tileData[x, y];
                if (data != null && data.shooters != null && data.shooters.Count > 0)
                {
                    foreach (var shooter in data.shooters)
                    {
                        totalCapacity += shooter.capacity;
                    }
                }
            }
        }

        TextAsset textAssetColorEntry = Resources.Load<TextAsset>($"ColorEntry/{level}");
        if (textAssetColorEntry != null)
        {
            entries = JsonConvert.DeserializeObject<List<ColorEntry>>(textAssetColorEntry.text);
        }

        if (entries.Count == 0)
        {
            for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < levelData.shooterGridSize.x; x++)
                {
                    ShooterTileData data = tileData[x, y];
                    if (data != null && data.shooters != null && data.shooters.Count > 0)
                    {
                        foreach (var shooter in data.shooters)
                        {
                            ColorEntry colorEntry = new ColorEntry(shooter.capacity, shooter.color);
                            entries.Add(colorEntry);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Đã có sẵn data color entry!");
        }

        int totalEntryCapacity = 0;
        for (int i = 0; i < entries.Count; i++)
        {
            totalEntryCapacity += entries[i].quantity;
        }

        if (totalEntryCapacity != totalCapacity)
        {
            Debug.Log($"Total entry quantity = {totalEntryCapacity}");
            Debug.Log($"Total shooter capacity = {totalCapacity}");
            Debug.Log($"Total screws = {screws.Count}");
            Debug.LogError("Color Entries thiếu");
            return;
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            int countScrew = 0;
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                countScrew += layer.shapes[j].screws.Count;
            }
            layerCounts.Add(countScrew);
        }

        // --- Layer tracking (Option A): diff>=4, mỗi màu tối đa 3 lần trong 1 layer ---
        int layerIdx = 0;
        int layerEndExclusive = GetLayerEndExclusive(layerCounts.ToArray(), layerIdx);
        var pickedCountInLayer = new Dictionary<ColorEnum, int>();

        int indexRandomColorCreated = 0;
        int indexColorEntryAdded = 0;

        while (indexRandomColorCreated < totalCapacity)
        {
            float t = GetProgress01(indexRandomColorCreated, totalCapacity - 1);
            float valueDifficult = difficultCurve != null ? difficultCurve.Evaluate(t) : 1f;

            float lastKeyX = 0f;
            if (difficultCurve != null && difficultCurve.length > 0)
                lastKeyX = difficultCurve.keys[difficultCurve.length - 1].time;

            Debug.Log($"step={indexRandomColorCreated}/{totalCapacity}  t={t:F3}  diff={valueDifficult:F3}  lastKeyX={lastKeyX:F3}");

            // Y = số entry pool target
            int amountPoolShouldCreate = Mathf.RoundToInt(valueDifficult);
            amountPoolShouldCreate = Mathf.Clamp(amountPoolShouldCreate, 1, entries.Count);

            // Add thêm entry vào pool nếu thiếu
            int amountEntryToAdd = amountPoolShouldCreate - entriesPool.Count;
            if (amountEntryToAdd > 0)
            {
                for (int i = 0; i < amountEntryToAdd; i++)
                {
                    if (indexColorEntryAdded >= entries.Count) break;

                    var src = entries[indexColorEntryAdded];
                    if (src != null && src.quantity > 0)
                    {
                        var colorEntry = new ColorEntry(src.quantity, src.color);
                        entriesPool.Add(colorEntry);

                        for (int j = 0; j < colorEntry.quantity; j++)
                            colorPool.Add(colorEntry.color);
                    }

                    indexColorEntryAdded++;
                }
            }

            if (colorPool.Count == 0)
            {
                Debug.LogWarning("[DifficultyColorSequenceGenerator] colorPool empty before reaching totalColor. Check entries quantity.");
                break;
            }

            // chuyển layer nếu cần
            while (indexRandomColorCreated >= layerEndExclusive)
            {
                layerIdx++;
                pickedCountInLayer.Clear();
                layerEndExclusive = GetLayerEndExclusive(layerCounts.ToArray(), layerIdx);
                if (layerEndExclusive == int.MaxValue) break;
            }

            // -------- Pick with Option A: diff>=4, mỗi màu tối đa 3 lần trong layer --------
            int pickIndex = -1;
            ColorEnum pickedColor = default;

            bool TryPickValidFromCurrentPool()
            {
                if (colorPool.Count == 0) return false;

                // diff < 4 => pick random thường
                if (valueDifficult < 4f)
                {
                    pickIndex = UnityEngine.Random.Range(0, colorPool.Count);
                    pickedColor = colorPool[pickIndex];
                    return true;
                }

                const int MAX_DUP = 3;  // cho phép tối đa 3 lần/layer
                const int MAX_TRY = 30;

                // random theo trọng số (vì colorPool có lặp)
                for (int tr = 0; tr < MAX_TRY; tr++)
                {
                    int idx = UnityEngine.Random.Range(0, colorPool.Count);
                    var cand = colorPool[idx];

                    pickedCountInLayer.TryGetValue(cand, out int cur);
                    if (cur < MAX_DUP)
                    {
                        pickIndex = idx;
                        pickedColor = cand;
                        return true;
                    }
                }

                // fallback scan toàn pool
                for (int idx = 0; idx < colorPool.Count; idx++)
                {
                    var cand = colorPool[idx];
                    pickedCountInLayer.TryGetValue(cand, out int cur);
                    if (cur < MAX_DUP)
                    {
                        pickIndex = idx;
                        pickedColor = cand;
                        return true;
                    }
                }

                return false;
            }

            bool ok = TryPickValidFromCurrentPool();

            // Nếu diff>=4 và không pick được (tất cả màu đã đủ 3 lần trong layer),
            // thử “mở thêm entry” để tạo thêm màu mới (nếu còn entries)
            if (!ok && valueDifficult >= 4f)
            {
                int safeGuard = 0;
                while (!ok && indexColorEntryAdded < entries.Count && safeGuard++ < entries.Count)
                {
                    var src = entries[indexColorEntryAdded];
                    indexColorEntryAdded++;

                    if (src == null || src.quantity <= 0) continue;

                    var ce = new ColorEntry(src.quantity, src.color);
                    entriesPool.Add(ce);
                    for (int j = 0; j < ce.quantity; j++)
                        colorPool.Add(ce.color);

                    ok = TryPickValidFromCurrentPool();
                }
            }

            // vẫn không được => bất khả thi, fallback để không kẹt loop
            if (!ok)
            {
                Debug.LogWarning($"[DifficultyColorSequenceGenerator] OptionA rule cannot be satisfied at step {indexRandomColorCreated} (diff={valueDifficult:F2}). Fallback pick.");
                pickIndex = UnityEngine.Random.Range(0, colorPool.Count);
                pickedColor = colorPool[pickIndex];
            }

            // update count per-layer nếu diff>=4
            if (valueDifficult >= 4f)
            {
                pickedCountInLayer[pickedColor] = pickedCountInLayer.TryGetValue(pickedColor, out int v) ? v + 1 : 1;
            }

            // Snapshot poolColors để log (pool trước khi remove)
            Dictionary<ColorEnum, int> counts = BuildColorPoolCounts(colorPool);

            // Log step
            var step = new RandomColorStep
            {
                stepIndex = indexRandomColorCreated,
                valueDifficult = valueDifficult,
                poolTargetSize = amountPoolShouldCreate,
                poolSize = colorPool.Count,
                pickedColor = pickedColor,
            };
            foreach (var kv in counts)
                step.poolColors.Add(new ColorCount(kv.Key, kv.Value));
            randomSteps.Add(step);

            // Output
            listRandomColors.Add(pickedColor);

            // Giảm quantity trong entriesPool
            for (int i = 0; i < entriesPool.Count; i++)
            {
                if (entriesPool[i].color == pickedColor)
                {
                    entriesPool[i].quantity--;
                    if (entriesPool[i].quantity <= 0)
                        entriesPool.RemoveAt(i);
                    break;
                }
            }

            // Remove 1 viên màu khỏi pool
            colorPool.RemoveAt(pickIndex);

            indexRandomColorCreated++;
        }

        stepsByLayer = Tool.LayerSplitUtils.SplitByLayers(randomSteps, layerCounts);

        List<ColorEnum> results = new List<ColorEnum>();
        foreach (var stepLayer in stepsByLayer)
        {
            foreach (var step in stepLayer.steps)
            {
                results.Add(step.pickedColor);
            }
        }

        for (int i = 0; i < results.Count; i++)
        {
            if (i >= screws.Count)
            {
                break;
            }

            screws[i].color = listRandomColors[i];
            screws[i].Rebuild();
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                ShapeController shape = layer.shapes[j];
                List<ColorEnum> screwColors = new List<ColorEnum>();
                for (int k = 0; k < shape.screws.Count; k++)
                {
                    ScrewController screw = shape.screws[k];
                    screwColors.Add(screw.color);
                }
                ColorEnum shapeColor = GetTheMostColorInList(screwColors);
                shape.color = shapeColor;
                shape.Rebuild();
            }
        }

        if (totalCapacity != screws.Count)
        {
            Debug.LogError("Random Screw Color Fail");
            Debug.Log($"Total shooter capacity ({totalCapacity}) not equal to screws ({screws.Count})");
        }
        else
        {
            Debug.Log("Random Screw Color Success");
        }
    }

    [ContextMenu("Random Screw Color - Batch Shuffle Add All ")]
    public void RandomScrewColor_BatchShuffleAddAll()
    {
        entries.Clear();
        listRandomColors.Clear();
        randomSteps.Clear();
        layerCounts.Clear();
        colorPool.Clear();
        entriesPool.Clear();
        stepsByLayer.Clear();

        if (difficultCurve == null)
        {
            Debug.LogError("Chưa set difficultCurve rồi!!");
            return;
        }

        SetUp();

        string levelName = gameObject.name;
        string[] levelNames = levelName.Split('_');
        int level = int.Parse(levelNames[1]);

        TextAsset textAsset = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (textAsset.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(textAsset.bytes), "DeoDucDu0cD@u"));
        }

        levelData.shooterTileDatas.Sort((o1, o2) =>
        {
            return o2.pos.y.CompareTo(o1.pos.y);
        });

        ShooterTileData[,] tileData = new ShooterTileData[levelData.shooterGridSize.x, levelData.shooterGridSize.y];
        foreach (var data in levelData.shooterTileDatas)
        {
            tileData[data.pos.x, data.pos.y] = data;
        }

        int totalCapacity = 0;
        for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < levelData.shooterGridSize.x; x++)
            {
                ShooterTileData data = tileData[x, y];
                if (data != null && data.shooters != null && data.shooters.Count > 0)
                {
                    foreach (var shooter in data.shooters)
                    {
                        totalCapacity += shooter.capacity;
                    }
                }
            }
        }

        TextAsset textAssetColorEntry = Resources.Load<TextAsset>($"ColorEntry/{level}");
        if (textAssetColorEntry != null)
        {
            entries = JsonConvert.DeserializeObject<List<ColorEntry>>(textAssetColorEntry.text);
        }

        if (entries.Count == 0)
        {
            for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < levelData.shooterGridSize.x; x++)
                {
                    ShooterTileData data = tileData[x, y];
                    if (data != null && data.shooters != null && data.shooters.Count > 0)
                    {
                        foreach (var shooter in data.shooters)
                        {
                            ColorEntry colorEntry = new ColorEntry(shooter.capacity, shooter.color);
                            entries.Add(colorEntry);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Đã có sẵn data color entry!");
        }

        int totalEntryCapacity = 0;
        for (int i = 0; i < entries.Count; i++)
        {
            totalEntryCapacity += entries[i].quantity;
        }

        if (totalEntryCapacity != totalCapacity)
        {
            Debug.Log($"Total entry quantity = {totalEntryCapacity}");
            Debug.Log($"Total shooter capacity = {totalCapacity}");
            Debug.Log($"Total screws = {screws.Count}");
            Debug.LogError("Color Entries thiếu");
            return;
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            int countScrew = 0;
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                countScrew += layer.shapes[j].screws.Count;
            }
            layerCounts.Add(countScrew);
        }

        int generated = 0;
        int indexColorEntryAdded = 0;
        int indexRandomColorCreated = 0;

        // chạy theo batch cho đến khi generate đủ totalColor hoặc hết entries
        while (generated < totalCapacity && indexColorEntryAdded < entries.Count)
        {
            // t chuẩn để curve có thể chạm 1.0
            float t = (totalCapacity <= 1) ? 1f : (generated / (totalCapacity - 1f));
            float valueDifficult = (difficultCurve != null) ? difficultCurve.Evaluate(t) : 1f;

            int targetEntryCount = Mathf.Max(1, Mathf.RoundToInt(valueDifficult));

            // Build batch pool: add đủ targetEntryCount entries (hoặc tới khi hết entries)
            entriesPool.Clear();
            colorPool.Clear();

            while (entriesPool.Count < targetEntryCount && indexColorEntryAdded < entries.Count)
            {
                var src = entries[indexColorEntryAdded++];
                if (src == null || src.quantity <= 0) continue;

                var ce = new ColorEntry(src.quantity, src.color);
                entriesPool.Add(ce);

                // đổ hết màu của entry vào colorPool
                for (int j = 0; j < ce.quantity; j++)
                    colorPool.Add(ce.color);
            }

            // Nếu batch này không tạo được màu nào (do toàn quantity=0) thì break tránh loop rỗng
            if (colorPool.Count == 0)
                break;

            // Shuffle 1 lần rồi add ALL vào listRandomColors
            ShuffleInPlace(colorPool);

            // nếu bạn muốn đảm bảo không vượt quá totalColor (phòng hờ dữ liệu lệch):
            int remain = totalCapacity - generated;
            int take = Mathf.Min(remain, colorPool.Count);

            // DÙNG THẲNG GIÁ TRỊ CURVE: Y = số entry pool target (có thể > 1)
            int amountPoolShouldCreate = Mathf.RoundToInt(valueDifficult);

            // clamp hợp lý
            amountPoolShouldCreate = Mathf.Clamp(amountPoolShouldCreate, 1, entries.Count);

            // Snapshot: các loại màu trong colorPool + count
            Dictionary<ColorEnum, int> counts = BuildColorPoolCounts(colorPool);

            for (int i = 0; i < take; i++)
            {
                // Log step
                RandomColorStep step = new RandomColorStep
                {
                    stepIndex = indexRandomColorCreated,
                    valueDifficult = valueDifficult,
                    poolTargetSize = amountPoolShouldCreate,
                    poolSize = colorPool.Count,
                    pickedColor = colorPool[i],
                };
                foreach (var kv in counts)
                    step.poolColors.Add(new ColorCount(kv.Key, kv.Value));
                randomSteps.Add(step);

                listRandomColors.Add(colorPool[i]);
                indexRandomColorCreated++;
            }

            generated += take;
        }

        stepsByLayer = Tool.LayerSplitUtils.SplitByLayers(randomSteps, layerCounts);

        List<ColorEnum> results = new List<ColorEnum>();
        foreach (var stepLayer in stepsByLayer)
        {
            foreach (var step in stepLayer.steps)
            {
                results.Add(step.pickedColor);
            }
        }

        for (int i = 0; i < results.Count; i++)
        {
            if (i >= screws.Count)
            {
                break;
            }

            screws[i].color = listRandomColors[i];
            screws[i].Rebuild();
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                ShapeController shape = layer.shapes[j];
                List<ColorEnum> screwColors = new List<ColorEnum>();
                for (int k = 0; k < shape.screws.Count; k++)
                {
                    ScrewController screw = shape.screws[k];
                    screwColors.Add(screw.color);
                }
                ColorEnum shapeColor = GetTheMostColorInList(screwColors);
                shape.color = shapeColor;
                shape.Rebuild();
            }
        }

        if (totalCapacity != screws.Count)
        {
            Debug.LogError("Random Screw Color Fail");
            Debug.Log($"Total shooter capacity ({totalCapacity}) not equal to screws ({screws.Count})");
        }
        else
        {
            Debug.Log("Random Screw Color Success");
        }
    }

    [ContextMenu("Random Screw Color - Batch Shuffle Add All Plus")]
    public void RandomScrewColor_BatchShuffleAddAll_Plus()
    {
        entries.Clear();
        listRandomColors.Clear();
        randomSteps.Clear();
        layerCounts.Clear();
        colorPool.Clear();
        entriesPool.Clear();
        stepsByLayer.Clear();

        if (difficultCurve == null)
        {
            Debug.LogError("Chưa set difficultCurve rồi!!");
            return;
        }

        SetUp();

        string levelName = gameObject.name;
        string[] levelNames = levelName.Split('_');
        int level = int.Parse(levelNames[1]);

        TextAsset textAsset = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (textAsset.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(textAsset.bytes), "DeoDucDu0cD@u"));
        }

        levelData.shooterTileDatas.Sort((o1, o2) =>
        {
            return o2.pos.y.CompareTo(o1.pos.y);
        });

        ShooterTileData[,] tileData = new ShooterTileData[levelData.shooterGridSize.x, levelData.shooterGridSize.y];
        foreach (var data in levelData.shooterTileDatas)
        {
            tileData[data.pos.x, data.pos.y] = data;
        }

        int totalCapacity = 0;
        for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < levelData.shooterGridSize.x; x++)
            {
                ShooterTileData data = tileData[x, y];
                if (data != null && data.shooters != null && data.shooters.Count > 0)
                {
                    foreach (var shooter in data.shooters)
                    {
                        totalCapacity += shooter.capacity;
                    }
                }
            }
        }

        TextAsset textAssetColorEntry = Resources.Load<TextAsset>($"ColorEntry/{level}");
        if (textAssetColorEntry != null)
        {
            entries = JsonConvert.DeserializeObject<List<ColorEntry>>(textAssetColorEntry.text);
        }

        if (entries.Count == 0)
        {
            for (int y = levelData.shooterGridSize.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < levelData.shooterGridSize.x; x++)
                {
                    ShooterTileData data = tileData[x, y];
                    if (data != null && data.shooters != null && data.shooters.Count > 0)
                    {
                        foreach (var shooter in data.shooters)
                        {
                            ColorEntry colorEntry = new ColorEntry(shooter.capacity, shooter.color);
                            entries.Add(colorEntry);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Đã có sẵn data color entry!");
        }

        int totalEntryCapacity = 0;
        for (int i = 0; i < entries.Count; i++)
        {
            totalEntryCapacity += entries[i].quantity;
        }

        if (totalEntryCapacity != totalCapacity)
        {
            Debug.Log($"Total entry quantity = {totalEntryCapacity}");
            Debug.Log($"Total shooter capacity = {totalCapacity}");
            Debug.Log($"Total screws = {screws.Count}");
            Debug.LogError("Color Entries thiếu");
            return;
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            int countScrew = 0;
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                countScrew += layer.shapes[j].screws.Count;
            }
            layerCounts.Add(countScrew);
        }

        int generated = 0;
        int indexColorEntryAdded = 0;
        int indexRandomColorCreated = 0;

        // --- Layer tracking (để enforce rule theo từng layer) ---
        int layerIdx = 0;
        int layerEndExclusive = GetLayerEndExclusive(layerCounts.ToArray(), layerIdx); // stepIndex < layerEndExclusive nằm trong layer hiện tại
        var sigCountInLayer = new Dictionary<string, int>();

        // chạy theo batch cho đến khi generate đủ totalColor hoặc hết entries
        while (generated < totalCapacity && indexColorEntryAdded < entries.Count)
        {
            // t chuẩn để curve có thể chạm 1.0 (đúng theo generated hiện tại)
            float t = (totalCapacity <= 1) ? 1f : (generated / (totalCapacity - 1f));
            float valueDifficultBatch = (difficultCurve != null) ? difficultCurve.Evaluate(t) : 1f;

            int targetEntryCount = Mathf.Max(1, Mathf.RoundToInt(valueDifficultBatch));

            // Build batch pool: add đủ targetEntryCount entries (hoặc tới khi hết entries)
            entriesPool.Clear();
            colorPool.Clear();

            while (entriesPool.Count < targetEntryCount && indexColorEntryAdded < entries.Count)
            {
                var src = entries[indexColorEntryAdded++];
                if (src == null || src.quantity <= 0) continue;

                var ce = new ColorEntry(src.quantity, src.color);
                entriesPool.Add(ce);

                for (int j = 0; j < ce.quantity; j++)
                    colorPool.Add(ce.color);
            }

            if (colorPool.Count == 0)
                break;

            // Shuffle 1 lần
            ShuffleInPlace(colorPool);

            int remain = totalCapacity - generated;
            int take = Mathf.Min(remain, colorPool.Count);
            List<ColorCount> tempColorCounts = new List<ColorCount>();

            Dictionary<ColorEnum, int> tempCountSteps = BuildCountsFromRange(colorPool, 0);
            foreach (var kv in tempCountSteps)
                tempColorCounts.Add(new ColorCount(kv.Key, kv.Value));


            // --- Generate step-by-step trong batch ---
            for (int i = 0; i < take; i++)
            {
                int stepIndex = indexRandomColorCreated;

                // chuyển layer nếu cần (rule theo layer)
                while (stepIndex >= layerEndExclusive)
                {
                    layerIdx++;
                    sigCountInLayer.Clear();
                    layerEndExclusive = GetLayerEndExclusive(layerCounts.ToArray(), layerIdx);
                    if (layerEndExclusive == int.MaxValue) break;
                }

                // diff theo stepIndex để log + rule đúng “từng bước”
                float tt = (totalCapacity <= 1) ? 1f : (stepIndex / (totalCapacity - 1f));
                float diffStep = (difficultCurve != null) ? difficultCurve.Evaluate(tt) : 1f;

                int poolTarget = Mathf.Clamp(Mathf.RoundToInt(diffStep), 1, entries.Count);

                // Snapshot poolColors theo “phần còn lại” ở step này: [i..end]
                Dictionary<ColorEnum, int> countsStep = BuildCountsFromRange(colorPool, i);

                // Pick màu hiện tại (đã shuffle)
                ColorEnum picked = colorPool[i];

                // --- Enforce: trong 1 layer, với diff>=4, không được có signature xuất hiện lần thứ 4 ---
                if (diffStep >= 4f)
                {
                    // signature = pickedColor + poolColors
                    // (poolColors hiện là countsStep => cố định cho step này, chỉ đổi picked thì signature đổi)
                    const int MAX_DUP = 3; // cho phép tối đa 3 lần, lần thứ 4 thì cấm
                    int guard = 0;

                    while (guard++ < 30) // tránh loop vô hạn
                    {
                        string sig = MakeSignature(picked, tempCountSteps);

                        sigCountInLayer.TryGetValue(sig, out int cur);
                        if (cur < MAX_DUP)
                        {
                            // OK
                            break;
                        }

                        // Nếu sắp bị lần thứ 4: thử tìm một màu khác ở phía sau để swap
                        bool swapped = false;
                        for (int j = i + 1; j < colorPool.Count; j++)
                        {
                            var candidate = colorPool[j];
                            if (candidate.Equals(picked)) continue;

                            string sig2 = MakeSignature(candidate, tempCountSteps);
                            sigCountInLayer.TryGetValue(sig2, out int cur2);
                            if (cur2 < MAX_DUP)
                            {
                                // swap để đổi picked
                                (colorPool[i], colorPool[j]) = (colorPool[j], colorPool[i]);
                                picked = colorPool[i];
                                swapped = true;
                                break;
                            }
                        }

                        if (swapped) continue;

                        // Nếu không swap được: shuffle lại đoạn tail [i..end] để đổi picked
                        ShuffleTailInPlace(colorPool, i);
                        picked = colorPool[i];
                    }

                    // tăng count signature sau khi đã chốt picked
                    string finalSig = MakeSignature(picked, tempCountSteps);
                    sigCountInLayer[finalSig] = sigCountInLayer.TryGetValue(finalSig, out int v) ? v + 1 : 1;
                }

                // Log step
                var step = new RandomColorStep
                {
                    stepIndex = stepIndex,
                    valueDifficult = diffStep,
                    poolTargetSize = poolTarget,
                    poolSize = colorPool.Count - i, // pool còn lại tại thời điểm step
                    pickedColor = picked,
                };

                // đổ countsStep vào poolColors
                //foreach (var kv in countsStep)
                //    step.poolColors.Add(new ColorCount(kv.Key, kv.Value));

                step.poolColors = tempColorCounts;

                randomSteps.Add(step);
                listRandomColors.Add(picked);

                indexRandomColorCreated++;
            }

            generated += take;
        }

        stepsByLayer = Tool.LayerSplitUtils.SplitByLayers(randomSteps, layerCounts);

        List<ColorEnum> results = new List<ColorEnum>();
        foreach (var stepLayer in stepsByLayer)
        {
            foreach (var step in stepLayer.steps)
            {
                results.Add(step.pickedColor);
            }
        }

        for (int i = 0; i < results.Count; i++)
        {
            if (i >= screws.Count)
            {
                break;
            }

            screws[i].color = listRandomColors[i];
            screws[i].Rebuild();
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                ShapeController shape = layer.shapes[j];
                List<ColorEnum> screwColors = new List<ColorEnum>();
                for (int k = 0; k < shape.screws.Count; k++)
                {
                    ScrewController screw = shape.screws[k];
                    screwColors.Add(screw.color);
                }
                ColorEnum shapeColor = GetTheMostColorInList(screwColors);
                shape.color = shapeColor;
                shape.Rebuild();
            }
        }

        if (totalCapacity != screws.Count)
        {
            Debug.LogError("Random Screw Color Fail");
            Debug.Log($"Total shooter capacity ({totalCapacity}) not equal to screws ({screws.Count})");
        }
        else
        {
            Debug.Log("Random Screw Color Success");
        }
    }

    [ContextMenu("Calculate Total")]
    public void CalculateTotal()
    {
        SetUp();

        string levelName = gameObject.name;
        string[] levelNames = levelName.Split('_');
        int level = int.Parse(levelNames[1]);

        TextAsset textAsset = Resources.Load<TextAsset>($"LevelData/{level}");
        LevelData levelData;
        if (textAsset.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(textAsset.bytes), "DeoDucDu0cD@u"));
        }

        Dictionary<ColorEnum, int> shooterColors = new Dictionary<ColorEnum, int>();
        int totalCapacity = 0;
        for (int i = 0; i < levelData.shooterTileDatas.Count; i++)
        {
            ShooterTileData shooterTileData = levelData.shooterTileDatas[i];

            for (int j = 0; j < shooterTileData.shooters.Count; j++)
            {
                ShooterData shooterData = shooterTileData.shooters[j];

                if (!shooterColors.ContainsKey(shooterData.color))
                {
                    shooterColors.Add(shooterData.color, 0);
                }

                shooterColors[shooterData.color] += shooterData.capacity;
                totalCapacity += shooterData.capacity;
            }
        }

        string txt = "Shooter Colors:\n";
        foreach (var kvp in shooterColors)
        {
            txt += $"{kvp.Key.ToString()}: {kvp.Value}\n";
        }
        txt += $"Total shooter capacity: {totalCapacity}\n\n";

        Dictionary<ColorEnum, int> screwColors = new Dictionary<ColorEnum, int>();
        foreach (var screw in screws)
        {
            if (!screwColors.ContainsKey(screw.color))
            {
                screwColors.Add(screw.color, 0);
            }
            screwColors[screw.color]++;
        }

        txt += "Screw Colors:\n";
        foreach (var kvp in screwColors)
        {
            txt += $"{kvp.Key.ToString()}: {kvp.Value}\n";
        }
        txt += $"Total screws: {screws.Count}";

        Debug.Log(txt);
    }

    [ContextMenu("Gen Layers From Old Data")]
    public void GenLayersFromOldData()
    {
        SetUp();

        List<int> amountScrewToSpawns = new List<int>();
        foreach (var layer in layers)
        {
            int totalScrew = 0;
            foreach (var shape in layer.shapes)
            {
                totalScrew += shape.screws.Count;
            }
            amountScrewToSpawns.Add(totalScrew);
        }

        List<ColorEnum> listRandomColors = new List<ColorEnum>();
        foreach (var screw in screws)
        {
            listRandomColors.Add(screw.color);
        }

        ResetLevel();

        List<LevelLayer> allLayers = Resources.LoadAll<LevelLayer>("Layers").ToList();
        List<LevelLayer> generatedLayers = new List<LevelLayer>();

        for (int i = 0; i < amountScrewToSpawns.Count; i++)
        {
            List<LevelLayer> randomLayers = new List<LevelLayer>();
            foreach (var layer in allLayers)
            {
                int totalScrew = 0;
                foreach (var shape in layer.shapes)
                {
                    totalScrew += shape.screws.Count;
                }
                if (totalScrew == amountScrewToSpawns[i])
                {
                    randomLayers.Add(layer);
                }
            }

            if (randomLayers.Count == 0)
            {
                Debug.LogError($"Ko có layer nào có số đinh = {amountScrewToSpawns[i]}");
                Debug.LogError("Gen Layers Failed!");
                return;
            }

            Debug.Log($"Gen layer_{i + 1} với số đinh = {amountScrewToSpawns[i]} success!");

            LevelLayer randomLayer = randomLayers[Random.Range(0, randomLayers.Count)];
            List<LevelLayer> last3GeneratedLayers = new List<LevelLayer>();
            for (int j = i - 1; j >= 0 && j > (i - 4); j--)
            {
                last3GeneratedLayers.Add(generatedLayers[j]);
            }

            while (last3GeneratedLayers.Contains(randomLayer))
            {
                if (randomLayers.Count == 0) break;

                randomLayers.Remove(randomLayer);

                if (randomLayers.Count > 0)
                {
                    randomLayer = randomLayers[Random.Range(0, randomLayers.Count)];
                }
            }

            generatedLayers.Add(randomLayer);

            LevelLayer newLayer = (LevelLayer)UnityEditor.PrefabUtility.InstantiatePrefab(randomLayer, layerParent);
            newLayer.gameObject.transform.localPosition = new Vector3(Random.Range(newLayer.offsetMin.x, newLayer.offsetMax.x), Random.Range(newLayer.offsetMin.y, newLayer.offsetMax.y), 0);
            UnityEditor.PrefabUtility.UnpackPrefabInstance(newLayer.gameObject, UnityEditor.PrefabUnpackMode.OutermostRoot, UnityEditor.InteractionMode.UserAction);
        }

        Debug.Log("Gen Layers From Old Data Success!");

        SetUp();

        for (int i = 0; i < listRandomColors.Count; i++)
        {
            if (i >= screws.Count)
            {
                break;
            }

            screws[i].color = listRandomColors[i];
            screws[i].Rebuild();
        }

        for (int i = 0; i < layers.Count; i++)
        {
            LevelLayer layer = layers[i];
            for (int j = 0; j < layer.shapes.Count; j++)
            {
                ShapeController shape = layer.shapes[j];
                List<ColorEnum> screwColors = new List<ColorEnum>();
                for (int k = 0; k < shape.screws.Count; k++)
                {
                    ScrewController screw = shape.screws[k];
                    screwColors.Add(screw.color);
                }
                ColorEnum shapeColor = GetTheMostColorInList(screwColors);
                shape.color = shapeColor;
                shape.Rebuild();
            }
        }

        Debug.Log("Random Screw Color From Old Data Success " + gameObject.name);

        transform.GetChild(0).localPosition = new Vector3(0, 6.0f, 0);
    }

    [ContextMenu("Reset")]
    public void ResetLevel()
    {
        LevelLayer[] levelLayers = GetComponentsInChildren<LevelLayer>(true);
        for (int i = 0; i < levelLayers.Length; i++)
        {
            UnityEditor.Undo.DestroyObjectImmediate(levelLayers[i].gameObject);
        }

        layers.Clear();
        screws.Clear();
    }

    private ColorEnum GetTheMostColorInList(List<ColorEnum> colorEnums)
    {
        return colorEnums.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
    }

    // build counts cho đoạn [start..end]
    private static Dictionary<ColorEnum, int> BuildCountsFromRange(List<ColorEnum> pool, int start)
    {
        var counts = new Dictionary<ColorEnum, int>();
        for (int k = start; k < pool.Count; k++)
        {
            var c = pool[k];
            counts[c] = counts.TryGetValue(c, out int v) ? v + 1 : 1;
        }
        return counts;
    }

    // signature = pickedColor + poolColors (sort theo enum để stable)
    private static string MakeSignature(ColorEnum picked, Dictionary<ColorEnum, int> counts)
    {
        // vì eTypeColor là enum, sort theo int để ổn định
        var keys = new List<ColorEnum>(counts.Keys);
        keys.Sort((a, b) => ((int)a).CompareTo((int)b));

        System.Text.StringBuilder sb = new System.Text.StringBuilder(128);
        sb.Append((int)picked).Append('|');
        for (int i = 0; i < keys.Count; i++)
        {
            var k = keys[i];
            sb.Append((int)k).Append(':').Append(counts[k]).Append(',');
        }
        return sb.ToString();
    }

    // shuffle đoạn tail [start..end]
    private static void ShuffleTailInPlace<T>(List<T> list, int start)
    {
        for (int i = list.Count - 1; i > start; i--)
        {
            int j = UnityEngine.Random.Range(start, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // layerEndExclusive: tổng layerCounts[0..layerIdx] (nếu layerCounts null/empty -> vô hạn)
    private static int GetLayerEndExclusive(int[] layerCounts, int layerIdx)
    {
        if (layerCounts == null || layerCounts.Length == 0) return int.MaxValue;
        if (layerIdx < 0) return 0;
        if (layerIdx >= layerCounts.Length) return int.MaxValue;

        int sum = 0;
        for (int i = 0; i <= layerIdx; i++)
            sum += Mathf.Max(0, layerCounts[i]);

        return sum > 0 ? sum : 0;
    }

    // Fisher-Yates shuffle (không cần extension)
    private static void ShuffleInPlace<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // ---- Helpers ----
    private static float GetProgress01(int index, int length)
    {
        if (length <= 0) return 1f;
        float t = (float)index / length;
        return Mathf.Clamp01(t);
    }

    private static Dictionary<ColorEnum, int> BuildColorPoolCounts(List<ColorEnum> pool)
    {
        Dictionary<ColorEnum, int> counts = new Dictionary<ColorEnum, int>();
        for (int i = 0; i < pool.Count; i++)
        {
            ColorEnum c = pool[i];
            if (counts.ContainsKey(c)) counts[c]++;
            else counts[c] = 1;
        }
        return counts;
    }

#endif
    #endregion
}

[System.Serializable]
public class ColorEntry
{
    [Min(0)]
    public int quantity;
    public ColorEnum color;

    public ColorEntry(int quantity, ColorEnum color)
    {
        this.quantity = quantity;
        this.color = color;
    }
}

[System.Serializable]
public class ColorCount
{
    public ColorEnum color;
    public int count;

    public ColorCount(ColorEnum color, int count)
    {
        this.color = color;
        this.count = count;
    }
}

[System.Serializable]
public class RandomColorStep
{
    public int stepIndex;
    public float valueDifficult;
    public int poolTargetSize; // số entry target theo curve
    public int poolSize;       // colorPool.Count (số "viên" màu trong pool)
    public ColorEnum pickedColor;

    [Header("Các màu trong colorPool (kèm số lượng)")]
    public List<ColorCount> poolColors = new List<ColorCount>();
}

[System.Serializable]
public class RandomColorStepLayer
{
    public List<RandomColorStep> steps = new List<RandomColorStep>();
}