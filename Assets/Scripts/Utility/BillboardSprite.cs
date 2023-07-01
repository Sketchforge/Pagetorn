using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public bool onlyRotateY = true;
    public bool useStaticBillboard = false;
    private Camera theCam;
    // Update is called once per frame

    private void Awake()
    {
        theCam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        BillboardFace();
    }

    protected void BillboardFace()
    {
        if (!useStaticBillboard)
        {
            transform.LookAt(theCam.transform);
            //Debug.Log("isRotatingFace");
        }
        else
        {
            transform.rotation = theCam.transform.rotation;
        }

        if (onlyRotateY)
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

    }
}
