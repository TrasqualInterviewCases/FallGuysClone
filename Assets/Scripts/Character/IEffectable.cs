using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable
{
    void Respawn();

    void ApplyForce(Vector3 force, ForceMode forceMode);

    void GetStunned(float force, Vector3 Vector, ForceMode forceMode);

    IEnumerator StunCo();

    void Finish();
}
