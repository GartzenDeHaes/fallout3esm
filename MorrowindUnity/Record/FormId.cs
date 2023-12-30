using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace Fallout.NET.TES4
//{
	public struct FormId //: IComparable<FormID>
	{
		public uint Value;

		// Plugin Index (8 bits)
		public byte PluginIndex { get { return (byte)((Value >> 24) & 0xFF); } }

		// Form Type (4 bits)
		public byte FormType { get { return (byte)((Value >> 20) & 0x0F); } }

		// Local FormID (20 bits)
		public uint LocalFormID { get { return Value & 0xFFFFF; } }

		// Constructor to initialize the FormID
		public FormId(uint formId)
		{
			Value = formId;
		}

		// Display the FormID as a string
		public override string ToString()
		{
			return $"[{PluginIndex:X2}:{FormType:X1}:{LocalFormID:X5}]";
		}

		public static bool operator ==(in FormId left, in FormId right)
		{
			return left.Value == right.Value;
		}

		public static bool operator !=(in FormId left, in FormId right)
		{
			return left.Value != right.Value;
		}

		public static bool operator ==(in FormId left, in uint right)
		{
			return left.Value == right;
		}

		public static bool operator !=(in FormId left, in uint right)
		{
			return left.Value != right;
		}

		// IComparable implementation for sorting
		public int CompareTo(FormId other)
		{
			if (PluginIndex != other.PluginIndex)
				return PluginIndex.CompareTo(other.PluginIndex);

			if (FormType != other.FormType)
				return FormType.CompareTo(other.FormType);

			return LocalFormID.CompareTo(other.LocalFormID);
		}

		// Override Equals method
		public override bool Equals(object obj)
		{
			if (!(obj is FormId))
				return false;

			FormId other = (FormId)obj;
			return PluginIndex == other.PluginIndex && FormType == other.FormType && LocalFormID == other.LocalFormID;
		}

		// Override GetHashCode method
		public override int GetHashCode()
		{
			unchecked
			{
				//return HashCode.Combine(PluginIndex, FormType, LocalFormID);
				return (int)Value;
			}
		}

		//public void Deserialize(ITesReader reader, string name)
		//{
		//	Value = reader.ReadUInt32();
		//}
	}
//}
