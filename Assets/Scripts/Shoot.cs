using System;
using UnityEngine;

public class Shoot : MonoBehaviour {
    private static readonly int ShotDirection = Shader.PropertyToID("_ShotDirection");
    private static readonly int ShotOrigin = Shader.PropertyToID("_ShotOrigin");
   
    [SerializeField] private float shotFrequency = .1f;
    [SerializeField] private float shotHoleTime = 1f;
    [SerializeField] private Material smokeMaterial;
    
    private MaterialPropertyBlock _materialPropertyBlock;
    private Transform _mainCamera;
    
    private float _timeLastShot;
    private float _shotSize;

    private void Awake() {
        _mainCamera = Camera.main.transform;
    }

    private void Update() {
        if (Input.GetMouseButton(0) && CanShoot()) {
            ShootGun();
        }
        
        SmokeHole();

        bool CanShoot() {
            return Time.time > _timeLastShot + shotFrequency;
        }
    }

    private void ShootGun() {
        _timeLastShot = Time.time;
        Vector3 shotDirection = _mainCamera.forward;
        Vector3 shotOrigin = _mainCamera.position;
        smokeMaterial.SetVector(ShotDirection, shotDirection);
        smokeMaterial.SetVector(ShotOrigin, shotOrigin);
    }

    private void SmokeHole() {
        float holeSize = _timeLastShot + shotHoleTime - Time.time;
        holeSize = Ease.InCubic (holeSize);
        _shotSize = Mathf.Clamp(
            holeSize,
            0f, 1f);
        smokeMaterial.SetFloat("_ShotSize", _shotSize);
    }
}
