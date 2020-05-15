using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

public class FruitMove : MonoBehaviour
{
    #region //----> Variable
    Vector3 firstFingerPos;
    public float rotateSpeed;
    public float scaleSpeed = 0.1f;
    public string emptyPlatfromTag;
    public string chocolatePlatfromTag;
    public string chocolateFaceTag;
    public string stopPlatform;
    public FinishScene finishScene;
    public List<Transform> rotateControls;
    public List<GameObject> chocolateCoating;
    public Transform right;
    public Transform left;
    public Transform finishPostion;
    public GameObject rotateFruit;
    public GameObject fruitMain;
    public ParticleSystem chocolateSplash;
    public ParticleSystem starExplosion;
    public ParticleSystem chocolateDecal;
    public ParticleSystem confitte;
    public CPC_CameraPath cameraPathFinish;
    bool isMove = false;
    private ParticleSystem chocolateDecalClone;
    public RectTransform levelfail;
    public bool isActive;
    #endregion
    private void Update()
    {
        if (isActive)
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
    }
    private void FruitMoveController(FingerWay way)
    {
        switch (way)
        {
            case FingerWay.right:
                if (WayController(Vector3.right, out RaycastHit hitR))
                {
                    if (RotateController(Vector3.right, way))
                    {
                        FruitRotateMove(right.gameObject, Vector3.back, hitR, 90);
                    }
                    else
                    {
                        FruitRotateMoveStop(right.gameObject, Vector3.back, 5);
                    }
                }
                break;
            case FingerWay.left:
                if (WayController(Vector3.left, out RaycastHit hitL))
                {
                    if (RotateController(Vector3.left, way))
                    {
                        FruitRotateMove(left.gameObject, Vector3.forward, hitL, 90);
                    }
                    else
                    {
                        FruitRotateMoveStop(right.gameObject, Vector3.back, 5);
                    }
                }
                break;
            case FingerWay.down:
                if (WayController(Vector3.back, out RaycastHit hitD))
                {
                    FruitSlidingMove(rotateFruit, hitD.transform, hitD);
                }
                break;
            case FingerWay.top:
                if (WayController(Vector3.forward, out RaycastHit hitT))
                {
                    FruitSlidingMove(rotateFruit, hitT.transform, hitT);
                }
                break;
        }
    }
    private bool RotateController(Vector3 rayDirection, FingerWay way)
    {
        foreach (var item in rotateControls)
        {
            Ray ray = new Ray(item.position, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            {
                if (hit.transform.CompareTag(stopPlatform))
                {
                    FailControl(hit, way);
                    return false;
                }
            }
        }
        return true;
    }
    private void FailControl(RaycastHit hit, FingerWay way)
    {
        var stopPlatform = hit.transform.GetComponent<StopPlatformController>();
        stopPlatform.CountReduction(way);
        if (PlatformManager.ins.active.FailController())
        {
            levelfail.gameObject.SetActive(true);
            LeanTween.scale(levelfail, Vector3.zero, 0);
            LeanTween.scale(levelfail, Vector3.one, 1f).setEaseInOutQuart().setOnComplete(finishScene.ButtonCallback);
        }
    }
    private bool WayController(Vector3 rayDirection, out RaycastHit hit)
    {
        Ray ray = new Ray(rotateFruit.transform.position, rayDirection);
        Debug.DrawRay(rotateFruit.transform.position, rayDirection, Color.black, 50);
        var hits = Physics.RaycastAll(ray, 0.3f);
        foreach (var item in hits)
        {
            if (item.transform.CompareTag(emptyPlatfromTag) || item.transform.tag.Equals(chocolatePlatfromTag))
            {
                hit = item;
                return true;
            }
        }
        hit = default;
        return false;
    }
    private void FruitRotateMove(GameObject rotPoint, Vector3 axis, RaycastHit hit, float angle)
    {
        isMove = true;
        rotateFruit.transform.SetParent(rotPoint.transform);
        LeanTween.rotateAroundLocal(rotPoint, axis, angle, rotateSpeed).setOnComplete(
        x =>
        {
            FruitRotateMoveCallback(hit, rotPoint.transform);
            isMove = false;
        });
        FruitScale(rotateFruit, 0.19f, scaleSpeed);
        ChocolateDecalPlay();
    }
    private void FruitRotateMoveStop(GameObject rotPoint, Vector3 axis, float angle)
    {
        isMove = true;
        rotateFruit.transform.SetParent(rotPoint.transform);
        LeanTween.rotateAroundLocal(rotPoint, axis, angle, rotateSpeed).setOnComplete(
        x =>
        {
            LeanTween.rotateAroundLocal(rotPoint, axis, -angle, rotateSpeed);
            isMove = false;
        });
        FruitScale(rotateFruit, 0.19f, scaleSpeed);
    }
    private void FruitScale(GameObject fruit, float scale, float speed)
    {
        LeanTween.scale(fruit, new Vector3(scale, scale, scale), speed).setEaseInOutQuad().setOnComplete(FinishScaleCallback);
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
            else if (hit.transform.CompareTag(emptyPlatfromTag))
            {
                ChocolateDecalPlace();
            }
            isMove = false;
        });
        LeanTween.moveLocalY(rotPoint, rotPoint.transform.localPosition.y + 0.06f, 0.15f).setEaseInOutBack().setOnComplete(
        x =>
        {
            LeanTween.moveLocalY(rotPoint, rotPoint.transform.localPosition.y - 0.06f, 0.15f);
        });
        ChocolateDecalPlay();
    }
    private void ChocolateDecalPlace()
    {
        Ray ray = new Ray(rotateFruit.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1))
        {
            if (hit.transform.CompareTag(chocolateFaceTag))
            {
                if (hit.transform.GetChild(0).gameObject.activeSelf)
                {
                    var cloneDecalChocolate = Instantiate(chocolateDecal);
                    Vector3 setDecalPos = hit.transform.position;
                    setDecalPos.y += 0.0015f;
                    cloneDecalChocolate.transform.position = setDecalPos;
                    chocolateDecalClone = cloneDecalChocolate;
                }
            }
        }
    }
    private void ChocolateDecalPlay()
    {
        if (chocolateDecalClone != null)
        {
            chocolateDecalClone.gameObject.SetActive(true);
            chocolateDecalClone.Play();
            Destroy(chocolateDecalClone.gameObject, chocolateDecalClone.main.duration);
        }
    }
    private void FruitRotateMoveCallback(RaycastHit hit, Transform way)
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
        else if (hit.transform.CompareTag(emptyPlatfromTag))
        {
            ChocolateDecalPlace();
        }
    }
    private void ChocolateProccer()
    {
        if (IsChocolateFacePlace())
        {
            if (FinishControl())
            {
                FinishEffectBegin();
            }
        }
    }
    private bool IsChocolateFacePlace()
    {
        Ray ray = new Ray(rotateFruit.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1))
        {
            if (hit.transform.CompareTag(chocolateFaceTag))
            {
                if (!hit.transform.GetChild(0).gameObject.activeSelf)
                {
                    hit.transform.GetChild(0).gameObject.SetActive(true);
                    var cloneParticle = Instantiate(chocolateSplash, hit.point, Quaternion.identity);
                    cloneParticle.gameObject.SetActive(true);
                    cloneParticle.Play();
                    Destroy(cloneParticle.gameObject, 1.5f);
                }
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
    private void FinishEffectBegin()
    {
        LeanTween.moveLocalY(rotateFruit, 0.3f, 0.3f).setOnComplete(
        m =>
        {
            ForkMove();
            confitte.Play();
            var cloneExplosion = Instantiate(starExplosion, rotateFruit.transform.position, Quaternion.identity);
            cloneExplosion.gameObject.SetActive(true);
            Destroy(cloneExplosion, 1.5f);
            LeanTween.rotateAroundLocal(rotateFruit, Vector3.down, 180, 0.3f);
            finishScene.ChocolateCapRotate();
        });
    }
    private void ForkMove()
    {
        rotateFruit.transform.SetParent(finishPostion);
        LeanTween.rotateAroundLocal(finishPostion.gameObject, Vector3.down, 180, 0.6f).setDelay(0.3f).setOnComplete(
        x =>
        {
            cameraPathFinish.SetOnComplete(CameraComplete);
            cameraPathFinish.PlayPath(1);
            rotateFruit.SetActive(false);
        });
    }
    private void CameraComplete()
    {
        finishScene.FinishEffectStart();
    }
    public void IsGameActive()
    {
        isActive = true;
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