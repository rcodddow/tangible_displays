using System.Collections;
using System.Text;
using SimpleJSON;

/**
 * This (mostly empty) class is the parent class for all RosBridgeMsg's (the actual message) from
 * ROS. As the message can be empty....
 * <p>
 * This could be omitted I suppose, but it is retained here as (i) it nicely parallels the
 * ROSBRidgePacket class which encapsulates the top of the ROSBridge messages which are not
 * empty, and (ii) someday ROS may actually define a  minimal message.
 * 
 * Version History
 * 3.1 - changed methods to start with an upper case letter to be more consistent with c#
 * style.
 * 3.0 - modification from hand crafted version 2.0
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.1
 */

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
