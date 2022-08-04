using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SpaceEngineers.Game.ModAPI.Ingame;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;

using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        string steelPlateStr = "Steel Plate\n";
        string ConstructionCompsStr = "Construction Components\n";
        string InteriorPlateStr = "Interior Plate\n";
        string SmallPipeStr = "Small Pipe\n";
        string LargePipeStr = "Large Pipe\n";
        string RadioCommCompStr = "Radio Communication Components\n";
        string SolarCellStr = "Solar Cell\n";
        string PowerCellStr = "Power Cell\n";
        string ExplosivesStr = "Explosives\n";
        string MetalGridStr = "Metal Grid\n";
        string ComputerStr = "Computer\n";
        string MedCompStr = "Medical Components\n";
        string GlassStr = "Glass\n";
        string DisplayStr = "Display\n";
        string MotorStr = "Motor\n";
        string ReactorCompsStr = "Reactor Components\n";
        string DetectorCompsStr = "Detector Components\n";
        string ThrustCompsStr = "Thrust Components\n";
        string GravGenStr = "Gravity Generator Components\n";
        string CanvasStr = "Canvas\n";
        string GirderStr = "Girder\n";
        string SupercondConduitsStr = "Superсonductor Conduits\n";
        string ClearStr = "<|Clear|>\n";
        string cursor = "==> ";
        int count;
        string lcdOutputStr = "   -->Space Industries Entarnament<--\n             >>Component Requester<<    \n---------------------------------------------------------------------------------------\n";
        string ITEM;
        string language;


        public void Save()
        {
            Storage = language;
        }

    public Program()
        {
            language = Storage;
        }
        void Main(string argument)

        {
            ITEM = "-";
            IMyTextPanel LCD = GridTerminalSystem.GetBlockWithName("RequestMonitor-1") as IMyTextPanel;
            IMyCargoContainer Out = GridTerminalSystem.GetBlockWithName("Output") as IMyCargoContainer;
            List<IMyTerminalBlock> Cargo = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Cargo);

            if (argument == "Russian")
            {
                language = "Russian";
                Save();
            }
            else if (argument == "English")
            {
                language = "English";
                Save();
            }
            bool OK = false;
            LCD.WriteText(lcdOutputStr, false);
            //scrolling view pages
            switch (argument)
            {
                case "Up":
                    count--;
                    break;

                case "Down":
                    count++;
                    break;

                case "OK":
                    OK = true;
                    break;
            }
            //view pages
            if (count > 23)
            {
                count = 1;
            }
            if (count < 1)
            {
                count = 23;
            }
            //view
            switch (count)
            {
                case 1:
                    LCD.WriteText(cursor + steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "SteelPlate";
                    }
                    break;
                case 2:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(cursor + ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "Construction";
                    }
                    break;
                case 3:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(cursor + InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "InteriorPlate";
                    }
                    break;
                case 4:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(cursor + SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "SmallTube";
                    }
                    break;
                case 5:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(cursor + LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "LargeTube";
                    }
                    break;
                case 6:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(cursor + RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "RadioCommunication";
                    }
                    break;
                case 7:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(cursor + SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "SolarCell";
                    }
                    break;
                case 8:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(cursor + PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "PowerCell";
                    }
                    break;
                case 9:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(cursor + ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "Explosives";
                    }
                    break;
                case 10:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(cursor + MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "MetalGrid";
                    }
                    break;
                case 11:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(cursor + ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "Computer";
                    }
                    break;
                case 12:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(cursor + MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "Medical";
                    }
                    break;
                case 13:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(cursor + GlassStr, true);
                    LCD.WriteText(DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "BulletproofGlass";
                    }
                    break;
                case 14:
                    LCD.WriteText(steelPlateStr, true);
                    LCD.WriteText(ConstructionCompsStr, true);
                    LCD.WriteText(InteriorPlateStr, true);
                    LCD.WriteText(SmallPipeStr, true);
                    LCD.WriteText(LargePipeStr, true);
                    LCD.WriteText(RadioCommCompStr, true);
                    LCD.WriteText(SolarCellStr, true);
                    LCD.WriteText(PowerCellStr, true);
                    LCD.WriteText(ExplosivesStr, true);
                    LCD.WriteText(MetalGridStr, true);
                    LCD.WriteText(ComputerStr, true);
                    LCD.WriteText(MedCompStr, true);
                    LCD.WriteText(GlassStr, true);
                    LCD.WriteText(cursor + DisplayStr, true);
                    LCD.WriteText("Page: 1", true);
                    if (OK == true)
                    {
                        ITEM = "Display";
                    }

                    break;
                case 15:
                    LCD.WriteText(cursor + MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Motor";
                    }
                    break;
                case 16:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(cursor + ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Reactor";
                    }
                    break;
                case 17:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(cursor + DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Detector";
                    }
                    break;
                case 18:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(cursor + ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Thrust";
                    }
                    break;
                case 19:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(cursor + GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "GravityGenerator";
                    }
                    break;
                case 20:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(cursor + CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Canvas";
                    }
                    break;
                case 21:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(cursor + GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Girder";
                    }
                    break;
                case 22:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(cursor + SupercondConduitsStr, true);
                    LCD.WriteText(ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        ITEM = "Superconductor";
                    }
                    break;
                case 23:
                    LCD.WriteText(MotorStr, true);
                    LCD.WriteText(ReactorCompsStr, true);
                    LCD.WriteText(DetectorCompsStr, true);
                    LCD.WriteText(ThrustCompsStr, true);
                    LCD.WriteText(GravGenStr, true);
                    LCD.WriteText(CanvasStr, true);
                    LCD.WriteText(GirderStr, true);
                    LCD.WriteText(SupercondConduitsStr, true);
                    LCD.WriteText(cursor + ClearStr, true);
                    LCD.WriteText("Page: 2", true);
                    if (OK == true)
                    {
                        for (int a = 0; a < Cargo.Count; a++)
                        {
                            if ((Cargo[a].HasInventory == true) && (Cargo[a].GetInventory() != null) && (Cargo[a].GetInventory().IsFull == false))
                            {
                                if (Cargo[a].GetInventory().IsConnectedTo(Out.GetInventory()))
                                {
                                    List<MyInventoryItem> inv = new List<MyInventoryItem>();
                                    //var inv = 
                                    Out.GetInventory().GetItems(inv);
                                    for (int b = 0; b < inv.Count; b++)
                                    {
                                        Out.GetInventory().TransferItemTo(Cargo[a].GetInventory(), b, null, true);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            if (OK == true)
            {
                for (int i = 0; i < Cargo.Count; i++)
                {
                    if ((Cargo[i].HasInventory == true) && (Cargo[i].GetInventory() != null))
                    {
                        if (Cargo[i].GetInventory().IsConnectedTo(Out.GetInventory()))
                        {
                            var inventory = new List<MyInventoryItem>();
                            Cargo[i].GetInventory().GetItems(inventory);
                            for (int j = 0; j < inventory.Count; j++)
                            {
                                //string This_Item_Name = inventory[j].Content.SubtypeName;
                                string This_Item_Name = inventory[j].Type.SubtypeId;
                                if (This_Item_Name == ITEM)
                                {
                                    Cargo[i].GetInventory().TransferItemTo(Out.GetInventory(), j, null, true);
                                }

                            }
                        }
                    }
                }
            }
            ITEM = "";
            if (language == "Russian")
            {
                steelPlateStr = "Стальная Пластина\n";
                ConstructionCompsStr = "Строительный Компонент\n";
                InteriorPlateStr = "Пластина\n";
                SmallPipeStr = "Маленькая Стальная Труба\n";
                LargePipeStr = "Большая Стальная Труба\n";
                RadioCommCompStr = "Комплектующие для радио-связи\n";
                SolarCellStr = "Солнечная батарея\n";
                PowerCellStr = "Аккумулятор\n";
                ExplosivesStr = "Взрывчатка\n";
                MetalGridStr = "Металлическая решетка\n";
                ComputerStr = "Компьютер\n";
                MedCompStr = "Медицинские компоненты\n";
                GlassStr = "Бронированное стекло\n";
                DisplayStr = "Экран\n";
                MotorStr = "Мотор\n";
                ReactorCompsStr = "Компоненты реактора\n";
                DetectorCompsStr = "Компоненты детектора\n";
                ThrustCompsStr = "Детали ускорителя\n";
                GravGenStr = "Компоненты гравитационного генератора\n";
                CanvasStr = "Canvas\n";
                GirderStr = "Балка\n";
                SupercondConduitsStr = "Superconductor Conduits\n";
                ClearStr = "<|Очистить|>\n";
            }
            if (language == "English")
            {
                steelPlateStr = "Steel Plate\n";
                ConstructionCompsStr = "Construction Components\n";
                InteriorPlateStr = "Interior Plate\n";
                SmallPipeStr = "Small Pipe\n";
                LargePipeStr = "Large Pipe\n";
                RadioCommCompStr = "Radio Communication Components\n";
                SolarCellStr = "Solar Cell\n";
                PowerCellStr = "Power Cell\n";
                ExplosivesStr = "Explosives\n";
                MetalGridStr = "Metal Grid\n";
                ComputerStr = "Computer\n";
                MedCompStr = "Medical Components\n";
                GlassStr = "Glass\n";
                DisplayStr = "Display\n";
                MotorStr = "Motor\n";
                ReactorCompsStr = "Reactor Components\n";
                DetectorCompsStr = "Detector Components\n";
                ThrustCompsStr = "Thrust Components\n";
                GravGenStr = "Gravity Generator Components\n";
                CanvasStr = "Canvas\n";
                GirderStr = "Girder\n";
                SupercondConduitsStr = "Superсonductor Conduits\n";
                ClearStr = "<|Clear|>\n";
            }
        }
    }
}
