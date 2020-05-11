using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

public class FruitMove : MonoBehaviour
{
    Vector3 firstFingerPos;
    public float rotateSpeed;
    public float scaleSpeed = 0.1f;
    public string emptyPlatfromTag;
    public string chocolatePlatfromTag;
    public string chocolateFaceTag;
    public string stopPlatform;
    public List<Transform> rotateControls;
    public List<GameObject> chocolateCoating;
    public Transform right;
    public Transform left;
    public GameObject rotateFruit;
    public GameObject fruitMain;
    bool isMove = false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstFingerPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isMove)
                FruitMoveController(SelectWay(firstFingerPos, Input.mousePosition));
        }
    }
    private void FruitMoveController(FingerWay way)
    {
        switch (way)
        {
            case FingerWay.right:
                if (WayController(Vector3.right, out RaycastHit hitR))
                {
                    if (RotateController(Vector3.right))
                    {
                        FruitRotateMove(right.gameObject, Vector3.back, hitR,90);
                        Debug.Log("Right");
                    }
                    else
                    {
                        FruitRotateMoveStop(right.gameObject, Vector3.back, hitR, 5);
                    }
                }
                break;
            case FingerWay.left:
                if (WayController(Vector3.left, out RaycastHit hitL))
                {
                    if (RotateController(Vector3.left))
                    {
                        FruitRotateMove(left.gameObject, Vector3.forward, hitL,90);
                        Debug.Log("LEft");
                    }
                    else
                    {
                        FruitRotateMoveStop(right.gameObject, Vector3.back, hitL, 5);

                    }
                }
                break;
            case FingerWay.down:
                if (WayController(Vector3.back, out RaycastHit hitD))
                {
                    if (RotateController(Vector3.back))
                    {
                        FruitSlidingMove(rotateFruit, hitD.transform, hitD);
                        Debug.Log("Down");
                    }
                }
                break;
            case FingerWay.top:
                if (WayController(Vector3.forward, out RaycastHit hitT))
                {
                    if (RotateController(Vector3.forward))
                    {
                        FruitSlidingMove(rotateFruit, hitT.transform, hitT);
                        Debug.Log("Top");
                    }
                }
                break;
        }
    }
    private bool RotateController(Vector3 rayDirection)
    {
        foreach (var item in rotateControls)
        {
            Ray ray = new Ray(item.position, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            {
                if (hit.transform.CompareTag(stopPlatform))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private bool WayController(Vector3 rayDirection, out RaycastHit hit)
    {
        Ray ray = new Ray(rotateFruit.transform.position, rayDirection);
        Debug.DrawRay(rotateFruit.transform.position, rayDirection, Color.black, 50);
        var hits = Physics.RaycastAll(ray, 0.3f);
        Debug.Log("hits : " + hits.Length);
        foreach (var item in hits)
        {
            Debug.Log(item.transform.name + " :D" + " TAg :");
            if (item.transform.CompareTag(emptyPlatfromTag) || item.transform.tag.Equals(chocolatePlatfromTag))
            {
                hit = item;
                return true;
            }
        }
        hit = default;
        return false;
    }
    private void FruitRotateMove(GameObject rotPoint, Vector3 axis, RaycastHit hit,float angle)
    {
        isMove = true;
        rotateFruit.transform.SetParent(rotPoint.transform);
        LeanTween.rotateAroundLocal(rotPoint, axis, angle, rotateSpeed).setOnComplete(
        x =>
        {
            FinishCallback(hit, rotPoint.transform);
            isMove = false;
        });
        LeanTween.scale(rotateFruit, new Vector3(0.19f, 0.19f, 0.19f), scaleSpeed).setEaseInOutQuad().setOnComplete(FinishScaleCallback);
    }
    private void FruitRotateMoveStop(GameObject rotPoint, Vector3 axis, RaycastHit hit, float angle)
    {
        isMove = true;
        rotateFruit.transform.SetParent(rotPoint.transform);
        LeanTween.rotateAroundLocal(rotPoint, axis, angle, rotateSpeed).setOnComplete(
        x =>
        {
            LeanTween.rotateAroundLocal(rotPoint, axis, -angle, rotateSpeed);
            isMove = false;
        });
        LeanTween.scale(rotateFruit, new Vector3(0.19f, 0.21f, 0.21f), scaleSpeed).setEaseInOutQuad().setOnComplete(FinishScaleCallback);
    }
    private void FinishScaleCallback()
    {
        LeanTween.scale(rotateFruit, new Vector3(0.22f, 0.22f, 0.22f), 0.1f).setEaseInOutQuad();
    }
    private void FruitSlidingMove(GameObject rotPoint, Transform movePosition, RaycastHit hit)
    {
        isMove = true;
        Vector3 movePos = movePosition.position;
        movePos.y = rotPoint.transform.position.y;
        LeanTween.move(rotPoint, movePos, rotateSpeed).setOnComplete(x =>
        {
            if (hit.transform.CompareTag(chocolatePlatfromTag))
            {
                ChocolateProccer();
            }
            isMove = false;
        });
        LeanTween.moveLocalY(rotPoint, rotPoint.transform.localPosition.y + 0.06f, 0.15f).setEaseInOutBack().setOnComplete(
        x =>
        {
            LeanTween.moveLocalY(rotPoint, rotPoint.transform.localPosition.y - 0.06f, 0.15f);
        });
    }

    private void FinishCallback(RaycastHit hit, Transform way)
    {
        Vector3 pos = transform.localPosition;
        pos.x = hit.transform.position.x;
        pos.z = hit.transform.position.z;
        transform.localPosition = pos;
        fruitMain.transform.position = pos;
        rotateFruit.transform.SetParent(fruitMain.transform);
        Vector3 rotFruit = rotateFruit.transform.localPosition;
        rotFruit.x = 0;
        rotFruit.z = 0;
        rotateFruit.transform.localPosition = rotFruit;
        way.eulerAngles = Vector3.zero;
        //----> Choclate Olayı yapılcak .
        if (hit.transform.CompareTag(chocolatePlatfromTag))
        {
            ChocolateProccer();
        }
    }
    private void ChocolateProccer()
    {
        if (ChocolateFaceControl())
        {
            if (FinishControl())
            {
                Debug.Log("Finish");
            }
        }
    }
    private bool ChocolateFaceControl()
    {
        Ray ray = new Ray(rotateFruit.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1))
        {
            if (hit.transform.CompareTag(chocolateFaceTag))
            {
                hit.transform.GetChild(0).gameObject.SetActive(true);
            }
            return true;
        }
        return false;
    }
    private bool FinishControl()
    {
        for (int i = 0; i < chocolateCoating.Count; i++)
        {
            if (!chocolateCoating[i].activeSelf)
            {
                return false;
            }
        }
        return true;
    }
    public FingerWay SelectWay(Vector3 firstFingerPos, Vector3 finishFingerPos)
    {
        Vector3 disPos = firstFingerPos - finishFingerPos;
        float dis = Vector3.Distance(firstFingerPos, finishFingerPos);
        dis = Mathf.Abs(dis);
        float disY = Mathf.Abs(disPos.y);
        float disX = Mathf.Abs(disPos.x);
        if (dis <= 0)
        {
            return FingerWay.none;
        }

        if (disX > disY)
        {
            if (disPos.x > 0)
            {
                return FingerWay.left;
            }
            else
            {
                return FingerWay.right;
            }
        }
        else
        {
            if (disPos.y > 0)
            {
                return FingerWay.down;
            }
            else
            {
                return FingerWay.top;
            }
        }
    }
}

public enum FingerWay
{
    none,
    right,
    left,
    down,
    top
}