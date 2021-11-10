using GameReaderCommon;
using SimHub.Plugins;
using System;





namespace TomHogue.CarSetupPlugin
{



    [PluginDescription("Car Setup Focused Data")]
    [PluginAuthor("Tom Hogue")]
    [PluginName("Car Setup Plugin")]


    public class CarSetupPlugin : IPlugin, IDataPlugin, IWPFSettings
       // public class DataPlugin : IPlugin, IDataPlugin, IWPFSettings
    {

         public CarSetupPluginSettings Settings;
        // constants

     //   const double nextgenSurfaceArea = 0.0;
        const double nextgenWheelBase = 92; // 92 inches.  This is shorter than wheel base as it accomodates ride height measuring points on frame

     //   const double truckSurfaceArea = 0.0;
        const double truckWheelBase = 95; //95 inches



        const double newtonConversionFactor = 0.22481;
        const double metersConversionFactor = 39.3701;
        const double millimetersConversionFactor = 0.0393701;

        private readonly string modifiedTourCar = "Modified - Tour";
        private readonly string modifiedSK = "Modified - SK";
        
        private readonly string nextGenChevrolet = "NASCAR Chevrolet Camaro ZL1";
        private readonly string nextGenFord = "Ford Mustang";
        private readonly string nextGenToyota = "Toyota Camary";

        private readonly string truckChevrolet = "Chevrolet Silverado";
        private readonly string truckFord = "Ford F150";
        private readonly string truckToyota = "Toyota Tundra";

        private double truckRakeAngle = 0;
        private double nextRakeAngle = 0;



        //input variables
        private double lfCornerWeight = 0, rfCornerWeight = 0, rrCornerWeight = 0, lrCornerWeight = 0;
        private double lfDynamicShockDeflection = 0, rfDynamicShockDeflection = 0, lrDynamicShockDeflection = 0, rrDynamicShockDeflection = 0;
        private double lfShockDeflectionZero = 0, rfShockDeflectionZero = 0, lrShockDeflectionZero = 0, rrShockDeflectionZero = 0;
        private double totalStaticShockDeflection = 0;
        private double totalStaticWeight = 0;
        private double lfStaticCornerWeightPercent = 0, rfStaticCornerWeightPercent = 0, lrStaticCornerWeightPercent = 0, rrStaticCornerWeightPercent = 0;
        private double lfDynamicCornerWeightPercent = 0, rfDynamicCornerWeightPercent = 0, lrDynamicCornerWeightPercent = 0, rrDynamicCornerWeightPercent = 0;
        private double totalDynamicAdjustedShockDeflection = 0;

        //flags

        private bool shocksInUse = false; //some cars use shocks, some cars use dampers.  Flag to tell us which. False = dampers.

        private bool isOnPitRoadFlag = false;
        private bool isOnTrackFlag = false;
        private bool isInGarageFlag = false;

        private bool isTruckFlag = false;
        private bool isNextGenFlag = false;

        //NextGen Front Splitter is .3" above centerline height
     


        //Temp holders
        private string tmpString, tmpWeightString, tmpHeightString, tmpShockString;


        private string carModel;


        //output variables

        private double lfDynamicRideHeight = 0, rfDynamicRideHeight = 0, rrDynamicRideHeight = 0, lrDynamicRideHeight = 0;
        private double lfStaticRideHeight = 0, rfStaticRideHeight = 0, rrStaticRideHeight = 0, lrStaticRideHeight = 0;
        private double staticCenterRake = 0, dynamicCenterRake = 0;

        private double lfShockTravelAdjuster = 0, rfShockTravelAdjuster = 0, lrShockTravelAdjuster = 0, rrShockTravelAdjuster = 0;  //these are used to normalize shock travels for dynamic weight calculations






        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Called one time per game data update, contains all normalized game data, 
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        /// 
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        /// 
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // Define the value of our property (declared in init)
          //  pluginManager.SetPropertyValue("CurrentDateTime", this.GetType(), DateTime.Now);

            if (data.GameRunning)
            {
                if (data.OldData != null && data.NewData != null)
                {
                    carModel = pluginManager.GetPropertyValue("DataCorePlugin.GameData.CarModel").ToString();
                    if (carModel != null)
                    {

                        // SimHub.Logging.Current.Info("Car Model is" + carModel);

                        if (carModel == modifiedTourCar)
                        {

                            shocksInUse = true;
                            isTruckFlag = false;
                            isNextGenFlag = false;

                        }
                        else if (carModel == modifiedSK) { 
                        
                            shocksInUse = true;
                            isTruckFlag = false;
                            isNextGenFlag = false;
                        }

                        else if (carModel == nextGenChevrolet)
                        {
                            shocksInUse = true;
                            isTruckFlag = false;
                            isNextGenFlag = true;

                        }
                        else if (carModel == nextGenFord)
                        {
                             shocksInUse = true;
                             isTruckFlag = false;
                             isNextGenFlag = true;

                        }
                        else if (carModel == nextGenToyota)
                        {
                              shocksInUse = true;
                              isTruckFlag = false;
                              isNextGenFlag = true;

                        }
                        else if (carModel == truckFord)
                        {
                            shocksInUse = false;
                            isTruckFlag = true;
                            isNextGenFlag = false;

                        }
                        else if (carModel == truckChevrolet)
                        {
                            shocksInUse = false;
                            isTruckFlag = true;
                            isNextGenFlag = false;

                        }
                        else if (carModel == truckToyota)
                        {
                            shocksInUse = false;
                            isTruckFlag = true;
                            isNextGenFlag = false;

                        } else
                        {
                            shocksInUse = false;
                            isTruckFlag = false;
                            isNextGenFlag = false;
                        }

                        pluginManager.SetPropertyValue("CarModel", this.GetType(), carModel);

//                        if (shocksInUse == true)
//                        {
//                            pluginManager.SetPropertyValue("Shocks", this.GetType(), "True");
//                        }
 //                       else
 //                      {
 //                           pluginManager.SetPropertyValue("Shocks", this.GetType(), "False");
 //
//                        }
                    } //end if car model

                    if (pluginManager.GetPropertyValue("GameRawData.Telemetry.OnPitRoad") != null)
                    {
                        tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.OnPitRoad").ToString();
                        if (tmpString == "True")
                        {
                            isOnPitRoadFlag = true;
                        }
                        else
                        {
                            isOnPitRoadFlag = false;
                        }
                        //                    pluginManager.SetPropertyValue("isOnPitRoadFlag", this.GetType(), isOnPitRoadFlag);
                    }


                    if (pluginManager.GetPropertyValue("GameRawData.Telemetry.IsOnTrack") != null)

                    {
                        tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.IsOnTrack").ToString();
                        if (tmpString == "True")
                        {
                            isOnTrackFlag = true;
                        }
                        else
                        {
                            isOnTrackFlag = false;
                        }
                        //                    pluginManager.SetPropertyValue("isOnTrackFlag", this.GetType(), isOnTrackFlag);
                    }


                    if (pluginManager.GetPropertyValue("GameRawData.Telemetry.IsInGarage") != null)
                    {
                        tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.IsInGarage").ToString();
                        if (tmpString == "True")
                        {
                            isInGarageFlag = true;
                        }
                        else
                        {
                            isInGarageFlag = false;
                        }


                        //                    pluginManager.SetPropertyValue("isInGarageFlag", this.GetType(), isInGarageFlag);

                    }


                    // Static Corner Weights

                    if (pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftFront.CornerWeight") != null) { 
                        tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftFront.CornerWeight").ToString();

                        tmpWeightString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpWeightString += tmpString[i];
                            }
                            else
                             if (tmpString[i] == '.')
                            {
                                tmpWeightString += tmpString[i];
                            }
                        }
                        lfCornerWeight = Convert.ToDouble(tmpWeightString) * newtonConversionFactor ;
                       

                        pluginManager.SetPropertyValue("Static.LF.CornerWeight", this.GetType(), lfCornerWeight);

                        tmpString = null;


                    }

                    if ( pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightFront.CornerWeight") != null ) {
                        
                        tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightFront.CornerWeight").ToString();

                        tmpWeightString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpWeightString += tmpString[i];
                            }
                            else
                             if (tmpString[i] == '.')
                            {
                                tmpWeightString += tmpString[i];
                            }
                        }
                        rfCornerWeight = Convert.ToDouble(tmpWeightString) * newtonConversionFactor;

