import sys
import clr
import System
clr.AddReference("System.Windows.Forms")

ip = _user_context.GetExternalIpAddress();
message = "Your current external IP address is: " + ip + "\n\n" + "Would you like to copy it to the clipboard?"
res = System.Windows.Forms.MessageBox.Show(message, "What is my IP?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information);
if res == System.Windows.Forms.DialogResult.Yes:
	_user_context.SetClipboardText(ip)
