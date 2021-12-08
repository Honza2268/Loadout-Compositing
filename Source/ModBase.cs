using System;
using System.Linq;
using HarmonyLib;
using Verse;

namespace Inventory
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    
    /*  Features:
     *      Tags:
     *          - Allow you to specify things for a pawn to have in their inventory/wear
     *          - Tags can be composited
     *      Bills:
     *          - Bills can be used to target things thing 'x where x is the number of pawns with tag y)
     *      Pawn AI:
     *          - Pawns automatically equip apparel in their tags if it is available
     *          - Pawns automatically pick up weapons/tools in their tags if available
     *      GUI:
     *          - Tags can be created from the Gear menu for pawns
     *          - Tags can use extended filters (stuffable, % wear, etc)
     */
    
    public class ModBase : Mod
    {
        public ModBase(ModContentPack content) : base(content)
        {
            new Harmony("wiri.is.stupid").PatchAll();
            
            
        }
    }
}