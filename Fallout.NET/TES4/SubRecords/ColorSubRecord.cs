//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Fallout.NET.Core;

//using UnityEngine;

//namespace Fallout.NET.TES4.SubRecords
//{
//	public class ColorSubRecord : SubRecord
//	{
//		public uint RGBA;

//		public override void Deserialize(BetterReader reader, string name)
//		{
//			base.Deserialize(reader, name);
//			UnityEngine.Debug.Assert(Size == 4);
//			RGBA = reader.ReadUInt32();
//		}
//	}
//}
