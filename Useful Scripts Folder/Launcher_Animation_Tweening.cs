using UnityEngine;

using DG.Tweening;
using System.Collections;

public class Launcher_Animation_Tweening : MonoBehaviour
{
    // Sequence 1은 그냥 버튼으로만 구현해.
    [Header("# DOTween Animation Component")]
    public DOTweenAnimation[] _animations;
    private Vector3[] _pos;

    private void Awake()
    {
        _pos = new Vector3[_animations.Length];

        for (int i = 0; i < _animations.Length; i++)
        {
            _pos[i] = _animations[i].transform.localPosition;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(.3f);

            foreach (var _anim in _animations)
            {
                _anim.DORestartById("IN");
            }

        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _animations.Length; i++)
        {
            _animations[i].transform.localPosition = _pos[i];
        }
    }


    public void OnClickIn()
    {
        foreach (var _anim in _animations)
        {
            _anim.DORestartById("IN");
        }
    }    

    public void OnClickOut()
    {
        foreach(var _anim in _animations)
        {
            _anim.DORestartById("OUT");
        }
    }

    

    

}
