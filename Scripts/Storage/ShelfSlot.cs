using UnityEngine;
using DG.Tweening;

public class ShelfSlot : MonoBehaviour
{
    [HideInInspector] public bool IsMoving;

    private Transform _itemTransform;
    private AnimationCurve _movementCurve;
    private Vector3 _firstGoal;
    private Quaternion _firstRotationGoal;
    private Vector3 _mainGoal;
    private Vector3 _goalScale;
    private Vector3 _movementVector;
    private Vector3 _movementStartVector;
    private float _minMovementSpeed;
    private float _secondMovementSpeed;
    private float _multipliedSpeed;
    private float _startDistance;
    private float _curveHieght;
    private float _shakeStenght = 0.3f;

    [SerializeField] private float _secondMovementTimer;

    private bool _isFirstGoal;




    public void PrepareForMovement(float firstGoalMaxDistance, float speed, float secondSpeed, float multipliedSpeed, float curveHieght, Vector3 forwardDistance, Vector3 startPosition, Quaternion startRotation, Vector3 startScale, AnimationCurve movementCurve)
    {
        _minMovementSpeed = speed;
        _multipliedSpeed = multipliedSpeed;
        _secondMovementSpeed = secondSpeed;
        _curveHieght = curveHieght;
        _firstGoal = startPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized * firstGoalMaxDistance + forwardDistance;
        _firstRotationGoal = Random.rotation;
        _itemTransform = transform.GetChild(0);


        _goalScale = transform.localScale;
        _itemTransform.parent = null;
        _movementCurve = movementCurve;

        _itemTransform.position = startPosition;
        _itemTransform.rotation = startRotation;
        _itemTransform.localScale = startScale;

        _startDistance = Vector3.Distance(_itemTransform.position, _firstGoal);
        _isFirstGoal = true;
        IsMoving = true;
    }
    public void MoveItem()
    {
        if (_isFirstGoal)
        {
            _itemTransform.position = Vector3.MoveTowards(_itemTransform.position, _firstGoal, _minMovementSpeed + _multipliedSpeed * Vector3.Distance(_itemTransform.position, _firstGoal) / _startDistance);
            _itemTransform.rotation = Quaternion.Lerp(_itemTransform.rotation, _firstRotationGoal, 1f - Vector3.Distance(_itemTransform.position, _firstGoal) / _startDistance);
            if (_itemTransform.position == _firstGoal)
            {
                _startDistance = Vector3.Distance(_itemTransform.position, transform.position);
                _secondMovementTimer = 0;
                _movementVector = _itemTransform.position;
                _movementStartVector = _itemTransform.position;
                _isFirstGoal = false;
            }
        }
        else
        {

            _secondMovementTimer += Time.deltaTime * _secondMovementSpeed;
            _movementVector = Vector3.Lerp(_movementStartVector, transform.position, _secondMovementTimer);
            _itemTransform.position = _movementVector + Vector3.up * _movementCurve.Evaluate(_secondMovementTimer) * _curveHieght;
            //_itemTransform.position = _movementVector;
            _itemTransform.rotation = Quaternion.Lerp(_itemTransform.rotation, transform.rotation, _secondMovementTimer);
            _itemTransform.localScale = Vector3.Lerp(_itemTransform.localScale, _goalScale, _secondMovementTimer);
            if (_secondMovementTimer >= 1)
            {
                _startDistance = Vector3.Distance(_itemTransform.position, transform.position);
                IsMoving = false;
                _itemTransform.parent = transform;
                _itemTransform.localPosition = Vector3.zero;
                _itemTransform.localRotation = Quaternion.identity;
                _itemTransform.localScale = Vector3.one;

                //_itemTransform.DOLocalJump(_itemTransform.localPosition, 0.075f, 1, 0.35f); 
                _itemTransform.DOShakeScale(0.35f, _shakeStenght, 10, 90, true, ShakeRandomnessMode.Harmonic);
            }
        }
    }
    public void MoveStar()
    {
        //if (_isFirstGoal)
        //{
        //    StarTransform.localPosition = Vector3.MoveTowards(StarTransform.localPosition, StarFirstGoal, _movementSpeed);
        //    if (StarTransform.localPosition == StarFirstGoal)
        //    {
        //        _isFirstGoal = false;
        //    }
        //}
        //else
        //{
        //    StarTransform.localPosition = Vector3.MoveTowards(StarTransform.localPosition, StarMainGoal, _movementSpeed);
        //    if (StarTransform.localPosition == StarMainGoal)
        //    {
        //        IsMoving = false;
        //        StarTransform.gameObject.SetActive(false);
        //        StarTransform.localScale = Vector3.zero;
        //        LevelUpManager.Current.StarConnects();
        //    }
        //}
    }
}
