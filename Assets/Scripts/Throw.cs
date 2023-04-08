using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {
    [SerializeField] private Throwable throwable;
    private bool _thrown;
    private void LateUpdate() {
        if (!_thrown && Input.GetMouseButtonDown(0)) {
            _thrown = true;
            throwable.Throw();
        }
    }
}