                        pluginManager.SetPropertyValue("Static.RF.CornerWeight", this.GetType(), rfCornerWeight);
                        tmpString = null;

                    }

                    if (pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightRear.CornerWeight") != null) { 
                        tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightRear.CornerWeight").ToString();

                        tmpWeightString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpWeightString += tmpString[i];
                            }
                            else
                             if (tmpString[i] == '.')
                            {
                                tmpWeightString += tmpString[i];
                            }
                        }
                        rrCornerWeight = Convert.ToDouble(tmpWeightString) * newtonConversionFactor;

                        pluginManager.SetPropertyValue("Static.RR.CornerWeight", this.GetType(), rrCornerWeight);
                        tmpString = null;

                    }

                    if (pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftRear.CornerWeight") != null) { 

                        tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftRear.CornerWeight").ToString();

                        tmpWeightString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpWeightString += tmpString[i];
                            }
                            else
                             if (tmpString[i] == '.')
                            {
                                tmpWeightString += tmpString[i];
                            }
                        }
                        lrCornerWeight = Convert.ToDouble(tmpWeightString) * newtonConversionFactor;

                        pluginManager.SetPropertyValue("Static.LR.CornerWeight", this.GetType(), lrCornerWeight);
                        tmpString = null;

                    }




                    /////Static Ride Heights

                    if ( pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftFront.RideHeight") != null) {  
                        tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftFront.RideHeight").ToString();

                            tmpHeightString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpHeightString += tmpString[i];
                                }
                                else
                                 if (tmpString[i] == '.')
                                {
                                    tmpHeightString += tmpString[i];
                                }
                            }
                            lfStaticRideHeight = Convert.ToDouble(tmpHeightString);
                            lfStaticRideHeight = Math.Round(lfStaticRideHeight * millimetersConversionFactor, 2);

                            pluginManager.SetPropertyValue("Static.LF.RideHeight", this.GetType(), lfStaticRideHeight);
                            tmpString = null;
                            tmpHeightString = null;

                        }


                    if (pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightFront.RideHeight") != null) { 
                            
                            tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightFront.RideHeight").ToString();
                            tmpHeightString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpHeightString += tmpString[i];
                                }
                                else
                                if (tmpString[i] == '.')
                                {
                                    tmpHeightString += tmpString[i];
                                }
                            }
                            rfStaticRideHeight = Convert.ToDouble(tmpHeightString);
                            rfStaticRideHeight = Math.Round(rfStaticRideHeight * millimetersConversionFactor, 2);

                            pluginManager.SetPropertyValue("Static.RF.RideHeight", this.GetType(), rfStaticRideHeight);
                            tmpString = null;

                        }


                    if( pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftRear.RideHeight") != null) { 
                            
                            tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.LeftRear.RideHeight").ToString();

                            tmpHeightString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpHeightString += tmpString[i];
                                }
                                else
                                 if (tmpString[i] == '.')
                                {
                                    tmpHeightString += tmpString[i];
                                }
                            }
                            lrStaticRideHeight = Convert.ToDouble(tmpHeightString);
                            lrStaticRideHeight = Math.Round(lrStaticRideHeight * millimetersConversionFactor, 2);


                            pluginManager.SetPropertyValue("Static.LR.RideHeight", this.GetType(), lrStaticRideHeight);
                            tmpString = null;


                        }


                        if ( pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightRear.RideHeight") != null) { 

                            tmpString = pluginManager.GetPropertyValue("GameRawData.SessionData.CarSetup.Chassis.RightRear.RideHeight").ToString();

                            tmpHeightString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpHeightString += tmpString[i];
                                }
                                else
                                 if (tmpString[i] == '.')
                                {
                                    tmpHeightString += tmpString[i];
                                }
                            }
                            rrStaticRideHeight = Convert.ToDouble(tmpHeightString);
                            rrStaticRideHeight = Math.Round(rrStaticRideHeight * millimetersConversionFactor, 2);

                            pluginManager.SetPropertyValue("Static.RR.RideHeight", this.GetType(), rrStaticRideHeight);
                            tmpString = null;

                        }


                    //Shock Deflections

                    if (shocksInUse == true)
                    {
                        if (pluginManager.GetPropertyValue("GameRawData.Telemetry.LFshockDefl") != null) {

                            tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.LFshockDefl").ToString();

                            tmpShockString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpShockString += tmpString[i];
                                }
                                else
                                if (tmpString[i] == '.')
                                {
                                    tmpShockString += tmpString[i];
                                }


                            }

                            lfDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                            lfDynamicShockDeflection = Math.Round(lfDynamicShockDeflection * metersConversionFactor, 2);

