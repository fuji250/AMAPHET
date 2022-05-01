using UnityEngine;
namespace Forgevision.InputCapture
{
    public class MotionRecorder : MonoBehaviour
    {
        //�^�悷��I�u�W�F�N�g�̑Ώ�
        [SerializeField]
        Transform _recordHead; 
        [SerializeField]
        Transform _recordRight;
        [SerializeField]
        Transform _recordLeft;
        MotionClip _motionClip;
        float _startTime;
        float _timer = 0f;
        //���[�V������1�b������̃L�[��
        readonly int _recordFPS = 30;
        enum RecordState
        {
            NONE,
            RECORDING,
            STOP
        }
        RecordState recordState = RecordState.NONE;
        void Update()
        {
            CaptureUpdate();
        }
        void CaptureUpdate()
        {
            switch (recordState)
            {
                case RecordState.RECORDING:
                    break;
                case RecordState.STOP:
                    Debug.Log("�^��I���B");
                    recordState = RecordState.NONE;
                    return;
                default:
                    return;
            }
            //�P�b�Ԃ� recordFPS �񐔕������L���v�`������B
            if (_timer > (1f / (float)_recordFPS))
            {
                _timer -= (1f / _recordFPS);
                float playTime = Time.time - _startTime;
                if (_recordHead != null)
                {
                    _motionClip.headCurve.AddKeyPostionAndRotation(playTime, _recordHead.position, _recordHead.rotation);
                }
                if (_recordRight != null)
                {
                    _motionClip.rightCurve.AddKeyPostionAndRotation(playTime, _recordRight.position, _recordRight.rotation);
                }
                if (_recordLeft != null)
                {
                    _motionClip.leftCurve.AddKeyPostionAndRotation(playTime, _recordLeft.position, _recordLeft.rotation);
                }
            }
            _timer += Time.deltaTime;
        }
        public void StartRecord(MotionClip motionClip)
        {
            this._motionClip = motionClip;
            recordState = RecordState.RECORDING;
            _startTime = Time.time;
            _timer = 0f;
            Debug.Log("�^��J�n�B");
        }
        public void StopRecord()
        {
            recordState = RecordState.STOP;
        }
    }
}