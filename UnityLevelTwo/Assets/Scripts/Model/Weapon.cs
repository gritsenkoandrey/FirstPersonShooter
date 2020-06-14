﻿using System.Collections.Generic;
using UnityEngine;


public abstract class Weapon : BaseObjectScene
{
    #region Fields

    public Ammunition Ammunition;
    public AmmunitionRPG AmmunitionRPG;
    [SerializeField] protected Transform _barrelOne;
    [SerializeField] protected Transform _barrelTwo;
    [SerializeField] protected Transform _barrelThree;

    // структура которая содержит количество патронов в обойме
    public Clip Clip;
    // очередь с нашими обоймами
    private Queue<Clip> _clips = new Queue<Clip>();

    [SerializeField] protected float _force = 999.0f;
    [SerializeField] protected float _rechargeTime = 0.2f;
    internal readonly int _maxCountAmmunition = 30;
    //private readonly int _minCountAmmunition = 30;
    // количество обойм в оружии
    private readonly int _countClip = 5;
    protected bool _isReady = true;

    public AmmunitionType[] AmmunitionTypes = { AmmunitionType.Bullet, AmmunitionType.Rpg };

    // отсчет времени между выстрелами
    protected ITimeRemaining _timeRemaining;

    private Animator _animator;
    private static readonly int FireEnable = Animator.StringToHash("FireEnabled");
    private static readonly int FireDisable = Animator.StringToHash("FireDisabled");
    private static readonly int ReloadOn = Animator.StringToHash("ReloadOn");
    private static readonly int ReloadOff = Animator.StringToHash("ReloadOff");

    [SerializeField] protected ParticleSystem _particleSystem;

    #endregion


    #region Properties

    public int CountClip
    {
        get { return _clips.Count; }
    }

    #endregion


    #region UnityMethods

    protected override void Awake()
    {
        base.Awake();

        // передаю таймремаининг + проверяется готовность на стрельбу с отсчитыванием времени
        _timeRemaining = new TimeRemaining(ReadyShoot, _rechargeTime);
        for (var i = 0; i <= _countClip; i++)
        {
            AddClip(new Clip { CountAmmunition = /*Random.Range(_minCountAmmunition, _maxCountAmmunition) */_maxCountAmmunition});
        }
        ReloadClip();

        // если сделать так то контроллер подхватит таймер и начнет его отсчитывать
        //_timeRemaining.AddTimeRemaining();

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    if (ServiceLocator.Resolve<WeaponController>().IsActive && Clip.CountAmmunition > 0)
        //    {
        //        FireOn();
        //    }
        //}
        //else
        //{
        //    FireOff();
        //}

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (ServiceLocator.Resolve<WeaponController>().IsActive && CountClip > 0)
            {
                ReloadClipOn();
            }
        }
        else
        {
            ReloadClipOff();
        }
    }

    #endregion


    #region Methods

    public abstract void Fire();

    protected void ReadyShoot()
    {
        _isReady = true;
    }

    protected void AddClip(Clip clip)
    {
        _clips.Enqueue(clip);
    }

    public void ReloadClip()
    {
        if (CountClip <= 0)
        {
            return;
        }
        Clip = _clips.Dequeue();
    }

    protected void FireAnimationOn()
    {
        _animator.SetTrigger(FireEnable);
    }

    protected void FireAnimationOff()
    {
        _animator.SetTrigger(FireDisable);
    }

    private void ReloadClipOn()
    {
        _animator.SetTrigger(ReloadOn);
    }

    private void ReloadClipOff()
    {
        _animator.SetTrigger(ReloadOff);
    }

    #endregion
}