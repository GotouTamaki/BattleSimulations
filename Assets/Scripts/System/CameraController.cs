using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup _targetGroup;

    public CinemachineTargetGroup GetTargetGroup => _targetGroup;

    public void SetTargetGroup(Transform target1, Transform target2)
    {
        _targetGroup.Targets[0].Object = target1;
        _targetGroup.Targets[1].Object = target2;
    }
}
