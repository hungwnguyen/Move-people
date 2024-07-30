using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

namespace HungwX
{
    [RequireComponent(typeof(GuidePointManager))]
    public class WinPosController : MonoBehaviour
    {
        [SerializeField] private Transform winPos = default;
        private GuidePointManager guidePointManager = default;
        private List<WinZone> winZones = new List<WinZone>();

        public void UpdateWinPos()
        {
            guidePointManager = this.GetComponent<GuidePointManager>();
            winZones = guidePointManager.GetWinZones();
            foreach (WinZone zone in winZones)
            {
                zone.winPos = winPos.position;
            }
            print("<color=green>Success</color>");
        }
    }
}
