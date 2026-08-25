using TMPro;
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public DrawingCanvas drawing;
    public LevelManager levelManager;

    public GameObject background;
    public GameObject button;
    public GameObject quit;
    public TMP_Text text;
    public TMP_Text text2;
    public TMP_Text text3;

    private void Start()
    {
        ClearText3();
    }

    public void StartGame()
    {
        ClearText3();

        if (button != null) button.SetActive(false);
        if (quit != null) quit.SetActive(false);
        if (background != null) background.SetActive(true);
        
        if (drawing != null) drawing.SetActive(false); 

        if (!levelManager.HasActiveFigures())
        {
            StartCoroutine(ShowCongratulations());
            return;
        }

        levelManager.PickRandomFigure();
        StartCoroutine(StartSearch());
    }

    private IEnumerator ShowCongratulations()
    {
        if (text != null) 
        {
            text.gameObject.SetActive(true);
            text.text = "Congratulations!";
        }

        if (text2 != null) 
        {
            text2.text = "You drew all figures!";
            yield return StartCoroutine(PopUpAnimation(text2, 0.5f)); 
        }

        yield return new WaitForSeconds(3f);
        
        levelManager.ResetAllFigures();

        if (text != null) text.gameObject.SetActive(false);
        if (text2 != null) text2.gameObject.SetActive(false);
        if (background != null) background.SetActive(true);
        
        if (button != null) button.SetActive(true);
        if (quit != null) quit.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator StartSearch()
    {
        if (text != null) text.gameObject.SetActive(true);
        if (text2 != null) text2.gameObject.SetActive(false);

        string baseText = "Your figure";
        float delay = 0.4f;

        int totalSteps = Random.Range(2, 7);

        for (int i = 0; i < totalSteps; i++)
        {
            int dotsCount = (i % 3) + 1;
            text.text = baseText + new string('.', dotsCount);
            yield return new WaitForSeconds(delay);
        }

        if (text != null) text.gameObject.SetActive(false);

        if (text2 != null) 
        {
            text2.text = "Draw: " + levelManager.GetCurrentFigureName();
            yield return StartCoroutine(PopUpAnimation(text2, 0.5f)); 
        }

        if (drawing != null) drawing.SetActive(true);

        yield return new WaitForSeconds(2f); 
    
        if (background != null) background.SetActive(false);
        if (text2 != null) text2.gameObject.SetActive(false);

        if (text3 != null)
        {
            text3.text = "Draw: " + levelManager.GetCurrentFigureName();
            text3.gameObject.SetActive(true);
        }
    }

    public void FinishAndCheck()
    {
        ClearText3();

        float matchPercent = levelManager.CheckDrawingPercent();

        if (background != null) background.SetActive(true);
        
        if (text != null)
        {
            text.gameObject.SetActive(true);
            text.text = "Result:";
        }

        if (text2 != null) 
        {
            text2.text = matchPercent.ToString("F0") + "% Match!";
            StartCoroutine(PopUpAnimation(text2, 0.5f)); 
        }

        if (drawing != null) drawing.SetActive(false);

        StartCoroutine(ResetUi());
    }

    private IEnumerator ResetUi()
    {
        yield return new WaitForSeconds(1.5f);

        if (text != null) text.gameObject.SetActive(false);
        if (text2 != null) text2.gameObject.SetActive(false);
        ClearText3();

        if (button != null) button.SetActive(true);
        if (quit != null) quit.SetActive(true);
    }

    private void ClearText3()
    {
        if (text3 != null)
        {
            text3.text = "";
            text3.gameObject.SetActive(false);
        }
    }

    private IEnumerator PopUpAnimation(TMP_Text targetText, float duration)
    {
        targetText.transform.localScale = Vector3.zero;
        
        targetText.gameObject.SetActive(true);

        float time = 0f;
        while (time < duration)
        {
            targetText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        targetText.transform.localScale = Vector3.one;
    }
}