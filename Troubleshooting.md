
# Troubleshooting Visual Studio Extensions

If Visual Studio fails to compile properly due to extension-related errors, you can reset the experimental instance's user data. This is useful when debugging extensions.

## How to Reset the Experimental Instance

1. Press the **Windows** key or open the **Search Bar** and type "Developer Command Prompt for Visual Studio".

![Developer Command Prompt](Images/DeveloperCommandPrompt.jpg)

2. Select the **Developer Command Prompt** that matches your Visual Studio version.
3. Run the following command:

   ```bash
   devenv /rootsuffix Exp /resetuserdata
   ```
4. This will reset the experimental instance and uninstall any extensions related to it.

Now the code block for the bash command is correctly closed using three backticks.
