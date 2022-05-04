using UnityEngine;
using Photon.Pun;

namespace Forgevision.InputCapture
{
    public class MotionPlayer : MonoBehaviour
    {
        //SpawnManager spawnManager;


        //���[�V�����Đ�����Ώ�
        [SerializeField]
        public Transform _targetHead;
        //[SerializeField]
        //Transform _targetRight;
        //[SerializeField]
        //Transform _targetLeft;
        MotionClip _motionClip;
        float _startTime = 0f;
        float _delayTimeSec = 0f;

        enum PlayState
        {
            NONE,
            STOP,
            PLAYING
        }
        PlayState playState = PlayState.NONE;

        private void Start()
        {

        }

        void Update()
        {
            MotionUpdate();
        }
        //motionClip����f�[�^�����o���āA�Ώۂ�Position�y��Rotation��ω�������B
        void MotionUpdate()
        {
            switch (playState)
            {
                case PlayState.PLAYING:
                    break;
                default:
                    return;
            }
            float playTime = Time.time - _startTime - _delayTimeSec;
            
            if(playTime < 0f)
            {
                return;
            }
            if (_targetHead != null)
            {
                _targetHead.SetPositionAndRotation(
                      new Vector3(_motionClip.headCurve.pos_xCurve.Evaluate(playTime)
                    , _motionClip.headCurve.pos_yCurve.Evaluate(playTime)
                    , _motionClip.headCurve.pos_zCurve.Evaluate(playTime))
                    , new Quaternion(_motionClip.headCurve.rot_xCurve.Evaluate(playTime)
                    , _motionClip.headCurve.rot_yCurve.Evaluate(playTime)
                    , _motionClip.headCurve.rot_zCurve.Evaluate(playTime)
                    , _motionClip.headCurve.rot_wCurve.Evaluate(playTime)));
            }
            /*
            if (_targetRight != null)
            {
                _targetRight.SetPositionAndRotation(
                      new Vector3(_motionClip.rightCurve.pos_xCurve.Evaluate(playTime)
                    , _motionClip.rightCurve.pos_yCurve.Evaluate(playTime)
                    , _motionClip.rightCurve.pos_zCurve.Evaluate(playTime))
                    , new Quaternion(_motionClip.rightCurve.rot_xCurve.Evaluate(playTime)
                    , _motionClip.rightCurve.rot_yCurve.Evaluate(playTime)
                    , _motionClip.rightCurve.rot_zCurve.Evaluate(playTime)
                    , _motionClip.rightCurve.rot_wCurve.Evaluate(playTime)));
            }
            if (_targetLeft != null)
            {
                _targetLeft.SetPositionAndRotation(
                      new Vector3(_motionClip.leftCurve.pos_xCurve.Evaluate(playTime)
                    , _motionClip.leftCurve.pos_yCurve.Evaluate(playTime)
                    , _motionClip.leftCurve.pos_zCurve.Evaluate(playTime))
                    , new Quaternion(_motionClip.leftCurve.rot_xCurve.Evaluate(playTime)
                    , _motionClip.leftCurve.rot_yCurve.Evaluate(playTime)
                    , _motionClip.leftCurve.rot_zCurve.Evaluate(playTime)
                    , _motionClip.leftCurve.rot_wCurve.Evaluate(playTime)));
            }
            */
        }
        //_delayTime_sec�b�x��čĐ�������B
        public void MotionPlay(MotionClip motionClip, float delayTimeSec =1f)
        {           
            /*
            if (_targetHead == null && _targetRight == null && _targetLeft == null)
            {
                Debug.LogWarning("���[�V�����Đ��Ώۂ��ݒ肳��Ă��܂���B");
                return;
            }
            */
            if (_targetHead == null)
            {
                Debug.LogWarning("���[�V�����Đ��Ώۂ��ݒ肳��Ă��܂���B");
                return;
            }
            _startTime = Time.time;
            _motionClip = motionClip;
            _delayTimeSec = delayTimeSec;
            Debug.Log("���[�V�����Đ��B�x����" + string.Format("{0:#.#}", this._delayTimeSec)�@ + "�b");
            playState = PlayState.PLAYING;
        }
        public void MotionStop()
        {
            if (playState != PlayState.PLAYING)
            {
                return;
            }
            Debug.Log("���[�V������~�B");
            playState = PlayState.STOP;
        }

    }
}