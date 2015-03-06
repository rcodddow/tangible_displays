/* * * * *
 * Tangible Displays
 * ------------------------------
 * 
 * This is part of the Tangible Displays Project, written over the week of March 2-6 in
 * Hamamatsu Japan.
 * 
 * This module provides the base message class
 * * * * */
using System.Collections;
using System.Text;
using SimpleJSON;

namespace COMProtocolLib {
	public class COMMessage  {

		public COMMessage() {}

		public COMMessage(JSONNode msg) {}

		public virtual string ToYAMLString() {
			StringBuilder x = new StringBuilder();
			x.Append("{");
			x.Append("}");
			return x.ToString();
		}

	}
}
