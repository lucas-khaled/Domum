using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image playerLifeBar;
    public Image XpBar;
    public Image Skill;

    #region SINGLETON

    public static UIController uiController;

    private void Awake()
    {
        //EventsController.onTyvaMira += TurnPointer;
        uiController = this;
    }

    #endregion

    #region WORLD CANVAS

    public void InitCBT(string text, GameObject CBTprefab, Transform hitCanvas)
    {
        GameObject temp = Instantiate(CBTprefab) as GameObject;

        RectTransform tempRect = temp.GetComponent<RectTransform>();
        temp.transform.SetParent(hitCanvas);
        tempRect.transform.localPosition = CBTprefab.transform.localPosition;
        tempRect.transform.localScale = CBTprefab.transform.localScale;
        tempRect.transform.localRotation = CBTprefab.transform.localRotation;
        temp.GetComponent<Animator>().SetTrigger("Hit");
        temp.GetComponent<Text>().text = text;

        Destroy(temp.gameObject, 2);
    }

    public void LifeBar(Image lifebar, float value)
    {
        lifebar.fillAmount = value;
    }
    #endregion

    public void LifeBar(float value)
    {
        playerLifeBar.fillAmount = value;
    }

    public void XPbar(float value)
    {
        XpBar.fillAmount = value;
    }
    public void SkillCD(float value)
    {
        Skill.fillAmount = value;

    }

    /*private void TurnPointer(bool ligado)
    {
        pointer.gameObject.SetActive(ligado);
        StartCoroutine(DrawPointer());
    }

    IEnumerator DrawPointer()
    {
        yield return new WaitForSeconds(2);
        Vector3 screenPointerPosition = CameraController.cameraInstance.cam.WorldToScreenPoint(pointerPosition.position);

        pointer.transform.SetParent(Canvas2D.transform);
        pointer.transform.position = screenPointerPosition;
    }*/

}
