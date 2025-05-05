# J-Rdp changelog

## 0.3.0
2025-05-05

Changes and improvements:
- The project is now under the GPLv3 license.
- The program now has a tray icon with a context menu, allowing the following:
  - Toggle profiles on or off. Click to switch to the profile (disabling other profiles), or ctrl-click to toggle its enabled state.
  - Open the config file.
  - Choose whether to show the log in a console window and save log to file.
- The config file will now be generated if it is missing on startup or when opened from the tray menu.
- Old log files will now be deleted after 30 days. (Configurable)
- Command line arguments have been changed. See the readme file for details.
- Empty profile names will now be treated as missing, meaning that they will be displayed as "(Unnamed profile)" instead of an empty name.
- Profile names now have a limit of 50 characters.
- Various minor optimizations and stability improvements.

Bug fixes:
- Fixed that changing the config file could trigger the detection event twice.

## 0.2.1
2025-01-22

Changes and improvements:
- Added protection against multiple instances of the program.
- Simplified log in the console window. Log file remains detailed.
- Sample profiles in the config file have been improved.
- Improved logging.

Bug fixes:
- Potential fix for the issue where the program minimized instead of going invisible on Windows 11.
- Fixed missing program name in task manager description.

## 0.2.0
2024-05-21

Changes and improvements:
- Improved logging.
- Logging can now be enabled or disabled.
- The console window title is now the program name instead of the full path.
- Moved some command line arguments to config file.
- Users can now provide their own nlog.config to customize logging.
- Delete delay is now configurable.
- "watchFolder" path must now be absolute. A relative path would previously be relative to the exe, which makes no sense.
- If "moveToFolder" path is relative, it is now relative to "watchFolder".
- Profiles can now be disabled.
- Existing settings in rdp files will be replaced, instead of duplicates being added at the end.
- ".rdp" file type is assumed in filter, if not explicitly stated. So the filter "\*.txt" will be interpreted as "\*.txt.rdp", while "\*.rdp" will be used as-is.
- Missing settings in config and profiles will use their default values.
- Improved config validation.

Bug fixes:
- Fixed a bug where changes to the config file was not detected after some time.

## 0.1.0
2024-03-27

Initial release.