//                            pluginManager.SetPropertyValue("Dynamic.LF.ShockDeflection", this.GetType(), lfDynamicShockDeflection);
                            tmpString = null;

                        }

                        if (pluginManager.GetPropertyValue("GameRawData.Telemetry.RFshockDefl") != null) {

                            tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.RFshockDefl").ToString();

                            tmpShockString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpShockString += tmpString[i];
                                }
                                else
                                if (tmpString[i] == '.')
                                {
                                    tmpShockString += tmpString[i];
                                }
                            }
                            rfDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                            rfDynamicShockDeflection = Math.Round(rfDynamicShockDeflection * metersConversionFactor, 2);

//                            pluginManager.SetPropertyValue("Dynamic.RF.ShockDeflection", this.GetType(), rfDynamicShockDeflection);
                            tmpString = null;

                        }

                        if (pluginManager.GetPropertyValue("GameRawData.Telemetry.LRshockDefl") != null) {

                            tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.LRshockDefl").ToString();

                            tmpShockString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpShockString += tmpString[i];
                                }
                                else
                                if (tmpString[i] == '.')
                                {
                                    tmpShockString += tmpString[i];
                                }
                            }
                            lrDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                            lrDynamicShockDeflection = Math.Round(lrDynamicShockDeflection * metersConversionFactor, 2);

 //                           pluginManager.SetPropertyValue("Dynamic.LR.ShockDeflection", this.GetType(), lrDynamicShockDeflection);
                            tmpString = null;

                        }

                        if ( pluginManager.GetPropertyValue("GameRawData.Telemetry.RRshockDefl") != null) {

                            tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.RRshockDefl").ToString();

                            tmpShockString = null;

                            for (int i = 0; i < tmpString.Length; i++)
                            {
                                if (Char.IsDigit(tmpString[i]))
                                {
                                    tmpShockString += tmpString[i];
                                }
                                else
                                if (tmpString[i] == '.')
                                {
                                    tmpShockString += tmpString[i];
                                }
                            }
                            rrDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                            rrDynamicShockDeflection = Math.Round(rrDynamicShockDeflection * metersConversionFactor, 2);

//                            pluginManager.SetPropertyValue("Dynamic.RR.ShockDeflection", this.GetType(), rrDynamicShockDeflection);
                            tmpString = null;

                        }



                    }
                    else if (shocksInUse == false)
                    {

                     if (pluginManager.GetPropertyValue("GameRawData.Telemetry.LFSHshockDefl") != null) {

                         tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.LFSHshockDefl").ToString();


                        tmpShockString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpShockString += tmpString[i];
                            }
                            else
                            if (tmpString[i] == '.')
                            {
                                tmpShockString += tmpString[i];
                            }


                        }

                        lfDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                        lfDynamicShockDeflection = Math.Round(lfDynamicShockDeflection * metersConversionFactor, 2);

