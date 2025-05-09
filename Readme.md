# J-Rdp
J-Rdp is a tool to automate the editing and launching of rdp files.\
(Remote Desktop Protocol files, used for remote access to another computer.)

It will watch one or more folders for new files, using name criteria filters.\
When a new file is found, several actions can be taken:
- Move the file to another location.
- Edit settings in the file.
- Launch the file.
- Delete the file.

An example use case is when connection to a development server is done with single use ("burner") rdp files, and we want to automatically apply some settings before launching and finally deleting the file.

This program is for Windows, but a Linux version is planned for the future.
<br/><br/>



# License
This project is licensed under the **GNU General Public License v3.0 (GPLv3)**, which is a copyleft license.\
This means:
- You are free to use, modify, and distribute this software.
- Any modifications or derivative works must also be released under GPLv3.
- You cannot relicense this software under a more restrictive or proprietary license.
- This software is provided "as-is" without any warranty.

By using or distributing this software, you agree to the terms of the **GPLv3 license**.\
For full details, see the [License file](./License.txt) or visit [GNU's official site](https://www.gnu.org/licenses/gpl-3.0.html).
<br/><br/>



# Launching
The application is normally used with a tray icon and context menu, but can also be run silently in the background.\
No installation is needed.

To run it normally with a tray icon, simply run the .exe file.

To run it without a gui, run it with the `-NoTray` argument.\
A pair of .bat files are provided for convenient launching with arguments, and to stop the app.

To run the app automatically on boot/login, use either of these methods:\
\- Create a shortcut and put it in your startup folder.\
\- Use the Windows task scheduler to run it on startup or login.
<br/><br/>



# Tray menu
Right click the "RDP" tray icon to open the context menu.\
From here you can toggle log settings, open the config file, and toggle profiles on or off.\
![image](https://github.com/user-attachments/assets/c2d94e5d-f030-4d08-b3ec-b4f7feb262db)

Note that the log settings are reset when the program starts, use command line arguments to set their default state.\
See the "Logging" chapter below for details.

Click a profile to enable it and disable all other profiles.\
Ctrl-click a profile to toggle it. This allows having multiple profiles enabled at the same time, and to disable any enabled profiles.
<br/><br/>



# Configuration

## General configuration
The application is using a configuration file named "J-Rdp config.json" in the same directory as the .exe file.\
An example file is provided, edit it as needed.\
A new example file will be genererated if the config file is missing during app launch or when clicking "Open config file" in the tray menu.

Two general settings can be configured:
- `pollingInterval`: Decides how often, in milliseconds, the watched folder(s) should be checked for new files.\
  Default if omitted: 1000. Must be between 100 and 30000.
- `deleteDelay`: Decides how long to wait before deleting a file after launching it.\
  Default if omitted: 3000. Must be between 100 and 30000.



## Profiles
To configure which folders to watch and which actions should be taken, provide one or more profiles in the config file.

The settings are:
- `enabled`: Set to false to disable the profile without having to remove it from the config file.\
  Can be toggled from the tray menu.\
  Optional. Default if omitted: true
- `name`: The name of the profile. Can be at most 50 characters. Used in the tray menu and in log messages.\
  Optional. Default if omitted or empty: "(Unnamed profile)".
- `watchFolder`: Path to the folder that should be watched. Must be absolute, e.g. "C:/MyFolder".\
  Accepts forward slash `/` or double (escaped) backslash `\\` as directory separators.
- `filter`: The file name to watch for. The file type ".rdp" will be assumed if not provided.\
  Accepts wildcards. `?` is exactly one character, `*` is zero or more characters.
- `moveToFolder`: A discovered file will be moved here before operations are made.\
  Optional. Path can be absolute or relative (to the watched folder).\
- `launch`: Set to true to launch the file after other operations are made.
  Optional. Default if omitted: false
- `delete`: Set to true to delete the file after all other operations are made.
  Optional: Default if omitted: false
- `settings`: An array of strings, each representing a setting to be made in the .rdp file.\
  Optional. See the chapter about rdp settings below.



## Logging
Logging to file can be enabled via the context menu, and is off by default.\
To enable logging to file by default, use the `-LogToFile` argument when running the .exe file.

By default, one .log file will be generated per day in the "Logs" folder. They are kept for 30 days.\
Read them with a text editor such as notepad, or your favourite log reader.

A simplified log can also be opened in a console window, available in the tray menu.\
To open the log console automatically when the app is run, use the `-ShowConsole` argument.

To customize the logging, provide an NLog config file named "nlog.config" in the .exe directory\
and it will be used instead of the default log settings.
<br/><br/>



# RDP settings
Provide the settings to be applied as an array of strings in the profile.\
If a setting is not already present in the file, it will be added at the end.\
If a setting is already present in the file, the original line will be deleted and a new line will be added at the end.

See the [Microsoft documentation](https://learn.microsoft.com/en-us/windows-server/remote/remote-desktop-services/clients/rdp-files).

Examples of some useful settings:
- `screen mode id:i:<value>`: 1 for windowed, 2 for full screen.
- `desktopwidth:i:<value>`: Resolution width.
- `desktopheight:i:<value>`: Resolution height.
- `winposstr:s:0,1,x1,y1,x2,y2`: Set the size and position of the window by defining a rectangle. See [this post](https://superuser.com/a/665413) for an explanation.
- `smart sizing:i:1`: Scale the contens of the window when it is resized.
- `use multimon:i:1`: Enable multi monitor support. Will use all monitors unless `selectedmonitors:s` is used.
- `selectedmonitors:s:<value>`: Select which monitors to use when using multiple monitors. Zero-based, comma-separated list.\
  Example: A value of "1,2" will use monitor 2 and 3, but not monitor 1.
- `displayconnectionbar:i:<value>`: 1 to show the connection bar, 0 to hide it. If hidden, it can be temporarily brought back with ctrl+alt+home.
- `redirectprinters:i:<value>`: 1 to make printers on the local computer available in the remote session.
- `keyboardhook:i:<value>`: Where to apply Windows key combinations. 0: Local computer, 1: Remote, 2: Remote in full screen mode only.

Note that some settings may be disabled by the provider. Trying to set these will trigger an error message when trying to run the file: "This RDP File is corrupted. The remote connection cannot be started."
<br/><br/>



# Changelog
See the [changelog file](./Changelog.md) for details on updates and changes.

<br/>

---
© 2025 *Johan Ljungberg*. This project is licensed under GPLv3.
