using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramaticExtensionAndRetraction
{
    public class PEAR : PartModule
    {

        [KSPEvent(active = true, guiActive = false, isPersistent = true, guiName = "Extend ALL Extendables")]
        public void ExtendAll()
        {
            try
            {
                if (!gTG)
                {
                    gTG = CheckFairing();
                }

                if (gTG)

                    foreach (var part in deployableList)
                    {
                        part.SendEvent("Extend");
                    }
                else
                {
                    ScreenMessage screenMessage = new ScreenMessage("PEAR is disabled - deploy fairings first",
                        3.0f, ScreenMessageStyle.KERBAL_EVA);
                    ScreenMessages.PostScreenMessage(screenMessage);
                }
            }
            catch
            {
                Debug.LogError("Error Ref- KSPEvent: ExtendAll; caught exception");
            }
        }


        [KSPEvent(active = true, guiActive = false, isPersistent = true, guiName = "Retract ALL Extendables")]
        public void RetractAll()
        {
            foreach (var part in deployableList)
            {
                part.SendEvent("Retract");
            }
        }

        [KSPField(isPersistant = true)]
        public bool gTG = false;

        private List<Part> deployableList;
       
        
        private bool CheckFairing()
        {
            bool canGo = true;

            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.HasModuleImplementing<ModuleProceduralFairing>())
                {
                    List<ProceduralFairings.FairingPanel> fairingPanels = part.GetComponent<ModuleProceduralFairing>().Panels;

                    try
                    {
                        int panelCount = fairingPanels.Count();

                        if (panelCount == 0)
                        {
                            canGo = true;
                        }
                        else canGo = false;
                    }
                    catch
                    { // internal error 
                    }
                }
            }

            return canGo;
        }

        private void CheckState()
        {
            try
            {
                Part ranPart = deployableList[0];

                if (ranPart != null)
                {
                    ModuleDeployablePart.DeployState deployState = (ModuleDeployablePart.DeployState)ranPart.GetModuleStartState();

                    if (deployState == ModuleDeployablePart.DeployState.EXTENDED)
                    {
                        ExtendAll();
                    }
                    else return;
                }
            }

            catch
            {
                return;
            }
        }

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                try
                {
                    deployableList = new List<Part>();

                    foreach (var part in FlightGlobals.ActiveVessel.Parts)
                    {
                        if (part.HasModuleImplementing<PEAR>())
                        {
                            deployableList.Add(part);
                        }
                    }

                    CheckState();


                }
                catch
                {
                    // internal error
                }
            }



        }


    }
}
