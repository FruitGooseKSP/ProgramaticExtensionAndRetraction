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
   
        // Sets a turned on part to match the rest of those on the vessel
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

        // toggle extend & retract
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

        // set event as per power status
        public void PowerUpPearModule(Part _part)
        {
            _part.GetComponent<PearModule>().Events["ExtendAll"].active = true;

        }

        // enables PEAR to function correctly on vessel switch (Issue #4)
        public void PearWatcher(Vessel vOld, Vessel vNew)
        {
            foreach (var part in vNew.Parts)
            {
                if (part.HasModuleImplementing<PearPowerController>())
                {
                    if (part.GetComponent<PearPowerController>().powerIsOn)
                    {
                        PowerUpPearModule(part);
                    }

                }
            }


        }

        public void Start()
        {
            blackList = new List<String>(File.ReadAllLines(filePath));

            GameEvents.onVesselSwitching.Add(PearWatcher);


            // Confirm part isn't on blacklist and add PEAR events according to power status

            if (HighLogic.LoadedSceneIsFlight)
            {
                foreach (var ves in FlightGlobals.Vessels)
                {
                    foreach (var part in ves.Parts)
                    {
                        if (blackList.Contains(part.name))
                        {
                            try
                            {
                                part.RemoveModule(part.GetComponent<PearPowerController>());
                                part.RemoveModule(part.GetComponent<PearModule>());
                            }
                            catch { continue; }
                        }

                        else if (part.HasModuleImplementing<PearPowerController>())
                        {
                            if (part.GetComponent<PearPowerController>().powerIsOn)
                            {
                                PowerUpPearModule(part);
                            }

                        }
                    }

                }

            }
        }

        // dynamically remove PEAR ability in the Editor for cleaner handling

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


        public void OnDisable()
        {
            GameEvents.onVesselSwitching.Remove(PearWatcher);

        }

       


    }
}