//                        pluginManager.SetPropertyValue("Dynamic.LF.ShockDeflection", this.GetType(), lfDynamicShockDeflection);
                        tmpString = null;

                    }

                    if (pluginManager.GetPropertyValue("GameRawData.Telemetry.RFSHshockDefl") != null ) {
                        tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.RFSHshockDefl").ToString();

                        tmpShockString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpShockString += tmpString[i];
                            }
                            else
                            if (tmpString[i] == '.')
                            {
                                tmpShockString += tmpString[i];
                            }
                        }
                        rfDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                        rfDynamicShockDeflection = Math.Round(rfDynamicShockDeflection * metersConversionFactor, 2);

//                        pluginManager.SetPropertyValue("Dynamic.RF.ShockDeflection", this.GetType(), rfDynamicShockDeflection);
                        tmpString = null;

                    }

                     if (pluginManager.GetPropertyValue("GameRawData.Telemetry.LRSHshockDefl") != null) {

                        tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.LRSHshockDefl").ToString();

                        tmpShockString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpShockString += tmpString[i];
                            }
                            else
                            if (tmpString[i] == '.')
                            {
                                tmpShockString += tmpString[i];
                            }
                        }
                        lrDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                        lrDynamicShockDeflection = Math.Round(lrDynamicShockDeflection * metersConversionFactor, 2);

 //                       pluginManager.SetPropertyValue("Dynamic.LR.ShockDeflection", this.GetType(), lrDynamicShockDeflection);
                        tmpString = null;

                    }

                     if (pluginManager.GetPropertyValue("GameRawData.Telemetry.RRSHshockDefl") != null) {

                        tmpString = pluginManager.GetPropertyValue("GameRawData.Telemetry.RRSHshockDefl").ToString();

                        tmpShockString = null;

                        for (int i = 0; i < tmpString.Length; i++)
                        {
                            if (Char.IsDigit(tmpString[i]))
                            {
                                tmpShockString += tmpString[i];
                            }
                            else
                            if (tmpString[i] == '.')
                            {
                                tmpShockString += tmpString[i];
                            }
                        }
                        rrDynamicShockDeflection = Convert.ToDouble(tmpShockString);
                        rrDynamicShockDeflection = Math.Round(rrDynamicShockDeflection * metersConversionFactor, 2);

 //                       pluginManager.SetPropertyValue("Dynamic.RR.ShockDeflection", this.GetType(), rrDynamicShockDeflection);
                        tmpString = null;

                    }

                    }//end if shocksInUse

                    //Dynamic Ride Heights

                    if (isInGarageFlag == true)
                    {
                        lfShockDeflectionZero = lfDynamicShockDeflection;
                        rfShockDeflectionZero = rfDynamicShockDeflection;
                        lrShockDeflectionZero = lrDynamicShockDeflection;
                        rrShockDeflectionZero = rrDynamicShockDeflection;

                    } //end isInPit


                        lfDynamicRideHeight = lfStaticRideHeight + (lfShockDeflectionZero - lfDynamicShockDeflection);
                        rfDynamicRideHeight = rfStaticRideHeight + (rfShockDeflectionZero - rfDynamicShockDeflection);
                        lrDynamicRideHeight = lrStaticRideHeight + (lrShockDeflectionZero - lrDynamicShockDeflection);
                        rrDynamicRideHeight = rrStaticRideHeight + (rrShockDeflectionZero - rrDynamicShockDeflection);


                    
                    pluginManager.SetPropertyValue("Dynamic.LF.RideHeight", this.GetType(), lfDynamicRideHeight);
                    pluginManager.SetPropertyValue("Dynamic.RF.RideHeight", this.GetType(), rfDynamicRideHeight);
                    pluginManager.SetPropertyValue("Dynamic.LR.RideHeight", this.GetType(), lrDynamicRideHeight);
                    pluginManager.SetPropertyValue("Dynamic.RR.RideHeight", this.GetType(), rrDynamicRideHeight);


                    //Calculating ShockTravel adjustments for Dynamic Weight Calculations

                    //Commented out SetPropertyValues were used for debug
                    //
                    if (isInGarageFlag == true)
                    {
                        totalStaticWeight = lfCornerWeight + rfCornerWeight + lrCornerWeight + rrCornerWeight;
                        totalStaticShockDeflection = lfDynamicShockDeflection + rfDynamicShockDeflection + lrDynamicShockDeflection + rrDynamicShockDeflection;

//                        pluginManager.SetPropertyValue("totalStaticWeight", this.GetType(), totalStaticWeight);
//                        pluginManager.SetPropertyValue("totalStaticShockDeflection", this.GetType(), totalStaticShockDeflection);

                        lfStaticCornerWeightPercent = lfCornerWeight / totalStaticWeight;
                        rfStaticCornerWeightPercent = rfCornerWeight / totalStaticWeight;
                        lrStaticCornerWeightPercent = lrCornerWeight / totalStaticWeight;
                        rrStaticCornerWeightPercent = rrCornerWeight / totalStaticWeight;

//                        pluginManager.SetPropertyValue("lfStaticCornerWeightPercent", this.GetType(), lfStaticCornerWeightPercent);
//                        pluginManager.SetPropertyValue("rfStaticCornerWeightPercent", this.GetType(), rfStaticCornerWeightPercent);
//                        pluginManager.SetPropertyValue("lrStaticCornerWeightPercent", this.GetType(), lrStaticCornerWeightPercent);
//                        pluginManager.SetPropertyValue("rrStaticCornerWeightPercent", this.GetType(), rrStaticCornerWeightPercent);

                        lfShockTravelAdjuster = (lfStaticCornerWeightPercent * totalStaticShockDeflection) - lfDynamicShockDeflection;
                        rfShockTravelAdjuster = (rfStaticCornerWeightPercent * totalStaticShockDeflection) - rfDynamicShockDeflection;
                        lrShockTravelAdjuster = (lrStaticCornerWeightPercent * totalStaticShockDeflection) - lrDynamicShockDeflection;
                        rrShockTravelAdjuster = (rrStaticCornerWeightPercent * totalStaticShockDeflection) - rrDynamicShockDeflection;

 //                       pluginManager.SetPropertyValue("lfShockTravelAdjuster", this.GetType(), lfShockTravelAdjuster);
 //                       pluginManager.SetPropertyValue("rfShockTravelAdjuster", this.GetType(), rfShockTravelAdjuster);
 //                       pluginManager.SetPropertyValue("lrShockTravelAdjuster", this.GetType(), lrShockTravelAdjuster);
 //                       pluginManager.SetPropertyValue("rrShockTravelAdjuster", this.GetType(), rrShockTravelAdjuster);




                        totalDynamicAdjustedShockDeflection = (lfDynamicShockDeflection - lfShockTravelAdjuster) 
                                                            + (rfDynamicShockDeflection - rfShockTravelAdjuster) 
                                                            + (lrDynamicShockDeflection - lrShockTravelAdjuster) 
                                                            + (rrDynamicShockDeflection - rrShockTravelAdjuster);


                    }

                    //Calculating Dynamic Corner Weights
                    lfDynamicCornerWeightPercent = (lfDynamicShockDeflection + lfShockTravelAdjuster) / totalDynamicAdjustedShockDeflection;
                    rfDynamicCornerWeightPercent = (rfDynamicShockDeflection + rfShockTravelAdjuster) / totalDynamicAdjustedShockDeflection;
                    lrDynamicCornerWeightPercent = (lrDynamicShockDeflection + lrShockTravelAdjuster) / totalDynamicAdjustedShockDeflection;
                    rrDynamicCornerWeightPercent = (rrDynamicShockDeflection + rrShockTravelAdjuster) / totalDynamicAdjustedShockDeflection;


                        pluginManager.SetPropertyValue("Dynamic.LF.CornerWeight", this.GetType(), lfDynamicCornerWeightPercent);
                        pluginManager.SetPropertyValue("Dynamic.RF.CornerWeight", this.GetType(), rfDynamicCornerWeightPercent);
                        pluginManager.SetPropertyValue("Dynamic.LR.CornerWeight", this.GetType(), lrDynamicCornerWeightPercent);
                        pluginManager.SetPropertyValue("Dynamic.RR.CornerWeight", this.GetType(), rrDynamicCornerWeightPercent);
                    

                    //Static Center Rake

                    if (rfStaticRideHeight != 0 && lfStaticRideHeight != 0 && lrStaticRideHeight != 0 && rrStaticRideHeight != 0)
                    {

                        staticCenterRake =  ((rrStaticRideHeight - lrStaticRideHeight) / 2 + lrStaticRideHeight) - ((rfStaticRideHeight - lfStaticRideHeight) / 2 + lfStaticRideHeight);
                        pluginManager.SetPropertyValue("Static.CenterRake", this.GetType(), Math.Round(staticCenterRake, 2));

                    }

                    //Calculating Downforce
                    if (rfDynamicRideHeight != 0 && lfDynamicRideHeight != 0 && lrDynamicRideHeight != 0 && rrDynamicRideHeight != 0)
                    {

                        dynamicCenterRake = ((rrDynamicRideHeight - lrDynamicRideHeight) / 2 + lrDynamicRideHeight) - ((rfDynamicRideHeight - lfDynamicRideHeight) / 2 + lfDynamicRideHeight);
                        pluginManager.SetPropertyValue("Dynamic.CenterRake", this.GetType(), Math.Round(dynamicCenterRake, 2));

                        if (isTruckFlag == true)
                        {
                            truckRakeAngle = Math.Atan(truckWheelBase / dynamicCenterRake);  //arctan of the slope (wheelbase/centerRake)

                        } else if (isNextGenFlag == true)
                        {

                            nextRakeAngle = Math.Atan(nextgenWheelBase / dynamicCenterRake);  //returns radians
                        }


                    }






                } //end if data.OldData
            } //end if data.GameRunning
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here ! 
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            //return new SettingsControlDemo(this);
            return null;


        }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {

            SimHub.Logging.Current.Info("Car Setup Plugin Starting");


            // Load settings
            Settings = this.ReadCommonSettings<CarSetupPluginSettings>("GeneralSettings", () => new CarSetupPluginSettings());

            //Commented out AddProperty statements were used for debug

            // Declare a property available in the property list
            //pluginManager.AddProperty("CurrentDateTime", this.GetType(), DateTime.Now);

            pluginManager.AddProperty("Static.LF.CornerWeight", this.GetType(), 0);
            pluginManager.AddProperty("Static.RF.CornerWeight", this.GetType(), 0);
            pluginManager.AddProperty("Static.RR.CornerWeight", this.GetType(), 0);
            pluginManager.AddProperty("Static.LR.CornerWeight", this.GetType(), 0);

            pluginManager.AddProperty("Dynamic.LF.CornerWeight", this.GetType(), 0);
            pluginManager.AddProperty("Dynamic.RF.CornerWeight", this.GetType(), 0);
            pluginManager.AddProperty("Dynamic.RR.CornerWeight", this.GetType(), 0);
            pluginManager.AddProperty("Dynamic.LR.CornerWeight", this.GetType(), 0);

 /*           pluginManager.AddProperty("lfShockTravelAdjuster", this.GetType(), 0);
            pluginManager.AddProperty("rfShockTravelAdjuster", this.GetType(), 0);
            pluginManager.AddProperty("lrShockTravelAdjuster", this.GetType(), 0);
            pluginManager.AddProperty("rrShockTravelAdjuster", this.GetType(), 0);


            pluginManager.AddProperty("totalStaticWeight", this.GetType(), 0);
            pluginManager.AddProperty("totalStaticShockDeflection", this.GetType(), 0);

            pluginManager.AddProperty("lfStaticCornerWeightPercent", this.GetType(), 0);
            pluginManager.AddProperty("rfStaticCornerWeightPercent", this.GetType(), 0);
            pluginManager.AddProperty("lrStaticCornerWeightPercent", this.GetType(), 0);
            pluginManager.AddProperty("rrStaticCornerWeightPercent", this.GetType(), 0);
 */

            pluginManager.AddProperty("Static.LF.RideHeight", this.GetType(), 0);
            pluginManager.AddProperty("Static.RF.RideHeight", this.GetType(), 0);
            pluginManager.AddProperty("Static.LR.RideHeight", this.GetType(), 0);
            pluginManager.AddProperty("Static.RR.RideHeight", this.GetType(), 0);

            pluginManager.AddProperty("Static.CenterRake", this.GetType(), 0);

//            pluginManager.AddProperty("Dynamic.LF.ShockDeflection", this.GetType(), 0);
//            pluginManager.AddProperty("Dynamic.RF.ShockDeflection", this.GetType(), 0);
//            pluginManager.AddProperty("Dynamic.LR.ShockDeflection", this.GetType(), 0);
//            pluginManager.AddProperty("Dynamic.RR.ShockDeflection", this.GetType(), 0);

            pluginManager.AddProperty("Dynamic.LF.RideHeight", this.GetType(), 0);
            pluginManager.AddProperty("Dynamic.RF.RideHeight", this.GetType(), 0);
            pluginManager.AddProperty("Dynamic.LR.RideHeight", this.GetType(), 0);
            pluginManager.AddProperty("Dynamic.RR.RideHeight", this.GetType(), 0);


            pluginManager.AddProperty("Dynamic.CenterRake", this.GetType(), 0);


            //using as a a log function
            pluginManager.AddProperty("CarModel", this.GetType(), 0);
//            pluginManager.AddProperty("Shocks", this.GetType(), 0);
//            pluginManager.AddProperty("isOnPitRoadFlag", this.GetType(),0);
//            pluginManager.AddProperty("isOnTrackFlag", this.GetType(), 0);
//            pluginManager.AddProperty("isInGarageFlag", this.GetType(), 0);


            // Declare an event 
            //   pluginManager.AddEvent("SpeedWarning", this.GetType());

            // Declare an action which can be called
            //   pluginManager.AddAction("IncrementSpeedWarning", this.GetType(), (a, b) =>
            //   {
            //   Settings.SpeedWarningLevel++;
            //   SimHub.Logging.Current.Info("Speed warning changed");
            //   });

            // Declare an action which can be called
            //   pluginManager.AddAction("DecrementSpeedWarning", this.GetType(), (a, b) =>
            //  {
            //       Settings.SpeedWarningLevel--;
            //   });
        }
    }
}