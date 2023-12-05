using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class LevelsRandom
{
    public Level[] Levels;
}

public class Map : MonoBehaviour
{
    [SerializeField] private Transform startLevels;
    [SerializeField] private LevelsRandom[] levelsRandom;
    [SerializeField] private LevelMap prefabLevelMap;
    [SerializeField] private LevelMap prefabLevelMapLine;
    [SerializeField] private VerticalLayoutGroup content;
    [SerializeField] private Image map;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textButton;
    [SerializeField] private float speed;
    [SerializeField] private Image blackPanel;
    [SerializeField] private Analytics analytics;

    public int CurrentLevel { get; private set; }

    private List<LevelMap> _levels = new List<LevelMap>();
    private bool _isWin;
    private int _numLocation = -1;

    private void Start()
    {
        CurrentLevel = PlayerPrefs.HasKey("Level") ? PlayerPrefs.GetInt("Level") : 1;

        for (int i = 0; i < startLevels.childCount; i++)
        {
            if (startLevels.GetChild(i).TryGetComponent(out Level level))
            {
                if (_levels.Count != 0)
                {
                    LevelMap levelMap = Instantiate(prefabLevelMapLine, content.transform);
                    levelMap.transform.SetSiblingIndex(0);
                    levelMap.Level = level;
                    levelMap.Number = _levels.Count + 1;
                    _levels.Add(levelMap);
                }
                else
                {
                    LevelMap levelMap = Instantiate(prefabLevelMap, content.transform);
                    levelMap.transform.SetSiblingIndex(0);
                    levelMap.Level = level;
                    levelMap.Number = _levels.Count + 1;
                    _levels.Add(levelMap);
                }
            }
        }

        if (CurrentLevel > _levels.Count && PlayerPrefs.HasKey("NumList"))
        {
            while (_levels.Count < CurrentLevel - 1)
            {
                LevelMap levelMap = Instantiate(prefabLevelMapLine, content.transform);
                levelMap.transform.SetSiblingIndex(0);
                levelMap.Level = _levels[0].Level;
                levelMap.Number = _levels.Count + 1;
                _levels.Add(levelMap);
            }

            _numLocation = Array.IndexOf(levelsRandom, levelsRandom[PlayerPrefs.GetInt("NumList")]);
            LevelMap levelMapCurrent = Instantiate(prefabLevelMapLine, content.transform);
            levelMapCurrent.transform.SetSiblingIndex(0);
            levelMapCurrent.Level = levelsRandom[_numLocation].Levels[PlayerPrefs.GetInt("NumLevel")];
            levelMapCurrent.Number = _levels.Count + 1;
            _levels.Add(levelMapCurrent);
            
            GenerateLevels();
        }
        else if(CurrentLevel >= _levels.Count - 4)
            GenerateLevels();

        button.interactable = false;
        CheckCurrentLevel(false);
        UIController.Instance.SetUIGame(false);
        map.gameObject.SetActive(true);
        textButton.text = $"LEVEL {CurrentLevel}";
        StartCoroutine(AnimMap(false));
    }

    private void GenerateLevels()
    {
        int i = 0;
        int num = 0;
        
        while (i == 0)
        {
            num = Random.Range(0, levelsRandom.Length);

            if (num != _numLocation)
            {
                _numLocation = num;
                i++;
            }
        }

        List<Level> levels = new List<Level>();

        while (levels.Count < 3)
        {
            Level level = levelsRandom[num].Levels[Random.Range(0, levelsRandom[num].Levels.Length)];

            if (!levels.Contains(level))
            {
                levels.Add(level);
                
                LevelMap levelMap = Instantiate(prefabLevelMapLine, content.transform);
                levelMap.transform.SetSiblingIndex(0);
                levelMap.Level = level;
                levelMap.Number = _levels.Count + 1;
                _levels.Add(levelMap);
            }
        }
    }

    private void CheckCurrentLevel(bool isAnim)
    {
        for (int i = 0; i < _levels.Count; i++)
        {
            if(_levels[i].Number > CurrentLevel) _levels[i].SetStatus(0, false);
            if(_levels[i].Number == CurrentLevel) _levels[i].SetStatus(1, isAnim);
            if(_levels[i].Number < CurrentLevel) _levels[i].SetStatus(2, false);
        }
    }

    public void ClickWin()
    {
        if (!_isWin)
        {
            analytics.EndLevel(CurrentLevel, LevelFinishedResult.win);
            ActivateMap();
        }
    }

    public void ClickActivateLevel()
    {
        analytics.StartLevel(CurrentLevel);
        ActivateLevel();
    }

    private void ActivateLevel()
    {
        _isWin = false;
        blackPanel.color = new Color(0, 0, 0, 0);
        blackPanel.gameObject.SetActive(true);
        blackPanel.DOColor(new Color(0, 0, 0, 1), 0.5f).OnComplete(() =>
        {
            map.gameObject.SetActive(false);
            for (int i = 0; i < levelsRandom.Length; i++)
            {
                if (levelsRandom[i].Levels.Contains(_levels[CurrentLevel - 1].Level))
                {
                    PlayerPrefs.SetInt("NumList", i);
                    PlayerPrefs.SetInt("NumLevel", Array.IndexOf(levelsRandom[i].Levels, _levels[CurrentLevel - 1].Level));
                }
            }
            _levels[CurrentLevel - 1].Level.gameObject.SetActive(true);
            blackPanel.DOColor(new Color(0, 0, 0, 0), 0.5f)
                .OnComplete(() =>
                {
                    blackPanel.gameObject.SetActive(false);
                    UIController.Instance.SetUIGame(true);
                });
        });
    }

    private void ActivateMap()
    {
        _isWin = true;
        blackPanel.color = new Color(0, 0, 0, 0);
        blackPanel.gameObject.SetActive(true);
        blackPanel.DOColor(new Color(0, 0, 0, 1), 0.5f).OnComplete(() =>
        {
            UIController.Instance.HideWinPanel();
            UIController.Instance.SetUIGame(false);
            foreach (var level in _levels) level.Level.gameObject.SetActive(false);
            CurrentLevel++;
            PlayerPrefs.SetInt("Level", CurrentLevel);
            PlayerPrefs.Save();
            button.interactable = false;
            map.gameObject.SetActive(true);
            textButton.text = $"LEVEL {CurrentLevel}";
            
            if(CurrentLevel >= _levels.Count - 4)
                GenerateLevels();
            
            blackPanel.DOColor(new Color(0, 0, 0, 0), 0.5f)
                .OnComplete(() =>
                {
                    blackPanel.gameObject.SetActive(false);
                    CheckCurrentLevel(true);
                    StartCoroutine(AnimMap(true));
                });
        });
    }

    private IEnumerator AnimMap(bool isAnim)
    {
        float num1 = 300f + content.spacing;
        float num2 = 0;

        for (int i = 0; i < _levels.Count; i++)
        {
            if (i + 1 == CurrentLevel)
            {
                num2 = i;
                break;
            }
        }

        float posY = num1 * num2;

        if (isAnim)
        {
            while (content.GetComponent<RectTransform>().anchoredPosition.y > -posY)
            {
                content.GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(content.GetComponent<RectTransform>().anchoredPosition,
                    new Vector3(0, -posY, 0), Time.deltaTime * speed);
                yield return null;
            }
        }
        else
        {
            content.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -posY, 0);
        }
        
        button.interactable = true;
    }
}
