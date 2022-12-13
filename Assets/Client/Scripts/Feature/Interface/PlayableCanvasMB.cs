using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayableCanvasMB : MonoBehaviour
{
    [SerializeField]
    private Canvas _playableCanvas;

    [SerializeField]
    private Image _laserCooldownImage;

    [SerializeField]
    private Text _laserChargeValueText;
    [SerializeField]
    private Text _rotateAngleText;
    [SerializeField]
    private Text _speedText;
    [SerializeField]
    private Text _shipCoordinatesText;

    private int xShipCoordinates;
    private int yShipCoordinates;

    public void Disable()
    {
        _playableCanvas.enabled = false;
    }

    public void Enable()
    {
        _playableCanvas.enabled = true;
    }

    public void RefreshLaserCooldown(float currentValue, float maxValue)
    {
        _laserCooldownImage.fillAmount = currentValue / maxValue;
    }

    public void RefreshLaserChargeValue(int laserChargeValue)
    {
        _laserChargeValueText.text = laserChargeValue.ToString();
    }

    public void RefreshShipRotateAngle(int rotateAngle)
    {
        _rotateAngleText.text = rotateAngle.ToString();
    }

    public void RefreshShipSpeed(Vector3 forceVector)
    {
        var finalSpeed = Mathf.Ceil(forceVector.magnitude);

        _speedText.text = finalSpeed.ToString();
    }

    public void RefreshShipCoordinates(Vector3 position)
    {
        xShipCoordinates = Mathf.CeilToInt(position.x);
        yShipCoordinates = Mathf.CeilToInt(position.z);

        _shipCoordinatesText.text = $"{xShipCoordinates}, {yShipCoordinates}";
    }
}
