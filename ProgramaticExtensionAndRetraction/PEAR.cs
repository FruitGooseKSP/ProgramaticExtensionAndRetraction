using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramaticExtensionAndRetraction
{
    public class PEAR : PartModule
    {
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, isPersistent = true, guiName = "Toggle PEAR Status")]
        public void TogglePearStatus()
        {
            if (isActive)
            {
                pearStatus = "OFFLINE";
                isActive = false;
                this.part.GetComponent<PEAR>().Events["ExtendAll"].active = false;
                this.part.GetComponent<PEAR>().Events["RetractAll"].active = false;
            }
            else if (!isActive)
            {
                pearStatus = "Active";
                isActive = true;
                CheckState();
            }
        }


        // Event to extend all

        [KSPEvent(active = true, guiActive = true, isPersistent = true, guiName = "Extend ALL Extendables")]
        public void ExtendAll()
        {
            try
            {
                if (!gTG)
                {
                    gTG = CheckFairing();
                }

                if (gTG)
                {
                    foreach (var part in FlightGlobals.ActiveVessel.Parts)
                    {
                        if (part.HasModuleImplementing<PEAR>())
                        {


                            if (part.GetComponent<PEAR>().pearStatus == "Active")
                            {
                                part.SendEvent("Extend");
                                part.GetComponent<PEAR>().Events["ExtendAll"].active = false;
                                part.GetComponent<PEAR>().Events["RetractAll"].active = true;
                            }
                            else continue;
                        }
                    }
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
                Debug.LogError("Error Ref- PEAR: KSPEvent: ExtendAll; caught exception");
            }
        }

        // event to retract all

        [KSPEvent(active = true, guiActive = true, isPersistent = true, guiName = "Retract ALL Extendables")]
        public void RetractAll()
        {
            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                if (part.HasModuleImplementing<PEAR>())
                {

                    if (part.GetComponent<PEAR>().isActive) 
                    {
                        part.SendEvent("Retract");
                        part.GetComponent<PEAR>().Events["ExtendAll"].active = true;
                        part.GetComponent<PEAR>().Events["RetractAll"].active = false;

                    }
                    

                }
            }
        }

        [KSPField(isPersistant = true)]
        public bool gTG = false;

        [KSPField(isPersistant = true)]
        public bool isActive = true;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "PEAR Status:")]
        public string pearStatus = "Active";

        private List<Part> deployableList;
       

           
        // if fairing present, prevent extend action to avoid panels potentially extending through the fairing
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

        // back up for persistance
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
                        part.GetComponent<PEAR>().Events["ExtendAll"].active = false;
                        part.GetComponent<PEAR>().Events["RetractAll"].active = true;
                    }
                    else
                    {
                        RetractAll();
                        part.GetComponent<PEAR>().Events["ExtendAll"].active = true;
                        part.GetComponent<PEAR>().Events["RetractAll"].active = false;
                    }
                }

               

               
            }

            catch
            {
                return;
            }


            

        }

        public void SetActiveStatus()
        {
            foreach (var part in deployableList)
            {
                if (!part.GetComponent<PEAR>().isActive)
                {
                    part.GetComponent<PEAR>().pearStatus = "OFFLINE";
                }
                else part.GetComponent<PEAR>().pearStatus = "Active";
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
                    SetActiveStatus();

                }
                catch
                {
                    // internal error
                }
            }



        }


       


    }
}
