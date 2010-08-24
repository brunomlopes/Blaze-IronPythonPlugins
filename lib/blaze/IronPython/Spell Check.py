import sys
import clr
import spellcheck
import System
## clr.AddReference("System");
clr.AddReference("ContextLib");
from ContextLib import *

len = len(sys.argv)

args = ''
			
if len > 1:
	for i in range(1, len, 1):
		args += sys.argv[i]
		if i < len-1:
			args += ' '

if args != '':
	if not spellcheck.check_word(args):
		spellcheck.correct_word(args)
	else:
		System.Windows.Forms.MessageBox.Show("Your spelling is correct!", "Spell check", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
