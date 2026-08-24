using TMPro;
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    public DrawingCanvas drawing;
    public LevelManager levelManager;

    public GameObject background;
    public GameObject button;
    public TMP_Text text;
    public TMP_Text text2;

    public void StartGame()
    {
        if (button != null) button.SetActive(false);
        if (background != null) background.SetActive(true);
        
        if (drawing != null) drawing.SetActive(false); 

        levelManager.PickRandomFigure();

        StartCoroutine(StartSearch());
    }

    private IEnumerator StartSearch()
    {
        if (text != null) text.gameObject.SetActive(true);
        if (text2 != null) text2.gameObject.SetActive(false);

        string baseText = "Your figure";
        float delay = 0.4f;

        for (int cycle = 0; cycle < 2; cycle++)
        {
            for (int dots = 1; dots <= 3; dots++)
            {
                text.text = baseText + new string('.', dots);
                yield return new WaitForSeconds(delay);
            }
        }

        if (text != null) text.gameObject.SetActive(false);

        if (text2 != null) 
        {
            text2.text = "Draw: " + levelManager.GetCurrentFigureName();
            yield return StartCoroutine(PopUpAnimation(text2, 0.5f)); 
        }

        if (drawing != null) drawing.SetActive(true);

        yield return new WaitForSeconds(1.5f); 
        
        if (text2 != null) text2.gameObject.SetActive(false);
        if (background != null) background.SetActive(false);
    }

    public void FinishAndCheck()
    {
        float matchPercent = levelManager.CheckDrawingPercent();

        if (background != null) background.SetActive(true);
        
        text.gameObject.SetActive(true);
        text.text = "Result:";

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
        if (button != null) button.SetActive(true);
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