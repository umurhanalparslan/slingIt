using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public static Animal instance;

    private Rigidbody2D rb;

    public bool isTruePortalActive;

    public int sceneNumber;

    public int nextLevelIndex;

    public void Awake()
    {
        instance = this;
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        State = AnimalState.BeforeThrown;
    }

    void Update()
    {
        NextLevel();

    }
    public void Onthrow()
    {
        rb.isKinematic = false;
        State = AnimalState.Trown;
    }
    public AnimalState State
    {
        get;
        set;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag  == "truePortal")
        {
            isTruePortalActive = true;

            Debug.Log("Doðru Geçiþ");
        }

        if (collision.gameObject.tag == "falsePortal")
        {
            StartCoroutine(SceneWait());
        }

        if(collision.gameObject.tag == "respawnCollider")
        {
            if (!isTruePortalActive)
            {
                StartCoroutine(SceneWait());
                
            }
            
        }
    }

    public void NextLevel()
    {
        if (isTruePortalActive == true)
        {
           StartCoroutine(LevelSceneWait());
        }
    }

    IEnumerator SceneWait()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneNumber);
    }
    IEnumerator LevelSceneWait()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(nextLevelIndex);
    }
}
