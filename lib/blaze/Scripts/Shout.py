import sys
import clr
import System
clr.AddReference("System.Windows.Forms")

len = len(sys.argv)
args = ''

if len > 0:
	for index, arg in enumerate(sys.argv):
		args += arg
		if index < len-1:
			args += ' '

if args != '':
	System.Windows.Forms.MessageBox.Show(args, "Shout!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
else:
	System.Windows.Forms.MessageBox.Show("You must specify something to shout!", "Shout!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
