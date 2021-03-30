using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable
{
    IEnumerator Respawn();

    void ApplyForce(Vector3 force, ForceMode forceMode);

    void GetStunned(float force, Vector3 position, float radius);

    IEnumerator StunCo();

    void Finish();
}
