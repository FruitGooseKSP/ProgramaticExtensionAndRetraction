using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramaticExtensionAndRetraction
{
    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    public class PEAR : MonoBehaviour
    {

        private static bool extendStatus;

        public static bool groupExtendStatus
        {
            get
            {
                return PEAR.extendStatus;
            }
            set
            {
                extendStatus = value;
            }
        }
        private int GetModuleType(Part _part)
        {
            if (_part.HasModuleImplementing<ModuleDeployableSolarPanel>())
            {
                return 0;
            }
            else if (_part.HasModuleImplementing<ModuleDeployableAntenna>())
            {
                return 1;
            }
            else if (_part.HasModuleImplementing<ModuleDeployableRadiator>())
            {
                return 2;
            }
            else return 0;
        }
    
        // Global toggling of extend and retract
        public static void TogglePowerAction(Part part, bool powerIsOn)
        {
            if (powerIsOn)
            {
                part.AddModule("PearModule", true);


                if (extendStatus)
                {
                    int type = part.GetComponent<PearModule>().moduleType;

                    if (type == 0)
                    {
                        part.GetComponent<ModuleDeployableSolarPanel>().Events["Extend"].Invoke();
                    }
                    else if (type == 1)
                    {
                        part.GetComponent<ModuleDeployableAntenna>().Events["Extend"].Invoke();
                    }
                    else if (type == 2)
                    {
                        part.GetComponent<ModuleDeployableRadiator>().Events["Extend"].Invoke();
                    }

                    part.GetComponent<PearModule>().Events["RetractAll"].active = true;
                    part.GetComponent<PearModule>().Events["ExtendAll"].active = false;
                }

                else
                {
                    int type = part.GetComponent<PearModule>().moduleType;

                    if (type == 0)
                    {
                        part.GetComponent<ModuleDeployableSolarPanel>().Events["Retract"].Invoke();
                    }
                    else if (type == 1)
                    {
                        part.GetComponent<ModuleDeployableAntenna>().Events["Retract"].Invoke();
                    }
                    else if (type == 2)
                    {
                        part.GetComponent<ModuleDeployableRadiator>().Events["Retract"].Invoke();
                    }

                    part.GetComponent<PearModule>().Events["RetractAll"].active = false;
                    part.GetComponent<PearModule>().Events["ExtendAll"].active = true;
                }

            }
            else
            {
                part.RemoveModule(part.GetComponent<PearModule>());
            }

        } 

        public static void ProcessPear(bool isTypeExtend)
        {
            groupExtendStatus = isTypeExtend;

            if (isTypeExtend)
            {
                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.HasModuleImplementing<PearModule>())
                    {
                        int type = part.GetComponent<PearModule>().moduleType;

                        if (type == 0)
                        {
                            part.GetComponent<ModuleDeployableSolarPanel>().Events["Extend"].Invoke();
                        }
                        else if (type == 1)
                        {
                            part.GetComponent<ModuleDeployableAntenna>().Events["Extend"].Invoke();
                        }
                        else if (type == 2)
                        {
                            part.GetComponent<ModuleDeployableRadiator>().Events["Extend"].Invoke();
                        }

                        part.GetComponent<PearModule>().Events["RetractAll"].active = true;
                        part.GetComponent<PearModule>().Events["ExtendAll"].active = false;
                    }
                }
            }
            else
            {
                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.HasModuleImplementing<PearModule>())
                    {
                        int type = part.GetComponent<PearModule>().moduleType;

                        if (type == 0)
                        {
                            part.GetComponent<ModuleDeployableSolarPanel>().Events["Retract"].Invoke();
                        }
                        else if (type == 1)
                        {
                            part.GetComponent<ModuleDeployableAntenna>().Events["Retract"].Invoke();
                        }
                        else if (type == 2)
                        {
                            part.GetComponent<ModuleDeployableRadiator>().Events["Retract"].Invoke();
                        }

                        part.GetComponent<PearModule>().Events["RetractAll"].active = false;
                        part.GetComponent<PearModule>().Events["ExtendAll"].active = true;
                    }
                }
            }
        }

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                { 
                   // remotely add ability to extend/retract

                    if (part.HasModuleImplementing<PearPowerController>())
                    {
                       
                        if (part.GetComponent<PearPowerController>().powerIsOn && !part.HasModuleImplementing<PearModule>())
                        {
                            part.AddModule("PearModule");
                        }
                        else if (!part.GetComponent<PearPowerController>().powerIsOn && part.HasModuleImplementing<PearModule>())
                        {
                            part.RemoveModule(FlightGlobals.ActiveVessel.GetComponent<PearModule>());
                        }
                    }

                    if (part.HasModuleImplementing<PearModule>())
                    {
                        part.GetComponent<PearModule>().moduleType = GetModuleType(part);
                    }

                }
            }
        }

        




    }
}
