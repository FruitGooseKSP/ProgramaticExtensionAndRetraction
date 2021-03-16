using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramaticExtensionAndRetraction
{
    public class PearPowerController : PartModule
    {
        // power toggle button

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, isPersistent = true, guiName = "Toggle PEAR")]
        public void TogglePear()
        {
            if (this.part.GetComponent<PearPowerController>().powerIsOn)
            {
                this.part.GetComponent<PearPowerController>().powerIsOn = false;
                this.part.GetComponent<PearPowerController>().pearStatus = GetPearStatus();

                if (HighLogic.LoadedSceneIsFlight)
                {
                    PEAR.TogglePowerAction(this.part, false);
                }
                
            }

            else
            {
                this.part.GetComponent<PearPowerController>().powerIsOn = true;
                this.part.GetComponent<PearPowerController>().pearStatus = GetPearStatus();

                if (HighLogic.LoadedSceneIsFlight)
                {
                    PEAR.TogglePowerAction(this.part, true);
                }

            }


        }

        // status message

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "PEAR Status:")]
        public string pearStatus;

        [KSPField(isPersistant = true)]
        public bool powerIsOn = true;


        private string GetPearStatus()
        {
            switch (powerIsOn)
            {
                case true:
                    return "Active";
                    break;
                case false:
                    return "OFFLINE";
                default:
                    return "";
            }

        }

        public void Start()
        {
            // set initial status

            if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
            {
                if (this.part.GetComponent<PearPowerController>().powerIsOn)
                {
                    pearStatus = "Active";
                }
                else pearStatus = "OFFLINE";

            }
        }




    }
}
