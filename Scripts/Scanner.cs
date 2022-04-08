using UnityEngine;
using ZXing;
using UnityEngine.UI;

public class Scanner : MonoBehaviour
{

    [SerializeField]
    private RectTransform _scanArea;

    [SerializeField]
    private RawImage _background;

    [SerializeField]
    private Text _text;

    [SerializeField]
    private AspectRatioFitter _arf;

    [SerializeField]
    private Button _btn;

    private bool _isCamAvaible;

    private WebCamTexture _camTexture;

    private void Start()
    {
        SetUpCamera();
    }

    private void Update()
    {
        UpdateCameraRenderer();
    }


    private void UpdateCameraRenderer()
    {
        if (!_isCamAvaible)
            return;

        float ratio = (float)_camTexture.width / (float)_camTexture.height;
        _arf.aspectRatio = ratio;


        float scaleY = _camTexture.videoVerticallyMirrored ? -1f : 1f;
        _background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orientation = -_camTexture.videoRotationAngle;
        _background.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

        //RelocateButton();


    }


    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            _isCamAvaible = false;
            return;
        }

        foreach (var device in devices)
        {
            if (!device.isFrontFacing)
            {
                _camTexture = new WebCamTexture(device.name, (int)_scanArea.rect.width, (int)_scanArea.rect.height);
            }
        }

        if (_camTexture == null)
        {
            Debug.Log("NO CAMERA FOUND.");
            return;
        }

        _camTexture.Play();
        _background.texture = _camTexture;
        _isCamAvaible = true;
    }

    public void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_camTexture.GetPixels32(), _camTexture.width, _camTexture.height);
            if (result != null)
                _text.text = result.Text;
            else
                _text.text = "ОШИБКА СЧИТЫВАНИЯ";
        }
        catch
        {
            _text.text = "ОШИБКА ПОПЫТКИ СЧИТЫВАНИЯ";
        }
    }

    public string GetUrl()
    {
        string url = _text.text;
        return url;
    }

    public void OpenUrl()
    {
        string url = GetUrl();
        if (url.Substring(0, 4) != "http")
            return;
        Application.OpenURL(url);
    }

}
