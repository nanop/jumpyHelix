using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixPieceController : MonoBehaviour {

    private Rigidbody rigid = null;
    private MeshCollider meshCollider = null;
    private MeshRenderer meshRender = null;
    private Renderer render = null;
    
    private void CacheComponents()
    {
        //Cache components
        if (rigid == null)
            rigid = GetComponent<Rigidbody>();
        if (meshCollider == null)
            meshCollider = GetComponent<MeshCollider>();
        if (meshRender == null)
            meshRender = GetComponent<MeshRenderer>();
        if (render == null)
            render = GetComponent<Renderer>();
    }

    /// <summary>
    /// Disable this piece
    /// </summary>
    public void Disable()
    {
        CacheComponents();
        meshRender.enabled = false;
        meshCollider.enabled = false;
    }


    /// <summary>
    /// Set this helix piece as dead piece
    /// </summary>
    public void SetDeadPiece()
    {
        CacheComponents();
        gameObject.tag = "Finish";
        meshRender.material = GameManager.Instance.DeadPieceMaterial;
    }

    /// <summary>
    /// Set this helix piece as normal piece
    /// </summary>
    public void SetNormalPiece()
    {
        CacheComponents();
        meshRender.material = GameManager.Instance.NomarPieceMaterial;
    }


    public void Shatter()
    {
        if (meshRender.enabled)
        {
            StartCoroutine(Shattering());
        }    
    }
    private IEnumerator Shattering()
    {
        meshRender.material = GameManager.Instance.BrokenPieceMaterial;
        meshCollider.enabled = false;
        Vector3 forcePoint = transform.parent.position;
        transform.parent = null;
        Vector3 point_1 = transform.position;
        Vector3 point_2 = meshRender.bounds.center + Vector3.up * (meshRender.bounds.size.y / 2f);
        Vector3 dir = (point_2 - point_1).normalized;
        rigid.isKinematic = false;
        rigid.AddForceAtPosition(dir * 10f, forcePoint, ForceMode.Impulse);
        rigid.AddTorque(Vector3.left * 100f);
        rigid.velocity = Vector3.down * 10f;
        yield return new WaitForSeconds(5f);
        rigid.isKinematic = true;
        Disable();
    }

}
