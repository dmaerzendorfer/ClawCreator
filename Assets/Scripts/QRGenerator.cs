using UnityEngine;
using QRCoder;
using QRCoder.Unity;

public class QrGenerator : MonoBehaviour
{
    [SerializeField] private PolaroidAnimations polaroidAnimations;

    [SerializeField] private SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateQRCode(0, 0, 0, 0, 0, 0);
    }

    public void updateQRCode(int hairId, int noseId, int mouthId, int eyeId, int outfitId, int color)
    {
        polaroidAnimations.FallOut(() =>
        {
            Texture2D texture = generateQR("https://clawsembly.vercel.app/?hairId=" + hairId + "&noseId=" + noseId +
                                           "&mouthId=" + mouthId + "&eyeId=" + eyeId + "&outfitId=" + outfitId +
                                           "&color=" + color);
            sprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            polaroidAnimations.SlideIn();
        });
    }

    Texture2D generateQR(string text)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(3);
        return qrCodeAsTexture2D;
    }
}