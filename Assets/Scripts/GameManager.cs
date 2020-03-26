using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ParticleSystem Sparsles;
    public GameObject CurrentBlock;
    public GameObject PreviousBlock;
    public Text Score;
    public Text gOver;
    public int Level = 0;
    public bool GameOver;

    void Start()
    {
        NewBlock();
    }

    private void NewBlock()
    {
        if (PreviousBlock != null)
        {
            CurrentBlock.transform.position = new Vector3(Mathf.Round(CurrentBlock.transform.position.x),
                                                          CurrentBlock.transform.position.y, Mathf.Round(CurrentBlock.transform.position.z));
            CurrentBlock.transform.localScale = new Vector3(PreviousBlock.transform.localScale.x - Mathf.Abs(CurrentBlock.transform.position.x - PreviousBlock.transform.position.x),
                                                               PreviousBlock.transform.localScale.y,
                                                             PreviousBlock.transform.localScale.z - Mathf.Abs(CurrentBlock.transform.position.z - PreviousBlock.transform.position.z));
            CurrentBlock.transform.position = Vector3.Lerp(CurrentBlock.transform.position, PreviousBlock.transform.position, 0.5f) + Vector3.up * 5f;
        }
        if (CurrentBlock.transform.localScale.x <= 0f || CurrentBlock.transform.localScale.z <= 0f)
        {
            CurrentBlock.AddComponent<Rigidbody>();
            CurrentBlock.GetComponent<Rigidbody>().mass=1000000f;
            GameOver = true;
            return;
        }
        
        PreviousBlock = CurrentBlock;
        CurrentBlock = Instantiate(PreviousBlock);
        CurrentBlock.name = Level + "";
        CurrentBlock.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((Level / 50f) % 1f, 0.6f, 1f));
        Score.text = "" + Level;
        Level++;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + new Vector3(0, 10, 0),0.5f);
        Camera.main.backgroundColor = CurrentBlock.GetComponent<MeshRenderer>().material.color;
        Sparsles.transform.position = CurrentBlock.transform.position;
        Sparsles.Play();
        Sparsles.GetComponent<ParticleSystemRenderer>().material.color = CurrentBlock.GetComponent<MeshRenderer>().material.color;
    }

    void Update()
    {
        if (GameOver)
        {
            gOver.gameObject.SetActive(true);
            StartCoroutine(Restart());
            return;
        }

        var time = Mathf.Abs(Time.realtimeSinceStartup % 2f - 1f);
        var Position1 = PreviousBlock.transform.position + Vector3.up * 10f;
        var Position2 = Position1 + ((Level % 2 == 0) ? Vector3.left : Vector3.forward) * 120;

        if (Level % 2 == 0)
        {
            CurrentBlock.transform.position = Vector3.Lerp(Position2, Position1, time);
        }
        else CurrentBlock.transform.position = Vector3.Lerp(Position1, Position2, time);

        if (Input.GetKeyDown(KeyCode.Space)||Input.GetButtonDown("Fire1"))
        {
            NewBlock();
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSecondsRealtime(3);
        FindObjectOfType<LevelChanger>().FadeToLevel(1);
    }
}
