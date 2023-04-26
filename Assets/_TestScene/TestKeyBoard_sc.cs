using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TestKeyBoard_sc : MonoBehaviour
{
    // private int _zKeyCount;
    // private float _lastZKeyTime;

    public Sprite sprCharge;
    
    private float _holdStartTime;
    private float _holdTotalTime;
    private readonly BoolReactiveProperty _isHoldingZ = new();
    private const float FCheckHold = 0.3f;

    private void Start()
    {
        _isHoldingZ.Subscribe(OnChange, Error, OnCompleted);
        var pressKeyStream = Observable.EveryUpdate().Where(_ => Input.GetKeyUp(KeyCode.Z));
        var onceStream = pressKeyStream.Buffer(pressKeyStream.Throttle(TimeSpan.FromSeconds(.2f))).Where(
            down => down.Count == 1 && _holdTotalTime < FCheckHold).Subscribe(_ =>
        {
            print($"Press Once.");
            _holdTotalTime = 0;
        });

        var doubleStream = pressKeyStream.Buffer(pressKeyStream.Throttle(TimeSpan.FromSeconds(.2f))).Where(
            down => down.Count >= 2).Subscribe(_ => { print($"Double Press."); });

        var holdStream = Observable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.C));


        var keyDownStream = Observable
            .EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ =>
            {
                _isHoldingZ.Value = true;
                // print($"_isHoldingZ.Value = {_isHoldingZ.Value}");
            });

        // 放開 C 鍵
        var keyUpStream = Observable
            .EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.Z))
            .Subscribe(_ =>
            {
                _isHoldingZ.Value = false;
                // print($"_isHoldingZ.Value = {_isHoldingZ.Value}");
            });
    }

    private void Error(Exception obj)
    {
        Debug.LogError(obj);
    }

    private void OnCompleted()
    {
        print($"Completed.");
    }

    private void OnChange(bool obj)
    {
        _isHoldingZ.Value = obj;
        switch (_isHoldingZ.Value)
        {
            case true:
                _holdStartTime = Time.time;
                ChargeEffect();
                break;
            case false:
                _holdTotalTime = Time.time - _holdStartTime;
                if (_holdTotalTime >= FCheckHold)
                {
                    HoldZKey();
                    // _holdTotalTime = 0;
                }

                break;
        }
    }

    private void ChargeEffect()
    {
        //todo: 做個蓄力的視覺效果
    }

    private void HoldZKey()
    {
        print($"_holdTotalTime = {_holdTotalTime}");
        print("Hold!");
    }
}