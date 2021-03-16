using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramaticExtensionAndRetraction
{
    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    public class PEAR : MonoBehaviour
    {
        private List<string> blackList;
        private string filePath = KSPUtil.ApplicationRootPath + "/GameData/FruitKocktail/PEAR/PluginData/blacklist.txt";
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
   
        public static void TogglePowerAction(Part part, bool powerOn)
        {
            if (powerOn)
            {
                if (extendStatus)
                {
                    try
                    {
                        part.GetComponent<ModuleDeployablePart>().Extend();
                        part.GetComponent<PearModule>().Events["RetractAll"].active = true;
                        part.GetComponent<PearModule>().Events["ExtendAll"].active = false;
                    }

                    catch
                    {
                        Debug.LogError("Error: PEAR.TogglePowerAction 1");
                    }
                }
                else if (!extendStatus)
                {
                    try
                    {
                        part.GetComponent<ModuleDeployablePart>().Retract();
                        part.GetComponent<PearModule>().Events["RetractAll"].active = false;
                        part.GetComponent<PearModule>().Events["ExtendAll"].active = true;
                    }

                    catch
                    {
                        Debug.LogError("Error: PEAR.TogglePowerAction 2");
                    }
                }
            }

            else
            {
                part.GetComponent<PearModule>().Events["RetractAll"].active = false;
                part.GetComponent<PearModule>().Events["ExtendAll"].active = false;
            }



        }

        public static void ProcessPear(bool isTypeExtend)
        {
            extendStatus = isTypeExtend;

            if (isTypeExtend)
            {
                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (part.HasModuleImplementing<PearPowerController>())
                    {
                        if (part.GetComponent<PearPowerController>().powerIsOn)
                        {
                            try
                            {
                                part.GetComponent<ModuleDeployablePart>().Extend();
                            }
                            catch { continue; }
                            
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
                    if (part.HasModuleImplementing<PearPowerController>())
                    {
                        if (part.GetComponent<PearPowerController>().powerIsOn)
                        {
                            try
                            {
                                if (part.GetComponent<ModuleDeployablePart>().retractable)
                                {
                                    part.GetComponent<ModuleDeployablePart>().Retract();
                                }
                            }
                            catch { continue; }

                        }

                        if (part.HasModuleImplementing<PearModule>())
                        {
                            part.GetComponent<PearModule>().Events["RetractAll"].active = false;
                            part.GetComponent<PearModule>().Events["ExtendAll"].active = true;

                        }
                    }
                }
            }
        }

        public void PowerUpPearModule(Part _part)
        {
            _part.GetComponent<PearModule>().Events["ExtendAll"].active = true;

        }

        public void Start()
        {
            blackList = new List<String>(File.ReadAllLines(filePath));

            if (HighLogic.LoadedSceneIsFlight)
            {
                foreach (var part in FlightGlobals.ActiveVessel.Parts)
                {
                    if (blackList.Contains(part.name))
                    {
                        part.RemoveModule(part.GetComponent<PearPowerController>());
                        part.RemoveModule(part.GetComponent<PearModule>());
                    }

                    if (part.HasModuleImplementing<PearPowerController>())
                    {
                        if (part.GetComponent<PearPowerController>().powerIsOn)
                        {
                            PowerUpPearModule(part);
                        }

                    }
                }
            }
        }


        public void Update()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (var part in EditorLogic.fetch.ship.Parts)
                {
                    if (blackList.Contains(part.name) && part.HasModuleImplementing<PearPowerController>())
                    {
                        part.RemoveModule(part.GetComponent<PearPowerController>());
                        part.RemoveModule(part.GetComponent<PearModule>());
                    }
                }
            }
        }

       


    }
}
