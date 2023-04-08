using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour {
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float timeToExplode = 3f;
    private Rigidbody _rigidbody;
    private TrailRenderer _trailRenderer;
    private ProceduralAnimation _proceduralAnimation;
    private Transform _mainCamera;
    private Smoke _smoke;
    private bool _exploded;
    private bool _armed;
    private float _timeArmed;
    
    private void Start() {
        _smoke = GetComponent<Smoke>();
        _rigidbody = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _proceduralAnimation = FindObjectOfType<ProceduralAnimation>();
        _mainCamera = Camera.main.transform;
    }

    public void Throw() {
        _trailRenderer.enabled = true;
        _proceduralAnimation.carryObject = null;
        _rigidbody.velocity = (_mainCamera.forward + _mainCamera.up).normalized * throwForce;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _armed = true;
    }

    private void FixedUpdate() {
        if (_armed)
            _timeArmed += Time.fixedDeltaTime;

        if (!_exploded && _timeArmed > timeToExplode && _rigidbody.velocity.sqrMagnitude < 0.01f)
            Explode();
    }

    private void Explode() {
        _exploded = true;
        Debug.Log("EXPLODE");
        _smoke.Explode();
    }
}
