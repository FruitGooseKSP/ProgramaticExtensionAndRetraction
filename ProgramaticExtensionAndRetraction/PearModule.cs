using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramaticExtensionAndRetraction
{
    class PearModule : PartModule
    {
        // Extend all button

        [KSPEvent(isPersistent = false, guiActive = true, guiActiveEditor = false, active = true, guiName = "Extend All Extendables")]
        public void ExtendAll()
        {
            if (moduleType == 0)
            {
                this.part.GetComponent<ModuleDeployableSolarPanel>().Events["Extend"].Invoke();
            }
            else if (moduleType == 1)
            {
                this.part.GetComponent<ModuleDeployableAntenna>().Events["Extend"].Invoke();
            }
            else if (moduleType == 2)
            {
                this.part.GetComponent<ModuleDeployableRadiator>().Events["Extend"].Invoke();
            }

            isExtended = true;
            this.part.GetComponent<PearModule>().Events["RetractAll"].active = true;
            this.part.GetComponent<PearModule>().Events["ExtendAll"].active = false;
            PEAR.ProcessPear(true);

        }

        //retract all button

        [KSPEvent(isPersistent = false, guiActive = true, guiActiveEditor = false, active = true, guiName = "Retract All Extendables")]
        public void RetractAll()
        {
            if (moduleType == 0)
            {
                this.part.GetComponent<ModuleDeployableSolarPanel>().Events["Retract"].Invoke();
            }
            else if (moduleType == 1)
            {
                this.part.GetComponent<ModuleDeployableAntenna>().Events["Retract"].Invoke();
            }
            else if (moduleType == 2)
            {
                this.part.GetComponent<ModuleDeployableRadiator>().Events["Retract"].Invoke();
            }

            isExtended = false;
            this.part.GetComponent<PearModule>().Events["RetractAll"].active = false;
            this.part.GetComponent<PearModule>().Events["ExtendAll"].active = true;
            PEAR.ProcessPear(false);

        }


        [KSPField(isPersistant = true)]
        public int moduleType;  // 0 = solar, 1 = antenna, 2 = radiator

        [KSPField(isPersistant = true)]
        public bool isExtended;


        public void Start()
        {
            // set start (instantiated) status/position

            if (HighLogic.LoadedSceneIsFlight)
            {
                isExtended = PEAR.groupExtendStatus;

               if (isExtended)
                {
                    this.part.GetComponent<PearModule>().Events["RetractAll"].active = true;
                    this.part.GetComponent<PearModule>().Events["ExtendAll"].active = false;
                }

                else
                {
                    this.part.GetComponent<PearModule>().Events["RetractAll"].active = false;
                    this.part.GetComponent<PearModule>().Events["ExtendAll"].active = true;
                }
           
            }

        }

       






    }
}
